using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;


// 캐릭터 해금 관련  
public class OpenCharInfo
{
    int id;
    bool isOpen; 
}

// 캐릭터 정보를 관리해주는 매니저 
public class InfoManager : MonoBehaviour
{
    public static InfoManager instance;

    public bool isTest; 

    // 모든 캐릭터 정보 리스트
    private Dictionary<int, Character> allPlayerList = new Dictionary<int, Character>();

    // 자신이 소지한 캐릭터 정보 리스트
    private Dictionary<int, Character> myCharacterPlayerList = new Dictionary<int, Character>();

    // 파티 조합을 구성한 리스트
    private Dictionary<int, Character> partyCharacters  = new Dictionary<int, Character>();

    // 선택한 캐릭터 리스트 
    private Dictionary<int, Character> selectPlayerList = new Dictionary<int, Character>();

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
        // 테스트 정보로 캐릭터 세팅 
        TestSetPlayers();
    }


    // 게임 시작 시 디폴트 캐릭터를 생성한다.
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
        if(myCharacterPlayerList.Count > 0 || isTest == false)
        {
            return; 
        }
        Character tempPlayer = new CursedTurtle();
        CharStat stat = PlayerDatabase.instance.GetCharStat(1);
        tempPlayer.MyStat = stat;
        tempPlayer.MyID = 1;
        AddMyPlayerInfo(tempPlayer.MyID, tempPlayer);

        // 슬라임
        Character testSlime = new Character();
        CharStat stat2 = PlayerDatabase.instance.GetCharStatByWheelerID(101);
        if (stat2 != null)
        {
            testSlime.MyID = 101;
            testSlime.MyStat = stat2;
            AddMyPlayerInfo(testSlime.MyID, testSlime);
        }

        // 나무귀신
        Character testTree = new Character();
        CharStat stat3 = PlayerDatabase.instance.GetCharStatByWheelerID(102);
        if(stat3 != null)
        {
            testTree.MyID = 102;
            testTree.MyStat = stat3;
            AddMyPlayerInfo(testTree.MyID, testTree); 
        }
        
    }

    // 캐릭터 생성
    public void CreateCharacter()
    {
        Character tempPlayer = new Character();
        tempPlayer.MyID = 1001;
        tempPlayer.objectID = 1;
        // todo db를 통해만들수있도록 해보기 
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

    // 자신의 소지 캐릭터 리스트에 추가하기 
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

    // 자신이 소지한 캐릭터 정보 반환
    public Character GetMyPlayerInfo(int uniqueID)
    {
        if (myCharacterPlayerList.ContainsKey(uniqueID) == true)
        {
            return myCharacterPlayerList[uniqueID];
        }
        return null;
    }
    // 자신의 캐릭터 정보 리스트 반환
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

    //  키값을 받으면 캐릭터 정보 반환
    public Character GetPlayerInfo(int key)
    {
        if (allPlayerList.ContainsKey(key) == true)
        {
            return allPlayerList[key];
        }
        else
            return null;

        //_playerList.Clear();
    }

    // 자신의 파티 캐릭터들 초기화
    public void InitMyPartyPlayList()
    {
        partyCharacters.Clear();

        foreach (var pair in myCharacterPlayerList)
        {
            var character = pair.Value.DeepCopy();
            character.InitCurrentHP();
            character.InitCurrentMP();
            character.InitCurrentCP();
            character.isDead = false; 
            partyCharacters.Add(pair.Key, character);
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


    // 선택한 캐릭터 설정 
    public void SetSelectPlayer(int key, bool isLink)
    {
        //var info = GetMyPlayerInfo(key);
        if(partyCharacters == null || partyCharacters.Count <= 0 || isLink == false)
        {
            InitMyPartyPlayList();  
        }
        
        if(partyCharacters.TryGetValue(key, out var info))
        {
            //info.InitCurrentHP();
            //info.InitCurrentMP();
            if(isLink == true)
            {
                selectPlayerList.Add(key, info);
            }
            else
            {
                var character = info.DeepCopy();
                character.InitCurrentHP();
                character.InitCurrentMP();
                character.InitCurrentCP();
                character.isDead = false;

                selectPlayerList.Add(key, character);
            }
        }
    }

    // 선택한 캐릭터들을 설정한다
    public void SetSelectPlayers(int[] keys, bool isLink = true)
    {
        selectPlayerList.Clear();

        for (int i = 0; i < keys.Length; i++)
        {
            SetSelectPlayer(keys[i], isLink);
        }
    }

    // 선택했던 캐릭터리스트 반환
    public Dictionary<int, Character> GetSelectPlayers()
    {
        return selectPlayerList;
    }

    // 선택하지 않은 캐릭터 리스트 반환
    public Dictionary<int, Character> GetUnselectCharacters()
    {
        // 선택하지않은 캐릭터 리스트를 담을 변수
        Dictionary<int, Character> unselectList = new Dictionary<int, Character>();

        // 선택한 캐릭터들 
        var selectList = GetSelectPlayers(); 

        // 자신이 소지한 캐릭터들
        var myCharacterList = GetMyPlayerInfoPairList();

        // 선택하지않은 캐릭터들을 찾아 리스트에 추가
        foreach(var pair in myCharacterList)
        {
            if (selectList.TryGetValue(pair.Key, out Character character) == false)
            {
                unselectList.Add(pair.Key, character);
            }
        }


        return unselectList;
    }


    // 
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
    // 스테이터스 성장
    /////////////////////////////////////
    

    // 공격 성장
    // 공격력 : 공격력+ (비율 * (level - 1)) 
    public int GetGrowUpAttack(int _level,  int _attack = 5, float _rate = 1.0f)
    {
        int result = 0;
        result = _attack + Mathf.RoundToInt(_rate * (_level-1));

        return result; 
    }

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

    // 체력 재생

    public int GetGrowUpHPRecovery(int _level, int _hpr = 2, float _rate = 1.0f)
    {
        //  level * rate + HPR(2)
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
    // 마나 재생
    public int GetGrowUpMPRecovery(int _level, int _mpr = 1, float _rate = 1.0f)
    {
        int result = 0;
        result = _mpr + Mathf.RoundToInt(_rate * (_level - 1));

        return result;
    }

    // 공격속도 
    public float GetGrowUpAttackSpeed(int _level, float _aspd = 1.0f, float _rate = 1.0f)
    {
        float result = _aspd;
        if (_level % 3 == 0)
        {
            result =  _aspd + Mathf.Round(_rate * (_level - 1));
        }


        return result;
    }


    public int GetGrowUpSpeed(int _level, int _spd = 1, float _rate = 1.0f)
    {
        //  SPD(1) + Mathf.RoundToInt(_rate * (_level - 1))
        int result = _spd;
        if (_level % 3 == 0)
        {
            result = _spd+ Mathf.RoundToInt(_rate * (_level - 1));  
        }

        return result;
    }
}

