using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : UiBase, IRewardObserver
{

    public List<Item> rewardsItems = new List<Item>();

    [Header("UI ������Ʈ")]
    [SerializeField] private GameObject go_UI = null;

    [SerializeField] private Text txt_StageClearText = null;
    [SerializeField] private Text rewardText; 
    [SerializeField] private Text txt_CurrentScore = null;



    bool isClear = false;

 

    // PRIVATE FUNCTION : ���� ����Ʈ�� �׸���. 
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
                Debug.Log("TEST - ������ ȹ�� - " + rewardsItems[i].itemName);
                slot.AddItem(rewardsItems[i]);
            }
        }
    }

    // PRIVATE FUNCTION : Ŭ���� Ÿ��Ʋ�� �׸���. 
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

    
    // PRIVATE FUNCTION : �ش� UI�� ��ü �׸���. 
    private void DrawStageClearUI()
    {
        // Ÿ��Ʋ �׸��� 
        DrawClearTitle(); 

        // ���� ����Ʈ �׸��� 
        DrawRewardList();
    }

    public void ShowStageClearUI()
    {
        // �������� UI �׸��� 
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
