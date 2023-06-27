using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


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

// �̺�Ʈ ���� 
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
    //�Ϲ� / ����Ʈ /  ���� ������������? 
    MonsterSlot monsterSlot; 
    public int wave;
    public int maxWave; // ���̺� ����̶�� �ִ� ���̺� �� 
    public string stageName;
    public int mapID;   // ��� ���� �׷����ϴ°� 

    // ������ ���͸���Ʈ ID, ���� ��ġ 
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


    // �Ű�������� ����Ʈ�� ���� �����͸� �����Ѵ�. 
    public void SetMonsterList(List<int> list)
    {
        AppearMonsterList.Clear();
        // ����Ʈ�� ���� ������ �־��ش� 
        AppearMonsterList = list.ToList();
    }


    ~StageAppearMonsterGroup()
    {
        AppearMonsterList.Clear(); 
    }
}

[System.Serializable]
// ���������� ���� �̺�Ʈ ������ ��� Ŭ���� 
public class StageEventInfo
{
    public uint stageId;
    // ���� �̺�Ʈ ����  
    public EventCategory mainEventCategory;
    // ���� �̺�Ʈ 
    public EventCategory subEventCategory;
    // �� ���� 

    // ���丮 �̺�Ʈ�� ��Ÿ�� ���丮 �ƽ� ���� 
    // 

    // �����̺�Ʈ�� ��Ÿ�� ���� �׷� ����
    public StageAppearMonsterGroup monsterGroup;

    // ���� �׷� ���� 
    public void CreateMonsterGroup(MonsterSlot monsterSlot = MonsterSlot.NORMAL, int waveCount = 0,
             string stageName = "", int mapID = 0)
    {
        monsterGroup = new StageAppearMonsterGroup(monsterSlot, waveCount, stageName, mapID);
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
    int tableOrder; 

    public StageType stageType;
    public MonsterType monsterType;
    public EventSlot eventSlot;

    // ���������� ��� �ִ� ���� �̺�Ʈ �������� ����ִ� ����Ʈ 
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

    // ���� �ʱ�ȭ 
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


// �������� ����(���� / ��)�� ��� ���� �Ŵ��� 
public class StageInfoManager : MonoBehaviour
{
    public static StageInfoManager instance;

    public string stageName;
    public int currentChapter;      // ���� ���� é�� �� 
    public int enemyCount;              // ���� �� �� �� 
    public int itemCount;
    public int selectMonsterStageNum;   // ������ ���� �������� ��ȣ 
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

        selectStage = stageList[_subStageNumber];    // ������ �������� ����    
        selectMonsterStageNum = _monsterNum;    // ���� �������� ������ ����
    }
    public void SetStageInfo(StageTableClass _stage)
    {
        selectStage = _stage;
    }


    // ������ �������� ��ȯ 
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

    // ��ġ�� �������� ���� �������� 
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

    // ��ġ�� ���������� ������ �������� ��ȯ
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
