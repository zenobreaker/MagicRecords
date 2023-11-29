
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class TrainingRoomUi : UiBase
{
    // 필요한 오브젝트
    public GameObject enemyClearButton; 
    public GameObject buttonBase;
    public GameObject scrollBase;

    public GameObject attackSwitchButton;  // 공격 전환 버튼
    public TextMeshProUGUI switchButtonText;

    public Button firstSummonButton; 
    
    // 몬스터 소환 버튼
    public Button normalMonsterButton;  // 일반 등급에 몬스터
    public Button bossMonsterButton;    // 보스 등급


    public GameObject contentObject; 
    public GameObject imageSlot;    // 배치될 이미지 슬롯 오브젝트
    
    private GameObject prevSelectSlot;  // 이전에 선택안 슬롯 오브젝트
    private MonsterData selectData;   // 선택한 정보

    public TextMeshProUGUI nameText;   // 이름 텍스트 
    // ��ȯ ��ư 
    public Button summonButton;

    private bool isAttackSwitch = true;    // 공격 설정 플래그값

    // PRIVATE - 선택한 오브젝트가 없다면 일부 UI들을 초기 상태로 돌린다. 
    private void InitSelectObject()
    {
        // ui 이름 텍스트 비워주기
        if (nameText != null)
        {
            nameText.text = "";
        }

        // 소환 버튼 비활성화
        summonButton.interactable = false;

    }

    public void OnEnable()
    {
        InitSelectObject();

        SetVisibleButton();

        VisibleEnemyClearButton();

        VisibleEnemyAttackSwitchButton();

        // 공격 버튼 텍스트 활성화
        DrawAttackSwitchButtonText();
    }

    public void SetVisibleButton()
    {
        if(firstSummonButton  != null)
        {
            firstSummonButton.gameObject.SetActive(true);
        }
    }

    // 1. 소환 버튼 그리기 
    public void SummonEnemyButton()
    {
        if (buttonBase == null) return;
        // 2. 버튼 베이스 오브젝트 활성화 
        buttonBase.SetActive(!buttonBase.activeSelf);
        
        // 스크롤베이스를 비활성화 시킨다.
        if(scrollBase != null)
        {
            scrollBase.SetActive(false);
        }
    }


    // 3. 등급을 선택하기
    public void SummonTargetGradeEnemyMonster(int grade)
    {
        if (MonsterDatabase.instance == null) return;

        var list = MonsterDatabase.instance.GetMonsterDatas((MonsterGrade)grade);
        if (list == null) return;

        DrawMonsterGroupUi(list);
    }

    //PRIVATE - 선택한 오브젝트에 해당 이미지를 할당시키는 함수
    private void SetImageSlot(GameObject subObject, MonsterData data)
    {
        if (subObject == null || data == null)
            return;

        // 이미지 컴포넌트를 가져온다. 
        var image = subObject.GetComponent<Image>();
        if (image != null)
        {
            // 가져온 이미지 컴포넌트에 해당 이미지 그린다.
            image.sprite = data.monsterSprite;
        }

        // 버튼 컴포넌트를 가져온다.
        var button = subObject.GetComponent<Button>();
        if (button != null)
        {
            // 버튼 컴포넌트에 있는 모든 이벤트 전부 제거
            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(() =>
            {
                // 이전에 선택한지 플래그
                bool isPrevSelected = false; 
                if(selectData != null)
                {
                    if (selectData.monsterID == data.monsterID)
                        isPrevSelected = true; 
                }

                // 오브젝트 초기화 
                InitSelectObject(); 

                // 이전에 선택한거랑 같다면 데이터 지우고 취소처리
                if(isPrevSelected == true)
                {
                    selectData = null;
                    return; 
                }
                // 선택한 데이터 저장 
                selectData = data; 

                // ui 텍스트에 고른 데이터 이름 그리기 
                if (nameText != null)
                {
                    nameText.text = data.monsterName;
                }

                // 소환 버튼 활성화
                summonButton.interactable = true; 

                if (prevSelectSlot != null)
                {
                    prevSelectSlot.transform.GetChild(0)?.gameObject.SetActive(false);
                }
                prevSelectSlot = subObject; 

            });
        }

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

    // 적 공격 버튼 보이기
    private void VisibleEnemyAttackSwitchButton()
    {
        if (attackSwitchButton == null) return;

        if (GameManager.MyInstance?.enemyTeam != null)
        {
            if (GameManager.MyInstance?.enemyTeam.Count > 0)
            {
                attackSwitchButton.gameObject.SetActive(true);
            }
            else
            {
                attackSwitchButton.gameObject.SetActive(false);
            }
        }
        else
        {
            attackSwitchButton.gameObject.SetActive(false);
        }
    }


    // 5. 소환 이벤트 
    public void SummonButtonEvent()
    {
        // 켜져 있는 UI 전부 끄기 
        if (scrollBase != null)
        {
            scrollBase.SetActive(false);
        }

        if(buttonBase != null)
        {
            buttonBase.SetActive(false);
        }


        // 선택된 데이터가 없다면 동작 중지
        if (selectData == null)
        {
            Debug.Log("Not exist the selected Data");
            return; 
        }

        if (GameManager.MyInstance != null)
        {
            // 이전에 등장한 적이 있다면 모든 적을 죽인다. 
            // todo 이 기능은 나누어 볼 수 있을 지도
            GameManager.MyInstance.AllClearEnemyTeam();

            // 설정한 적을 부른다. 
            GameManager.MyInstance.RespwanTrainingBot(selectData);
        }

        // 제거 버튼 보이기
        VisibleEnemyClearButton();

        // 공겨 버튼 보이기
        VisibleEnemyAttackSwitchButton();
    }



    // 소환 해제 버튼 
    public void CloseSummonButton()
    {
        if(GameManager.MyInstance != null)
        {
            GameManager.MyInstance.AllClearEnemyTeam();
        }

        VisibleEnemyClearButton();
    }


    // PRIVATE - 공격 실행버튼 텍스트 그리기
    private void DrawAttackSwitchButtonText()
    {
        if (switchButtonText != null)
        {
            if (isAttackSwitch == false)
            {
                switchButtonText.text = "Attack On";
            }
            else
            {
                switchButtonText.text = "Attack Off";
            }
        }
    }

    // 공격 시행 버튼 기능
    public void SwitchEnemyAttack()
    {
        if (GameManager.MyInstance != null)
        {
            isAttackSwitch = !isAttackSwitch;
            GameManager.MyInstance.SwitchEnmeyContol(isAttackSwitch); 
        }

        DrawAttackSwitchButtonText();
    }



}

