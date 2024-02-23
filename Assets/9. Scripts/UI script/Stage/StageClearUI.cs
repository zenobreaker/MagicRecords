using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : UiBase, IRewardObserver
{

    public List<Item> rewardsItems = new List<Item>();

    [Header("UI 컴포넌트")]
    [SerializeField] private GameObject go_UI = null;

    [SerializeField] private Text txt_StageClearText = null;
    [SerializeField] private Text rewardText; 
    [SerializeField] private Text txt_CurrentScore = null;



    bool isClear = false;

 

    // PRIVATE FUNCTION : 보상 리스트를 그린다. 
    private void DrawRewardList()
    {
        if (content == null || rewardsItems.Count <= 0) 
            return; 

        for(int i = 0; i < rewardsItems.Count; i++)
        {
            var slotObject = content.transform.GetChild(i);
            if(slotObject == null) continue;

            if(slotObject.TryGetComponent<Slot>(out Slot slot))
            {
                Debug.Log("TEST - 아이템 획득 - " + rewardsItems[i].itemName);
                slot.AddItem(rewardsItems[i]);
            }
        }
    }

    // PRIVATE FUNCTION : 클리어 타이틀을 그린다. 
    private void DrawClearTitle()
    {
        if(isClear == true)
        {
            txt_StageClearText.text = "Clear!";
        }
        else
        {
            txt_StageClearText.text = "Failure..";
        }
    }

    
    // PRIVATE FUNCTION : 해당 UI를 전체 그린다. 
    private void DrawStageClearUI()
    {
        // 타이틀 그리기 
        DrawClearTitle(); 

        // 보상 리스트 그리기 
        DrawRewardList();
    }

    public void ShowStageClearUI()
    {
        // 스테이지 UI 그리기 
        DrawStageClearUI();
    }

    public void SetClearFlag(bool isClear)
    {
        this.isClear = isClear; 
    }

    public void SetRewardItemList(List<Item> items)
    {
        rewardsItems.Clear();

        rewardsItems = items;
    }

    void IRewardObserver.NotifyReward(List<Item> rewardList)
    {
        rewardsItems = rewardList;

        if(rewardsItems != null)
            InitScrollviewObject(rewardsItems.Count);
    }
}
