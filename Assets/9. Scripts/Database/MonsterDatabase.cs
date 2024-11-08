using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


public enum MonsterGrade { NONE= 0, NORMAL = 1, ELITE, BOSS };

// json으로 이루어진 데이터를 가공하는 용도의 클래스 
[System.Serializable]
public class MonsterJson
{
    public uint monsterID;                 // 고유 식별 id
    public int monsterGrade;
    public string monsterName;
    public string monsterImage;
    public string monsterPrefabName;       // 프리팹 위치 경로값에 쓰일 이름 
    public int statID;  
}

// MonsterJson 클래스를 리스트로 담고 있는 클래스
[System.Serializable]
public class MonsterJsonAllData
{
    public MonsterJson[] monsterJsonData;
}

// 인게임에 보여줄 오브젝트나 UI를 저장하는 클래스 
[System.Serializable]
public class MonsterData
{
    public uint monsterID;                 // 고유 식별 id
    public int statID;                      // stat 정보 id 
    public MonsterGrade monsterGrade;
    public string monsterName;
    public Sprite monsterSprite;
    public string prefabPath;       // 프리팹 위치 경로값
    public GameObject monsterPrefab;
}

// 스테이지 json 파일 관리 
[System.Serializable]
public class StageMonsterJsonAllData
{
   public StageMonsterJson[] stageMonsterJsons;
}

[System.Serializable]
public class StageMonsterJson
{
    public int id;
    public int chapter;
    public string monsterGroup;
    public int monsterGrade;
    public int mapID;
}

[System.Serializable]
public class StageInfo
{
    public int id;
    public int chapter;
    public List<int> monsterGroup; 
    public int monsterGrade;
    public int mapID;
    public int wave;
}

[System.Serializable]
public class CharacterStatJson
{
    public int id;
    public string memo;
    public int hp;
    public int mp;
    public int attack;
    public int defense;
    public int speed;
    public int attackSpeed;
    public int hpRegen;
    public int mpRegen;
    public float critRate;
    public float critDmg;
}

[System.Serializable]
public class CharacterStatJsonAllData
{
    public CharacterStatJson[] characterStatJson;
}

[System.Serializable]
public class MonsterStatJson
{
    public int statID;
    public int monsterID;
    public int hp;
    public int attack;
    public int defense;
    public float speed;
    public int monsterGrade; 
}

[System.Serializable]
public class MonsterStatJsonAllData
{
    public MonsterStatJson[] monsterStatJson;
}



public class MonsterDatabase : MonoBehaviour
{
    public static MonsterDatabase instance;

 

    [Header("일반몬스터")]
    public List<MonsterData> data_Normals;

    public Dictionary<int, CharStat> monsterStatDic = new Dictionary<int, CharStat>();

    [Header("앨리트 몬스터")]
    public List<MonsterData> data_Elites;

    [Header("보스 몬스터")]
    public List<MonsterData> data_Bosses;

    [Header("몬스터 정보")]
    public List<MonsterData> data_Monsters;

    [Header("몬스터 등급별 추가 스탯")]
    public List<PlayerData> data_MonsterStat;

    [Header("스테이지별 정보")]
    public List<StageInfo> stageInfoList;

    [Header("스테이지 일반 몬스터 정보")]
    public List<StageInfo> stageNormalInfoList;
    [Header("스테이지 엘리트 몬스터 정보")]
    public List<StageInfo> stageEliteInfoList;
    [Header("스테이지 보스 몬스터 정보")]
    public List<StageInfo> stageBossInfoList;

    [Header("몬스터 능력치 JSON 데이터")]
    public TextAsset monsterStatJsonData;

    [Header("몬스터 JSON 데이터")]
    public TextAsset monsterJsonData;

    [Header("챕터 스테이지 JSON 데이터")]
    public TextAsset stageJsonData;

    private MonsterJsonAllData allData; // 몬스터 정보 
    private MonsterStatJsonAllData monsterStatJsonAllData;  // 몬스터 스탯 정보 
    private StageMonsterJsonAllData stageAllData; // 스테이지 정보 


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // 몬스터 json데이터를 일반 클래스로 변환하는작업 
        InitializeConverJsonToMonsterStatData(); 

        InitializeConvertJsonToMonsterData();

        InitializeStageDataFromJson();

     

        DontDestroyOnLoad(this.gameObject);
    }

    void InitializeConverJsonToMonsterStatData()
    {
        monsterStatJsonAllData = JsonUtility.FromJson<MonsterStatJsonAllData>(monsterStatJsonData.text);
        if (monsterStatJsonAllData == null) return;

        foreach(var data in monsterStatJsonAllData.monsterStatJson)
        {
            if (data == null) continue;

            CharStat stat = new CharStat();
            stat.hp = data.hp;
            stat.attack = data.attack;
            stat.defense =  data.defense;
            stat.speed = (int)data.speed;
            stat.myGrade = (MonsterGrade)data.monsterGrade;

            monsterStatDic.Add(data.monsterID, stat);
        }
    }

    // 몬스터 json데이터를 일반 클래스로 변환하는작업 
    void InitializeConvertJsonToMonsterData()
    {
        allData = JsonUtility.FromJson<MonsterJsonAllData>(monsterJsonData.text);
        if (allData == null) return;

        data_Monsters = new List<MonsterData>();
        // json 데이터를 일반 클래스에 대입한다. 
        foreach (var data in allData.monsterJsonData)
        {
            // 클래스 생성 
            MonsterData monsterData = new MonsterData
            {
                monsterID = data.monsterID,
                statID = data.statID,
                monsterName = data.monsterName,
                monsterGrade = (MonsterGrade)data.monsterGrade
            };

            string imagePath = "image/monster/" + data.monsterImage;
            monsterData.monsterSprite = Resources.Load<Sprite>(imagePath);
            string objectPath = "prefabs/monster/" + data.monsterPrefabName;
            monsterData.monsterPrefab = Resources.Load<GameObject>(objectPath);

            // 데이터 넣어주기 
            data_Monsters.Add(monsterData);
        }
    }

    // 스테이지 관련
    // 챕터별 몬스터 json 정보 세팅
    private void InitializeStageDataFromJson()
    {
        stageAllData = JsonUtility.FromJson<StageMonsterJsonAllData>(stageJsonData.text);
        if (stageAllData == null) return;

        stageInfoList = new List<StageInfo>();

        stageNormalInfoList = new List<StageInfo>();
        stageEliteInfoList = new List<StageInfo>();
        stageBossInfoList = new List<StageInfo>();

        if (stageAllData.stageMonsterJsons == null) return;

        foreach (var data in stageAllData.stageMonsterJsons)
        {
            // 클래스 생성 
            StageInfo stageInfo = new StageInfo();

            stageInfo.id = data.id;
            stageInfo.chapter = data.chapter;
            stageInfo.mapID = data.mapID;

            stageInfo.monsterGroup = new List<int>();
            // 몬스터 그룹 
            string[] stringGroup = data.monsterGroup.Split(',');
            for (int i = 0; i < stringGroup.Length; i++)
            {
                //print(stringGroup[i]);
                // 대상 int로 변경하기
                if (int.TryParse(stringGroup[i], out int target))
                {
                    stageInfo.monsterGroup.Add(target);
                }
            }


            // 스테이지 등급 
            stageInfo.monsterGrade = data.monsterGrade;

            // 데이터 넣어주기 
            stageInfoList.Add(stageInfo);

            // 몬스터 등급별로 리스트 분리 
            if (data.monsterGrade == (int)MonsterGrade.NORMAL)
            {
                stageNormalInfoList.Add(stageInfo);
            }
            else if (data.monsterGrade == (int)MonsterGrade.ELITE)
            {
                stageEliteInfoList.Add(stageInfo);
            }
            else if (data.monsterGrade == (int)MonsterGrade.BOSS)
            {
                stageBossInfoList.Add(stageInfo);
            }
        }
    }

   


    public int[] getRandomData(int length, int min, int max)
    {
        int[] randArray = new int[length];

        for (int i = 0; i < length; i++)
        {
            randArray[i] = Random.Range(min, max);
            for (int j = 0; j < i; j++)
            {
                if (randArray[i] == randArray[j])
                {
                    i++;
                    break;
                }
            }
        }

        return randArray;
    }

   

    public List<MonsterData> GetMonsterDatas(MonsterGrade grade)
    {
        List<MonsterData> list = new List<MonsterData>();

        foreach (var data in data_Monsters)
        {
            if (data.monsterGrade != grade)
                continue;

            list.Add(data);
        }

        return list;
    }

    public List<MonsterData> GetRandomNormalMData()
    {
        List<MonsterData> randArray = data_Normals;
        List<MonsterData> resultArray = new List<MonsterData>();

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, randArray.Count);
            resultArray.Add(randArray[rand]);
            if (randArray.Count > 1)
                randArray.RemoveAt(rand);
        }

        return resultArray;
    }

    public MonsterData GetMonsterData(uint monsterId)
    {
        MonsterData monster = null;

        foreach (var data in data_Normals)
        {
            if (data.monsterID == monsterId)
            {
                monster = data;
            }
        }

        foreach (var data in data_Elites)
        {
            if (data.monsterID == monsterId)
            {
                monster = data;
            }
        }

        foreach (var data in data_Bosses)
        {
            if (data.monsterID == monsterId)
            {
                monster = data;
            }
        }

        return monster;
    }

    public uint GetRandomMonsterId(MonsterGrade monsterGrade)
    {
        List<MonsterData> randArray;
        int rand;

        switch (monsterGrade)
        {
            case MonsterGrade.NORMAL:
                randArray = data_Normals;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
            case MonsterGrade.ELITE:
                randArray = data_Elites;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
            case MonsterGrade.BOSS:
                randArray = data_Bosses;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
        }



        return 0;
    }

    public MonsterData GetRandomMonster(MonsterGrade monsterGrade)
    {
        List<MonsterData> randArray;
        int rand;

        switch (monsterGrade)
        {
            case MonsterGrade.NORMAL:
                randArray = data_Normals;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
            case MonsterGrade.ELITE:
                randArray = data_Elites;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
            case MonsterGrade.BOSS:
                randArray = data_Bosses;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
        }

        return null;
    }

    // id값을 받으면 해당하는 id의 몬스터를 만들어서 전달한다. 
    // 반환하는건 게임오브젝트지만 prefab을 기반으로 구성된 리스트에서 일정 클래스에 데이터를 미리 만들어줘서 전달함
    public GameObject CreateMonsterUnit(int id, MonsterGrade grade = MonsterGrade.NORMAL)
    {
        if (monsterStatDic == null)
            return null;

        foreach (var data in data_Monsters)
        {
            if (data.monsterID != id)
                continue;

            // 프리팹 오브젝트 만들기
            // 오브젝트를 생성하고 그 오브젝트의 
            var monsterObject = Instantiate(data.monsterPrefab);
            if (monsterObject == null) continue;

            // 모든 처리를 다하기 전까진 비활성화
            monsterObject.SetActive(false);

            // id에 맞는 스탯 데이터 가져오기 1
            if (monsterStatDic.TryGetValue((int)data.monsterID, out CharStat value))
            {
                if (monsterObject.TryGetComponent(out WheelerController wheeler))
                {
                    if (wheeler.MyPlayer == null)
                    {
                        Character player = new Character();
                        // wheeler.. 딕셔너리에 character를 저장해서 넣을까..
                        wheeler.MyPlayer = player;
                    }

                    wheeler.MyPlayer.MyStat = value.Clone();
                    wheeler.MyPlayer.InitCurrentHP();
                    wheeler.MyPlayer.InitCurrentMP();
                    wheeler.MyPlayer.InitCurrentCP();
                    wheeler.InitPattren();
                    // 처리를 다했으니 켜준다. 
                    monsterObject.SetActive(true);
                    return monsterObject;
                }
            }
        }

        return null;
    }

    // todo 임시로 정해진 스테이터스클래스를 반환한다. 이후에 데이터를 조작해서 해당 값을 보내도록
    public PlayerData GetMonsterStatus(int id)
    {
        PlayerData result;

        int count = 0;
        foreach (var data in data_MonsterStat)
        {
            if (count >= data_MonsterStat.Count) break;

            if (data == null) continue;


            if (id == count)
            {
                result = data;
                return result;
            }

            count++;

        }

        return null;
    }


 
    // 챕터 번호와 등급을 받으면 랜덤한 스테이지 ID를 반환
    public int GetRandomStageIDFromChapterAndGrade(int chapter, int grade = 1, MonsterGrade monsterGrade = MonsterGrade.NONE)
    {
        if (stageInfoList == null) return 0;

        List<StageInfo> targetList = new List<StageInfo>();
        foreach (var data in stageInfoList)
        {
            if (data == null) continue;
            // chapter 검사 
            if (data.chapter != chapter)
                continue;


            // 해당 등급에 맞는 리스트에서 해당 값을 가져온다. 
            if (monsterGrade == MonsterGrade.NORMAL)
            {
                targetList = stageNormalInfoList;
            }
            else if (monsterGrade == MonsterGrade.ELITE)
            {
                targetList = stageEliteInfoList;
            }
            else if (monsterGrade == MonsterGrade.BOSS)
            {
                targetList = stageBossInfoList;
            }

            if (targetList.Count <= 0)
                return 0;

            var findList = targetList.FindAll(x => x.chapter == data.chapter);

            int min = findList.Min(p => p.id);
            int max = findList.Max(p => p.id);

            int random = Random.Range(min, max + 1);

            foreach (var item in findList)
            {
                if (random != item.id)
                    continue;

                return item.id;
            }

        }


        return 0;
    }

    // chapter와 등급 난이도를 받으면 리스트에 해당 몬스터 ID를 넣어 반환 
    public void GetMonsterIDListFromTargetStage(int chapter, int gameLevel, StageAppearInfo appearInfo)
    {
        if (appearInfo == null) return;

        // 나타날 정보 생성 
        //eventInfo.CreateAppearInfo();

        if (appearInfo == null) return; 
        var targetList = appearInfo.appearIDList;

        // 스테이지 ID를 가져온다. 
        var stageID = GetRandomStageIDFromChapterAndGrade(chapter, gameLevel, 
            appearInfo.monsterGrade);

        if (stageID <= 0 || stageAllData == null ||
            stageAllData.stageMonsterJsons == null) return;

        foreach (var data in stageInfoList)
        {
            if (data == null || data.id != stageID) continue;

            // todo 보스 몬스터 정보로 안들어오니 확인해야한다.
            // 해당 데이터에 있는 몬스터 ID 리스트를 순회하면서 정보를 넣어 전달 
            foreach (var monsterID in data.monsterGroup)
            {
                targetList.Add(monsterID);
            }
            // 등장하는 map id 연결 
            appearInfo.mapID = data.mapID;
            // 게임에 진행할 wave 수 설정
            if (data.wave == 0)
            {
                appearInfo.wave = 1;
            }
            else
            {
                appearInfo.wave = data.wave;
            }
            return;
        }
    }

    // chapter와 등급 난이도 값을 받으면 해당되는 몬스터 ID 값을 반환 
    public int GetMonsterIDByChapterAndGameLevel(int chapter, int gameLvel)
    {
        int resultID = 0;



        return resultID; 
    }


    
}
