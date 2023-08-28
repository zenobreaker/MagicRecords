using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// 캐릭터 해금 관련 
public class OpenCharInfo
{
    int id;
    bool isOpen; 
}

// 캐릭터 정보리스트를 관리하는 매니저
public class InfoManager : MonoBehaviour
{
    // 
    public static InfoManager instance;

    public static int coin;   // 유저가 사용하는 게임재화 

    // 전체 캐릭터 정보 리스트 
    private Dictionary<int, Character> allPlayerList = new Dictionary<int, Character>();

    // 현재 내가 가지고 있는 캐릭터 리스트 
    private Dictionary<int, Character> myCharacterPlayerList = new Dictionary<int, Character>();

    // 스테이지에서 싸우는 캐릭터들의 리스트 
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
        // 임시 플레이어 객체 생성 후 리스트에 추가
        //TestSetPlayers();
    }


    // 플레이어 정보 세팅한다.
    public void SetDefaultAndAllPlayers()
    {
        var statDict = MonsterDatabase.instance.GetCharactersStatDict();
        if (statDict == null) return;

        foreach(var statPair in statDict)
        {
            Character tempPlayer = new Character();
            tempPlayer.MyID = statPair.Key;
            tempPlayer.objectID = (uint)statPair.Key;
            tempPlayer.MyStat = statPair.Value;
            tempPlayer.MyStat.level = 1;
            tempPlayer.MyStat.ApplyOption();
            AddPlayerInfo(tempPlayer.MyID, tempPlayer);
        }
    }

    public void TestSetPlayers()
    {
        //Character tempPlayer = new Character();
        //CharStat stat = MonsterDatabase.instance.GetCharStat(1);
        //tempPlayer.MyStat = stat; 
        //AddPlayerInfo(tempPlayer.MyID, tempPlayer);
        //AddMyPlayerInfo(tempPlayer.MyID);

        //Character tempPlayer2 = new Character();
        //tempPlayer2.MyStat = new CharStat(1, 0, 0, 0, 0, 0, 10);
        //tempPlayer2.MyID = 1002;
        //tempPlayer.objectID = 2;
        //AddPlayerInfo(tempPlayer2.MyID, tempPlayer2);
        //AddMyPlayerInfo(tempPlayer2.MyID);
    }

    // 캐릭터  만들기
    public void CreateCharacter()
    {
        Character tempPlayer = new Character();
        tempPlayer.MyID = 1001;
        tempPlayer.objectID = 1;
        // todo db나 별도의 저장공간에서 가져와서 세팅
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

    // 자신이 소지한 캐릭터 추가하기 
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

    // 자신이 소지한 캐릭터 반환 
    public Character GetMyPlayerInfo(int uniqueID)
    {
        if (myCharacterPlayerList.ContainsKey(uniqueID) == true)
        {
            return myCharacterPlayerList[uniqueID];
        }
        return null;
    }
    // 자신이 소지한 캐릭터 리스트 반환 
    public List<Character> GetMyPlayerInfoList()
    {
        var list = new List<Character>(myCharacterPlayerList.Values);
        return list;
    }

    public Dictionary<int, Character> GetMyPlayerInfoPairList()
    {
        return myCharacterPlayerList;
    }

    //  모든 캐릭터 리스트를 담는 것 중에 키값으로 해당 캐릭터 반환 
    public Character GetPlayerInfo(int key)
    {
        if (allPlayerList.ContainsKey(key) == true)
        {
            return allPlayerList[key];
        }
        else
            return null;
        // 데이터를 저장했다면 기존 데이터는 지운다. 
        //_playerList.Clear();
    }

    // 스킬 정보를 최신화한다. 
    public void ApplySkillDataSelcetedPlayer(int id)
    {
        var info = GetPlayerInfo(id);
        if (info == null) return;

        SetSelectMyPlayerApplyData(id, info);
    }

    // 자신이 갖고 있는 플레이어 정보 중 선택한 플레이어 정보를 담는다.
    public void SetSelectPlayer(int key)
    {
        var info = GetMyPlayerInfo(key);
        if (info != null)
        {
            selectPlayerList.Add(key, info);
        }
    }

    public void SetSelectPlayers(int[] keys)
    {
        selectPlayerList.Clear();

        for (int i = 0; i < keys.Length; i++)
        {
            SetSelectPlayer(keys[i]);
        }
    }

    // 선택한 플레이어 정보 반환 
    public Dictionary<int, Character> GetSelectPlayers()
    {
        return selectPlayerList;
    }

    // 자신이 가지고 있는 캐릭터들 중 선택되지 않은 녀석들을 반환
    public Dictionary<int, Character> GetUnselectCharacters()
    {
        // 미선택된 캐릭터리스트
        Dictionary<int, Character> unselectList = new Dictionary<int, Character>();

        // 선택했던 리스트
        var selectList = GetSelectPlayers(); 

        // 전체 캐릭터 리스트 
        var myCharacterList = GetMyPlayerInfoPairList();

        // 전체 리스트에서 선택햇던 애들은 제외 
        foreach(var pair in myCharacterList)
        {
            if (selectList.TryGetValue(pair.Key, out Character character) == false)
            {
                unselectList.Add(pair.Key, character);
            }
        }


        return unselectList;
    }


    // 선택한 캐릭터들의 능력치 적용
    public void SetSelectMyPlayerApplyData(int id, Character player)
    {
        if (player == null) return;

        selectPlayerList[id] = player;
    }

    public List<Character> GetSelectPlayerList()
    {
        var list = new List<Character>(selectPlayerList.Values);
        return list;
    }


    /////////////////////////////////////
    // 성장 관련 
    /////////////////////////////////////
    
    // 특정 캐릭터가 성장하면 성장 계산 공식을 통해 각 스탯별 성장값을 반환한다.
    // 캐릭터 마다 레벨 마다 다를 수 있다. 
    // todo. 캐릭터의 타입별로 나눌 수도 있다고 가정할 수 이ㄸ 후엔 
    // todo. 구상하고 보니 캐릭터 기본 스탯이 필요한거같다. 기존 능력치에 증가 해서 붙이고
    // 나중에 저장해서 불러올 땐 계수값으로 하긴 힘드니까.. 
    // 캐릭터 성장  스탯 - 남생이(거북이)
    public void GetGrowUpTurtle(CharStat _targetStat)
    {

        
    }


    // 레벨 값을 받으면 계수만큼 계산해서 반환 
    // 기본 공식 : 기본 스탯 + (성장 계수 * (level - 1)) 
    // 공격력 
    public int GetGrowUpAttack(int _level,  int _attack = 5, float _rate = 1.0f)
    {
        // 공식 todo 공식값이 성립되면 수정해야한다. 
        int result = 0;
        result = _attack + Mathf.RoundToInt(_rate * (_level-1));

        return result; 
    }
    // 방어력 
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
        // 공식 level * rate + HPR(2)
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

    // 공격 속도 (todo 2023 04 24일까지 없음 추가되면 여기도 수정) 
    public float GetGrowUpAttackSpeed(int _level, float _aspd = 1.0f, float _rate = 1.0f)
    {
        // 특정 레벨일 때만 증가 
        float result = _aspd;
        if (_level % 3 == 0)
        {
            result =  _aspd + Mathf.Round(_rate * (_level - 1));
        }


        return result;
    }


    // 이동 속도 
    public int GetGrowUpSpeed(int _level, int _spd = 1, float _rate = 1.0f)
    {
        // 특정 레벨일 때만 증가 
        // 공식 SPD(1) + Mathf.RoundToInt(_rate * (_level - 1))
        int result = _spd;
        if (_level % 3 == 0)
        {
            result = _spd+ Mathf.RoundToInt(_rate * (_level - 1));  
        }

        return result;
    }
}
