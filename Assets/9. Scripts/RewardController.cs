using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum RewardType { NONE = 0, COIN, EXP, ITEM }
[System.Serializable]
public class RewardCard
{
    public RewardType rewardType;
    public int value;
}

public class RewardController : MonoBehaviour
{
    [Header("보상 UI")]
    [SerializeField] GameObject go_BaseUI = null;
    [SerializeField] GameObject[] go_RewardCard = null;     // 보상 카드들 
    [SerializeField] Image[] img_RewardIcon = null;         // 보상 카드내 이미지
    [SerializeField] Text[] txt_RewardName = null;          // 보상 이름 텍스트 
    [SerializeField] Text[] txt_RewardFlavor = null;        // 보상 설명 텍스트 
    [SerializeField] Button btn_Confirm = null;           // 결정 버튼
    [SerializeField] Image img_Selecter = null;             // 선택자 


    
    int[] values;       // 보상 값 
    
    public int maxRewardType = 4;
    public int maxRewardCardCount = 2;
    public int minValue;
    public int maxValue;
    public bool isConfirm = false; 

    [SerializeField] List<RewardCard> rewardCards;  // 직렬화 안한다고 오브젝트를 찾을 수 없다니 ㄷ 
    RewardCard cur_RewardCard;

    // 보상 카드 선택자 표시
    public void ViewSelecterImage()
    {
        img_Selecter.gameObject.SetActive(true);
        img_Selecter.transform.position = EventSystem.current.currentSelectedGameObject.transform.position;

        for (int i = 0; i < go_RewardCard.Length; i++)
        {
            if(EventSystem.current.currentSelectedGameObject.gameObject == go_RewardCard[i])
            {
                cur_RewardCard = rewardCards[i];
                break;
            }
        }

        if(cur_RewardCard != null)
        {
            btn_Confirm.interactable = true;
        }
    }
    
    public void SetRewardCardList(int _max)
    {
        if (rewardCards.Count < _max)
        {
            for (int i = rewardCards.Count; i < _max; i++)
            {
                rewardCards.Add(new RewardCard());
            }
        }
        
    }


    // 보상 카드 값 설정 
    public void SetRewardCard()
    {
        int r_RewardType;
        int t_Value = 0;

        SetRewardCardList(maxRewardCardCount);


        for (int i = 0; i < rewardCards.Count; i++)
        {
            r_RewardType = Random.Range(1, maxRewardType);
           

            switch ((RewardType)r_RewardType)
            {
                case RewardType.COIN:
                    rewardCards[i].rewardType = RewardType.COIN;
                    txt_RewardName[i].text = "코인";
                    t_Value = SetValue();
                    rewardCards[i].value = t_Value;
                    txt_RewardFlavor[i].text = string.Format( "{0:#,###}"+ "코인", t_Value.ToString());
                    break;
                case RewardType.EXP:
                    rewardCards[i].rewardType = RewardType.EXP;
                    txt_RewardName[i].text = "경험치";
                    t_Value = SetValue();
                    rewardCards[i].value = t_Value;
                    txt_RewardFlavor[i].text = string.Format("{0:#,###}" + "경험치", t_Value.ToString());
                    break;
                case RewardType.ITEM:
                    rewardCards[i].rewardType = RewardType.ITEM;
                    txt_RewardName[i].text = "아이템 ";
                    break;
            }
        }
      
    }

    public int SetValue()
    {
        int t_value;

        t_value = Random.Range(minValue, maxValue);

        return t_value;
    }


    // 결정 버튼에 첨부될 이벤트 
    public void ConfirmReward()
    {
        // TODO : LobbyManager는 로비에서만 쓰이니 해당 결과는 게임매니저나 기타 다른 오브젝트에서
        // 처리하도록 한다.
        if (cur_RewardCard == null) return; 

        switch (cur_RewardCard.rewardType)
        {
            case RewardType.COIN:
                Debug.Log("코인 획득! : " + cur_RewardCard.value);
                //LobbyManager.MyInstance.IncreseCoin(cur_RewardCard.value);
                InfoManager.instance.money += cur_RewardCard.value;
                break;
            case RewardType.EXP:
                // 경험치 추가 로직 
                Debug.Log("경험치 획득! : " + cur_RewardCard.value);
                //InfoManager.instance.GetPlayerInfo()
                break;
            case RewardType.ITEM:
                break;
        }

        go_BaseUI.SetActive(false);
        isConfirm = true;
    }

    public void ShowUI()
    {
        isConfirm = false;
        go_BaseUI.SetActive(true);
        cur_RewardCard = null;

        if (btn_Confirm != null)
        {
            btn_Confirm.interactable = false;
        }
    }
}
