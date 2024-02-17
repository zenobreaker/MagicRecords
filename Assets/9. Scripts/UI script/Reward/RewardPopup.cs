using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopup : UiBase
{
    List<Item> rewardList = new List<Item> ();

    // 보상 데이터 설정하기 
    public void SetRewardItemList(List<Item> list)
    {
        if (list.Count <= 0) return;

        rewardList = list;
        
        InitScrollviewObject(rewardList.Count);
    }

    // 보상 아이템 그리기
    public void DrawRewardItemList()
    {
        if(rewardList.Count <= 0 || content == null) return;

        for(int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);  

            if(child.TryGetComponent<Slot>(out Slot component))
            {
                component.AddItem(rewardList[i]);
            }
        }

    }


    public void DrawRewardPopup()
    {
        // 보상 아이템을 그린다.
        DrawRewardItemList();
    }

    public void ConfirmButton()
    {
        Destroy(gameObject);
    }


}
