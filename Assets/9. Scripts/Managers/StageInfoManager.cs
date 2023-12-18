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

    private bool isStageLockChet = false;

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

    public void SetStageList(int _chapter, List<StageTableClass> _stageTableList)
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


    // 스테이지 선택
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
            Debug.Log("선택한 데이터가없습니다.");
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


   // 진행중인 현재 챕터 갱신
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



     // 스테이지들을 배치한다.
    void SetStagePosOneLine(int length, float rate = 1.0f)
    {
        if (stageLocateList != null)
        {
            return;
        }

        stageLocateList = new List<List<int>>();

        List<int> start = new List<int>(1) { 1, 1, 1 };
        List<int> end = new List<int>(1) { 1 };
        stageLocateList.Add(start);

        
        int diff = length - 2;

        int maxEventNodeCount = 3;
        int minEventNodeCount = 1;

        eliteAppearPoint = length / 2;

        bool isElitePosFlag = false;
        bool isShopFlag = false; 
        for (int i = 1; i < length - 1; i++)
        {
            var sublist = new List<int>();
          
            if (i > eliteAppearPoint)
            {
                isElitePosFlag = true;
            }
            int currentNodeCount = Random.Range(minEventNodeCount, maxEventNodeCount + 1);
            for (int j = 0; j < currentNodeCount; j++)
            {
                // 엘리트가 배치되었다면 
                if (isElitePosFlag == true)
                {
                    //상점과 이벤트 스테이지를 배치한다.
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
                // 그게 아니면 전투
                else
                {
                    sublist.Add((int)StageType.BATTLE);
                }

            }

            // 배치 종료
            isElitePosFlag = false;
            stageLocateList.Add(sublist);
        }

        // 종료엔 보스를 넣는다.
        stageLocateList.Add(end);

        return;
    }

    // StageTableClass 리스트를 참조하면서 각 타입별로 배치한다.
    void SetStageTableClassListByStateTypeData(ref List<StageTableClass> list)
    {
        if (list == null || list.Count <= 0) return;

        // 
        foreach (StageTableClass tableClass in list)
        {
            if (tableClass == null || tableClass.eventInfoList == null) continue;

            foreach (var eventInfo in tableClass.eventInfoList)
            {
                if (eventInfo == null) continue;

                // 전투 타입일 경우 
                if (eventInfo.stageType == StageType.BATTLE)
                {
                    // 몬스터 값을 가져온다.
                    // todo 챕터 및 난이도 조정하는 법 수정
                    MonsterDatabase.instance.GetMonsterIDListFromTargetStage(
                        currentChapter, (int)1,
                        eventInfo);
                }
                // 이벤트 타입일 경우  
                else if (eventInfo.stageType == StageType.EVENT)
                {
                    // 
                    int randomCategory = Random.Range(0, (int)EventCategory.MAX);
                    // todo 
                    randomCategory = (int)EventCategory.FIND_RECORD;
                    eventInfo.mainEventCategory = (EventCategory)randomCategory;
                    
                    // 1. 
                    if (eventInfo.mainEventCategory == EventCategory.FIND_RECORD)
                    {
                        //
                        if (RecordManager.instance != null)
                        {
                            RecordManager.instance.GetStageEventRewardRecord(eventInfo);
                        }
                    }
                    // 2. 
                    else
                    {

                    }
                }
                // 
                else if(eventInfo.stageType == StageType.SHOP)
                {
                    // todo. 
                }

            }
        }
    }

    // 몬스터 스테이지에게 등급을 설정한다.
    void SetMonsterStageGrade(ref List<StageTableClass> list)
    {
        if (list == null) return;

        
        int stageCount = list.Count;
        
        int maxEliteMonsterCount = 1;
        
        int resultDivineStageCount = stageCount / 2;

        if (playLevelPair.Count() < 0)
            return;

        float value = 0;
        
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
                if (eventInfo == null )
                {
                    continue;
                }

                if (eventInfo.appearInfo == null)
                    eventInfo.CreateAppearInfo();

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

    // 첫 스테이지를 제외하고 모든 스테이지를 잠근다.
    private void LockedAllStage()
    {
        if (stageTables == null) return;

        for (int i = 1; i < stageTables.Count; i++)
        {
            if (stageTables[i] == null) continue;
            stageTables[i].isLocked = false;
        }
    }

    // 단계에 맞는 Stage Table 클래스 List를 생성
    public void CreateStageTableList(int level = 1)
    {
        initJoinPlayGameModeFlag = true; 

        if (stageDictList.ContainsKey(currentChapter) == true && 
            stageDictList[currentChapter] != null && 
            stageDictList[currentChapter].Count > 0)
            return; 

        // 1. 난이도 별 최대 스테이지 책정
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

        // 스테이지를 한 줄로 배치하고 해당 하는 기능의 넘버를 담는 리스트를 생성
         SetStagePosOneLine(maxStageCount);
        // 1-1 한줄 리스트 만큼 순회하면서 해당하는 기능의 클래스 생성하기
        for (int i = 0; i < stageLocateList.Count; i++)
        {
            StageTableClass stageTable = new StageTableClass
            {
                //  order 
                tableOrder = i + 1,
                // 2.1 스테이지 이름 생성
                stageName = currentChapter + "-" + i + 1,
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

        // 마지막 스테이지는 보스 스테이지로 고정해놓는다
        if (stageTables.Last() != null)
        {
            stageTables.Last().isBossStage = true;
        }

        // 3. 
        // 몬스터 스테이지라면 등급 설정하기
        SetMonsterStageGrade(ref stageTables);
        // 스테이지 타입별로 스테이지 정보에 세부사항 할당
        SetStageTableClassListByStateTypeData(ref stageTables);

        // 4. 첫 번째를 제외한 스테이지는 전부 잠금처리
        LockedAllStage();

        SetStageList(currentChapter, stageTables); 

    }



}
