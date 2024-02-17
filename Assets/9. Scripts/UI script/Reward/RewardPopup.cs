using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopup : UiBase
{
    List<Item> rewardList = new List<Item> ();

    // ���� ������ �����ϱ� 
    public void SetRewardItemList(List<Item> list)
    {
        if (list.Count <= 0) return;

        rewardList = list;
        
        InitScrollviewObject(rewardList.Count);
    }

    // ���� ������ �׸���
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
        // ���� �������� �׸���.
        DrawRewardItemList();
    }

    public void ConfirmButton()
    {
        Destroy(gameObject);
    }


}
