using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;



// 아래의 클래스를 리스트로 담고 있는 클래스
[System.Serializable]
public class MonsterJsonAllData
{
    public MonsterJson[] monsterJsonData; 
}

// json으로 이루어진 데이터를 가공하는 용도의 클래스 
[System.Serializable]
public class MonsterJson
{
    public uint monsterID;                 // 고유 식별 id
    public int monsterType;
    public string monsterName;
    public string monsterImage;
    public string monsterPrefabName;       // 프리팹 위치 경로값
}

[System.Serializable]
public class MonsterData
{
    public uint monsterID;                 // 고유 식별 id
    public MonsterType monsterType;
    public string monsterName;
    public Sprite monsterSprite;
    public string prefabPath;       // 프리팹 위치 경로값
    public GameObject monsterPrefab;
}



public class MonsterDatabase : MonoBehaviour
{
    public static MonsterDatabase instance; 

    [Header("일반몬스터")]
    public List<MonsterData> data_Normals;

    [Header("앨리트 몬스터")]
    public List<MonsterData> data_Elites;

    [Header("보스 몬스터")]
    public List<MonsterData> data_Bosses;

    [Header("몬스터 정보")]
    public List<MonsterData> data_Monsters;

    [Header("몬스터 스탯")]
    public List<PlayerData> data_MonsterStat;

    [Header("몬스터 JSON 데이터")]
    public TextAsset monsterJsonData;

    private MonsterJsonAllData allData;

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
        InitializeConvertJsonToMonsterData(); 

        DontDestroyOnLoad(this.gameObject);
    }

    // 몬스터 json데이터를 일반 클래스로 변환하는작업 
    void InitializeConvertJsonToMonsterData()
    {
        allData = JsonUtility.FromJson<MonsterJsonAllData>(monsterJsonData.text);

        data_Monsters = new List<MonsterData>();
        // json 데이터를 일반 클래스에 대입한다. 
        foreach (var data in allData.monsterJsonData)
        {
            // 클래스 생성 
            MonsterData monsterData = new MonsterData();

            monsterData.monsterID = data.monsterID;
            monsterData.monsterName = data.monsterName;
            monsterData.monsterType = (MonsterType)data.monsterType;
            string imagePath = "Monster/" + data.monsterImage;
            monsterData.monsterSprite = Resources.Load<Sprite>(imagePath);
            string objectPath = "Prefabs/Monster/" + data.monsterPrefabName;
            monsterData.monsterPrefab = Resources.Load<GameObject>(objectPath);

            // 데이터 넣어주기 
            data_Monsters.Add(monsterData);
        }
    }


    public int[] getRandomData(int length,int min, int max)
    {
        int[] randArray = new int[length];
        
        for (int i=0; i < length; i++){
            randArray[i] = Random.Range(min, max);
            for (int j = 0; j < i; j++)
            {
               if(randArray[i] == randArray[j])
                {
                    i++;
                    break;
                }
            }
        }

        return randArray;
    }


    public List<MonsterData> GetRandomNormalMData()
    {
        List<MonsterData> randArray = data_Normals;
        List<MonsterData> resultArray = new List<MonsterData>();

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, randArray.Count);
            resultArray.Add(randArray[rand]);
            if(randArray.Count > 1)
                randArray.RemoveAt(rand);
        }

        return resultArray;
    }

    public MonsterData GetMonsterData(uint monsterId)
    {
        MonsterData monster = null;

        foreach(var data in data_Normals)
        {
            if(data.monsterID == monsterId)
            {
                monster = data; 
            }
        }

        foreach(var data in data_Elites)
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

    public uint GetRandomMonsterId(MonsterType monsterType)
    {
        List<MonsterData> randArray;
        int rand;

        switch (monsterType)
        {
            case MonsterType.NORMAL:
                randArray = data_Normals;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
            case MonsterType.ELITE:
                randArray = data_Elites;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
            case MonsterType.BOSS:
                randArray = data_Bosses;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand].monsterID;
        }

        

        return 0;
    }

    public MonsterData GetRandomMonster(MonsterType monsterType)
    {
        List<MonsterData> randArray;
        int rand; 

        switch (monsterType)
        {
            case MonsterType.NORMAL:
                randArray = data_Normals;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
            case MonsterType.ELITE:
                randArray = data_Elites;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
            case MonsterType.BOSS:
                randArray = data_Bosses;
                rand = Random.Range(0, randArray.Count);
                return randArray[rand];
        }

        return null;
    }

    // todo 임시로 정해진 스테이터스클래스를 반환한다. 이후에 데이터를 조작해서 해당 값을 보내도록
    public PlayerData GetMonsterStatus(int id)
    {
        PlayerData result;

        int count = 0; 
        foreach(var data in data_MonsterStat)
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


    // 챕터 번호와 난이도를 받으면 해당되는 몬스터 id를 반환한다. 
    int GetMonsterIDFromChapter(int chpater, int level = 1, bool isBoss = false)
    {
        // 보스일 경우 보스 정보를 가져온다. 
        if(isBoss == true)
        {

        }



        return 0; 
    }
}


