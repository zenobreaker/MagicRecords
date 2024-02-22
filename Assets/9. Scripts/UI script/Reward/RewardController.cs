using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


// ������ ������ ���� �������̽� 
public interface IRewardObserver
{
    void NotifyReward(List<Item> rewradList);
}


// Ư�� �������� �������� ��Ʈ�ѷ��� �޼ҵ带 ȣ���ؼ� ������ �ް� �Ѵ�. 
public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopup;
  
    List<IRewardObserver> myRewardObservers = new List<IRewardObserver>();


    // PRIVATE METHOD : ������ ����ġ ��� �� �������� ��ȯ
    List<Item> CalcRewardWeight(List<(Item, int)> rewardItemList)
    {
        List<Item> itemList = new List<Item>();

        if(rewardItemList == null)
            return itemList;

        // ���� ����ġ ���� �˾� ���� ���� ���� ���� �Ŵ������� ���ɱ�
        foreach (var reward in rewardItemList)
        {
            var item = reward.Item1;

            // ����ġ ��� (100����� ����ϱ� ����)
            int chance = Random.Range(0, 101);
            // �������� ���� ���� ������ ����ġ���� �۰ų� ���ٸ� �ش� ����� ������ ���� �� �ִ�.
            if (chance <= reward.Item2)
            {
                itemList.Add(item);
            }
        }

        return itemList;
    }

    // ������ ���� ���
    public void AddObserver(IRewardObserver observer)
    {
        myRewardObservers.Add(observer);
    }

    public void RemoveObserver(IRewardObserver observer)
    {
        myRewardObservers.Remove(observer);
    }

    // ������ �ִ� �Լ� 
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

 
    // ���������� Ŭ�����ϼ̳׿�? ������ ��������
    public void GainReward(int stageID)
    {
        // �ش� �������� ID�� ���� ���� ���� ��������
        var stageReward = RewardDatabase.instance.GetStageRewardData(stageID);
        if (stageReward == null) return;


       GiveReward(stageReward);
    }

    // ������ �޾Ҵٴ� �˾��� ����ݴϴ�.
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
