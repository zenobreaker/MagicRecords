using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraningRoomUi : UiBase
{
    // �ʿ��� ������Ʈ�� 
    public GameObject enemyClearButton; 
    public GameObject buttonBase;
    public GameObject scrollBase;

    public Button firstSummonButton; 

    // ī�װ� 
    public Button normalMonsterButton;  // �Ϲ� ���� ��ȯ ��ư
    public Button bossMonsterButton;    // ���� ���� ��ȯ ��ư 


    public GameObject contentObject; 
    public GameObject imageSlot;    // ��ũ�� �信 ���� ������ ������Ʈ 
    
    private GameObject prevSelectSlot;  // ������ ������ ������Ʈ
    private MonsterData selectData;   // ������ ������ 

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

        SetVisibleButton();

        VisibleEnemyClearButton();
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
        
        // ���� ���̽��� ���� ������ �������� �Ѵ�. 
        if(scrollBase != null)
        {
            scrollBase.SetActive(false);
        }
    }


    // 3. ���� �� �ϳ��� ���� (�Ϲ�/����)
    public void SummonTargetGradeEnemyMonster(int grade)
    {
        // �����ͺ��̽����� ���� ������ �����´�.
        if (MonsterDatabase.instance == null) return;

        var list = MonsterDatabase.instance.GetMonsterDatas((MonsterGrade)grade);
        if (list == null) return;

        // 
        DrawMonsterGroupUi(list);
    }

    //PRIVATE - ���Կ� ��� �����ϱ� 
    private void SetImageSlot(GameObject subObject, MonsterData data)
    {
        if (subObject == null || data == null)
            return;

        // �̹��� ���� 
        var image = subObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = data.monsterSprite;
        }

        // ��ư �̺�Ʈ �߰� 
        var button = subObject.GetComponent<Button>();
        if (button != null)
        {
            // �̺�Ʈ �ʱ�ȭ 
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                // ������ ������ ���������� �˻�
                bool isPrevSelected = false; 
                if(selectData != null)
                {
                    if (selectData.monsterID == data.monsterID)
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
                    nameText.text = data.monsterName;
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

        // �ش� ������Ʈ Ȱ��ȭ
        subObject.SetActive(true);
    }


    // 4. �ش� ī�װ����� ������ ���� UI ���� 
    public void DrawMonsterGroupUi(List<MonsterData> list)
    {
        if (list == null || scrollBase == null ||
            contentObject == null || imageSlot == null )
            return;

        // ������ ���� ������Ʈ�� �ִٸ� ���� ��Ȱ��ȭ ó�� 
        if(contentObject.gameObject.transform.childCount > 0)
        {
            for(int i = 0; i < contentObject.gameObject.transform.childCount; i++)
            {
                if(contentObject.gameObject.transform.GetChild(i) != null)
                    contentObject.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

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

    // �� ���� ��ư ���̰� �ϴ� ��� 
    private void VisibleEnemyClearButton()
    {
        // ���� ���ٸ� ������ �ʴ´�.
        if (enemyClearButton == null) return;


        if (GameManager.MyInstance?.enemyTeam != null)
        {
            if (GameManager.MyInstance?.enemyTeam.Count > 0)
            {
                enemyClearButton.gameObject.SetActive(true);
            }
            else
            {
                enemyClearButton.gameObject.SetActive(false);
            }

        }
        else
        {
            enemyClearButton.gameObject.SetActive(false);
        }
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

        VisibleEnemyClearButton();
    }



    // ��ȯ ���� ��ư 
    public void CloseSummonButton()
    {
        GameManager.MyInstance?.AllClearEnemyTeam();

        VisibleEnemyClearButton();
    }



}
