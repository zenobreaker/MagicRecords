using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư�� �������� �������� ��Ʈ�ѷ��� �޼ҵ带 ȣ���ؼ� ������ �ް� �Ѵ�. 
public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopup; 


    // �Ϲ� Ž�簡 ��� �����׿� ������ ��������
    public void GainNormalAdventureReawrd()
    {

    }

    // ���� ���������� Ŭ�����ϼ̳׿�? ������ ��������
    public void GainBossRaidReward(int bossStageID)
    {
        // �ش� �������� ID�� ���� ���� ���� ��������
        var rewradData = RewardDatabase.instance.GetStageRewardData(bossStageID);
        if (rewradData == null) return;


        OpenRewardPopup(rewradData.rewardItemList);
    }

    // ������ �޾Ҵٴ� �˾��� ����ݴϴ�.
    public void OpenRewardPopup(List<Item> list)
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
