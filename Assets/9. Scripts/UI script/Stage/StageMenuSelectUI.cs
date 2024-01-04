using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    private List<StageAppearInfo> infoList = new List<StageAppearInfo>();

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

    private void OnDisable()
    {
        infoList.Clear();
    }


    public int GetSelectEvent()
    {
        return selectIconNumber;
    }

    // 
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

        if (infoList.Count > 0 && selectIconNumber>-1)
        {
            var testInfo = infoList[selectIconNumber];

            if (testInfo != null )
            {
                StageType type = testInfo.stageType;
                if(type == StageType.BATTLE)
                {
                    StringBuilder idList = new StringBuilder();
                    foreach(var id  in testInfo.appearIDList )
                    {
                        idList.Append(id.ToString()+" ");
                    }
                    var stageID = testInfo.stageID;
                    // todo 
                    Debug.Log( "스테이지 ID" + stageID  + "등장하는 몬스터 ID 리스트" + idList);
                }
            }
        }


        // 선택된것에 따라 버튼 UI 활성화 조정
        if (selectIconNumber != -1 && confirmButton != null)
        {
            confirmButton.interactable = true;
        }
        else if (selectIconNumber == -1 && confirmButton != null)
        {
            confirmButton.interactable = false;
        }

    }

    // 이벤트 슬롯 배치
    public void DeploySelectEventSlot(ref StageNodeInfo stageTable)
    {
        if (stageTable == null) return;

        if (stageTable.eventInfoList == null) return;

        if (contentObject == null) return;

        // UI가 있는지 검사
        int childCount = contentObject.transform.childCount;
        int infoCount = stageTable.stageAppearInfos.Count;
        if (childCount < infoCount)
        {
            for (int i = 0; i < stageTable.stageAppearInfos.Count; i++)
            {
                Instantiate(eventSlot, contentObject.transform);
            }
        }

        // 사전에 꺼둔다
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

            // 버튼 기능할당
            var button = slot.GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectEventIcon(index));
            }

            // �̹��� ��ü 
            var slotImage = slot.GetComponent<Image>();
            var appearInfo = stageTable.stageAppearInfos[i];
            infoList.Add(appearInfo);
            DrawStageIcon(slotImage, appearInfo);
        }
    }


    // UI에 정보를 토대로 아이콘을 그린다.
    void DrawStageIcon(Image image, StageAppearInfo stageAppearInfo)
    {
        if (image == null || stageAppearInfo == null)
            return;


        Sprite sprite = null;
        switch (stageAppearInfo.stageType)
        {
            case StageType.BATTLE:
                if (stageAppearInfo != null)
                {
                    if (stageAppearInfo.monsterGrade == MonsterGrade.NORMAL)
                        sprite = Resources.Load<Sprite>("Image/Icon_Monster1");
                    if (stageAppearInfo.monsterGrade == MonsterGrade.ELITE)
                        sprite = Resources.Load<Sprite>("Image/Icon_Monster2");
                    if (stageAppearInfo.monsterGrade == MonsterGrade.BOSS)
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
