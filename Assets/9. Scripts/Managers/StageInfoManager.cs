using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Random = UnityEngine.Random;


// ���� ���̵� 
public enum GamePlayLevel
{ 
    NORMAL = 1,     // �Ϲ�
    HARD = 2,       // �ϵ�
    SPECIAL = 3,    // Ư�� 
};

// 1. ����  ��Ʋ 2. �̺�Ʈ 3. ȥ�� 4. ����
public enum StageType 
{
    NONE = 0, 
    BATTLE = 1,
    EVENT, 
    SHOP,
    MULTY, 
    MAX = MULTY,
    TEST = 999,
};

public enum EventSlot 
{
    NONE = 0,
    STORY = 1,
    BUFF, 
    DEBUFF,
    SPECIAL
};

// �̺�Ʈ ���� 
public enum EventCategory
{
    STORY = -1,     
    FIND_RECORD = 0,        // ���ڵ� 
    FIND_RELRIC,        // ���� �߰�
    SPECIAL,        // ȥ���� (���ڵ带 ȹ���ϰ� ���� ����)
    SHOP,           // ������ ���� 
    MAX = SHOP,
};

// �̺�Ʈ ���� Ÿ��
public enum EventRewardType
{
    NONE = 0, 
    GAME_MONEY,     // ������ȭ 
    RELIC,          // ����
    MEMORY,         // �޸� 
    PRIVATE_REWRAD, // �ش� �̺�Ʈ ���� 
};


// �������� Ÿ�Ժ� ��Ÿ���� ����
[System.Serializable]
public class StageAppearInfo
{
    // ���� ���� 
    //�Ϲ� / ����Ʈ /  ���� ������������? 
    public MonsterGrade monsterGrade; 
    public int wave;
    public int maxWave; // ���̺� ����̶�� �ִ� ���̺� �� 
    public string stageName;
    public int mapID;   // ��� ���� �׷����ϴ°� 

    // ������ ���ͳ� ��Ÿ ID ����Ʈ
    public List<int> appearIDList = new List<int>();

    public StageAppearInfo(MonsterGrade monsterGrade = MonsterGrade.NORMAL, int wave = 1, 
        string stageName = "", int mapID = 0)
    {
        this.monsterGrade = monsterGrade;
        this.wave = wave;
        this.maxWave = wave;
        this.stageName = stageName;
        this.mapID = mapID;
    }


    // �Ű�������� ����Ʈ�� ���� �����͸� �����Ѵ�. 
    public void SetAppearIDList(List<int> list)
    {
        appearIDList.Clear();
        // ����Ʈ�� ���� ������ �־��ش� 
        appearIDList = list.ToList();
    }


    ~StageAppearInfo()
    {
        appearIDList.Clear(); 
    }
}

[System.Serializable]
// ���������� ���� �̺�Ʈ ������ ��� Ŭ���� 
public class StageEventInfo
{
    public uint stageId;

    public StageType stageType;
    // ���� �̺�Ʈ ����  
    public EventCategory mainEventCategory;
    // ���� �̺�Ʈ 
    public EventCategory subEventCategory;
    // �� ���� 
    // ���丮 �̺�Ʈ�� ��Ÿ�� ���丮 �ƽ� ���� 
    
    // �������� Ÿ�Կ� ���� ��Ÿ�� �׷� ����
    public StageAppearInfo appearInfo;

    // ��Ÿ�� �������� 
    public void CreateAppearInfo()
    {
        appearInfo = new StageAppearInfo();
    }

};

/// <summary>
/// �� Ŭ������ ���������� ��ġ�Ǵ� ���鿡 ���� ������ ���� Ŭ�����̴�.
/// �̺�Ʈ���� �����ϸ� ���� ���� �ܿ� ���� �̺�Ʈ���� ��ġ�Ͽ� �����ش� 
/// ���� - ���͸� ��ġ ��Ų��.
/// 
/// </summary>
[System.Serializable]
public class StageTableClass
{
    public int tableOrder; 
    //�������� �̸�
    public string stageName;

    public StageType type; 
    // ���������� ��� �ִ� ���� �̺�Ʈ �������� ����ִ� ����Ʈ 
    public List<StageEventInfo> eventInfoList;

    // �������� Ÿ�Ժ��� ��Ÿ�� id ����Ʈ
    public List<int> appearTargetIDList = new List<int>(); 

    public bool isBossStage;
    public bool isLocked;
    public bool isCleared;

    public StageTableClass()
    {
        Init(); 
    }

    public StageTableClass(int tableOrder, 
        List<StageEventInfo> eventInfoList,
        bool isBossStage, bool isLocked, bool isCleared)
    {
        this.tableOrder = tableOrder;
 
        this.eventInfoList = eventInfoList;
        this.isBossStage = isBossStage;
        this.isLocked = isLocked;
        this.isCleared = isCleared;
    }

    // ���� �ʱ�ȭ 
    public void Init()
    {
        tableOrder = 0;
        stageName = ""; 

        eventInfoList = null;

        isBossStage = false;
        isLocked = false;
        isCleared = false;
    }
}


// �������� ����(���� / ��)�� ��� ���� �Ŵ��� 
public class StageInfoManager : MonoBehaviour
{
    public static StageInfoManager instance;

    public static bool FLAG_ADVENTURE_MODE = false;     // Ž�� ��� ���� ������ �÷��� 
    public static bool initJoinPlayGameModeFlag = false;    // ù �������� �÷��� 
    public static readonly int LEVEL_NORMAL_MAX_STAGE_COUNT = 5;
    public static readonly int LEVEL_HARD_MAX_STAGE_COUNT = 7;
    public const int MAX_STAGE_COUNT = 5;
    public const int MAX_STAGE_SELECT_COUNT = 3;     // �� ���������� �� �� �ִ� ���� �������� ��


    public string stageName;
    public int currentChapter;      // ���� ���� é�� �� 
    public int maxChapter; 
    public int enemyCount;              // ���� �� �� �� 
    public int itemCount;
    public int selectStageEventNum;   // ������ ���� �������� ��ȣ 

    int eliteAppearPoint;   // ����Ʈ�� ������ �� �ִ� �ּڰ� 

    public bool isTest = false;
    [SerializeField] private StageTableClass selectStageTable;

    [SerializeField] List<StageTableClass> stageTables = null;
    [SerializeField] GamePlayLevel gameLevel = 0; // ���� ���̵�
    [SerializeField] public Dictionary<GamePlayLevel, float> playLevelPair = new Dictionary<GamePlayLevel, float>();
    [SerializeField] List<List<int>> stageLocateList;       // �������� ��ġ ������ ���� ����Ʈ 
    // é��, stagetable �� 
    private Dictionary<int, List<StageTableClass>> stageDictList = new Dictionary<int, List<StageTableClass>>(); 

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        if (playLevelPair.Count <= 0)
        {
            // �븻 20�� 
            playLevelPair.Add(GamePlayLevel.NORMAL, 0.2f);

            // �ϵ� 40��
            playLevelPair.Add(GamePlayLevel.HARD, 0.4f);

            // Ư�� 30��
            playLevelPair.Add(GamePlayLevel.SPECIAL, 0.3f);
        }

        currentChapter = 1;
        // todo �ӽ÷� �ϳ��� �س���.
        maxChapter = 1; 
    }

    public void SetStageList(int _chapter, ref List<StageTableClass> _stageTableList)
    {
        stageDictList[_chapter] = _stageTableList;
    }

    public void SetStageList(Dictionary<int, List<StageTableClass>> stageTableDic)
    {
        stageDictList = stageTableDic;
    }

    public Dictionary<int, List<StageTableClass>> GetStageList()
    {
        return stageDictList;
    }


    // �÷����� ���������� �����س��� �Լ� 
    public void ChoiceStageInfoForPlaying(int _chapter, int _selectStageNumber, int _selectEventNumber)
    {
        if(stageDictList.Count <= 0)
        {
            selectStageEventNum = 0; 
            return;
        }
        isTest = false; 
        currentChapter = _chapter;
        var stageInfo = stageDictList[_chapter];

        if (_selectStageNumber - 1 < 0)
        {
            Debug.Log("������ �������� �ε����� 0 �̸� ����");
            return;
        }

        selectStageTable = stageInfo[_selectStageNumber-1];    // ������ �������� ����    
        selectStageEventNum = _selectEventNumber;    // ���� �������� ������ ����
    }

    public void ChoiceTestStage()
    {
        // �׽�Ʈ�� �������� ���̺� Ŭ���� 
        StageTableClass testStageClass = new StageTableClass();
        StageEventInfo stageEventInfo = new StageEventInfo();
        stageEventInfo.stageType = StageType.TEST;

        testStageClass.eventInfoList = new List<StageEventInfo> { stageEventInfo };
        
        isTest = true;
        selectStageEventNum = 0;
        selectStageTable = testStageClass;
    }

    public void SetStageInfo(StageTableClass _stage)
    {
        selectStageTable = _stage;
    }

    // ������ �������� ���̺� Ŭ���� ��ȯ
    public StageTableClass GetStageTableClass()
    {
        return selectStageTable;
    }

    // ������ �������� �̺�Ʈ Ŭ���� ��ȯ 
    public StageEventInfo GetStageEventClass()
    {
        if (selectStageTable == null || selectStageTable.eventInfoList == null) return null; 

        return selectStageTable.eventInfoList[selectStageEventNum];
    }

    // ��ġ�� �������� ���� �������� 
    public StageTableClass GetLocatedStageInfo(int _count)
    {
        if (stageDictList.Count <= 0) return null;

        var stageTable = stageDictList[currentChapter];
        if(stageTable == null)
        {
            return null; 
        }

        return stageTable[_count];
    }


    // ��ġ�� �������� ��������Ʈ�� �����´�. 
    public void GetLocatedStageInfoList(out List<StageTableClass> list, int chpapter = 1)
    {
        list = new List<StageTableClass>();

        if (stageDictList.TryGetValue(chpapter, out List<StageTableClass> stageTable) == false)
        {
            
        }
        else
        {
            list = stageTable;
        }
    }

    public void GetLocatedStageInfoListByCurrenChapter(out List<StageTableClass> list)
    {
        GetLocatedStageInfoList(out list, currentChapter);
    }


    // �������� Ŭ���� �� �ٸ� �������� ���� ���Ž�Ų��
    public void RefreshCurrentChapterStageTableClass()
    {
        if (currentChapter == 0 || isTest == true) return;
        
        var stageTables = stageDictList[currentChapter];
        if (stageTables == null || selectStageTable == null) return;

        foreach (var stageTable in stageTables)
        {
            // ������ ���������� Ŭ���� ó�� 
            if(selectStageTable.tableOrder == stageTable.tableOrder)
            {
                stageTable.isCleared = true;
                stageTable.isLocked = true;

                // ������ ���������� �������̰� Ŭ���� �ߴٸ� ���� é�ͷ� �Ѿ��.
                if(stageTables.Last().isCleared == true)
                {
                    currentChapter += 1;
                    // �Ѿ é�Ͱ� ������ ������ �Ұ����ϹǷ� ���� ��� �÷��׸� ����.
                    if(currentChapter > maxChapter)
                    {
                        StageInfoManager.FLAG_ADVENTURE_MODE = false; 
                        // ���Ӹ�尡 ������ ���� �׵��� �޾ƿԴ� ���ڵ�� ���� 
                        if(RecordManager.instance != null)
                        {
                            RecordManager.instance.ClearRecords();
                        }
                        // é�͸� �ٽ� 1�� ����
                        currentChapter = 1;
                        stageDictList.Clear(); // �����ߴ� ������������ ���� ���� 
                    }

                    break;
                }
            }
            // ������ ���������� ����  �������� ��� ����
            else if(selectStageTable.tableOrder + 1 == stageTable.tableOrder)
            {
                stageTable.isLocked = false;
            }
        }
    }



    // �������� ���� ����  
    // ���̸� �˷��ָ� �� ���� ���� ������ ������ ä��� �Լ� 
    void SetStagePosOneLine(int length, float rate = 1.0f)
    {
        // �̹� ����� ���ٸ� ���� �������� �ʴ´�. 
        if (stageLocateList != null)
        {
            return;
        }

        stageLocateList = new List<List<int>>();

        // ���۰� ���� �����̴�. 
        List<int> start = new List<int>(1) { 1, 1, 1 };
        List<int> end = new List<int>(1) { 1 };
        stageLocateList.Add(start);

        // ���� ���۰� ���� �� ���� ���̸� ����Ѵ�. 
        int diff = length - 2;

        // ���� ���̿� ��ġ�� �� 80%�� ���� 20�� �̺�Ʈ�� 
        // ��ȹ ���� - �̺�Ʈ ���������� ������ �ϳ� ��ġ�ϰ� �� �ȿ��� ������ �߰��� ��ġ���� �����Ѵ�.
        // 1�ܰ� 1�������� ��� 5�� �������� �ۿ� ������ 1 1 2 3 1 �� ��ġ�Ѵ�.
        // �� �̺�Ʈ�� ��ġ�� ���� ������ �ߺ����� ��ġ�Ǹ� ����ϴ� ������ ��ġ�� �� ���������� �����Ѵ�. 
        // ���� ��ġ��ŭ �ݺ��ؼ� ����,

        // �� �������� ��忡 �� �̺�Ʈ ��� �ִ� ���� 3��
        int maxEventNodeCount = 3;
        int minEventNodeCount = 1;

        eliteAppearPoint = length / 2;

        bool isElitePosFlag = false;
        bool isShopFlag = false; 
        // �� ��° ������������ ��ġ 
        for (int i = 1; i < length - 1; i++)
        {
            var sublist = new List<int>();
            // �������� �������� ��ġ�� ��ü ���̿��� ���� ��ġ ���� ���� ���¶�� ����Ʈ�� �̺�Ʈ�� ���ÿ� ��ġ�ȴ�. 
            if (i > eliteAppearPoint)
            {
                isElitePosFlag = true;
            }
            int currentNodeCount = Random.Range(minEventNodeCount, maxEventNodeCount + 1);
            for (int j = 0; j < currentNodeCount; j++)
            {
                // ����Ʈ�� ��ġ�Ǿ��ٴ� �÷��׸� ������� �̺�Ʈ�� ��ġ�Ѵ�. 
                if (isElitePosFlag == true)
                {
                    // ���� �̺�Ʈ ��ġ Ȯ�� 
                    // �����͸� �����ϴµ� �־ ���ϵ��� StageType�� ������ ���� enum������ �ٷ� ���� 
                    int randShop = Random.Range(0, 10);
                    if (isShopFlag == false && randShop < 3 )
                    {
                        sublist.Add((int)StageType.SHOP);
                        isShopFlag = true; 
                    }
                    else
                    {
                        sublist.Add((int)StageType.EVENT);
                    }
                }              
                // ���� ���������� ���� 
                else
                {
                    sublist.Add((int)StageType.BATTLE);
                }

            }

            // �ٽ� �÷��� �� ��ġ
            isElitePosFlag = false;
            stageLocateList.Add(sublist);
        }

        // ��� ������ �������� �������� �������� �־��ش�. 
        stageLocateList.Add(end);

        return;
    }

    // StageTableClass ����Ʈ ���� ������ �������� Ÿ�Կ� ���� ���ͳ� �̺�Ʈ�� ��ġ��Ų��.
    void SetStageTableClassListByStateTypeData(ref List<StageTableClass> list)
    {
        if (list == null || list.Count <= 0) return;

        // �������� Ÿ�� �˻� 
        foreach (StageTableClass tableClass in list)
        {
            if (tableClass == null || tableClass.eventInfoList == null) continue;

            foreach (var eventInfo in tableClass.eventInfoList)
            {
                if (eventInfo == null) continue;

                // �������� Ÿ���� ���� Ÿ���� ��� 
                if (eventInfo.stageType == StageType.BATTLE)
                {
                    // ������ ���� ID ����Ʈ �����
                    // todo ���� ���� ������ �����ؾ��Ѵ�.
                    MonsterDatabase.instance.GetMonsterIDListFromTargetStage(
                        currentChapter, (int)1,
                        eventInfo);
                }
                // �̺�Ʈ �߻� Ÿ���� ��� 
                else if (eventInfo.stageType == StageType.EVENT)
                {
                    // ���� �̺�Ʈ �� ���� 
                    int randomCategory = Random.Range(0, (int)EventCategory.MAX);
                    // todo �ϴ� ���� �׽�Ʈ������ ������ ���ڵ� ������ 
                    randomCategory = (int)EventCategory.FIND_RECORD;
                    eventInfo.mainEventCategory = (EventCategory)randomCategory;
                    
                    // 1. ���ڵ尡 �����ϴ� �̺�Ʈ 
                    if (eventInfo.mainEventCategory == EventCategory.FIND_RECORD)
                    {
                        // ���⼭ ���� ��ų ���ڵ� ����
                        if (RecordManager.instance != null)
                        {
                            RecordManager.instance.GetStageEventRewardRecord(eventInfo);
                        }
                    }
                    // 2. ����(= ��Ƽ��Ʈ)�� ��Ÿ���� �̺�Ʈ
                    else
                    {

                    }
                }
                // ������ ��Ÿ���� �̺�Ʈ 
                else if(eventInfo.stageType == StageType.SHOP)
                {
                    // todo. ������ ���� ������ �����ϰ� �������� ���� �ʿ�
                }

            }
        }
    }

    // ����Ʈ�� ������ ���� �κи� �ٽ� ����� ������ (�������ʹ� ����) 
    void SetMonsterStageGrade(ref List<StageTableClass> list)
    {
        if (list == null) return;

        // ���������� ���� ���̸� ����ؼ� ������ å���Ѵ�.
        // ����Ʈ�� ������ ���������� ���� �� ������ �̰� ���������� �����ϸ� �ȵȴ�. 
        // 5���� ���������������� 1���� ����Ʈ�� ���ԵǾ� �ִ�. 
        int stageCount = list.Count;
        // ����Ʈ ���� �� 
        int maxEliteMonsterCount = 1;
        // �� �������� ��� ������ ������. 
        int resultDivineStageCount = stageCount / 2;

        if (playLevelPair.Count() < 0)
            return;

        float value = 0;
        // �� ���� 0���� ũ�ٸ� ����Ʈ ���� ������ �߰����� ���� �����Ѵ�. ���̵��� ���� Ȯ���� �ٸ���. 
        if (gameLevel == GamePlayLevel.NORMAL)
        {
            // NORAML Ȯ��
            value = playLevelPair[GamePlayLevel.NORMAL];
        }
        else if (gameLevel == GamePlayLevel.HARD)
        {
            value = playLevelPair[GamePlayLevel.HARD];
        }
        else if (gameLevel == GamePlayLevel.SPECIAL)
        {
            value = playLevelPair[GamePlayLevel.SPECIAL];
        }

        var randValue = Random.Range(0, 1.0f);
        if (randValue <= value)
        {
            // Ȯ���� ��÷�Ǹ� ���� �߰� 
            maxEliteMonsterCount += resultDivineStageCount;
        }

        int currentElitLocateCount = 0;
        bool possibleElite = false; // ����Ʈ ��ġ�������� üũ 
        // tableclass ����Ʈ�� ���鼭 ���� �Ҵ��Ѵ�. 
        for (int i = 0; i < list.Count; i++)
        {
            var stage = list[i];
            if (stage == null) continue;

            if (i >= eliteAppearPoint)
                possibleElite = true;

            bool isBossStage = false;
            if (stage.isBossStage == true)
            {
                isBossStage = true;
            }

            // ����Ʈ ���ο� �������� monstertype�͸� �ǵ��. 
            foreach (var eventInfo in stage.eventInfoList)
            {
                if (eventInfo == null || eventInfo.appearInfo == null)
                {
                    continue;
                }

                // ���� Ÿ���� �ƴϸ� �ȵȴ�.
                if (eventInfo.stageType != StageType.BATTLE)
                {
                    continue;
                }

                // ���� �������� ������ ���� ���� ����
                if (isBossStage == true)
                {
                    eventInfo.appearInfo.monsterGrade = MonsterGrade.BOSS;
                    continue;
                }

                // ����Ʈ�� ������ �ϳ��� ��ġ 
                if (possibleElite == true &&
                    currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    eventInfo.appearInfo.monsterGrade = MonsterGrade.ELITE;
                }
                // �ϳ� �̻� ��ġ ���� �� 
                else if (possibleElite == true &&
                    currentElitLocateCount >= 1 &&
                     currentElitLocateCount < maxEliteMonsterCount)
                {
                    // ���� ���������� �� ���������� ���� ����� �����Ѵ�. 
                    var eliteCount = Random.Range(0, 1.0f);
                    // 8%������� �߰��� ��ġ��Ų��. 
                    if (eliteCount <= 0.08f)
                    {
                        eventInfo.appearInfo.monsterGrade = MonsterGrade.ELITE;
                    }
                }
                else
                {
                    eventInfo.appearInfo.monsterGrade = MonsterGrade.NORMAL;
                }
            }

        }
    }

    // ��� �������� ��� ó�� (1�������� ����)
    // Ư�� �������� Ŭ���� ���ο� ���� ���� 
    private void LockedAllStage()
    {
        if (stageTables == null) return;

        for (int i = 1; i < stageTables.Count; i++)
        {
            if (stageTables[i] == null) continue;
            stageTables[i].isLocked = true;
        }
    }

    // �������� ���̺� ����
    public void CreateStageTableList(int level = 1)
    {
        // 0. �ϳ��� ���θ� �����Ѵ�. �� �������� ��忡�� �̺�Ʈ �ִ� 3���� ���. 
        // 3���� �̺�Ʈ�� ���� ����, Ư�� ���, ���� ���� �̺�Ʈ��� ����������. 
        // �������� ���� �ִ� 5�� (�Ϲ�), �ϵ�� 7�� 

        // todo ����Ȱ� �ִ��� �˻��ؾ��Ѵ�. 

        // ����Ȱ� ���ٸ� �����͸� ó������ �������ϹǷ� �÷��׵� ���ش�. 
        initJoinPlayGameModeFlag = true; 

        // �̹� ������������ �������ٸ� ������ �������ʴ´�. 
        if (stageDictList.ContainsKey(currentChapter) == true && 
            stageDictList[currentChapter] != null && 
            stageDictList[currentChapter].Count > 0)
            return; 

        // 1. ���̵��� ���� �ִ� ���������� �����Ѵ� 
        int maxStageCount = LEVEL_NORMAL_MAX_STAGE_COUNT;
        if (level == (int)GamePlayLevel.HARD)
        {
            maxStageCount = LEVEL_HARD_MAX_STAGE_COUNT;
        }

        if (stageTables == null)
        {
            stageTables = new List<StageTableClass>();
        }
        else
        {
            stageTables.Clear();
        }

        // �������� ��ġ�� ����Ʈ�� ���� 
         SetStagePosOneLine(maxStageCount);
        // 1-1 �������� ��ġ�� ����Ʈ ��ŭ �����ϱ�
        for (int i = 0; i < stageLocateList.Count; i++)
        {
            StageTableClass stageTable = new StageTableClass
            {
                // �������� �� ���� order ����
                tableOrder = i + 1,
                // 2.1 �������� �̸� ����
                stageName = currentChapter + "-" + i + 1,
                // 2.2 ���������� Ÿ�� ���� 
                //  stageType = (StageType)stageLocateList[i].First(),
                eventInfoList = new List<StageEventInfo>()
            };
            for (int j = 0; j < stageLocateList[i].Count; j++)
            {
                StageEventInfo info = new StageEventInfo
                {
                    stageType = (StageType)stageLocateList[i][j]
                };

                stageTable.eventInfoList.Add(info);
            }

            if (stageTables != null)
            {
                stageTables.Add(stageTable);
            }

        }

        // ������ ���������� ���� ���������� �����س��´�.
        if (stageTables.Last() != null)
        {
            stageTables.Last().isBossStage = true;
        }

        // 3. 
        // ���� Ÿ���� ������ ����ȭ�ؾ��Ѵ�. 
        SetMonsterStageGrade(ref stageTables);
        // Ÿ�Ժ� ���� ���� 
        SetStageTableClassListByStateTypeData(ref stageTables);

        // 4. �������� ��ױ�
        LockedAllStage();

        SetStageList(currentChapter, ref stageTables); 

    }



}
