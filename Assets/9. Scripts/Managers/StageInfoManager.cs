using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Random = UnityEngine.Random;


// 게임 난이도 
public enum GamePlayLevel
{ 
    NORMAL = 1,     
    HARD = 2,       
    SPECIAL = 3,    
};

// 1. 전투 2. 이벤트 3. 상점  4. 다용도
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

public enum ContentType
{
    NONE = 0,
    ADVENTURE = 1, 
    BOSS_RAID = 2, 
}

public enum EventSlot 
{
    NONE = 0,
    STORY = 1,
    BUFF, 
    DEBUFF,
    SPECIAL
};

// 이벤트 카테고리 
public enum EventCategory
{
    STORY = -1,     
    FIND_RECORD = 0,        // 레코드를 발견하는 이벤트
    FIND_RELRIC,            // 유믈 발견
    SPECIAL,                // 특수
    SHOP,                   // 상점
    MAX = SHOP,
};

// 이벤트 보상
public enum StageRewardType
{
    NONE = 0, 
    GAME_MONEY,     // 재화 보상
    RELIC,          // 유물
    MEMORY,         // 
    PRIVATE_REWRAD, // 전용 보상
};


// 스테이지에 등장하는 정보를 담는 클래스
[System.Serializable]
public class StageAppearInfo
{
    public StageType stageType;
    public int stageID; 
    public EventCategory eventCategory;
    public MonsterGrade monsterGrade; 
    public int wave;
    public int maxWave;  
    public string stageName;
    public int mapID;   

    // 등장하는 정보의 ID를 담는다
    public List<int> appearIDList = new List<int>();

    // 이 스테이지의 보상 정보를 담는다. 
    public List<int> rewardIDList = new List<int>(); 

    public StageAppearInfo(MonsterGrade monsterGrade = MonsterGrade.NORMAL, int wave = 1, 
        string stageName = "", int mapID = 0)
    {
        this.monsterGrade = monsterGrade;
        this.wave = wave;
        this.maxWave = wave;
        this.stageName = stageName;
        this.mapID = mapID;
    }


    // 등장하려는 이벤트들의 ID값을 담아둔다.
    public void SetAppearIDList(List<int> list)
    {
        appearIDList.Clear();
        appearIDList = list.ToList();
    }

    
    public void SetStageReward(List<int> reawrdList)
    {
        rewardIDList = reawrdList.ToList();
    }


    ~StageAppearInfo()
    {
        appearIDList.Clear();
        rewardIDList.Clear();
    }
}

// 이벤트 정보를 담는 클래스
[System.Serializable]
public class StageEventCutScene
{
    public uint stageId;

    // 주요 이벤트 카테고리
    public EventCategory mainEventCategory;
    // 서브 이벤트 카테고리
    public EventCategory subEventCategory;

    // 등장하는 컷씬 ID
    public int cutSceneID;

};


[System.Serializable]
public class StageNodeInfo
{
    // 정렬용 오더
    public int tableOrder; 
    // 스테이지 이름 
    public string stageName;

    public ContentType contentType; 
    // 이벤트 컷씬 리스트
    public List<StageEventCutScene> eventInfoList;

    // 등장하는 스테이지 정보 리스트
    public List<StageAppearInfo> stageAppearInfos = new List<StageAppearInfo>();


    public bool isBossStage;
    public bool isLocked;
    public bool isCleared;

    public StageNodeInfo()
    {
        Init(); 
    }

    public StageNodeInfo(int tableOrder, 
        string stageName,
        ContentType contentType,
        List<StageEventCutScene> eventInfoList,
        bool isBossStage, bool isLocked, bool isCleared)
    {
        this.tableOrder = tableOrder;
        this.stageName = stageName;
        this.contentType = contentType;
        this.eventInfoList = eventInfoList;
        this.isBossStage = isBossStage;
        this.isLocked = isLocked;
        this.isCleared = isCleared;
    }

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


public class StageInfoManager : MonoBehaviour
{
    public static StageInfoManager instance;

    public static bool FLAG_ADVENTURE_MODE = false;     // 탐사 모드 상태인지 확인하는 값
    public static bool initJoinPlayGameModeFlag = false;    // 게임 모드를 막 시작한 참인지 확인 하는 값 
    public static readonly int LEVEL_NORMAL_MAX_STAGE_COUNT = 5;
    public static readonly int LEVEL_HARD_MAX_STAGE_COUNT = 7;
    public const int MAX_STAGE_COUNT = 5;
    public const int MAX_STAGE_SELECT_COUNT = 3;     


    public string stageName;
    public int currentChapter;      // 현재 챕터
    public int maxChapter; 
    public int enemyCount;              // 적 개수 
    public int itemCount;
    public int selectStageEventNum;   // 선택한 스테이지 이벤트 인덱스 번호

    int eliteAppearPoint;   // 엘리트가 등장하는 위치 값

    public bool isTest = false;
    [SerializeField] private StageNodeInfo selectStageNodeInfo;

    [SerializeField] List<StageNodeInfo> stageTables = null;
    [SerializeField] GamePlayLevel gameLevel = 0; 
    [SerializeField] public Dictionary<GamePlayLevel, float> playLevelPair = new Dictionary<GamePlayLevel, float>();
    [SerializeField] List<List<int>> stageLocateList;       
    // é��, stagetable �� 
    private Dictionary<int, List<StageNodeInfo>> stageDictList = new Dictionary<int, List<StageNodeInfo>>();

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
            // 노말 20퍼
            playLevelPair.Add(GamePlayLevel.NORMAL, 0.2f);

            // 하드 40
            playLevelPair.Add(GamePlayLevel.HARD, 0.4f);

            // 특수 30
            playLevelPair.Add(GamePlayLevel.SPECIAL, 0.3f);
        }

        currentChapter = 1;
        // todo 
        maxChapter = 1; 
    }

    public void SetStageList(int _chapter, List<StageNodeInfo> _stageTableList)
    {
        stageDictList[_chapter] = _stageTableList;
    }

    public void SetStageList(Dictionary<int, List<StageNodeInfo>> stageTableDic)
    {
        stageDictList = stageTableDic;
    }

    public Dictionary<int, List<StageNodeInfo>> GetStageList()
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

        selectStageNodeInfo = stageInfo[_selectStageNumber-1];     // 선택한 스테이지    
        selectStageEventNum = _selectEventNumber;               // 선택한 스테이지 안 이벤트 값
    }

    public void ChoiceTestStage()
    {
        StageNodeInfo testStageNode= new StageNodeInfo();
        StageAppearInfo appearInfo = new StageAppearInfo();
        appearInfo.stageType = StageType.TEST;

        testStageNode.stageAppearInfos = new List<StageAppearInfo> { appearInfo };
        
        isTest = true;
        selectStageEventNum = 0;
        selectStageNodeInfo = testStageNode;
    }

    public void SetStageInfo(StageNodeInfo stage)
    {
        selectStageNodeInfo = stage;
    }

    // 선택한 스테이지 노드 반환
    public StageNodeInfo GetStageNodeInfo()
    {
        return selectStageNodeInfo;
    }

    // 선택한 스테이지의 이벤트 컷씬 클래스를 반환한다.
    public StageEventCutScene GetStageEventClass()
    {
        if (selectStageNodeInfo == null || selectStageNodeInfo.eventInfoList == null) return null; 

        return selectStageNodeInfo.eventInfoList[selectStageEventNum];
    }

    // 선택한 스테이지 노드의 appearInfo 클래스 반환
    public StageAppearInfo GetStageAppearInfoByCurrentStageNode()
    {
        return selectStageNodeInfo.stageAppearInfos[selectStageEventNum];
    }



    // 해당 값의 배치된 스테이지 테이블 정보를 반환
    public StageNodeInfo GetLocatedStageInfo(int _count)
    {
        if (stageDictList.Count <= 0) return null;

        var stageTable = stageDictList[currentChapter];
        if(stageTable == null)
        {
            return null; 
        }

        return stageTable[_count];
    }


    // 해당 챕터의 배치된 스테이지 테이블 리스트들을 반환 
    public void GetLocatedStageInfoList(out List<StageNodeInfo> list, int chpapter = 1)
    {
        list = new List<StageNodeInfo>();

        if (stageDictList.TryGetValue(chpapter, out List<StageNodeInfo> stageTable) == false)
        {
            
        }
        else
        {
            list = stageTable;
        }
    }

    public void GetLocatedStageInfoListByCurrenChapter(out List<StageNodeInfo> list)
    {
        GetLocatedStageInfoList(out list, currentChapter);
    }


    public void RefreshCurrentStageInfo()
    {
        if (selectStageNodeInfo == null)
            return;

        if (selectStageNodeInfo.contentType == ContentType.NONE)
            return;
        else if (selectStageNodeInfo.contentType == ContentType.ADVENTURE)
            RefreshCurrentChapterStageNodeInfo();
        else if(selectStageNodeInfo.contentType == ContentType.BOSS_RAID)
        {
            // todo 보스레이드 관련 레벨 증가 및 다음 던전 해금 
        }
    }

   // 진행중인 현재 챕터 갱신
    public void RefreshCurrentChapterStageNodeInfo()
    {
        if (currentChapter == 0 || isTest == true  || stageDictList[currentChapter] == null) return;
        
        var stageTables = stageDictList[currentChapter];
        if (stageTables == null || selectStageNodeInfo == null) return;

        foreach (var stageTable in stageTables)
        {
            if(selectStageNodeInfo.tableOrder == stageTable.tableOrder)
            {
                stageTable.isCleared = true;
                stageTable.isLocked = true;

                if(stageTables.Last().isCleared == true)
                {
                    currentChapter += 1;
                    if(currentChapter > maxChapter)
                    {
                        StageInfoManager.FLAG_ADVENTURE_MODE = false; 
                        if(RecordManager.instance != null)
                        {
                            RecordManager.instance.ClearRecords();
                        }
                        currentChapter = 1;
                        stageDictList.Clear(); 
                    }

                    break;
                }
            }
            else if(selectStageNodeInfo.tableOrder + 1 == stageTable.tableOrder)
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

    // StageNodeInfo 리스트를 참조하면서 각 타입별로 배치한다.
    void SetStageNodeInfoListByStateTypeData(ref List<StageNodeInfo> list)
    {
        if (list == null || list.Count <= 0) return;

        // 
        foreach (StageNodeInfo tableClass in list)
        {
            if (tableClass == null || tableClass.stageAppearInfos == null) continue;

            foreach (var appearInfo in tableClass.stageAppearInfos)
            {
                if (appearInfo == null) continue;

                // 전투 타입일 경우 
                if (appearInfo.stageType == StageType.BATTLE)
                {
                    // 몬스터 값을 가져온다.
                    // todo 챕터 및 난이도 조정하는 법 수정
                    MonsterDatabase.instance.GetMonsterIDListFromTargetStage(
                        currentChapter, (int)1,
                        appearInfo);
                }
                // 이벤트 타입일 경우  
                else if (appearInfo.stageType == StageType.EVENT)
                {
                    // 
                    int randomCategory = Random.Range(0, (int)EventCategory.MAX);
                    // todo 
                    randomCategory = (int)EventCategory.FIND_RECORD;
                    appearInfo.eventCategory = (EventCategory)randomCategory;
                    
                    // 1. 
                    if (appearInfo.eventCategory == EventCategory.FIND_RECORD)
                    {
                        //
                        if (RecordManager.instance != null)
                        {
                            RecordManager.instance.GetStageEventRewardRecord(appearInfo);
                        }
                    }
                    // 2. 
                    else
                    {

                    }
                }
                // 
                else if(appearInfo.stageType == StageType.SHOP)
                {
                    // todo. 
                }

            }
        }
    }

    // 몬스터 스테이지에게 등급을 설정한다.
    void SetMonsterStageGrade(ref List<StageNodeInfo> list)
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
        bool possibleElite = false; 
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

            foreach (var appearInfo in stage.stageAppearInfos)
            {
                if (appearInfo == null )
                {
                    continue;
                }

                if (appearInfo.stageType != StageType.BATTLE)
                {
                    continue;
                }

                if (isBossStage == true)
                {
                    appearInfo.monsterGrade = MonsterGrade.BOSS;
                    continue;
                }

                if (possibleElite == true &&
                    currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    appearInfo.monsterGrade = MonsterGrade.ELITE;
                }
                else if (possibleElite == true &&
                    currentElitLocateCount >= 1 &&
                     currentElitLocateCount < maxEliteMonsterCount)
                {
                    var eliteCount = Random.Range(0, 1.0f);
                    if (eliteCount <= 0.08f)
                    {
                        appearInfo.monsterGrade = MonsterGrade.ELITE;
                    }
                }
                else
                {
                    appearInfo.monsterGrade = MonsterGrade.NORMAL;
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
    public void CreateAdventureStageNodeList(int level = 1)
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
            stageTables = new List<StageNodeInfo>();
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
            StageNodeInfo stageTable = new StageNodeInfo
            {
                //  order 
                tableOrder = i + 1,
                // 2.1 스테이지 이름 생성
                stageName = currentChapter + "-" + i + 1,
                contentType = ContentType.ADVENTURE,
                //  stageType = (StageType)stageLocateList[i].First(),
                eventInfoList = new List<StageEventCutScene>()
            };
            for (int j = 0; j < stageLocateList[i].Count; j++)
            {
                // todo 컷신클래스 정보 추가

                StageAppearInfo info = new StageAppearInfo
                {
                    stageType = (StageType)stageLocateList[i][j]
                };

                stageTable.stageAppearInfos.Add(info);
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
        SetStageNodeInfoListByStateTypeData(ref stageTables);

        // 4. 첫 번째를 제외한 스테이지는 전부 잠금처리
        LockedAllStage();

        SetStageList(currentChapter, stageTables); 

    }


    // 보스레이드 
    #region Boss Raid 
    
    // 보스 


    #endregion

}
