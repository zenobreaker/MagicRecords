using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageMenuSelectUI : MonoBehaviour
{
    public GameObject scrollviw = null; 
    public GameObject contentObject = null;
    public GameObject eventSlot = null;

    private int selectIconNumber = -1;

    [SerializeField] Sprite monsterNormalSprite;
    [SerializeField] Sprite monsterEliteSprite;
    [SerializeField] Sprite monsterBossSprite;
    [SerializeField] Sprite eventSprite;
    [SerializeField] Sprite shopSprite;


    private void OnEnable()
    {
        selectIconNumber = -1;

        if (contentObject == null) return;

        for (int i = 0; i < contentObject.transform.childCount; i++)
        {
            var slot = contentObject.transform.GetChild(i).gameObject;
            var selectUI = slot.transform.GetChild(0).gameObject;
            if (selectUI != null)
            {
                selectUI.SetActive(false);
            }
        }
    }


    public int GetSelectEvent()
    {
        return selectIconNumber;
    }

    // 아이콘 선택 기능 - 버튼에 할당되는 이벤트 
    public void SelectEventIcon(int _select)
    {
        if (contentObject == null) return;

        // 같은 대상을 선택했을 경우 
        if (selectIconNumber == _select)
        {
            selectIconNumber = -1;
        }
        // 선택하지 않은 다른 대상 
        else
        {
            selectIconNumber = _select;
        }


        for (int i = 0; i < contentObject.transform.childCount; i++)
        {
            int index = i;
            // 뷰포트에있는 슬롯의 설정 UI를 보이게한다. 
            var eventSlot = contentObject.transform.GetChild(index).gameObject;
            if (eventSlot == null) continue; 

            var selectUI = eventSlot.transform.GetChild(0);
            if (selectUI == null) continue; 

            if (selectIconNumber == index)
            {
                selectUI.gameObject.SetActive(true);
            }
            else
            {
                selectUI.gameObject.SetActive(false);
            }
        }

    }

    // 이벤트 슬롯 배치하기 
    public void DeploySelectEventSlot(ref StageTableClass stageTable)
    {
        if (stageTable == null) return;

        if (stageTable.eventInfoList == null) return;

        if (contentObject == null) return;

        // UI에 슬롯 개수와 이벤트에 이벤트 개수를 비교하여 슬롯을 만들거나 끈다
        int childCount = contentObject.transform.childCount;
        int infoCount = stageTable.eventInfoList.Count;
        if (childCount < infoCount)
        {
            for (int i = 0; i < stageTable.eventInfoList.Count; i++)
            {
                Instantiate(eventSlot, contentObject.transform);
            }
        }

        // 그려진 슬롯은 다 꺼둔다.
        for (int i = 0; i < childCount; i++)
        {
            var slot = contentObject.transform.GetChild(i);
            if (slot == null) continue;
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < infoCount; i++)
        {
            var slot = contentObject.transform.GetChild(i);
            if (slot == null) continue;

            slot.gameObject.SetActive(true);
            slot.transform.name = "slot" + i + 1;

            // 버튼 기능 넣기 
            var button = slot.GetComponent<Button>();
            if (button != null && button.onClick.GetPersistentEventCount() == 0)
            {
                int index = i; 
                button.onClick.AddListener(() => SelectEventIcon(index));
            }

            // 이미지 교체 
            var slotImage = slot.GetComponent<Image>();
            var eventInfo = stageTable.eventInfoList[i];
            if (eventInfo != null && slotImage != null)
            {
                if (eventInfo.stageType == StageType.BATTLE)
                {
                    if (eventInfo.monsterType == MonsterType.NORMAL)
                    {
                        slotImage.sprite = monsterNormalSprite;
                    }
                    else if (eventInfo.monsterType == MonsterType.ELITE)
                    {
                        slotImage.sprite = monsterEliteSprite;
                    }
                    else if (eventInfo.monsterType == MonsterType.BOSS)
                    {
                        slotImage.sprite = monsterBossSprite;
                    }
                }
                else if (eventInfo.stageType == StageType.EVENT)
                {
                    slotImage.sprite = eventSprite;
                }
                else if (eventInfo.stageType == StageType.SHOP)
                {
                    slotImage.sprite = shopSprite;
                }
            }
        }
    }


}
