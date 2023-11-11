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

    public Button confirmButton;

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

        if (confirmButton != null)
        {
            confirmButton.interactable = false;
        }
    }


    public int GetSelectEvent()
    {
        return selectIconNumber;
    }

    // ������ ���� ��� - ��ư�� �Ҵ�Ǵ� �̺�Ʈ 
    public void SelectEventIcon(int _select)
    {
        if (contentObject == null) return;

        // ���� ����� �������� ��� 
        if (selectIconNumber == _select)
        {
            selectIconNumber = -1;
        }
        // �������� ���� �ٸ� ��� 
        else
        {
            selectIconNumber = _select;
        }


        for (int i = 0; i < contentObject.transform.childCount; i++)
        {
            int index = i;
            // ����Ʈ���ִ� ������ ���� UI�� ���̰��Ѵ�. 
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


        // ������ �����ϰų� ���������� Ȯ�� ��ư�� Ȱ��ȭ�� ǥ��
        if(selectIconNumber != -1 && confirmButton != null)
        {
            confirmButton.interactable = true;
        }
        else if (selectIconNumber == -1 && confirmButton != null)
        {
            confirmButton.interactable = false;
        }

    }

    // �̺�Ʈ ���� ��ġ�ϱ� 
    public void DeploySelectEventSlot(ref StageTableClass stageTable)
    {
        if (stageTable == null) return;

        if (stageTable.eventInfoList == null) return;

        if (contentObject == null) return;

        // UI�� ���� ������ �̺�Ʈ�� �̺�Ʈ ������ ���Ͽ� ������ ����ų� ����
        int childCount = contentObject.transform.childCount;
        int infoCount = stageTable.eventInfoList.Count;
        if (childCount < infoCount)
        {
            for (int i = 0; i < stageTable.eventInfoList.Count; i++)
            {
                Instantiate(eventSlot, contentObject.transform);
            }
        }

        // �׷��� ������ �� ���д�.
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

            // ��ư ��� �ֱ� 
            var button = slot.GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectEventIcon(index));
            }

            // �̹��� ��ü 
            var slotImage = slot.GetComponent<Image>();
            var eventInfo = stageTable.eventInfoList[i];

            DrawStageIcon(slotImage, eventInfo);
        }
    }


    // ���������� �´� ���� �̹��� ����
    void DrawStageIcon(Image image, StageEventInfo stageEventInfo)
    {
        if (image == null || stageEventInfo == null)
            return;


        Sprite sprite = null;
        switch (stageEventInfo.stageType)
        {
            case StageType.BATTLE:
                if (stageEventInfo.appearInfo != null)
                {
                    if (stageEventInfo.appearInfo.monsterGrade == MonsterGrade.NORMAL)
                        sprite = Resources.Load<Sprite>("Image/Icon_Monster1");
                    if (stageEventInfo.appearInfo.monsterGrade == MonsterGrade.ELITE)
                        sprite = Resources.Load<Sprite>("Image/Icon_Monster2");
                    if (stageEventInfo.appearInfo.monsterGrade == MonsterGrade.BOSS)
                        sprite = Resources.Load<Sprite>("Image/Icon_Monster3");
                }
                break;
            case StageType.EVENT:
                //todo �̺�Ʈ id�� ���� �̹����� �޸� �Ҽ��յ���
                sprite = Resources.Load<Sprite>("Image/Event");
                break;
            case StageType.SHOP:
                sprite = Resources.Load<Sprite>("Image/Shop");
                break;
        }


        image.sprite = sprite;
    }
}

