using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraningRoomUi : UiBase
{
    // 필요한 오브젝트들 
    public GameObject buttonBase;
    public GameObject scrollBase;

    public Button firstSummonButton; 

    // 카테고리 
    public Button normalMonsterButton;  // 일반 몬스터 소환 버튼
    public Button bossMonsterButton;    // 보스 몬스터 소환 버튼 


    public GameObject contentObject; 
    public GameObject imageSlot;    // 스크롤 뷰에 보일 아이템 오브젝트 
    
    private GameObject prevSelectSlot;  // 이전에 선택한 오브젝트
    private CharacterData selectData;   // 선택한 데이터 

    public TextMeshProUGUI nameText;   // 선택한 대상의 이름을 표시해주는 텍스트
    // 소환 버튼 
    public Button summonButton;

    // PRIVATE - 선택한 정보에 대해서 관련한 오브젝트들 초기화 
    private void InitSelectObject()
    {
        // ui에 나타나는 이름 변경 
        if (nameText != null)
        {
            nameText.text = "";
        }

        // 소환 버튼 활성화 
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

    // 1. 적 소환 버튼 클릭 
    public void SummonEnemyButton()
    {
        if (buttonBase == null) return;
        // 2. 베이스가 등장 
        buttonBase.SetActive(!buttonBase.activeSelf);
        
        if(scrollBase != null)
            scrollBase.SetActive(!buttonBase.activeSelf);
    }


    // 3. 선택 중 하나를 선택 (일반/보스)
    public void SummonTargetGradeEnemyMonster(int grade)
    {
        // 데이터베이스에서 몬스터 정보를 가져온다.
        var list = MonsterDatabase.instance.GetCharacterList((MonsterGrade)grade);
        if (list == null) return;

        // 
        DrawMonsterGroupUi(list);
    }

    //PRIVATE - 슬롯에 기능 설정하기 
    private void SetImageSlot(GameObject subObject, CharacterData data)
    {
        if (subObject == null || data == null)
            return;

        // 이미지 세팅 
        var image = subObject.GetComponent<Image>();
        if (image != null)
        {
            image.sprite = data.portrait;
        }

        // 버튼 이벤트 추가 
        var button = subObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                // 이전에 선택한 데이터인지 검사
                bool isPrevSelected = false; 
                if(selectData != null)
                {
                    if (selectData.characterID == data.characterID)
                        isPrevSelected = true; 
                }

                // 이전에 선택한 정보 전부 초기화.
                InitSelectObject(); 

                // 이전에 선택한 정보랑 같다면 더 이상 진행하지 않도록 
                if(isPrevSelected == true)
                {
                    return; 
                }
                // 선택한 데이터 설정 
                selectData = data; 

                // ui에 나타나는 이름 변경 
                if (nameText != null)
                {
                    nameText.text = data.name;
                }

                // 소환 버튼 활성화 
                summonButton.interactable = true; 

                // 이 오브젝트의 select ui 켜주기 
                // 이전에 선택한 오브젝트가 잇다면 해당 오브젝트의 selectui는 끈다.
                if (prevSelectSlot != null)
                {
                    prevSelectSlot.transform.GetChild(0)?.gameObject.SetActive(false);
                }
                // 선택한 이 오브젝트를 이전 값에 넣어주기 
                prevSelectSlot = subObject; 

            });
        }
    }


    // 4. 해당 카테고리에서 나눠진 몬스터 UI 등장 
    public void DrawMonsterGroupUi(List<CharacterData> list)
    {
        if (list == null || scrollBase == null ||
            contentObject == null || imageSlot == null )
            return;

        // 몬스터 리스트 수 만큼 ui 버튼 만들기
        int count = 0; 
        foreach(var data in list)
        {
            if (data == null) continue; 

            // 이미 부모 오브젝트에 지정한 오브젝트들이 있다면 그것부터 사용 
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
            // 없다면 새로 생성 
            else
            {
                var subObject = Instantiate(imageSlot, contentObject.gameObject.transform);
                SetImageSlot(subObject, data);
            }

            count++; 
        }

        // 베이스 켜주기 
        if (scrollBase != null)
            scrollBase.SetActive(true);
    }

    // 5. 소환버튼을 누르면 몬스터 선택 후 등장한 모든 UI가 꺼지고 몬스터 소환 
    public void SummonButtonEvent()
    {
        // 켜져 있는 UI 슬롯 베이스, 카테고리 분류 베이스 등을 끈다.
        if (scrollBase != null)
        {
            scrollBase.SetActive(false);
        }

        if(buttonBase != null)
        {
            buttonBase.SetActive(false);
        }


        // 몬스터 소환 루틴
        // todo 게임 매니저를 통해 몬스터를 소환하게 한다. 
        if (selectData == null)
        {
            Debug.Log("Not exist the selected Data");
            return; //  선택된 데이터가 없으면 안된다. 
        }

        GameManager.MyInstance?.RespwanTrainingBot(selectData);

    }





}
