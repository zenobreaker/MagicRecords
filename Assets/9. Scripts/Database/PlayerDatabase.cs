using System.Collections;
using System.Collections.Generic;
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


public class PlayerDatabase : MonoBehaviour
{
    public static PlayerDatabase instance;

    [Header("플레이어 캐릭터 리스트")]
    public List<PlayerableObjectData> playerObjectList;

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

        DontDestroyOnLoad(this.gameObject);
    }


    public List<PlayerableObjectData> GetPlayerableObjectList()
    {
        return playerObjectList;
    }

    public PlayerableObjectData GetPlayerableObject(uint _id)
    {
        foreach (var data in playerObjectList)
        {
            if (data.id == _id)
            {
                return data;
            }
        }

        // 받아온 데이터가 없다면 디폴트값을 던져준다
        return playerObjectList[0];
    }

    public void SetData(PlayerableObjectData _data)
    {
        playerObjectList.Add(_data);
    }
}


