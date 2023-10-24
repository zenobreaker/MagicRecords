using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum PlayerableType
{
    PLAYER, NON_PLAYER, AUTO_PLAYER
}

[System.Serializable]
public class PlayerableObjectData 
{
    public uint id;
    public PlayerableType type;
    public GameObject playerObject; 
}


[System.Serializable]
public class CharacterData
{
    public int id;
    public int characterID;     // 대상이 되는 캐릭터의 ID
    public int statID;
    public string name;
    public Sprite portrait;
    public CharStat charStat = new CharStat();
    public GameObject prefab;
    public MonsterGrade monsterGrade = MonsterGrade.NONE;
}


[System.Serializable]
public class CharacterDataJson
{
    public int id;
    public int characterID;
    public string namekeycode;
    public string portrait;
    public string prefabName;
    public int statID;
    public int monsterGrade;
}

[System.Serializable]
public class CharacterDataJsonAllData
{
    public CharacterDataJson[] characterDataJson;
}




public class PlayerDatabase : MonoBehaviour
{
    public static PlayerDatabase instance;

    private CharacterDataJsonAllData characterDataAllData;
    private CharacterStatJsonAllData characterAllData;  // 캐릭터 능력치 정보 

    [Header("캐릭터 정보 JSON 데이터")]
    public TextAsset characterData;

    [Header("캐릭터 기본 능력치 JSON 데이터")]
    public TextAsset characterStatJson;

    [Header("캐릭터")]
    public List<CharacterData> characterdataList = new List<CharacterData>();

    public Dictionary<int, CharStat> charStatDic = new Dictionary<int, CharStat>();


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

        InitializeCharacterStatData();

        InitializeCharacterData();


        DontDestroyOnLoad(this.gameObject);
    }


    // 캐릭터 스탯 정보 초기화 
    void InitializeCharacterStatData()
    {
        characterAllData = JsonUtility.FromJson<CharacterStatJsonAllData>(characterStatJson.text);

        if (characterAllData.characterStatJson == null)
            return;

        foreach (var data in characterAllData.characterStatJson)
        {
            if (data == null) continue;

            CharStat charStat = new CharStat();
            charStat.attack = data.attack;
            charStat.defense = data.defense;
            charStat.attackSpeed = data.attackSpeed;
            charStat.speed = data.speed;
            charStat.hp = data.hp;
            charStat.hpRegen = data.hpRegen;
            charStat.mp = data.mp;
            charStat.mpRegen = data.mpRegen;
            charStat.critRate = data.critRate;
            charStat.critDmg = data.critDmg;

            charStatDic.Add(data.id, charStat);
        }
    }

    // 캐릭터 json 데이터를 일반 클래스로 변환
    public void InitializeCharacterData()
    {
        if (characterData == null) return;

        characterDataAllData = JsonUtility.FromJson<CharacterDataJsonAllData>(characterData.text);
        if (characterDataAllData == null) return;

        foreach (var character in characterDataAllData.characterDataJson)
        {
            if (character == null) continue;

            CharacterData characterData = new CharacterData();
            characterData.id = character.id;
            characterData.characterID = character.characterID;
            characterData.statID = character.statID;
            characterData.name = character.namekeycode;
            if (charStatDic.TryGetValue(character.id, out var stat))
                characterData.charStat = stat;

            string imagePath = "Portrait/" + character.portrait + "_portrait";
            Sprite sprite= Resources.Load<Sprite>(imagePath);
            if(sprite == null)
                sprite = Resources.Load<Sprite>("Portrait/DefaultMonster");
            characterData.portrait = sprite;
            string objectPath = "Prefabs/Characters/" + character.prefabName;
            characterData.prefab = Resources.Load<GameObject>(objectPath);
            characterData.monsterGrade = (MonsterGrade)character.monsterGrade;
            characterdataList.Add(characterData);
        }
    }

    // 등급에 맞는 몬스터를 리스트로 정리해서 반환
    public List<CharacterData> GetCharacterList(MonsterGrade grade)
    {
        List<CharacterData> list = new List<CharacterData>();

        foreach (var data in characterdataList)
        {
            if (data.monsterGrade != grade)
                continue;

            CharacterData characterData = new CharacterData();

            characterData.id = data.id;
            characterData.characterID = data.characterID;
            characterData.statID = data.statID;
            characterData.monsterGrade = data.monsterGrade;
            characterData.portrait = data.portrait;
            characterData.prefab = data.prefab;
            characterData.name = data.name;
            characterData.charStat = data.charStat.Clone();

            list.Add(characterData);
        }

        return list;
    }

    // id 값을 받으면 CharStat 클래스를 반환한다. exp는 계산되지않음
    public CharStat GetCharStat(int id)
    {
        if (charStatDic.TryGetValue(id, out CharStat charStat))
        {
            return charStat;
        }
        return null;
    }

    // id stat 값을 딕셔너리로 반환
    public Dictionary<int, CharStat> GetCharactersStatDict()
    {
        Dictionary<int, CharStat> list = new Dictionary<int, CharStat>();
        foreach (var data in characterAllData.characterStatJson)
        {
            if (data == null) continue;

            CharStat charStat = new CharStat();
            charStat.attack = data.attack;
            charStat.defense = data.defense;
            charStat.attackSpeed = data.attackSpeed;
            charStat.speed = data.speed;
            charStat.hp = data.hp;
            charStat.hpRegen = data.hpRegen;
            charStat.mp = data.mp;
            charStat.mpRegen = data.mpRegen;
            charStat.critRate = data.critRate;
            charStat.critDmg = data.critDmg;

            list.Add(data.id, charStat);
        }

        return list;

    }


    // 캐릭터 데이터 관련 
    // id 값을 받으면 해당 캐릭터 data를 반환
    public CharacterData GetCharacterData(int id)
    {
        return characterdataList.Where(x => x.id == id).FirstOrDefault();
    }

    // 캐릭터 data list 를 반환
    public List<CharacterData> GetCharacterDataList()
    {
        return characterdataList;
    }
}


