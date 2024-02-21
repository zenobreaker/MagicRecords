using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.UIElements;



public enum RewardType
{
    NONE = 0,
    MATERIAL,
    EXP,
    ITEM,
    ADVENTRUE_CREDIT,     // 탐사 진행 중 얻게 되는 탐사 전용 재화
    RECORD,
}

[System.Serializable]
public class RewardJsonData
{
    public int rewardID;
    public int type;        // 대상의 타입
    public int itemID;      // 대상의 ID
    public int amount;      // 수량   
    public int weight;      // 가중치 
}

[System.Serializable]
public class RewardJsonDataAllData
{
    public RewardJsonData[] rewardJsonData;
}

[System.Serializable]
public class RewardData
{
    public int rewardID;
    public RewardType rewardType;
    public int itemID;
    public int amount;
    public int weight;
}

[System.Serializable]
public class StageRewardJsonData
{
    public int stageID;
    public string rewardIDList;
    public string viewItemIDList;
}

[System.Serializable]
public class StageRewardJsonAllData
{
    public StageRewardJsonData[] stageRewardJsonData;
}


[System.Serializable]
public class StageRewardData
{
    public int stageID;
    public List<(Item, int)> rewardItemList;
    public List<Item> viewItemList;
}

public class RewardDatabase : MonoBehaviour
{
   public static RewardDatabase instance;

    public RewardJsonDataAllData rewardJsonAllData;
    public StageRewardJsonAllData stageRewardJsonAllData;

    [Header("보상 json")]
    public TextAsset rewardJson;
    public TextAsset stageRewardJson;


    public Dictionary<int, RewardData> rewardDataDict = new Dictionary<int, RewardData>();
    public Dictionary<int, StageRewardData> stageRewardDataDict = new Dictionary<int, StageRewardData>();

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitializeRewardJsonData();

        InitializeStageRewardJsonData();
    }

    public void InitializeRewardJsonData()
    {
        rewardJsonAllData = JsonUtility.FromJson<RewardJsonDataAllData>(rewardJson.text);

        if (rewardJsonAllData.rewardJsonData == null)
            return; 

        foreach(var json in rewardJsonAllData.rewardJsonData)
        {
            var rewardData = CreateRewardDataForJsonData(json);
            if (rewardData == null)
                continue; 

            rewardDataDict.Add(rewardData.rewardID, rewardData);
        }
    }

    private Item GetItemByItemDataBase(int itemID)
    {
        if (ItemDatabase.instance != null)
        {
            return ItemDatabase.instance.GetItemByUID(itemID);
        }

        return null;
    }

    private Item GetItemByItemDataBase(string stringID)
    {
        if (ItemDatabase.instance != null && int.TryParse(stringID, out int parseID))
        {
            return GetItemByItemDataBase(parseID);
        }
        else
        {
            return null; 
        }
    }

   

    public void InitializeStageRewardJsonData()
    {
        stageRewardJsonAllData = JsonUtility.FromJson<StageRewardJsonAllData>(stageRewardJson.text);
        if (stageRewardJsonAllData.stageRewardJsonData == null) return; 

        foreach(var json in stageRewardJsonAllData.stageRewardJsonData)
        {
            if (json == null)
                continue; 

            StageRewardData stageRewardData = new StageRewardData();
            stageRewardData.stageID = json.stageID;

            // 보상 관련 
            var idArr = json.rewardIDList.Split(',');
            stageRewardData.rewardItemList = new List<(Item,int)>();
            for (int i = 0; i < idArr.Length; i++)
            {
                var id = idArr[i];
                var weight = 0;
                if (int.TryParse(id, out int rewardID))
                {
                    if (rewardDataDict.TryGetValue(rewardID, out RewardData reward))
                    {
                        var item = GetItemByItemDataBase(reward.itemID);

                        weight = reward.weight;
                        stageRewardData.rewardItemList.Add((item, weight));
                    }
                }
            }

            // 정보등에 보여지는 아이템 관련 
            var viewItemIDArr = json.viewItemIDList.Split(',');
            stageRewardData.viewItemList = new List<Item>();
            for (int i = 0; i < viewItemIDArr.Length;i++)
            {
                var id = viewItemIDArr[i];
                var item = GetItemByItemDataBase(id);
                stageRewardData.viewItemList.Add(item);
            }

            stageRewardDataDict.Add(stageRewardData.stageID, stageRewardData);
        }

    }


    public RewardData CreateRewardDataForJsonData(RewardJsonData rewardJsonData)
    {
        if (rewardJsonData == null) return null; 

        RewardData rewardData = new RewardData();

        rewardData.rewardID = rewardJsonData.rewardID;
        rewardData.rewardType = (RewardType)rewardJsonData.type;
        rewardData.amount = rewardJsonData.amount;

        return rewardData;
    }


    public StageRewardData GetStageRewardData(int stageID)
    {
        if (stageRewardDataDict == null) return null;

        if (stageRewardDataDict.TryGetValue(stageID, out StageRewardData stageRewardData))
            return stageRewardData;
        else
            return null; 
    }
}
