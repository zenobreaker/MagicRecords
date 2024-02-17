using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 특정 컨텐츠가 끝나면이 컨트롤러의 메소드를 호출해서 보상을 받게 한다. 
public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopup; 


    // 일반 탐사가 모두 끝났네요 보상을 받으세요
    public void GainNormalAdventureReawrd()
    {

    }

    // 보스 스테이지를 클리어하셨네요? 보상을 받으세요
    public void GainBossRaidReward(int bossStageID)
    {
        // 해당 스테이지 ID로 보상 정보 관련 가져오기
        var rewradData = RewardDatabase.instance.GetStageRewardData(bossStageID);
        if (rewradData == null) return;


        OpenRewardPopup(rewradData.rewardItemList);
    }

    // 보상을 받았다는 팝업을 띄워줍니다.
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
