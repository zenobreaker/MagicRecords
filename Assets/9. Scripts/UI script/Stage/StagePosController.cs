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

    [SerializeField] GameObject go_MonsterMenu = null;

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
    //[SerializeField] Image[] img_StageIcon = null;

    [SerializeField] StageSelectSlot[] btn_MonsterSlots = null;

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

    public int cur_MainChpaterNum;      // 메인 챕터 번호 
    public int selectMonsterNum;        // 선택한 몬스터 스테이지 번호 
    int cur_SelectStageNum;             // 현재 선택한 스테이지

    int cur_Population;                 // 선택한 몬스터 스테이지의 몬스터 수
                                        // 엘리트가 등장할 수 있는 최솟값 
    int eliteAppearPoint;
    bool isSetted;                      // 스테이지 세팅 확인 

    string[] forestFields = { "Forest1", "Forest2", "Forest3" };
    string[] iceFields = { "Forest1", "Forest2", "Forest3" };
    string[] desertFields = { "Forest1", "Forest2", "Forest3" };

    private void Start()
    {
        //RandomLocateStage();
        //SetMonsterImagetoStage();
        img_SelectFrame.gameObject.SetActive(false);

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
        cur_MainChpaterNum = 1;
        StageInfoManager.instance.currentChapter = cur_MainChpaterNum;
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
        // tableclass 리스트를 돌면서 값을 할당한다. 
        for(int i = 0 ; i < list.Count; i++)
        {
            var stage = list[i];
            if (stage == null) continue;

            if (i < eliteAppearPoint ||
                stage.eventInfoList == null)
                continue;

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
                if (currentElitLocateCount < maxEliteMonsterCount &&
                    maxEliteMonsterCount >= 1)
                {
                    currentElitLocateCount++;

                    eventInfo.monsterType = MonsterType.ELITE;
                }
                // 하나 이상 배치 했을 때 
                else if (currentElitLocateCount >= 1 &&
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
            }

        }
    }

    // 길이를 알려주면 그 사이 값을 일정한 비율로 채우는 함수 
    List<List<int>> SetStagePosOneLine(int length, float rate = 1.0f)
    {
        List<List<int>> list = new List<List<int>>();
        // 시작과 끝은 고정이다. 
        List<int> start = new List<int>(1) { 1, 1, 1};
        List<int> end = new List<int>(1) { 1 };
        list.Add(start);

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
            
            list.Add(sublist);
        }

        // 모든 루프를 돌았으니 마지막은 고정값을 넣어준다. 
        list.Add(end);

        return list;
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
                    monsterDB.GetMonsterIDListFromTargetStage(cur_MainChpaterNum, (int)1,
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
            //    monsterDB.GetMonsterIDListFromTargetStage(cur_MainChpaterNum, (int)1,
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
        List<List<int>> stageLocateList = SetStagePosOneLine(maxStageCount); 
        // 1-1 스테이지 배치형 리스트 만큼 생성하기
        for(int i = 0; i < stageLocateList.Count; i++)
        {
            StageTableClass stageTable = new StageTableClass
            {
                // 스테이지 값 정렬 order 세팅
                tableOrder = i + 1,
                // 2.1 스테이지 이름 설정
                stageName = cur_MainChpaterNum + "-" + i + 1,
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
    

        // 4. 스테이지 UI 기능 잠그기
        LockedAllStage();

        // 4. 만들어진 정보대로 스테이지 그림 그리기
        DrawStageIcon();

    }


    public void CreateStage()
    {
        // 보스 스테이지를 클리어 했다면 챕터 번호를 증가시켜서 스테이지 새로 받아내야한다. 
        var clearedCurrentChapter = CheckCurrentChapterBossStageClear(); 

        // 챕터 텍스트 변경 
        SetChpaterText();
        if(clearedCurrentChapter == true)
        {
            cur_MainChpaterNum++; // 챕터 증가 
            if (cur_MainChpaterNum < spt_StageBg.Length)
            {
                img_StageTheme.sprite = spt_StageBg[cur_MainChpaterNum - 1];
            }

            // 2022 08 14 스테이지 리스트 구조를 변경했으므로 굳이 지난 챕터 리스트는 지우지 않는다.
            // 보스스테이지를 클리어 했으므로 스테이지 리스트를 전부 초기화시킨다.
            //StageInfoManager.instance.GetLocatedStageInfoList().Clear();
            Debug.Log("신규 챕터로 배치합니다.====");
            StageInfoManager.instance.currentChapter = cur_MainChpaterNum;
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
        // 모든 스테이지 잠금 처리 (1스테이지 제외)
        LockedAllStage();
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
            switch (cur_MainChpaterNum)
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
            //stageTable.stageId = (uint)(stageId + cur_MainChpaterNum + i);
         

            stageTables.Add(stageTable);
        }
        isSetted = false;
        StageInfoManager.instance.
            SetStageList(cur_MainChpaterNum, stageTables);
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

    // 특정 랜덤 이벤트 스테이지 배치 
    void RandomLocateEventStage(int _slotNum)
    {
        int _ran = UnityEngine.Random.Range(0, 10);

        switch (_ran)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                break;
            case 4:
            case 5:
            case 6:
                break;
            case 7:
            case 8:
                break;
            case 9:
                break;

        }
    }

    // 스테이지에 맞는 몬스터 이미지 세팅
    void DrawStageIcon()
    {
        for (int i = 0; i < MAX_STAGE_COUNT; i++)
        {
            for (int j = 0; j < stageTables.Count; j++)
            {
                string stageName = cur_MainChpaterNum.ToString() + "-" + (i+1).ToString();
                btn_MonsterSlots[i].SetSlotText(stageName);
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

        cur_SelectStageNum = stageNum;
        
        // 선택한 스테이지 정보 
        var selectStage = StageInfoManager.instance.GetLocatedStageInfo(stageNum);
        if (selectStage == null) return; 

        // 나타나는 몬스터 아이콘 출력 
        for (int i = 0; i < img_MonsterIcon.Length; i++)
        {
           
            // 몬스터DB에서 몬스터 정보를 가져온다.
            //var data = monsterDB.GetMonsterData(selectStage.monsterGroups[i].
            //    monsterIdCounts[0].Item1);
            //// 가져온 정보에서 몬스터 스프라이트를 그린다. 
            //img_MonsterIcon[i].sprite = data.spt_Monster;
        }

        // 선택자 아이콘 끄기
        img_SelectFrame.gameObject.SetActive(false);

        selectMonsterNum = -1;

        if (UIPageManager.instance != null)
        {
            UIPageManager.instance.OpenClose(go_MonsterMenu);
        }
    } 

    public void CloseMonsterMenu()
    {
        //cur_SelectStageNum = -1;
        UIPageManager.instance.OpenClose(go_MonsterMenu);
    }


    // 버튼 컴포넌트에 첨부됨 
    // 팝업 몬스터 아이콘 선택 
    public void SelectMonsterIcon(int _select)
    {
        if (cur_SelectStageNum == -1)
            return;

        if (selectMonsterNum == _select)
        {
            if (img_SelectFrame.gameObject.activeSelf)
                img_SelectFrame.gameObject.SetActive(false);

            selectMonsterNum = 0;
        }
        else
        {
            selectMonsterNum = _select; 
            if (!img_SelectFrame.gameObject.activeSelf)
                img_SelectFrame.gameObject.SetActive(!img_SelectFrame.gameObject.activeSelf);
        }


        switch (_select)
        {
            case 1:
                img_SelectFrame.transform.position = img_MonsterIcon[0].transform.position;
                break;
            case 2:
                img_SelectFrame.transform.position = img_MonsterIcon[1].transform.position;
                break;
            case 3:
                img_SelectFrame.transform.position = img_MonsterIcon[2].transform.position;
                break;
        }
    }

    // 스테이지 팝업에서 선택 후 입장 버튼 기능 
    // 스테이지 입장
    public void GototheStage()
    {
        if ( 0 < selectMonsterNum)
        {
            CloseMonsterMenu();
            //LobbyManager.MyInstance.HideLobbyUI();
            //LobbyManager.MyInstance.SceneChange();
            //GameManager.MyInstance.stageName = _stageName;
            choiceAlert.ActiveAlert(true);
            choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
            choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
        }
    }

    // 캐릭터 세팅
    void SetStageCharacters(Character selectPlayer)
    {
        // 선택한 캐릭터 정보 저장 
        List<int> idList = new List<int>
                    {
                        selectPlayer.MyID
                    };
        InfoManager.instance.SetSelectPlayers(idList.ToArray());

        if (InfoManager.instance.GetSelectPlayerList().Count > 0)
        {
            //씬 변경
            LoadingSceneController.LoadScene("GameScene");
            
            Debug.Log("챕터 " + cur_MainChpaterNum +
                "스테이지 번호 : " + cur_SelectStageNum + ", 타입 번호 : " + (selectMonsterNum));

            StageInfoManager.instance.SetStageInfo(cur_MainChpaterNum, cur_SelectStageNum, selectMonsterNum);

            Debug.Log("적 숫자 : - " + cur_Population);
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
        txt_Chpater.text = "Stage " + cur_MainChpaterNum;
    }

    // 클리어한 스테이지 잠금 
    public void LockedClearStage()
    {
        var stageTableList = StageInfoManager.instance.GetLocatedStageInfoList();
        if (stageTableList.Count <= 0) return;

        btn_MonsterSlots[cur_SelectStageNum].SetButtonInteractive(false);
         //img_StageIcon[cur_SelectStageNum].GetComponent<Button>().interactable = false;

        // 다음 스테이지 해금 
        if (cur_SelectStageNum + 1 < stageTableList.Count)
        {
            //img_StageIcon[cur_SelectStageNum + 1].GetComponent<Button>().interactable = true;
            btn_MonsterSlots[cur_SelectStageNum+1].SetButtonInteractive(true);
        }

    }

    // 모든 스테이지 잠금 처리 (1스테이지 제외)
    // 특정 스테이지 클리어 여부에 따라 세팅 
    public void LockedAllStage()
    {
      
        //img_StageIcon[0].GetComponent<Button>().interactable = true;
        btn_MonsterSlots[0].SetButtonInteractive(true);

        for (int i = 0; i < stageTables.Count; i++)
        {
            if (btn_MonsterSlots[i] == null)
                continue;

            // 해당 스테이지가 클리어 상태인가 
            if(stageTables[i].isCleared == true)
            {
                if (i + 1 < stageTables.Count)
                {
                    stageTables[i + 1].isLocked = false;
                }
            }

            if (stageTables[i].isLocked == true)
            {
                btn_MonsterSlots[i].SetButtonInteractive(false);
            }
            else 
            {
                btn_MonsterSlots[i].SetButtonInteractive(true);
            }

            // 이벤트 추가 람ㄷ다식은 for문에서 해당 반복변수로 인자를 줄 때 같은 변수를 주어서 그 참조값을 통해 전달하기때문에
            // 변수주소가 다른걸 줘야한다.
            var temp = i; 
            //if (img_StageIcon[i].GetComponent<Button>().onClick.GetPersistentEventCount() == 0)
            if (btn_MonsterSlots[i].GetButtonEventCount()  == 0)
            {
                btn_MonsterSlots[i].SetButtonOnlyOneEventLisetener(()=>OpenMonsterStageMenu(temp));
                //img_StageIcon[i].GetComponent<Button>().onClick.RemoveAllListeners();
                //img_StageIcon[i].GetComponent<Button>().onClick.AddListener(
                //    () => OpenMonsterStageMenu(temp));
            }

        }
    }



    // 다음 챕터로 변경 
    public void SetNextChapter()
    {
        if (cur_MainChpaterNum < finalChapterNum)
        {
            img_StageTheme.sprite = spt_StageBg[cur_MainChpaterNum];
            CreateStage();     // 스테이지 재정렬
        }
        else if (cur_MainChpaterNum >= finalChapterNum)
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
        Debug.Log("보스 스테이지" + stageTables[cur_SelectStageNum].isBossStage);
        return stageTables[cur_SelectStageNum].isBossStage;
    }
}
