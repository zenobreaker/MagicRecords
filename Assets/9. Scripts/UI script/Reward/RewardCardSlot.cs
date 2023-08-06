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
        DrawInitUI();
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

    public void DrawUiObject(Sprite sprite, string name, string desc)
    {
        if (icon == null || text == null) return;

        icon.sprite = sprite;

        // 받는건 키코드기 때문에 랭귀지매니저에게 변역요청을 해본다 .
        if(LanguageManager.Instance == null)
        {
            // 없다면 그냥 키코드채로 부활시켜놓기 
            nameText.text = name;
        }
        else
        {
            nameText.text = LanguageManager.Instance.GetLocaliztionValue(name);
        }

        text.text = desc; 
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
