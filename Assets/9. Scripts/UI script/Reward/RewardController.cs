using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Ư�� �������� �������� ��Ʈ�ѷ��� �޼ҵ带 ȣ���ؼ� ������ �ް� �Ѵ�. 
public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopup;
    public List<Item> myRewardList; 

    // ������ �ִ� �Լ� 
    public List<Item> GiveReward(StageRewardData stageReward)
    { 
        List<Item> list = new List<Item>();

        // ���� ����ġ ���� �˾� ���� ���� ���� ���� �Ŵ������� ���ɱ�
        foreach (var reward in stageReward.rewardItemList)
        {
            var item = reward.Item1;

            // ����ġ ��� (100����� ����ϱ� ����)
            int chance = Random.Range(0, 101);
            // �������� ���� ���� ������ ����ġ���� �۰ų� ���ٸ� �ش� ����� ������ ���� �� �ִ�.
            if(chance <= reward.Item2 )
            {
                list.Add(item);
            }
        }

        return list;
    }

 
    // ���������� Ŭ�����ϼ̳׿�? ������ ��������
    public void GainReward(int stageID)
    {
        // �ش� �������� ID�� ���� ���� ���� ��������
        var stageReward = RewardDatabase.instance.GetStageRewardData(stageID);
        if (stageReward == null) return;


        myRewardList = GiveReward(stageReward);
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
