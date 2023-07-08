using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;


public class StagePosController : MonoBehaviour
{
    // 최대 스테이지 수 
    public static readonly int LEVEL_NORMAL_MAX_STAGE_COUNT = 5;
    public static readonly int LEVEL_HARD_MAX_STAGE_COUNT = 7;
    public const int MAX_STAGE_COUNT = 5;
    public const int MAX_STAGE_SELECT_COUNT = 3;     // 각 스테이지별 고를 수 있는 하위 스테이지 수

    [SerializeField] MonsterDatabase monsterDB = null;

    [SerializeField] StageMenuSelectUI stageMenuUI = null;

    [Header("몬스터 스테이지 테마 이미지")]
    [SerializeField] Image img_StageTheme = null;  // 배경 이미지 
    [Header("몬스터 스테이지 테마 이미지 모음")]
    [SerializeField] Sprite[] spt_StageBg = null;   // 배경 스프라이트

    [Header("몬스터 스테이지 아이콘")]
    [SerializeField] Sprite[] spt_stageMIcon = null;

    [Header("몬스터 이미지")]
    [SerializeField] Image[] img_MonsterIcon = null;

    [Header("스테이지")]
    [SerializeField] Text txt_Chpater = null;

    [SerializeField] GameObject scrollview = null;
    [SerializeField] GameObject contentObject = null; 
    [SerializeField] StageSelectSlot stageSlot = null; 

    [SerializeField] ChoiceAlert choiceAlert = null;
    [SerializeField] GameObject go_Alert;
    [SerializeField] Image img_SelectFrame = null;  // 선택자 

    public string[] monsterNames;               // 몬스터 
    public MonsterData[,] monsterDatas = new MonsterData[5, 3];
    public int[,] monsterPopulation = new int[5, 3];

    [SerializeField] List<StageTableClass> stageTables = null;
    [SerializeField] GamePlayLevel gameLevel = 0; // 게임 난이도
    [SerializeField] public Dictionary<GamePlayLevel, float> playLevelPair  = new Dictionary<GamePlayLevel, float>();
    // 변수 
    public MonsterData cur_SelectMD;    // 선택한 몬스터 
    public int maxStageNum;             // 스테이지 최대 수
    public int finalChapterNum;         // 마지막 챕터 번호

    public int curMainChpaterNum;      // 메인 챕터 번호 
    public int selectEventSlotNumber;       // 선택한 이벤트 ID
    int curSelectStageNum;             // 현재 선택한 스테이지

    int eliteAppearPoint;   // 엘리트가 등장할 수 있는 최솟값 
    bool isSetted;                      // 스테이지 세팅 확인 

    List<List<int>> stageLocateList;       // 스테이지 배치 정보를 가진 리스트 

    string[] forestFields = { "Forest1", "Forest2", "Forest3" };
    string[] iceFields = { "Forest1", "Forest2", "Forest3" };
    string[] desertFields = { "Forest1", "Forest2", "Forest3" };

    private void Start()
    {
        if(playLevelPair.Count <= 0)
        {
            // 노말 20퍼 
            playLevelPair.Add(GamePlayLevel.NORMAL, 0.2f);

            // 하드 40퍼
            playLevelPair.Add(GamePlayLevel.HARD, 0.4f);

            // 특수 30퍼
            playLevelPair.Add(GamePlayLevel.SPECIAL, 0.3f);
        }


        // 저장한게 없다면 챕터는 1로 초기화시켜놓는다
        curMainChpaterNum = 1;
        StageInfoManager.instance.currentChapter = curMainChpaterNum;

    }

    // 현재 진행중인 챕터의 (마지막) 보스 스테이지를 클리어 했는지 검사
    public bool CheckCurrentChapterBossStageClear()
    {
        var lastStage = StageInfoManager.instance.GetLoacatedStageLastStageInfo();
        if (lastStage == null) return false; 

        if (lastStage.isBossStage == true)
        {
            if(lastStage.isCleared == true)
            {
                return true; 
            }
        }

        return false; 
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
        for(int i = 0 ; i < list.Count; i++)
        {
            var stage = list[i];
            if (stage == null) continue;

            if (i >= eliteAppearPoint)
                possibleElite = true;

            bool isBossStage = false;
            if(stage.isBossStage == true)
            {
                isBossStage = true;
            }

            // 리스트 내부에 정보에서 monstertype것만 건든다. 
            foreach (var eventInfo in stage.eventInfoList)
            {
                if (eventInfo == null) continue;

                // 보스 타입은 제외 
                if (eventInfo.stageType != StageType.MONSTER)
                {
                    continue;
                }

                if(isBossStage == true)
                {
                    eventInfo.monsterType = MonsterType.BOSS;
                    continue; 
                }

                // 엘리트는 무조건 하나는 배치 
                if (possibleElite == true && 
                    currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    eventInfo.monsterType = MonsterType.ELITE;
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
                        eventInfo.monsterType = MonsterType.ELITE;
                    }
                }
                else
                {
                    eventInfo.monsterType = MonsterType.NORMAL;
                }
            }

        }
    }

    // 길이를 알려주면 그 사이 값을 일정한 비율로 채우는 함수 
    void SetStagePosOneLine(int length, float rate = 1.0f)
    {
        // 이미 만들어 졌다면 따로 생성하지 않는다. 
        if(stageLocateList != null)
        {
            return;
        }

        stageLocateList = new List<List<int>>();

        // 시작과 끝은 고정이다. 
        List<int> start = new List<int>(1) { 1, 1, 1};
        List<int> end = new List<int>(1) { 1 };
        stageLocateList.Add(start);

        // 먼저 시작과 끝을 뺀 남은 길이를 계산한다. 
        int diff = length - 2;

        // 남은 길이에 배치할 때 80%로 몬스터 20로 이벤트다 
        // 또 이벤트를 배치할 때도 상점이 중복으로 배치되면 곤란하니 상점만 배치할 땐 독립적으로 적용한다. 
        // 남은 수치만큼 반복해서 대입,

        int maxShopCount = 1;
        int maxEventCount = 1;
        int currentShopCount = 0;
        int currentEventCount = 0;

        // 한 스테이지 노드에 들어갈 이벤트 노드 최대 수는 3개
        int maxEventNodeCount = 3;
        int minEventNodeCount = 1;

        eliteAppearPoint = length / 2; 

        bool isBothPosFlag = false; 
        for (int i = 1; i < length - 1; i++)
        {
            var sublist = new List<int>();
            // 넣으려는 스테이지 위치가 전체 길이에서 일정 위치 값을 넘은 상태라면 엘리트랑 이벤트가 동시에 배치된다. 
            if (i > eliteAppearPoint)
            {
                isBothPosFlag = true; 
            }
            int currentNodeCount = Random.Range(minEventNodeCount, maxEventNodeCount+1);
            for (int j = 0; j < currentNodeCount; j++)
            {
                int randEvent = Random.Range(0, 11);
                // 이벤트 스테이지 당첨
                if (randEvent == 0 || randEvent == 1)
                {
                    bool eventPosFlag = false;
                    // 여기서 한 번 더 결정 지어야한다. 
                    // 이미 상점이 포함된 상태라면 상점을 추가하지 않는다. 
                    int randDecideEvent = Random.Range(0, 2);
                    if (randDecideEvent == 1 && maxShopCount <= currentShopCount)
                    {
                        currentShopCount += 1;
                        sublist.Add(3);
                        eventPosFlag = true; 
                    }
                    else if(maxEventCount <= currentEventCount)
                    {
                        currentEventCount += 1;
                        sublist.Add(2);
                        eventPosFlag = true;
                    }
                    else
                    {
                        sublist.Add(1);
                    }

                    // 이벤트 플래그가 배치되어진다면 엘리트도 배치되게 하기 위한 배치를 넣어준다.
                    if (isBothPosFlag == true && eventPosFlag == true &&
                       currentNodeCount < maxEventNodeCount)
                    {
                        sublist.Add(1);
                    }
                }
                // 몬스터 스테이지로 결정 
                else
                {
                    sublist.Add(1);
                }

            }

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
        foreach(StageTableClass tableClass in list)
        {
            if(tableClass == null || tableClass.eventInfoList == null) continue;


            bool isBossStage = false; 
            // 1. 보스 스테이지 인지 검사 
            if (tableClass.isBossStage == true)
            {
                isBossStage = true; 
            }

            foreach ( var eventInfo in tableClass.eventInfoList )
            {
                if (eventInfo == null) continue;

                // 스테이지 타입이 몬스터 타입일 경우 
                if (eventInfo.stageType == StageType.MONSTER)
                {
                    // 가져온 몬스터 ID값을 정리 
                    // 몬스터 그룹 만들기 
                    eventInfo.CreateMonsterGroup(eventInfo.monsterType, 1, "", 1);
                    // 몬스터 ID리스트 생성 
                    monsterDB.GetMonsterIDListFromTargetStage(curMainChpaterNum, (int)1,
                        ref eventInfo.monsterGroup.AppearMonsterList);
                }

            }


            // 2. 이벤트일 경우
            //if(tableClass.stageType == StageType.EVENT)
            //{

            //}
            //// 3. 몬스터일 경우 
            //else if(tableClass.stageType == StageType.MONSTER)
            //{
            //    if (monsterDB == null) return;

            //    // 가져온 몬스터 ID값을 정리 
            //    tableClass.eventInfoList = new List<StageEventInfo>();


            //    // 몬스터 그룹 만들기 
            //    mainEventInfo.CreateMonsterGroup(MonsterType.NORMAL, 1, "", 1);
            //    // 몬스터 ID리스트 생성 
            //    monsterDB.GetMonsterIDListFromTargetStage(curMainChpaterNum, (int)1,
            //        ref mainEventInfo.monsterGroup.AppearMonsterList);
            //    tableClass.eventInfoList.Add(mainEventInfo);
            //}
            //// 5. 상점일 경우 
            //else if(tableClass.stageType == StageType.SHOP)
            //{

            //}
            //// 5. 혼합형일 경우 
            //else if(tableClass.stageType == StageType.MULTY)
            //{

            //}
        }
    }


    public void UpgradeCreateStage(int level = 1)
    {
        // 0. 하나의 라인만 생각한다. 한 스테이지 노드에서 이벤트 최대 3개를 출력. 
        // 3개의 이벤트는 몬스터 전투, 특정 사건, 상점 등의 이벤트들로 나뉘어진다. 
        // 스테이지 노드는 최대 5개 (일반), 하드는 7개 

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
        for(int i = 0; i < stageLocateList.Count; i++)
        {
            StageTableClass stageTable = new StageTableClass
            {
                // 스테이지 값 정렬 order 세팅
                tableOrder = i + 1,
                // 2.1 스테이지 이름 설정
                stageName = curMainChpaterNum + "-" + i + 1,
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
        // 스테이지 정보 전달 
        StageInfoManager.instance.SetStageList(curMainChpaterNum, ref stageTables);

        // 스크롤뷰 그리기 
        DrawStageButtonByScrollview();
    }


    public void CreateStage()
    {
        // 보스 스테이지를 클리어 했다면 챕터 번호를 증가시켜서 스테이지 새로 받아내야한다. 
        var clearedCurrentChapter = CheckCurrentChapterBossStageClear(); 

        // 챕터 텍스트 변경 
        SetChpaterText();
        if(clearedCurrentChapter == true)
        {
            curMainChpaterNum++; // 챕터 증가 
            if (curMainChpaterNum < spt_StageBg.Length)
            {
                img_StageTheme.sprite = spt_StageBg[curMainChpaterNum - 1];
            }

            // 2022 08 14 스테이지 리스트 구조를 변경했으므로 굳이 지난 챕터 리스트는 지우지 않는다.
            // 보스스테이지를 클리어 했으므로 스테이지 리스트를 전부 초기화시킨다.
            //StageInfoManager.instance.GetLocatedStageInfoList().Clear();
            Debug.Log("신규 챕터로 배치합니다.====");
            StageInfoManager.instance.currentChapter = curMainChpaterNum;
        }

        // 배치한 리스트가 있다면 가져온다. 
        stageTables = StageInfoManager.instance.GetLocatedStageInfoList();
        if (stageTables == null || stageTables.Count <= 0)
        {
            // 랜덤으로 배치시키게 하는 메소드 
            RandomLocateStage();
        }

        // 클리어한 스테이지 잠그기 
        //LockedClearStage();
        // 스테이지에 맞는 몬스터 이미지 세팅
        //SetMonsterImagetoStage();
        img_SelectFrame.gameObject.SetActive(false);

        //StageInfoManager.instance.SetStageList();
    }

    /// <summary>
    /// 세팅된 리스트 데이터를 가져와서 배치한다. 
    /// </summary>
    void SetLocatedStage()
    {

    }

    void RandomLocateStageOver()
    {
        // 1. 총 진행할 큰 스테이지를 만든다. 
        // 1-1 몇갈래로 할까? 
        int branchCount = MAX_STAGE_SELECT_COUNT;
        // 1-2 최대 스테이지 개수 
       
    }

    // 랜덤으로 배치시키게 하는 메소드 
    void RandomLocateStage()
    {
        // 가져온 리스트가 이미 있다면 청소해놓는다 
        if (stageTables != null)
        {
            stageTables.Clear();
        }
        else
        {
            stageTables = new List<StageTableClass>();
        }

        for (int i = 0; i < MAX_STAGE_COUNT; i++)
        {
            int _randSelStage = UnityEngine.Random.Range(0, 8);
            int temp = i; // 문제 때문에 복사해서 사용 


            // TODO : 데이터 변경됨에 따라 아래 로직 변경해야됨
            // 그러네 보스전이 없어졌네..
            StageTableClass stageTable = new StageTableClass();
            //stageTable.stageType= StageType.MONSTER;

            // 랜덤 몬스터 배치 
            RadomLocateMonsterStage(ref stageTable, i);

            // 버튼에 이벤트가 등재되어 있지 않다면 실행 
            //if (img_StageIcon[temp].GetComponent<Button>().onClick.GetPersistentEventCount() == 0)
            //{
            //    img_StageIcon[temp].GetComponent<Button>().onClick.AddListener(() => OpenMonsterStageMenu(temp));
            //    //img_StageIcon[temp].GetComponent<Button>().interactable = true; 임시 테스트용 
            //}


            var stageId = 0;
            var mapId = 0;
            var stageName = "";
            // 테마별로 등장할 필드 설정 
            switch (curMainChpaterNum)
            {
                case 1:
                    // TODO : 맵 id 만드는 기능 필요 
                    stageId = 1000;
                    mapId = 1000;
                    stageName = forestFields[Random.Range(0, forestFields.Length)];
                    break;
                case 2:
                    stageId = 1000;
                    mapId = 1000;
                    stageName = iceFields[Random.Range(0, forestFields.Length)];
                    break;
                case 3:
                    stageId = 1000;
                    mapId = 1000;
                    stageName = desertFields[Random.Range(0, forestFields.Length)];
                    break;
            }

            // 첫 스테이지는 잠금 여부 o 
            if( i == 0 )
            {
                stageTable.isLocked = false;
            }
            else
            {
                stageTable.isLocked = true;
            }

            stageTable.isCleared = false;

            // todo id 만드는 방법을 추후에 연구해야 한다 지금은 챕터 + 스테이지 번호를 더한다.
            // id 설정 
            //stageTable.stageId = (uint)(stageId + curMainChpaterNum + i);
         

            stageTables.Add(stageTable);
        }
        isSetted = false;
        StageInfoManager.instance.
            SetStageList(curMainChpaterNum, ref stageTables);
    }


    // 랜덤 몬스터 배치
    void RadomLocateMonsterStage(ref StageTableClass _stageTableClass, int _slotNum) 
    {
        if(_stageTableClass == null)
        {
            Debug.Log("RadomLocateMonsterStage 데이터 x");
            return;
        }

        _stageTableClass.isBossStage = false; 
        //_stageTableClass.monsterType = MonsterType.NORMAL;

     
        int _ran = UnityEngine.Random.Range(0, 10);
        
        if (_ran == 3)
        {
        //    _stageTableClass.monsterType = MonsterType.ELITE;
        }
        else
        //    _stageTableClass.monsterType = MonsterType.NORMAL;

        // 슬롯이 마지막 슬롯이라면 보스전으로 변경 
        if (_slotNum == 4)
        {
            _stageTableClass.isBossStage = true;
         //   _stageTableClass.monsterType = MonsterType.BOSS;
        }

      //  SetAppearMonster(ref _stageTableClass);
    }

 
    //등장시킬 몬스터 타입 정보 
    void SetAppearMonster(ref StageTableClass stageTable)
    {
        if(stageTable == null)
        {
            Debug.Log("해당 데이터 존재 x");
            return; 
        }
        // TODO 나중에 스테이지 정보에 어떤 몬스터들이 나오는지 몇마리가 나오는지 등 정보가 전달 필요
        // 몬스터 등장 개수 설정
        int monsterCount = 0; 
        //if (stageTable.monsterType == MonsterType.NORMAL)
        //{
        //    monsterCount = Random.Range(1, 6);
        //}
        //else
        //    monsterCount = 1;

        if (stageTable.eventInfoList == null) 
        {
           // stageTable.monsterGroups = new List<StageAppearMonsterGroup>();
        }


        //TODO : 나 몬스터 최대 개수로만 나오게 설정하려 했던가??? 
        // 랜덤값으로 최대 갯수까지 나오게 한다음에 에너ㅌㅣ카운트 변수에 넣던가 하자
        // 데이터 만들 리스트가 생성되지 않았다면 생성해준다. 
        for (int i = 0; i < 3; i++)
        {
            // 몬스터ID / 몬스터 수 
            //stageTable.monsterGroups.Add(new StageAppearMonsterGroup());
            //for (int j = 0; j < monsterCount; j++)
            //{
            //    var monsterId = monsterDB.GetRandomMonsterId(stageTable.monsterType);
            //    uint count = 1;
                
            //    stageTable.monsterGroups[i].monsterIdCounts.Add((monsterId, count));
            //}
        }
    }


    // 경고창 출력
    public void OpenAlert()
    {
        if (isSetted)
        {
            RandomLocateStage();
        }
    }

    // 스테이지에 맵에서 스테이지를 선택하면 뜨는 팝업 
    // 몬스터 스테이지 메뉴 열기 
    public void OpenMonsterStageMenu(int stageNum)
    {
        Debug.Log("스테이지 번호 : " + stageNum);

        curSelectStageNum = stageNum;

        selectEventSlotNumber = -1;

        if (UIPageManager.instance != null && stageMenuUI != null)
        {
            var stageTableInfo = stageTables[stageNum - 1];
            stageMenuUI.DeploySelectEventSlot(ref stageTableInfo);
            UIPageManager.instance.OpenClose(stageMenuUI.gameObject);
        }
    } 

    public void CloseMonsterMenu()
    {
        //cur_SelectStageNum = -1;
        if (stageMenuUI != null)
        {
            UIPageManager.instance.OpenClose(stageMenuUI.gameObject);
        }
    }


    // 스테이지 팝업에서 선택 후 입장 버튼 기능 
    // 진행할 대상 선택
    public void SelectEventStage()
    {
        selectEventSlotNumber = stageMenuUI.GetSelectEvent();

        // 이벤트 선택 메뉴를 닫는다.
        CloseMonsterMenu();
        // 캐릭터 선텍 UI를 연다. 
        choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        // 확인버튼 기능에 기능 할당 
        choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
     
    }

    // 캐릭터 세팅
    void SetStageCharacters(Character selectPlayer)
    {
        // 이녀석이 없으면 실행하지 못한다. 
        if (StageInfoManager.instance == null || InfoManager.instance == null) return; 

        // 선택한 캐릭터 정보 저장 
        List<int> idList = new List<int>
                    {
                        selectPlayer.MyID
                    };
        InfoManager.instance.SetSelectPlayers(idList.ToArray());

        // 스테이지 선택한 정보로 저장
        StageInfoManager.instance.ChoiceStageInfoForPlaying(curMainChpaterNum, curSelectStageNum, selectEventSlotNumber);
        
        // 지정한 스테이지가 있는지 검사 후 씬을 옮긴다. 
        // 선택한 캐릭터가 있으면 씬 옮기기 
        if (InfoManager.instance.GetSelectPlayerList().Count > 0 && 
            StageInfoManager.instance.GetStageInfo() != null)
        {
            
            //씬 변경
            LoadingSceneController.LoadScene("GameScene");
        }
        // 없으면 캐릭터를 고르라고 알림 메세지 출력하기 
        else
        {
            // todo 
        }
    }

    // 연습장 
    public void GototheTestStage()
    {
        //CloseMonsterMenu();
        //GameManager.MyInstance.
        choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        //LobbyManager.MyInstance.JoinTestStage();
    }

    // 챕터 텍스트 변경 
    void SetChpaterText()
    {
        txt_Chpater.text = "Stage " + curMainChpaterNum;
    }


    // 모든 스테이지 잠금 처리 (1스테이지 제외)
    // 특정 스테이지 클리어 여부에 따라 세팅 
    public void LockedAllStage()
    {
        if (stageTables == null) return; 

        for (int i = 1; i < stageTables.Count; i++)
        {
            if (stageTables[i] == null) continue;
            stageTables[i].isLocked = true; 
        }
    }



    // 다음 챕터로 변경 
    public void SetNextChapter()
    {
        if (curMainChpaterNum < finalChapterNum)
        {
            img_StageTheme.sprite = spt_StageBg[curMainChpaterNum];
            CreateStage();     // 스테이지 재정렬
        }
        else if (curMainChpaterNum >= finalChapterNum)
        {
            Debug.Log("이번 챕터가 마지막입니다.");
            RLModeController.instance.EndGameMode(true);

        }
    }

    public string GetStageName()
    {
        return ""; // stageTables[cur_SelectStageNum].stageName;
    }

    public bool GetIsBoss()
    {
        Debug.Log("보스 스테이지" + stageTables[curSelectStageNum].isBossStage);
        return stageTables[curSelectStageNum].isBossStage;
    }



    //#  스크롤뷰 관련 함수들 

    public void DrawStageButtonByScrollview()
    {
        if (stageTables == null || stageSlot == null ||
            contentObject == null)
        {
            return;
        }

        var cotentRect = contentObject.GetComponent<RectTransform>();
        if (cotentRect == null)
        {
            return;
        }

        //  스테이지 만들기 전 검사
        int stageCount = stageTables.Count;
        int slotCount = contentObject.transform.childCount;

        if (slotCount < stageCount)
        {
            for (int i = 0; i < stageCount; i++)
            {
                var slot = Instantiate(stageSlot, contentObject.transform);
            }
        }
        else if (slotCount > stageCount)
        {
            int diff = slotCount - stageCount;
            if (diff > 0)
            {
                for (int i = diff + 1; i < stageCount; i++)
                {
                    var slot = contentObject.transform.GetChild(i);
                    if (slot == null) continue;
                    slot.gameObject.SetActive(false);
                }
            }
        }

        // 스테이지를 배치한다. 
        for(int i = 0; i < stageTables.Count; i++)
        {
            // 스테이지 버튼에 이름 그리기 
            var slot = contentObject.transform.GetChild(i);
            if (!slot.TryGetComponent<StageSelectSlot>(out var stageSelectSlot)) continue; 

            slot.gameObject.SetActive(true);

            string stageName = curMainChpaterNum.ToString() + "-" + (i + 1).ToString();
            stageSelectSlot.SetSlotText(stageName);
            int temp = i + 1; 
            if (stageSelectSlot.GetButtonEventCount() == 0)
            {
                // 슬롯에 기능 할당 
                stageSelectSlot.SetButtonOnlyOneEventLisetener(() => OpenMonsterStageMenu(temp));
            }
        }

        // 스크롤 기능 설정 
        if (scrollview == null) return;

        var scrollRect = scrollview.GetComponent<ScrollRect>();
        if (scrollRect == null) return;

        // 컨텐츠 그룹보다 작으면 스크롤 기능 제거 
        var contentLegnth = cotentRect.sizeDelta.x; 
        if (contentLegnth > Screen.width)
        {
            scrollRect.horizontal = true; 
        }
        else
        {
            scrollRect.horizontal = false;
        }
    }

    // 스테이지에 맞는 몬스터 이미지 세팅
    void DrawStageIcon()
    {
        for (int i = 0; i < MAX_STAGE_COUNT; i++)
        {
            for (int j = 0; j < stageTables.Count; j++)
            {
                string stageName = curMainChpaterNum.ToString() + "-" + (i + 1).ToString();
                //btn_MonsterSlots[i].SetSlotText(stageName);
                // if (stageTables[i].stageType == StageType.MONSTER)
                {
                    //switch (stageTables[i].monsterType)
                    //{

                    //    case MonsterType.NORMAL:
                    //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[0]);
                    //        break;
                    //    case MonsterType.ELITE:
                    //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[1]);
                    //        break;
                    //    case MonsterType.BOSS:
                    //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[2]);
                    //        break;
                    //}
                }
            }

        }
    }
}
