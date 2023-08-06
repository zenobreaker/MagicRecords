using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RewardCardSlot : MonoBehaviour
{
    // ������ �̹��� 
    public Image icon;
    // ī�� �̸� �ؽ�Ʈ
    public Text nameText; 
    // ������ ���� �ؽ�Ʈ
    public Text text;
    // ī�� ������ 
    public GameObject selectUI;

    // ��ư
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


    // �׷��� ������Ʈ���� �ʱ�ȭ�Ѵ�.
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

        // �޴°� Ű�ڵ�� ������ �������Ŵ������� ������û�� �غ��� .
        if(LanguageManager.Instance == null)
        {
            // ���ٸ� �׳� Ű�ڵ�ä�� ��Ȱ���ѳ��� 
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
