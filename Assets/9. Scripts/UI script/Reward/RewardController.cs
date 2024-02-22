using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// 옵저버 패턴을 위한 인터페이스 
public interface IRewardObserver
{
    void NotifyReward(List<Item> rewradList);
}


// 특정 컨텐츠가 끝나면이 컨트롤러의 메소드를 호출해서 보상을 받게 한다. 
public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopup;
  
    List<IRewardObserver> myRewardObservers = new List<IRewardObserver>();


    // PRIVATE METHOD : 보상의 가중치 계산 후 아이템을 반환
    List<Item> CalcRewardWeight(List<(Item, int)> rewardItemList)
    {
        List<Item> itemList = new List<Item>();

        if(rewardItemList == null)
            return itemList;

        // 보상 가중치 값을 알아 내기 위해 보상 관련 매니저에게 말걸기
        foreach (var reward in rewardItemList)
        {
            var item = reward.Item1;

            // 가중치 계산 (100백분율 계산하기 위함)
            int chance = Random.Range(0, 101);
            // 찬스에서 나온 값이 리워드 가중치보다 작거나 같다면 해당 대상은 보상을 받을 수 있다.
            if (chance <= reward.Item2)
            {
                itemList.Add(item);
            }
        }

        return itemList;
    }

    // 옵저버 패턴 사용
    public void AddObserver(IRewardObserver observer)
    {
        myRewardObservers.Add(observer);
    }

    public void RemoveObserver(IRewardObserver observer)
    {
        myRewardObservers.Remove(observer);
    }

    // 보상을 주는 함수 
    public void GiveReward(StageRewardData stageReward)
    {
        if (stageReward == null) return; 

        List<Item> list = new List<Item>();

        list = CalcRewardWeight(stageReward.rewardItemList);

        foreach (var obserber in myRewardObservers)
        {
            obserber.NotifyReward(list);
        }
    }

 
    // 스테이지를 클리어하셨네요? 보상을 받으세요
    public void GainReward(int stageID)
    {
        // 해당 스테이지 ID로 보상 정보 관련 가져오기
        var stageReward = RewardDatabase.instance.GetStageRewardData(stageID);
        if (stageReward == null) return;


       GiveReward(stageReward);
    }

    // 보상을 받았다는 팝업을 띄워줍니다.
    public void CallOpenRewardPopup(List<Item> list)
    {
        if (rewardPopup == null)
            return;

        // 
        var popup = Instantiate(rewardPopup);
        if (popup == null) return;

        // 
        popup.SetRewardItemList(list);

    }


}
