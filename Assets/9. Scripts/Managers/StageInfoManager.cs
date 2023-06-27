using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


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
    MONSTER = 1,
    EVENT, 
    SHOP, 
    MULTY, 
    MAX = MULTY
};
public enum MonsterSlot
{
    NORMAL = 1, 
    ELITE, 
    BOSS,
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
    STORY = 1,     
    BUFF,           
    DEBUFF, 
    SPECIAL, 
};

[System.Serializable]
public class StageAppearMonsterGroup
{
    //일반 / 엘리트 /  보스 스테이지인지? 
    MonsterSlot monsterSlot; 
    public int wave;
    public int maxWave; // 웨이브 방식이라면 최대 웨이브 값 
    public string stageName;
    public int mapID;   // 어떠한 맵을 그려야하는가 

    // 등장할 몬스터리스트 ID, 등장 수치 
    public List<int> AppearMonsterList = new List<int>();

    public StageAppearMonsterGroup(MonsterSlot monsterSlot = MonsterSlot.NORMAL, int wave = 0, 
        string stageName = "", int mapID = 0)
    {
        this.monsterSlot = monsterSlot;
        this.wave = wave;
        this.maxWave = wave;
        this.stageName = stageName;
        this.mapID = mapID;
    }


    // 매개변수대로 리스트에 몬스터 데이터를 설정한다. 
    public void SetMonsterList(List<int> list)
    {
        AppearMonsterList.Clear();
        // 리스트에 몬스터 정보를 넣어준다 
        AppearMonsterList = list.ToList();
    }


    ~StageAppearMonsterGroup()
    {
        AppearMonsterList.Clear(); 
    }
}

[System.Serializable]
// 스테이지에 들어가는 이벤트 정보가 담긴 클래스 
public class StageEventInfo
{
    public uint stageId;
    // 메인 이벤트 형식  
    public EventCategory mainEventCategory;
    // 서브 이벤트 
    public EventCategory subEventCategory;
    // 뭘 넣지 

    // 스토리 이벤트면 나타날 스토리 컷신 정보 
    // 

    // 몬스터이벤트면 나타날 몬스터 그룹 정보
    public StageAppearMonsterGroup monsterGroup;

    // 몬스터 그룹 생성 
    public void CreateMonsterGroup(MonsterSlot monsterSlot = MonsterSlot.NORMAL, int waveCount = 0,
             string stageName = "", int mapID = 0)
    {
        monsterGroup = new StageAppearMonsterGroup(monsterSlot, waveCount, stageName, mapID);
    }

};

/// <summary>
/// 이 클래스는 스테이지에 배치되는 노드들에 대한 정보를 가진 클래스이다.
/// 이벤트마다 상이하며 몬스터 전투 외에 여러 이벤트들을 배치하여 보여준다 
/// 몬스터 - 몬스터를 배치 시킨다.
/// </summary>
[System.Serializable]
public class StageTableClass
{
    int tableOrder; 

    public StageType stageType;
    public MonsterType monsterType;
    public EventSlot eventSlot;

    // 스테이지에 들어 있는 각종 이벤트 정보들을 담아있는 리스트 
    public List<StageEventInfo> eventInfoList;

    public bool isBossStage;
    public bool isLocked;
    public bool isCleared;

    public StageTableClass()
    {
        Init(); 
    }

    public StageTableClass(int tableOrder, StageType stageType, 
        MonsterType monsterType, EventSlot eventSlot, 
        List<StageEventInfo> eventInfoList,
        bool isBossStage, bool isLocked, bool isCleared)
    {
        this.tableOrder = tableOrder;
        this.stageType = stageType;
        this.monsterType = monsterType;
        this.eventSlot = eventSlot;
        this.eventInfoList = eventInfoList;
        this.isBossStage = isBossStage;
        this.isLocked = isLocked;
        this.isCleared = isCleared;
    }

    // 변수 초기화 
    public void Init()
    {
        tableOrder = 0; 

        stageType = StageType.NONE;
        monsterType = MonsterType.NORMAL;
        eventSlot = EventSlot.NONE;

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

    public string stageName;
    public int currentChapter;      // 진행 중인 챕터 수 
    public int enemyCount;              // 게임 내 적 수 
    public int itemCount;
    public int selectMonsterStageNum;   // 선택한 몬스터 스테이지 번호 
    //public MonsterData monsterData;
   
    [SerializeField] List<StageTableClass> stageTables = null;
    [SerializeField] private StageTableClass selectStage;

    private Dictionary<int, List<StageTableClass>> stage_dic_list = new Dictionary<int, List<StageTableClass>>(); 

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

    public void SetStageList(int _chapter, List<StageTableClass> _stageTableList)
    {
        stage_dic_list[_chapter] = _stageTableList;
    }


    public void SetStageInfo(int _count, int _subStageNumber, int _monsterNum)
    {
        if(stage_dic_list.Count <= 0)
        {
            selectMonsterStageNum = 0; 
            return;
        }

        currentChapter = _count;
        var stageList = stage_dic_list[_count];         

        selectStage = stageList[_subStageNumber];    // 선택한 스테이지 정보    
        selectMonsterStageNum = _monsterNum;    // 서브 스테이지 선택한 정보
    }
    public void SetStageInfo(StageTableClass _stage)
    {
        selectStage = _stage;
    }


    // 선택한 스테이지 반환 
    public StageTableClass GetStageInfo()
    {
        return selectStage;
    }

    public StageTableClass GetStageInfo(uint _id)
    {

        StageTableClass info; 
        for(int i = 0; i < stageTables.Count; i++)
        {
            //if(_id == stageTables[i].stageId)
            //{
            //    info = stageTables[i];
            //    return info;
            //}
        }

        return null;
    }

    // 배치된 스테이지 정보 가져오기 
    public StageTableClass GetLocatedStageInfo(int _count)
    {
        var stageTable = stage_dic_list[currentChapter];
        if(stageTable == null)
        {
            return null; 
        }

        return stageTable[_count];
    }


    public List<StageTableClass> GetLocatedStageInfoList()
    {
        if (stage_dic_list.Count <= 0) return new List<StageTableClass>();
        
        if(stage_dic_list.TryGetValue(currentChapter, out List<StageTableClass> stageTable) == false)
        {
            return null;
        }
        else
        {
            return stageTable;
        }
    }

    // 배치된 스테이지의 마지막 스테이지 반환
    public StageTableClass GetLoacatedStageLastStageInfo()
    {
        var stageList = GetLocatedStageInfoList();
        if(stageList == null || stageList.Count <= 0 )
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



}
