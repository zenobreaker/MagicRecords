using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;


// ĳ���� �ر� ���� 
public class OpenCharInfo
{
    int id;
    bool isOpen; 
}

// ĳ���� ��������Ʈ�� �����ϴ� �Ŵ���
public class InfoManager : MonoBehaviour
{
    // 
    public static InfoManager instance;

    public static int coin;   // ������ ����ϴ� ������ȭ 

    // ��ü ĳ���� ���� ����Ʈ 
    private Dictionary<int, Character> allPlayerList = new Dictionary<int, Character>();

    // ���� ���� ������ �ִ� ĳ���� ����Ʈ 
    private Dictionary<int, Character> myCharacterPlayerList = new Dictionary<int, Character>();

    // Ž���� ĳ���� ����Ʈ 
    private Dictionary<int, Character> partyCharacters  = new Dictionary<int, Character>();

    // ������������ �ο�� ĳ���͵��� ����Ʈ 
    private Dictionary<int, Character> selectPlayerList = new Dictionary<int, Character>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // 
        SetDefaultAndAllPlayers(); 
        // �ӽ� �÷��̾� ��ü ���� �� ����Ʈ�� �߰�
        TestSetPlayers();
    }


    // �÷��̾� ���� �����Ѵ�.
    public void SetDefaultAndAllPlayers()
    {
        var statDict = PlayerDatabase.instance.GetCharactersStatDict();
        if (statDict == null) return;

        foreach(var statPair in statDict)
        {
            Character tempPlayer = new Character();
            if (statPair.Key == 1)
            {
                tempPlayer = new CursedTurtle();
            }
            tempPlayer.MyID = statPair.Key;
            tempPlayer.objectID = statPair.Key;
            tempPlayer.MyStat = statPair.Value;
            tempPlayer.MyStat.level = 1;
            tempPlayer.MyStat.ApplyOption();
            AddPlayerInfo(tempPlayer.MyID, tempPlayer);
        }
    }

    public void TestSetPlayers()
    {
        if(myCharacterPlayerList.Count > 0)
        {
            return; 
        }
        Character tempPlayer = new CursedTurtle();
        CharStat stat = PlayerDatabase.instance.GetCharStat(1);
        tempPlayer.MyStat = stat;
        tempPlayer.MyID = 1;
        //AddPlayerInfo(tempPlayer.MyID, tempPlayer);
        AddMyPlayerInfo(tempPlayer.MyID, tempPlayer);

        //Character tempPlayer2 = new Character();
        //tempPlayer2.MyStat = new CharStat(1, 0, 0, 0, 0, 0, 10);
        //tempPlayer2.MyID = 1002;
        //tempPlayer.objectID = 2;
        //AddPlayerInfo(tempPlayer2.MyID, tempPlayer2);
        //AddMyPlayerInfo(tempPlayer2.MyID);
    }

    // ĳ����  �����
    public void CreateCharacter()
    {
        Character tempPlayer = new Character();
        tempPlayer.MyID = 1001;
        tempPlayer.objectID = 1;
        // todo db�� ������ ����������� �����ͼ� ����
        tempPlayer.MyStat = new CharStat(1, 10, 10, 20, 150, 100, 10);
        //tempPlayer.SetEquipment(EquipType.WEAPON);
    }

    public void AddPlayerInfo(int key, Character playerInfo)
    {
        if (playerInfo == null)
        {
            return;
        }

        allPlayerList.Add(key, playerInfo);
    }

    // �ڽ��� ������ ĳ���� �߰��ϱ� 
    public void AddMyPlayerInfo(int key)
    {
        var info = GetPlayerInfo(key);
        if (info != null)
        {
            info.MyStat.CalcMaxExp(info.MyStat.level);
            myCharacterPlayerList.Add(key, info);
        }
    }

    public void AddMyPlayerInfo(int id, Character character)
    {
        if (character == null) return;
        character.MyStat.CalcMaxExp(character.MyStat.level);
        character.MyStat.ApplyOption();
        myCharacterPlayerList.Add(id, character);
    }

    // �ڽ��� ������ ĳ���� ��ȯ 
    public Character GetMyPlayerInfo(int uniqueID)
    {
        if (myCharacterPlayerList.ContainsKey(uniqueID) == true)
        {
            return myCharacterPlayerList[uniqueID];
        }
        return null;
    }
    // �ڽ��� ������ ĳ���� ����Ʈ ��ȯ 
    public List<Character> GetMyPlayerInfoList()
    {
        var list = new List<Character>(myCharacterPlayerList.Values);
        return list;
    }

    public List<int> GetMyPlayerIDList()
    {
        var list = new List<int>(myCharacterPlayerList.Keys);
        return list; 
    }

    public Dictionary<int, Character> GetMyPlayerInfoPairList()
    {
        return myCharacterPlayerList;
    }

    //  ��� ĳ���� ����Ʈ�� ��� �� �߿� Ű������ �ش� ĳ���� ��ȯ 
    public Character GetPlayerInfo(int key)
    {
        if (allPlayerList.ContainsKey(key) == true)
        {
            return allPlayerList[key];
        }
        else
            return null;
        // �����͸� �����ߴٸ� ���� �����ʹ� �����. 
        //_playerList.Clear();
    }

    // ���ӿ� ������ ��Ƽ ĳ���� ����Ʈ ����
    public void InitMyPartyPlayList()
    {
        partyCharacters.Clear();

        foreach (var pair in myCharacterPlayerList)
        {
            pair.Value.InitCurrentHP();
            pair.Value.InitCurrentMP();
            pair.Value.isDead = false; 
            partyCharacters.Add(pair.Key, pair.Value);
        }
    }

    public Dictionary<int, Character> GetPartyPlayerList()
    {
        if (partyCharacters == null)
            return null; 

        return partyCharacters;
    }


    // ��ų ������ �ֽ�ȭ�Ѵ�. 
    public void ApplySkillDataSelcetedPlayer(int id)
    {
        var info = GetPlayerInfo(id);
        if (info == null) return;

        SetSelectMyPlayerApplyData(id, info);
    }


    // �ڽ��� ���� �ִ� �÷��̾� ���� �� ������ �÷��̾� ������ ��´�.
    public void SetSelectPlayer(int key)
    {
        //var info = GetMyPlayerInfo(key);
        if(partyCharacters == null || partyCharacters.Count <= 0)
        {
            InitMyPartyPlayList();  
        }
        
        if(partyCharacters.TryGetValue(key, out var info))
        {
            //info.InitCurrentHP();
            //info.InitCurrentMP();
            selectPlayerList.Add(key, info);
        }
    }

    // �÷����� ĳ���͵��� ����
    public void SetSelectPlayers(int[] keys)
    {
        selectPlayerList.Clear();

        for (int i = 0; i < keys.Length; i++)
        {
            SetSelectPlayer(keys[i]);
        }
    }

    // ������ �÷��̾� ���� ��ȯ 
    public Dictionary<int, Character> GetSelectPlayers()
    {
        return selectPlayerList;
    }

    // �ڽ��� ������ �ִ� ĳ���͵� �� ���õ��� ���� �༮���� ��ȯ
    public Dictionary<int, Character> GetUnselectCharacters()
    {
        // �̼��õ� ĳ���͸���Ʈ
        Dictionary<int, Character> unselectList = new Dictionary<int, Character>();

        // �����ߴ� ����Ʈ
        var selectList = GetSelectPlayers(); 

        // ��ü ĳ���� ����Ʈ 
        var myCharacterList = GetMyPlayerInfoPairList();

        // ��ü ����Ʈ���� �����޴� �ֵ��� ���� 
        foreach(var pair in myCharacterList)
        {
            if (selectList.TryGetValue(pair.Key, out Character character) == false)
            {
                unselectList.Add(pair.Key, character);
            }
        }


        return unselectList;
    }


    // ������ ĳ���͵��� �ɷ�ġ ����
    public void SetSelectMyPlayerApplyData(int id, Character player)
    {
        if (player == null) return;

        partyCharacters[id] = player;
    }

    public List<Character> GetSelectPlayerList()
    {
        var list = new List<Character>(selectPlayerList.Values);
        return list;
    }


    /////////////////////////////////////
    // ���� ���� 
    /////////////////////////////////////
    
    // Ư�� ĳ���Ͱ� �����ϸ� ���� ��� ������ ���� �� ���Ⱥ� ���尪�� ��ȯ�Ѵ�.
    // ĳ���� ���� ���� ���� �ٸ� �� �ִ�. 
    // todo. ĳ������ Ÿ�Ժ��� ���� ���� �ִٰ� ������ �� �̤� �Ŀ� 
    // todo. �����ϰ� ���� ĳ���� �⺻ ������ �ʿ��ѰŰ���. ���� �ɷ�ġ�� ���� �ؼ� ���̰�
    // ���߿� �����ؼ� �ҷ��� �� ��������� �ϱ� ����ϱ�.. 
    // ĳ���� ����  ���� - ������(�ź���)
    public void GetGrowUpTurtle(CharStat _targetStat)
    {

        
    }


    // ���� ���� ������ �����ŭ ����ؼ� ��ȯ 
    // �⺻ ���� : �⺻ ���� + (���� ��� * (level - 1)) 
    // ���ݷ� 
    public int GetGrowUpAttack(int _level,  int _attack = 5, float _rate = 1.0f)
    {
        // ���� todo ���İ��� �����Ǹ� �����ؾ��Ѵ�. 
        int result = 0;
        result = _attack + Mathf.RoundToInt(_rate * (_level-1));

        return result; 
    }
    // ���� 
    public int GetGrowUpDefense(int _level, int _defense = 5, float _rate =  1.0f)
    {
        int result = 0;
        result =  _defense + Mathf.RoundToInt(_rate * (_level - 1));

        return result; 
    }

    // HP
    public int GetGrowUpHP(int _level, int _hp = 10, float _rate = 1.0f)
    {

        int result = 0;
        result =  _hp + Mathf.RoundToInt(_rate * (_level - 1));

        return result; 
    }

    // ü�� ���

    public int GetGrowUpHPRecovery(int _level, int _hpr = 2, float _rate = 1.0f)
    {
        // ���� level * rate + HPR(2)
        int result = 0;
        result = _hpr + Mathf.RoundToInt(_rate * (_level - 1));

        return result;
    }
    // MP

    public int GetGrowUpMP(int _level, int _mp = 10, float _rate = 1.0f)
    {
        int result = 0;
        result =  _mp + Mathf.RoundToInt(_rate * (_level - 1));

        return result;
    }
    // ���� ��� 
    public int GetGrowUpMPRecovery(int _level, int _mpr = 1, float _rate = 1.0f)
    {
        int result = 0;
        result = _mpr + Mathf.RoundToInt(_rate * (_level - 1));

        return result;
    }

    // ���� �ӵ� (todo 2023 04 24�ϱ��� ���� �߰��Ǹ� ���⵵ ����) 
    public float GetGrowUpAttackSpeed(int _level, float _aspd = 1.0f, float _rate = 1.0f)
    {
        // Ư�� ������ ���� ���� 
        float result = _aspd;
        if (_level % 3 == 0)
        {
            result =  _aspd + Mathf.Round(_rate * (_level - 1));
        }


        return result;
    }


    // �̵� �ӵ� 
    public int GetGrowUpSpeed(int _level, int _spd = 1, float _rate = 1.0f)
    {
        // Ư�� ������ ���� ���� 
        // ���� SPD(1) + Mathf.RoundToInt(_rate * (_level - 1))
        int result = _spd;
        if (_level % 3 == 0)
        {
            result = _spd+ Mathf.RoundToInt(_rate * (_level - 1));  
        }

        return result;
    }
}
