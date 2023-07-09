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
public class StageAppearMonsterGroup
{
    //�Ϲ� / ����Ʈ /  ���� ������������? 
    MonsterType monsterType; 
    public int wave;
    public int maxWave; // ���̺� ����̶�� �ִ� ���̺� �� 
    public string stageName;
    public int mapID;   // ��� ���� �׷����ϴ°� 

    // ������ ���͸���Ʈ ID, ���� ��ġ 
    public List<int> AppearMonsterList = new List<int>();

    public StageAppearMonsterGroup(MonsterType monsterType = MonsterType.NORMAL, int wave = 0, 
        string stageName = "", int mapID = 0)
    {
        this.monsterType = monsterType;
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

    public StageType stageType;
    // ���� �̺�Ʈ ����  
    public EventCategory mainEventCategory;
    // ���� �̺�Ʈ 
    public EventCategory subEventCategory;
    // �� ���� 
    public MonsterType monsterType;
    public EventSlot eventSlot;
    // ���丮 �̺�Ʈ�� ��Ÿ�� ���丮 �ƽ� ���� 
    // 

    // �����̺�Ʈ�� ��Ÿ�� ���� �׷� ����
    public StageAppearMonsterGroup monsterGroup;

    // ���� �׷� ���� 
    public void CreateMonsterGroup(MonsterType monsterType = MonsterType.NORMAL, int waveCount = 0,
             string stageName = "", int mapID = 0)
    {
        monsterGroup = new StageAppearMonsterGroup(monsterType, waveCount, stageName, mapID);
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

    public string stageName;
    public int currentChapter;      // ���� ���� é�� �� 
    public int enemyCount;              // ���� �� �� �� 
    public int itemCount;
    public int selectStageEventNum;   // ������ ���� �������� ��ȣ 
    //public MonsterData monsterData;
   
    [SerializeField] List<StageTableClass> stageTables = null;
    [SerializeField] private StageTableClass selectStageTable;

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

        selectStageTable = stageInfo[_selectStageNumber];    // ������ �������� ����    
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


    public List<StageTableClass> GetLocatedStageInfoList()
    {
        if (stageDictList.Count <= 0) return new List<StageTableClass>();
        
        if(stageDictList.TryGetValue(currentChapter, out List<StageTableClass> stageTable) == false)
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
