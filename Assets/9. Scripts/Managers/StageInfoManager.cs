using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
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
    MAX = MULTY
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
    STORY = 1,     
    BUFF,           
    DEBUFF, 
    SPECIAL, 
};

[System.Serializable]
public class StageAppearMonsterInfo
{
    //�Ϲ� / ����Ʈ /  ���� ������������? 
    MonsterGrade monsterGrade; 
    public int wave;
    public int maxWave; // ���̺� ����̶�� �ִ� ���̺� �� 
    public string stageName;
    public int mapID;   // ��� ���� �׷����ϴ°� 

    // ������ ���͸���Ʈ ID, ���� ��ġ 
    public List<int> appearMonsterList = new List<int>();

    public StageAppearMonsterInfo(MonsterGrade monsterGrade = MonsterGrade.NORMAL, int wave = 1, 
        string stageName = "", int mapID = 0)
    {
        this.monsterGrade = monsterGrade;
        this.wave = wave;
        this.maxWave = wave;
        this.stageName = stageName;
        this.mapID = mapID;
    }


    // �Ű�������� ����Ʈ�� ���� �����͸� �����Ѵ�. 
    public void SetMonsterList(List<int> list)
    {
        appearMonsterList.Clear();
        // ����Ʈ�� ���� ������ �־��ش� 
        appearMonsterList = list.ToList();
    }


    ~StageAppearMonsterInfo()
    {
        appearMonsterList.Clear(); 
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
    public MonsterGrade monsterType;
    public EventSlot eventSlot;
    // ���丮 �̺�Ʈ�� ��Ÿ�� ���丮 �ƽ� ���� 
    // 

    // �����̺�Ʈ�� ��Ÿ�� ���� �׷� ����
    public StageAppearMonsterInfo appearMonsterInfo;

    // ���� �׷� ���� 
    public void CreateMonsterGroup()
    {
        appearMonsterInfo = new StageAppearMonsterInfo();
    }

};

/// <summary>
/// �� Ŭ������ ���������� ��ġ�Ǵ� ���鿡 ���� ������ ���� Ŭ�����̴�.
/// �̺�Ʈ���� �����ϸ� ���� ���� �ܿ� ���� �̺�Ʈ���� ��ġ�Ͽ� �����ش� 
/// ���� - ���͸� ��ġ ��Ų��.
/// </summary>
[System.Serializable]
public class StageTableClass
{
    public int tableOrder; 
    //�������� �̸�
    public string stageName;

    // ���������� ��� �ִ� ���� �̺�Ʈ �������� ����ִ� ����Ʈ 
    public List<StageEventInfo> eventInfoList;

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

    // �ִ� �������� �� 
    public static readonly int LEVEL_NORMAL_MAX_STAGE_COUNT = 5;
    public static readonly int LEVEL_HARD_MAX_STAGE_COUNT = 7;
    public const int MAX_STAGE_COUNT = 5;
    public const int MAX_STAGE_SELECT_COUNT = 3;     // �� ���������� �� �� �ִ� ���� �������� ��


    public string stageName;
    public int currentChapter;      // ���� ���� é�� �� 
    public int enemyCount;              // ���� �� �� �� 
    public int itemCount;
    public int selectStageEventNum;   // ������ ���� �������� ��ȣ 

    int eliteAppearPoint;   // ����Ʈ�� ������ �� �ִ� �ּڰ� 

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
    }

    public void SetStageList(int _chapter, ref List<StageTableClass> _stageTableList)
    {
        stageDictList[_chapter] = _stageTableList;
    }


    // �÷����� ���������� �����س��� �Լ� 
    public void ChoiceStageInfoForPlaying(int _chapter, int _selectStageNumber, int _selectEventNumber)
    {
        if(stageDictList.Count <= 0)
        {
            selectStageEventNum = 0; 
            return;
        }

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


    // �������� Ŭ���� �� �ٸ� �������� ���� ���Ž�Ų��
    public void RefreshCurrentChapterStageTableClass()
    {
        if (currentChapter == 0) return;

        var stageTables = stageDictList[currentChapter];
        if (stageTables == null || selectStageTable == null) return;

        foreach (var stageTable in stageTables)
        {
            // ������ ���������� Ŭ���� ó�� 
            if(selectStageTable.tableOrder == stageTable.tableOrder)
            {
                stageTable.isCleared = true;
                stageTable.isLocked = true;
            }
            // ������ ���������� ����  �������� ��� ����
            else if(selectStageTable.tableOrder + 1 == stageTable.tableOrder)
            {
                stageTable.isLocked = false;
            }
        }
    }
        


    // ��ġ�� ���������� ������ �������� ��ȯ
    public StageTableClass GetLoacatedStageLastStageInfo()
    {
        List<StageTableClass> stageList;
        GetLocatedStageInfoList(out stageList);
        if (stageList == null || stageList.Count <= 0 )
        {
            return null;
        }

        var stageTable = GetLocatedStageInfo(stageList.Count-1);
        if (stageTable != null)
        {
            return stageTable;
        }
            

        return null;
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
        // �� �̺�Ʈ�� ��ġ�� ���� ������ �ߺ����� ��ġ�Ǹ� ����ϴ� ������ ��ġ�� �� ���������� �����Ѵ�. 
        // ���� ��ġ��ŭ �ݺ��ؼ� ����,

        int maxShopCount = 1;
        int maxEventCount = 1;
        int currentShopCount = 0;
        int currentEventCount = 0;

        // �� �������� ��忡 �� �̺�Ʈ ��� �ִ� ���� 3��
        int maxEventNodeCount = 3;
        int minEventNodeCount = 1;

        eliteAppearPoint = length / 2;

        bool isBothPosFlag = false;
        for (int i = 1; i < length - 1; i++)
        {
            var sublist = new List<int>();
            // �������� �������� ��ġ�� ��ü ���̿��� ���� ��ġ ���� ���� ���¶�� ����Ʈ�� �̺�Ʈ�� ���ÿ� ��ġ�ȴ�. 
            if (i > eliteAppearPoint)
            {
                isBothPosFlag = true;
            }
            int currentNodeCount = Random.Range(minEventNodeCount, maxEventNodeCount + 1);
            for (int j = 0; j < currentNodeCount; j++)
            {
                int randEvent = Random.Range(0, 11);
                // �̺�Ʈ �������� ��÷
                if (randEvent == 0 || randEvent == 1)
                {
                    bool eventPosFlag = false;
                    // ���⼭ �� �� �� ���� ������Ѵ�. 
                    // �̹� ������ ���Ե� ���¶�� ������ �߰����� �ʴ´�. 
                    int randDecideEvent = Random.Range(0, 2);
                    if (randDecideEvent == 1 && maxShopCount <= currentShopCount)
                    {
                        currentShopCount += 1;
                        sublist.Add(3);
                        eventPosFlag = true;
                    }
                    else if (maxEventCount <= currentEventCount)
                    {
                        currentEventCount += 1;
                        sublist.Add(2);
                        eventPosFlag = true;
                    }
                    else
                    {
                        sublist.Add(1);
                    }

                    // �̺�Ʈ �÷��װ� ��ġ�Ǿ����ٸ� ����Ʈ�� ��ġ�ǰ� �ϱ� ���� ��ġ�� �־��ش�.
                    if (isBothPosFlag == true && eventPosFlag == true &&
                       currentNodeCount < maxEventNodeCount)
                    {
                        sublist.Add(1);
                    }
                }
                // ���� ���������� ���� 
                else
                {
                    sublist.Add(1);
                }

            }

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

            //bool isBossStage = false; 
            //// 1. ���� �������� ���� �˻� 
            //if (tableClass.isBossStage == true)
            //{
            //    isBossStage = true; 
            //}

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

            }


            // 2. �̺�Ʈ�� ���
            //if(tableClass.stageType == StageType.EVENT)
            //{

            //}
            //// 3. ������ ��� 
            //else if(tableClass.stageType == StageType.MONSTER)
            //{
            //    if (monsterDB == null) return;

            //    // ������ ���� ID���� ���� 
            //    tableClass.eventInfoList = new List<StageEventInfo>();


            //    // ���� �׷� ����� 
            //    mainEventInfo.CreateMonsterGroup(MonsterGrade.NORMAL, 1, "", 1);
            //    // ���� ID����Ʈ ���� 
            //    monsterDB.GetMonsterIDListFromTargetStage(curMainChpaterNum, (int)1,
            //        ref mainEventInfo.monsterGroup.AppearMonsterList);
            //    tableClass.eventInfoList.Add(mainEventInfo);
            //}
            //// 5. ������ ��� 
            //else if(tableClass.stageType == StageType.SHOP)
            //{

            //}
            //// 5. ȥ������ ��� 
            //else if(tableClass.stageType == StageType.MULTY)
            //{

            //}
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
                if (eventInfo == null) continue;

                // ���� Ÿ���� �ƴϸ� �ȵȴ�.
                if (eventInfo.stageType != StageType.BATTLE)
                {
                    continue;
                }

                if (isBossStage == true)
                {
                    eventInfo.monsterType = MonsterGrade.BOSS;
                    continue;
                }

                // ����Ʈ�� ������ �ϳ��� ��ġ 
                if (possibleElite == true &&
                    currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    eventInfo.monsterType = MonsterGrade.ELITE;
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
                        eventInfo.monsterType = MonsterGrade.ELITE;
                    }
                }
                else
                {
                    eventInfo.monsterType = MonsterGrade.NORMAL;
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
