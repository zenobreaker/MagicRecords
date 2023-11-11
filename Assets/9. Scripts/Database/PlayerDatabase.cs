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
    public int characterID;     // ����� �Ǵ� ĳ������ ID
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
    private CharacterStatJsonAllData characterAllData;  // ĳ���� �ɷ�ġ ���� 

    [Header("ĳ���� ���� JSON ������")]
    public TextAsset characterData;

    [Header("ĳ���� �⺻ �ɷ�ġ JSON ������")]
    public TextAsset characterStatJson;

    [Header("ĳ����")]
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


    // ĳ���� ���� ���� �ʱ�ȭ 
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

    // ĳ���� json �����͸� �Ϲ� Ŭ������ ��ȯ
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

    // ��޿� �´� ���͸� ����Ʈ�� �����ؼ� ��ȯ
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

    // id ���� ������ CharStat Ŭ������ ��ȯ�Ѵ�. exp�� ����������
    public CharStat GetCharStat(int id)
    {
        if (charStatDic.TryGetValue(id, out CharStat charStat))
        {
            return charStat;
        }
        return null;
    }

    // id stat ���� ��ųʸ��� ��ȯ
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


    // ĳ���� ������ ���� 
    // id ���� ������ �ش� ĳ���� data�� ��ȯ
    public CharacterData GetCharacterData(int id)
    {
        return characterdataList.Where(x => x.id == id).FirstOrDefault();
    }

    // ĳ���� data list �� ��ȯ
    public List<CharacterData> GetCharacterDataList()
    {
        return characterdataList;
    }
}