using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Random = UnityEngine.Random;


// 게임 난이도 
public enum GamePlayLevel
{ 
    NORMAL = 1,     // 일반
    HARD = 2,       // 하드
    SPECIAL = 3,    // 특수 
};

// 1. 몬스터  배틀 2. 이벤트 3. 혼합 4. 상점
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

// 이벤트 형태 
public enum EventCategory
{
    STORY = -1,     
    FIND_RECORD = 0,        // 레코드 
    FIND_RELRIC,        // 유물 발견
    SPECIAL,        // 혼합형 (레코드를 획득하고 전투 돌입)
    SHOP,           // 떠돌이 상인 
    MAX = SHOP,
};

// 이벤트 보상 타입
public enum EventRewardType
{
    NONE = 0, 
    GAME_MONEY,     // 게임재화 
    RELIC,          // 유물
    MEMORY,         // 메모리 
    PRIVATE_REWRAD, // 해당 이벤트 전용 
};


// 스테이지 타입별 나타나는 정보
[System.Serializable]
public class StageAppearInfo
{
    // 전투 관련 
    //일반 / 엘리트 /  보스 스테이지인지? 
    public MonsterGrade monsterGrade; 
    public int wave;
    public int maxWave; // 웨이브 방식이라면 최대 웨이브 값 
    public string stageName;
    public int mapID;   // 어떠한 맵을 그려야하는가 

    // 등장할 몬스터나 기타 ID 리스트
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


    // 매개변수대로 리스트에 몬스터 데이터를 설정한다. 
    public void SetAppearIDList(List<int> list)
    {
        appearIDList.Clear();
        // 리스트에 몬스터 정보를 넣어준다 
        appearIDList = list.ToList();
    }


    ~StageAppearInfo()
    {
        appearIDList.Clear(); 
    }
}

[System.Serializable]
// 스테이지에 들어가는 이벤트 정보가 담긴 클래스 
public class StageEventInfo
{
    public uint stageId;

    public StageType stageType;
    // 메인 이벤트 형식  
    public EventCategory mainEventCategory;
    // 서브 이벤트 
    public EventCategory subEventCategory;
    // 뭘 넣지 
    // 스토리 이벤트면 나타날 스토리 컷신 정보 
    
    // 스테이지 타입에 따른 나타날 그룹 정보
    public StageAppearInfo appearInfo;

    // 나타날 정보생성 
    public void CreateAppearInfo()
    {
        appearInfo = new StageAppearInfo();
    }

};

/// <summary>
/// 이 클래스는 스테이지에 배치되는 노드들에 대한 정보를 가진 클래스이다.
/// 이벤트마다 상이하며 몬스터 전투 외에 여러 이벤트들을 배치하여 보여준다 
/// 몬스터 - 몬스터를 배치 시킨다.
/// 
/// </summary>
[System.Serializable]
public class StageTableClass
{
    public int tableOrder; 
    //스테이지 이름
    public string stageName;

    public StageType type; 
    // 스테이지에 들어 있는 각종 이벤트 정보들을 담아있는 리스트 
    public List<StageEventInfo> eventInfoList;

    // 스테이지 타입별로 나타날 id 리스트
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

    // 변수 초기화 
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


// 스테이지 정보(몬스터 / 맵)를 담는 보조 매니저 
public class StageInfoManager : MonoBehaviour
{
    public static StageInfoManager instance;

    public static bool FLAG_ADVENTURE_MODE = false;     // 탐사 모드 진행 중인지 플래그 
    public static bool initJoinPlayGameModeFlag = false;    // 첫 진입인지 플래그 
    public static readonly int LEVEL_NORMAL_MAX_STAGE_COUNT = 5;
    public static readonly int LEVEL_HARD_MAX_STAGE_COUNT = 7;
    public const int MAX_STAGE_COUNT = 5;
    public const int MAX_STAGE_SELECT_COUNT = 3;     // 각 스테이지별 고를 수 있는 하위 스테이지 수


    public string stageName;
    public int currentChapter;      // 진행 중인 챕터 수 
    public int maxChapter; 
    public int enemyCount;              // 게임 내 적 수 
    public int itemCount;
    public int selectStageEventNum;   // 선택한 몬스터 스테이지 번호 

    int eliteAppearPoint;   // 엘리트가 등장할 수 있는 최솟값 

    public bool isTest = false;
    [SerializeField] private StageTableClass selectStageTable;

    [SerializeField] List<StageTableClass> stageTables = null;
    [SerializeField] GamePlayLevel gameLevel = 0; // 게임 난이도
    [SerializeField] public Dictionary<GamePlayLevel, float> playLevelPair = new Dictionary<GamePlayLevel, float>();
    [SerializeField] List<List<int>> stageLocateList;       // 스테이지 배치 정보를 가진 리스트 
    // 챕터, stagetable 순 
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
            // 노말 20퍼 
            playLevelPair.Add(GamePlayLevel.NORMAL, 0.2f);

            // 하드 40퍼
            playLevelPair.Add(GamePlayLevel.HARD, 0.4f);

            // 특수 30퍼
            playLevelPair.Add(GamePlayLevel.SPECIAL, 0.3f);
        }

        currentChapter = 1;
        // todo 임시로 하나만 해놓음.
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


    // 플레이할 스테이지를 설정해놓는 함수 
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
            Debug.Log("선택할 스테이지 인덱스가 0 미만 판정");
            return;
        }

        selectStageTable = stageInfo[_selectStageNumber-1];    // 선택한 스테이지 정보    
        selectStageEventNum = _selectEventNumber;    // 서브 스테이지 선택한 정보
    }

    public void ChoiceTestStage()
    {
        // 테스트용 스테이지 테이블 클래스 
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

    // 선택한 스테이지 테이블 클래스 반환
    public StageTableClass GetStageTableClass()
    {
        return selectStageTable;
    }

    // 선택한 스테이지 이벤트 클래스 반환 
    public StageEventInfo GetStageEventClass()
    {
        if (selectStageTable == null || selectStageTable.eventInfoList == null) return null; 

        return selectStageTable.eventInfoList[selectStageEventNum];
    }

    // 배치된 스테이지 정보 가져오기 
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


    // 배치된 스테이지 정보리스트를 가져온다. 
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


    // 스테이지 클리어 후 다른 스테이지 정보 갱신시킨다
    public void RefreshCurrentChapterStageTableClass()
    {
        if (currentChapter == 0 || isTest == true) return;
        
        var stageTables = stageDictList[currentChapter];
        if (stageTables == null || selectStageTable == null) return;

        foreach (var stageTable in stageTables)
        {
            // 선택한 스테이지의 클리어 처리 
            if(selectStageTable.tableOrder == stageTable.tableOrder)
            {
                stageTable.isCleared = true;
                stageTable.isLocked = true;

                // 선택한 스테이지가 마지막이고 클리어 했다면 다음 챕터로 넘어간다.
                if(stageTables.Last().isCleared == true)
                {
                    currentChapter += 1;
                    // 넘어갈 챕터가 없으면 진행이 불가능하므로 게임 모드 플래그를 끈다.
                    if(currentChapter > maxChapter)
                    {
                        StageInfoManager.FLAG_ADVENTURE_MODE = false; 
                        // 게임모드가 꺼짐에 따라 그동안 받아왔던 레코드들 삭제 
                        if(RecordManager.instance != null)
                        {
                            RecordManager.instance.ClearRecords();
                        }
                        // 챕터를 다시 1로 복귀
                        currentChapter = 1;
                        stageDictList.Clear(); // 진행했던 스테이지들을 전부 제거 
                    }

                    break;
                }
            }
            // 선택한 스테이지에 다음  스테이지 잠금 해제
            else if(selectStageTable.tableOrder + 1 == stageTable.tableOrder)
            {
                stageTable.isLocked = false;
            }
        }
    }



    // 스테이지 생성 관련  
    // 길이를 알려주면 그 사이 값을 일정한 비율로 채우는 함수 
    void SetStagePosOneLine(int length, float rate = 1.0f)
    {
        // 이미 만들어 졌다면 따로 생성하지 않는다. 
        if (stageLocateList != null)
        {
            return;
        }

        stageLocateList = new List<List<int>>();

        // 시작과 끝은 고정이다. 
        List<int> start = new List<int>(1) { 1, 1, 1 };
        List<int> end = new List<int>(1) { 1 };
        stageLocateList.Add(start);

        // 먼저 시작과 끝을 뺀 남은 길이를 계산한다. 
        int diff = length - 2;

        // 남은 길이에 배치할 때 80%로 몬스터 20로 이벤트다 
        // 기획 변경 - 이벤트 스테이지를 무조건 하나 배치하고 그 안에서 상점이 추가로 배치될지 결정한다.
        // 1단계 1스테이지 경우 5개 스테이지 밖에 없으니 1 1 2 3 1 로 배치한다.
        // 또 이벤트를 배치할 때도 상점이 중복으로 배치되면 곤란하니 상점만 배치할 땐 독립적으로 적용한다. 
        // 남은 수치만큼 반복해서 대입,

        // 한 스테이지 노드에 들어갈 이벤트 노드 최대 수는 3개
        int maxEventNodeCount = 3;
        int minEventNodeCount = 1;

        eliteAppearPoint = length / 2;

        bool isElitePosFlag = false;
        bool isShopFlag = false; 
        // 두 번째 스테이지부터 배치 
        for (int i = 1; i < length - 1; i++)
        {
            var sublist = new List<int>();
            // 넣으려는 스테이지 위치가 전체 길이에서 일정 위치 값을 넘은 상태라면 엘리트랑 이벤트가 동시에 배치된다. 
            if (i > eliteAppearPoint)
            {
                isElitePosFlag = true;
            }
            int currentNodeCount = Random.Range(minEventNodeCount, maxEventNodeCount + 1);
            for (int j = 0; j < currentNodeCount; j++)
            {
                // 엘리트가 배치되었다는 플래그를 얻었으니 이벤트를 배치한다. 
                if (isElitePosFlag == true)
                {
                    // 상점 이벤트 배치 확률 
                    // 데이터를 저장하는데 있어서 편리하도록 StageType에 상점에 대한 enum값으로 바로 대입 
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
                // 몬스터 스테이지로 결정 
                else
                {
                    sublist.Add((int)StageType.BATTLE);
                }

            }

            // 다시 플래그 원 위치
            isElitePosFlag = false;
            stageLocateList.Add(sublist);
        }

        // 모든 루프를 돌았으니 마지막은 고정값을 넣어준다. 
        stageLocateList.Add(end);

        return;
    }

    // StageTableClass 리스트 값을 받으면 스테이지 타입에 따른 몬스터나 이벤트를 배치시킨다.
    void SetStageTableClassListByStateTypeData(ref List<StageTableClass> list)
    {
        if (list == null || list.Count <= 0) return;

        // 스테이지 타입 검사 
        foreach (StageTableClass tableClass in list)
        {
            if (tableClass == null || tableClass.eventInfoList == null) continue;

            foreach (var eventInfo in tableClass.eventInfoList)
            {
                if (eventInfo == null) continue;

                // 스테이지 타입이 전투 타입일 경우 
                if (eventInfo.stageType == StageType.BATTLE)
                {
                    // 등장할 몬스터 ID 리스트 만들기
                    // todo 게임 레벨 변수를 전달해야한다.
                    MonsterDatabase.instance.GetMonsterIDListFromTargetStage(
                        currentChapter, (int)1,
                        eventInfo);
                }
                // 이벤트 발생 타입일 경우 
                else if (eventInfo.stageType == StageType.EVENT)
                {
                    // 메인 이벤트 값 설정 
                    int randomCategory = Random.Range(0, (int)EventCategory.MAX);
                    // todo 일단 개발 테스트용으로 무조건 레코드 나오게 
                    randomCategory = (int)EventCategory.FIND_RECORD;
                    eventInfo.mainEventCategory = (EventCategory)randomCategory;
                    
                    // 1. 레코드가 등장하는 이벤트 
                    if (eventInfo.mainEventCategory == EventCategory.FIND_RECORD)
                    {
                        // 여기서 등장 시킬 레코드 생성
                        if (RecordManager.instance != null)
                        {
                            RecordManager.instance.GetStageEventRewardRecord(eventInfo);
                        }
                    }
                    // 2. 유물(= 아티팩트)가 나타나는 이벤트
                    else
                    {

                    }
                }
                // 상점이 나타나는 이벤트 
                else if(eventInfo.stageType == StageType.SHOP)
                {
                    // todo. 상점에 나올 아이템 랜덤하게 나오도록 로직 필요
                }

            }
        }
    }

    // 리스트를 받으면 몬스터 부분만 다시 등급을 나눈다 (보스몬스터는 제외) 
    void SetMonsterStageGrade(ref List<StageTableClass> list)
    {
        if (list == null) return;

        // 스테이지의 최종 길이를 계산해서 비율을 책정한다.
        // 엘리트만 나오는 스테이지가 있을 수 있지만 이게 연속적으로 등장하면 안된다. 
        // 5개의 스테이지가잇으면 1개는 엘리트가 포함되어 있다. 
        int stageCount = list.Count;
        // 엘리트 몬스터 수 
        int maxEliteMonsterCount = 1;
        // 총 스테이지 노드 개수를 나눈다. 
        int resultDivineStageCount = stageCount / 2;

        if (playLevelPair.Count() < 0)
            return;

        float value = 0;
        // 그 값이 0보다 크다면 엘리트 생성 개수에 추가할지 말지 결정한다. 난이도에 따라 확률이 다르다. 
        if (gameLevel == GamePlayLevel.NORMAL)
        {
            // NORAML 확률
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
            // 확률에 당첨되면 갯수 추가 
            maxEliteMonsterCount += resultDivineStageCount;
        }

        int currentElitLocateCount = 0;
        bool possibleElite = false; // 엘리트 배치가능한지 체크 
        // tableclass 리스트를 돌면서 값을 할당한다. 
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

            // 리스트 내부에 정보에서 monstertype것만 건든다. 
            foreach (var eventInfo in stage.eventInfoList)
            {
                if (eventInfo == null || eventInfo.appearInfo == null)
                {
                    continue;
                }

                // 전투 타입이 아니면 안된다.
                if (eventInfo.stageType != StageType.BATTLE)
                {
                    continue;
                }

                // 보스 스테이지 정보를 세팅 중인 상태
                if (isBossStage == true)
                {
                    eventInfo.appearInfo.monsterGrade = MonsterGrade.BOSS;
                    continue;
                }

                // 엘리트는 무조건 하나는 배치 
                if (possibleElite == true &&
                    currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    eventInfo.appearInfo.monsterGrade = MonsterGrade.ELITE;
                }
                // 하나 이상 배치 했을 때 
                else if (possibleElite == true &&
                    currentElitLocateCount >= 1 &&
                     currentElitLocateCount < maxEliteMonsterCount)
                {
                    // 몬스터 스테이지면 이 스테이지의 몬스터 등급을 설정한다. 
                    var eliteCount = Random.Range(0, 1.0f);
                    // 8%정도라면 추가로 배치시킨다. 
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

    // 모든 스테이지 잠금 처리 (1스테이지 제외)
    // 특정 스테이지 클리어 여부에 따라 세팅 
    private void LockedAllStage()
    {
        if (stageTables == null) return;

        for (int i = 1; i < stageTables.Count; i++)
        {
            if (stageTables[i] == null) continue;
            stageTables[i].isLocked = true;
        }
    }

    // 스테이지 테이블 생성
    public void CreateStageTableList(int level = 1)
    {
        // 0. 하나의 라인만 생각한다. 한 스테이지 노드에서 이벤트 최대 3개를 출력. 
        // 3개의 이벤트는 몬스터 전투, 특정 사건, 상점 등의 이벤트들로 나뉘어진다. 
        // 스테이지 노드는 최대 5개 (일반), 하드는 7개 

        // todo 저장된게 있는지 검사해야한다. 

        // 저장된게 없다면 데이터를 처음부터 만들어야하므로 플래그도 켜준다. 
        initJoinPlayGameModeFlag = true; 

        // 이미 스테이지들을 만들어놨다면 별도로 만들지않는다. 
        if (stageDictList.ContainsKey(currentChapter) == true && 
            stageDictList[currentChapter] != null && 
            stageDictList[currentChapter].Count > 0)
            return; 

        // 1. 난이도에 따른 최대 스테이지를 생성한다 
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

        // 스테이지 배치형 리스트를 생성 
         SetStagePosOneLine(maxStageCount);
        // 1-1 스테이지 배치형 리스트 만큼 생성하기
        for (int i = 0; i < stageLocateList.Count; i++)
        {
            StageTableClass stageTable = new StageTableClass
            {
                // 스테이지 값 정렬 order 세팅
                tableOrder = i + 1,
                // 2.1 스테이지 이름 설정
                stageName = currentChapter + "-" + i + 1,
                // 2.2 스테이지에 타입 설정 
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

        // 마지막 스테이지는 보스 스테이지로 고정해놓는다.
        if (stageTables.Last() != null)
        {
            stageTables.Last().isBossStage = true;
        }

        // 3. 
        // 몬스터 타입은 별도로 세부화해야한다. 
        SetMonsterStageGrade(ref stageTables);
        // 타입별 정보 세팅 
        SetStageTableClassListByStateTypeData(ref stageTables);

        // 4. 스테이지 잠그기
        LockedAllStage();

        SetStageList(currentChapter, ref stageTables); 

    }



}
