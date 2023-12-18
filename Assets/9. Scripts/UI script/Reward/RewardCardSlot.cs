using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RewardCardSlot : MonoBehaviour
{
    // 아이콘 이미지 
    public Image icon;
    // 카드 이름 텍스트
    public Text nameText; 
    // 아이콘 설명 텍스트
    public Text text;
    // 카드 선택자 
    public GameObject selectUI;

    // 버튼
    public Button button; 

    public delegate void Callback();

    private void OnEnable()
    {
       // DrawInitUI();
    }

    private void OnDisable()
    {
        DrawInitUI();
    }


    // 그려낼 오브젝트들을 초기화한다.
    void DrawInitUI()
    {
        if (selectUI != null)
        {
            selectUI.SetActive(false);
        }

        if(text != null)
        {
            text.text = "";
        }

        icon.sprite = null;
    }

    public void DrawUiObjectByRecord(RewardCard rewardCard)
    {
        if (rewardCard == null) return;

        // 리코드 카드를 바라노니 그려지게 하라
        DrawUiObject(rewardCard.rewardSprite, rewardCard.name, "");

        // 설명문은 따로 처리한다. 

        // 레코드의 정보가져오기
        var record = RecordManager.instance.GetRecordInfoByID(rewardCard.recordID);
        if (record == null || record.specialOption == null)
            return;

        var value = record.specialOption.value;
        if(record.specialOption.isPercentage == true)
        {
            value *= 100.0f;
        }

        var duration = record.specialOption.duration;
        // 문자열 보간 형식으로 전달 받으면 위 변수로 처리하게한다.
        text.text = LanguageManager.Instance.GetLocalizationWithValues(
            rewardCard.description, duration, value, record.specialOption.isPercentage);
    }

    public void DrawUiObject(Sprite sprite, string name, string desc)
    {
        if (icon == null || text == null) return;

        icon.sprite = sprite;
        //todo 아이콘이 없기때문에 꺼둔다
        icon.gameObject.SetActive(false);

        // 받는건 키코드기 때문에 랭귀지매니저에게 변역요청을 해본다 .
        if(LanguageManager.Instance == null)
        {
            // 없다면 그냥 키코드채로 부활시켜놓기 
            nameText.text = name;
            text.text = desc;
        }
        else
        {
            nameText.text = LanguageManager.Instance.GetLocaliztionValue(name);

            text.text = LanguageManager.Instance.GetLocaliztionValue(desc); 
        }

    }

    public void DrawSelectUI(bool active)
    {
        if (selectUI != null)
        {
            selectUI.SetActive(active);
        }
    }

    public void SetButtonListener(Callback callback)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback());
    }
}
