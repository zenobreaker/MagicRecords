using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ChoiceAlert;

public enum RewardType { NONE = 0, COIN, EXP, ITEM, MEMORY}
[System.Serializable]
public class RewardCard
{
    public RewardType rewardType;
    public Sprite rewardSprite; // 보상 이미지 
    public float value;         // 보상 수치 
    public string description;
    public int memoryID;

    public RewardCard(RewardType rewardType, Sprite rewardSprite, float value)
    {
        this.rewardType = rewardType;
        this.rewardSprite = rewardSprite;
        this.value = value;
        SetDescriptionByType();
    }

    public void CreateMemoryReward(int memoryID)
    {
        this.memoryID = memoryID;
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
            case RewardType.MEMORY:
                // 메모리라면 해당 메모리의 문구를 가져온다.
                if (MemoryManager.instance != null)
                {
                    var memory = MemoryManager.instance.GetMemoryInfoByID(memoryID);
                    if (memory == null)
                    {
                        break; 
                    }

                    description = memory.description;
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


    public int selectIndex = 0;

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
        selectIndex = 0;
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
            selectIndex = -1;
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
    public void SetMemoryRewardList(int count)
    {
        if (MemoryManager.instance == null)
            return;

        //  보상 리스트를 받는다. 
        var list = MemoryManager.instance.GetRandomRewardMemory(count);

        rewardCards.Clear();
        // 보상 카드 리스트 세팅
        foreach(var memory in list)
        {
            if (memory == null) continue;

            Sprite sprite = Resources.Load<Sprite>(memory.spritePath);
            // 카드에 데이터 넣어준다. 
            RewardCard rewardCard = new RewardCard(RewardType.MEMORY, sprite, 0.0f);
            rewardCard.CreateMemoryReward(memory.id);
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
                    slot.DrawUiObject(rewardCard.rewardSprite, rewardCard.description);

                    // 슬롯을 찾아서 슬롯에 버튼 기능을 넣어준다.
                    var targetIndex = index;
                    slot.SetButtonListener(()=>SelectRewardCardUI(targetIndex));
                }
            }
            index++;
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
                //InfoManager.instance.money += cur_RewardCard.value;
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
