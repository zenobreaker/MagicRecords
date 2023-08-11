using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChoiceAlert;
using Newtonsoft.Json.Serialization;

public enum RewardType { NONE = 0, COIN, EXP, ITEM, RECORD}
[System.Serializable]
public class RewardCard
{
    public RewardType rewardType;
    public Sprite rewardSprite; // 보상 이미지 
    public float value;         // 보상 수치 
    public string name; 
    public string description;
    public int recordID;

    public RewardCard(RewardType rewardType, Sprite rewardSprite, string name, float value)
    {
        this.rewardType = rewardType;
        this.name = name; 
        this.rewardSprite = rewardSprite;
        this.value = value;
        SetDescriptionByType();
    }

    public void CreateMemoryReward(int recordID)
    {
        this.recordID = recordID;
        SetDescriptionByType();
    }

    // 특정 대상의 설명 스크립트를 가져온다
    private void SetDescriptionByType()
    {
        switch (rewardType)
        {
            case RewardType.COIN:
                break;
            case RewardType.ITEM:
                break;
            case RewardType.EXP:
                break;
            case RewardType.RECORD:
                // 메모리라면 해당 메모리의 문구를 가져온다.
                if (RecordManager.instance != null)
                {
                    var record = RecordManager.instance.GetRecordInfoByID(recordID);
                    if (record == null)
                    {
                        break; 
                    }

                    if(record.specialOption == null)
                    {
                        record.specialOption = RecordManager.instance.GetSpecialOptionToRecordInfo(recordID);
                    }

                    description = record.specialOption.effectName;
                }
                break;

        }

    }
}

// 게임 클리어 보상이나 메모리 선택 UI를 컨트롤하는 클래스 

public class RewardController : MonoBehaviour
{
    [Header("보상 UI")]
    [SerializeField] GameObject go_BaseUI = null;

    [Header("보상 카드 부모그룹 오브젝트")]
    [SerializeField] GameObject cardParent = null;

    [Header("보상 카드 슬롯")]
    [SerializeField] GameObject rewardCardSlot = null;

    [SerializeField] GameObject[] go_RewardCard = null;     // 보상 카드들 
    [SerializeField] Image[] img_RewardIcon = null;         // 보상 카드내 이미지
    [SerializeField] Text[] txt_RewardName = null;          // 보상 이름 텍스트 
    [SerializeField] Text[] txt_RewardFlavor = null;        // 보상 설명 텍스트 
    [SerializeField] Button btn_Confirm = null;           // 결정 버튼
    [SerializeField] Image img_Selecter = null;             // 선택자 

    public int maxRewardType = 4;
    public int maxRewardCardCount = 2;
    public int minValue;
    public int maxValue;

    private static readonly int initSelectIndex = -1; 
    public int selectIndex = initSelectIndex;

    public bool isConfirm = false;


    [SerializeField] List<RewardCard> rewardCards = new List<RewardCard>();  // 직렬화 안한다고 오브젝트를 찾을 수 없다니 ㄷ 
    RewardCard cur_RewardCard;

    private void OnEnable()
    {
        InitializeRewardUI();
    }

    public void InitializeRewardUI()
    {
        // 선택자 꺼두기 
        img_Selecter.gameObject.SetActive(false);

        // 인덱스값 초기화
        selectIndex = initSelectIndex;
    }

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

    // 보상 카드리스트 세팅
    public void SetRewardCardList(int _max)
    {
        rewardCards.Clear();

        for (int i = 0; i < _max; i++)
        {
            // 뭐라 말할 수 없는 그 아름다움... 
        }

    }

    // 해당 오브젝트를 선택하면 오브젝트의 선택자 UI가 나타나도록 설정 및 선택한 정보 저장
    private void SelectRewardCardUI(int index)
    {   
        // 하하 셀렉트 값에 값을 넣엇어
        //selectIndex = index;

        if (cardParent == null) return;

        // 같은 대상을 선택했을 경우 
        if (selectIndex == index)
        {
            selectIndex = initSelectIndex;
        }
        // 선택하지 않은 다른 대상 
        else
        {
            selectIndex = index;
        }


        for (int i = 0; i < cardParent.transform.childCount; i++)
        {
            int targetIndex = i;
            // 뷰포트에있는 슬롯의 설정 UI를 보이게한다. 
            var card = cardParent.transform.GetChild(targetIndex).gameObject;
            if (card == null) continue;

            var rewardCard = card.GetComponent<RewardCardSlot>();
            if (rewardCard == null) continue; 
           
            if (selectIndex == targetIndex)
            {
                rewardCard.DrawSelectUI(true);
            }
            else
            {
                rewardCard.DrawSelectUI(false);
            }
        }
    }

    // 메모리 보상 리스트 세팅
    public void SetRecordRewardList(int count)
    {
        if (RecordManager.instance == null)
            return;

        //  보상 리스트를 받는다. 
        var list = RecordManager.instance.GetRandomRewardMemory(count);
        if (list == null)
        {
            Debug.Log("SetRecordRewardList Error : list null");
            return;
        }

        rewardCards.Clear();
        // 보상 카드 리스트 세팅
        foreach(var record in list)
        {
            if (record == null) continue;

            Sprite sprite = Resources.Load<Sprite>(record.spritePath);
            // 카드에 데이터 넣어준다. 
            RewardCard rewardCard = new RewardCard(RewardType.RECORD, sprite, record.name, 0.0f);
            rewardCard.CreateMemoryReward(record.id);
            rewardCards.Add(rewardCard);
        }

        if (cardParent == null)
            return;

        int childCount = cardParent.transform.childCount;

        // 개수 검사
        if(childCount != rewardCards.Count)
        {
            int diff = Mathf.Abs(childCount - rewardCards.Count);
            // 개수가 다르면 오브젝트를 그 수치만큼 생성한다.
            for (int i = 0; i < diff; i++)
            {
                Instantiate(rewardCardSlot, cardParent.transform);
            }
        }

        int index = 0;
        foreach (RewardCard rewardCard in rewardCards)
        {
            if (rewardCard == null) continue;
            
            var cardSlot = cardParent.transform.GetChild(index);
            // 특정 컴포넌트를 찾아 기능을 던진다. 
            if (cardSlot.TryGetComponent<RewardCardSlot>(out var slot))
            {
                if (slot != null)
                {
                    // 슬롯 UI 그리기
                    //slot.DrawUiObject(
                    //    rewardCard.rewardSprite, rewardCard.name, rewardCard.description);
                    slot.DrawUiObjectByRecord(rewardCard);
                    // 슬롯을 찾아서 슬롯에 버튼 기능을 넣어준다.
                    var targetIndex = index;
                    slot.SetButtonListener(()=>SelectRewardCardUI(targetIndex));
                }
            }
            index++;
        }

        // 결과
    }


    // 결과 카드들을 그려주게 하는 함수 .
    public void DrawRewardCards()
    {
        if (UIPageManager.instance == null) return;

        if (rewardCards.Count <= 0) return;

        UIPageManager.instance.OpenClose(go_BaseUI);

        cardParent.SetActive(true);
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
        // 현재 보여주는 보상에 따라 기능이 달라지게 해본다.
        if (selectIndex <= initSelectIndex || rewardCards.Count <= selectIndex) return;

        var reward = rewardCards[selectIndex];
        // todo. 타입별로 기능이 추가되면 여기도 추가해야한다. 
        if(reward.rewardType == RewardType.RECORD)
        { 
            if(RecordManager.instance != null)
            {
                RecordManager.instance.SelectRecord(reward.recordID);
            }
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
