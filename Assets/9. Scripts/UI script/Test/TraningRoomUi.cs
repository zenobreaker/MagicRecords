using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraningRoomUi : UiBase
{
    // �ʿ��� ������Ʈ�� 
    public GameObject buttonBase;
    public GameObject scrollBase;

    public Button firstSummonButton; 

    // ī�װ� 
    public Button normalMonsterButton;  // �Ϲ� ���� ��ȯ ��ư
    public Button bossMonsterButton;    // ���� ���� ��ȯ ��ư 


    public GameObject contentObject; 
    public GameObject imageSlot;    // ��ũ�� �信 ���� ������ ������Ʈ 
    
    private GameObject prevSelectSlot;  // ������ ������ ������Ʈ
    private CharacterData selectData;   // ������ ������ 

    public TextMeshProUGUI nameText;   // ������ ����� �̸��� ǥ�����ִ� �ؽ�Ʈ
    // ��ȯ ��ư 
    public Button summonButton;

    // PRIVATE - ������ ������ ���ؼ� ������ ������Ʈ�� �ʱ�ȭ 
    private void InitSelectObject()
    {
        // ui�� ��Ÿ���� �̸� ���� 
        if (nameText != null)
        {
            nameText.text = "";
        }

        // ��ȯ ��ư Ȱ��ȭ 
        summonButton.interactable = false;

    }

    public void OnEnable()
    {
        InitSelectObject();
    }

    public void SetVisibleButton()
    {
        if(firstSummonButton  != null)
        {
            firstSummonButton.gameObject.SetActive(true);
        }
    }

    // 1. �� ��ȯ ��ư Ŭ�� 
    public void SummonEnemyButton()
    {
        if (buttonBase == null) return;
        // 2. ���̽��� ���� 
        buttonBase.SetActive(!buttonBase.activeSelf);
        
        if(scrollBase != null)
            scrollBase.SetActive(!buttonBase.activeSelf);
    }


    // 3. ���� �� �ϳ��� ���� (�Ϲ�/����)
    public void SummonTargetGradeEnemyMonster(int grade)
    {
        // �����ͺ��̽����� ���� ������ �����´�.
        var list = MonsterDatabase.instance.GetCharacterList((MonsterGrade)grade);
        if (list == null) return;

        // 
        DrawMonsterGroupUi(list);
    }

    //PRIVATE - ���Կ� ��� �����ϱ� 
    private void SetImageSlot(GameObject subObject, CharacterData data)
    {
        if (subObject == null || data == null)
            return;

        // �̹��� ���� 
        var image = subObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = data.portrait;
        }

        // ��ư �̺�Ʈ �߰� 
        var button = subObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                // ������ ������ ���������� �˻�
                bool isPrevSelected = false; 
                if(selectData != null)
                {
                    if (selectData.characterID == data.characterID)
                        isPrevSelected = true; 
                }

                // ������ ������ ���� ���� �ʱ�ȭ.
                InitSelectObject(); 

                // ������ ������ ������ ���ٸ� �� �̻� �������� �ʵ��� 
                if(isPrevSelected == true)
                {
                    return; 
                }
                // ������ ������ ���� 
                selectData = data; 

                // ui�� ��Ÿ���� �̸� ���� 
                if (nameText != null)
                {
                    nameText.text = data.name;
                }

                // ��ȯ ��ư Ȱ��ȭ 
                summonButton.interactable = true; 

                // �� ������Ʈ�� select ui ���ֱ� 
                // ������ ������ ������Ʈ�� �մٸ� �ش� ������Ʈ�� selectui�� ����.
                if (prevSelectSlot != null)
                {
                    prevSelectSlot.transform.GetChild(0)?.gameObject.SetActive(false);
                }
                // ������ �� ������Ʈ�� ���� ���� �־��ֱ� 
                prevSelectSlot = subObject; 

            });
        }
    }


    // 4. �ش� ī�װ����� ������ ���� UI ���� 
    public void DrawMonsterGroupUi(List<CharacterData> list)
    {
        if (list == null || scrollBase == null ||
            contentObject == null || imageSlot == null )
            return;

        // ���� ����Ʈ �� ��ŭ ui ��ư �����
        int count = 0; 
        foreach(var data in list)
        {
            if (data == null) continue; 

            // �̹� �θ� ������Ʈ�� ������ ������Ʈ���� �ִٸ� �װͺ��� ��� 
            if(contentObject.gameObject.transform.childCount >= count+1)
            {
                var subObjectTransform = contentObject.gameObject.transform.GetChild(count);
                if(subObjectTransform == null)
                {
                    count++;
                    continue;
                }

                SetImageSlot(subObjectTransform.gameObject, data);
            }
            // ���ٸ� ���� ���� 
            else
            {
                var subObject = Instantiate(imageSlot, contentObject.gameObject.transform);
                SetImageSlot(subObject, data);
            }

            count++; 
        }

        // ���̽� ���ֱ� 
        if (scrollBase != null)
            scrollBase.SetActive(true);
    }

    // 5. ��ȯ��ư�� ������ ���� ���� �� ������ ��� UI�� ������ ���� ��ȯ 
    public void SummonButtonEvent()
    {
        // ���� �ִ� UI ���� ���̽�, ī�װ� �з� ���̽� ���� ����.
        if (scrollBase != null)
        {
            scrollBase.SetActive(false);
        }

        if(buttonBase != null)
        {
            buttonBase.SetActive(false);
        }


        // ���� ��ȯ ��ƾ
        // todo ���� �Ŵ����� ���� ���͸� ��ȯ�ϰ� �Ѵ�. 
        if (selectData == null)
        {
            Debug.Log("Not exist the selected Data");
            return; //  ���õ� �����Ͱ� ������ �ȵȴ�. 
        }

        GameManager.MyInstance?.RespwanTrainingBot(selectData);

    }





}
