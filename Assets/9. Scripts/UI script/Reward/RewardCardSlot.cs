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
       // DrawInitUI();
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

    public void DrawUiObjectByRecord(RewardCard rewardCard)
    {
        if (rewardCard == null) return;

        // ���ڵ� ī�带 �ٶ��� �׷����� �϶�
        DrawUiObject(rewardCard.rewardSprite, rewardCard.name, "");

        // ������ ���� ó���Ѵ�. 

        // ���ڵ��� ������������
        var record = RecordManager.instance.GetRecordInfoByID(rewardCard.recordID);
        if (record == null || record.specialOption == null)
            return;

        var value = record.specialOption.value;
        if(record.specialOption.isPercentage == true)
        {
            value *= 100.0f;
        }

        var duration = record.specialOption.duration;
        // ���ڿ� ���� �������� ���� ������ �� ������ ó���ϰ��Ѵ�.
        text.text = LanguageManager.Instance.GetLocalizationWithValues(
            rewardCard.description, duration, value, record.specialOption.isPercentage);
    }

    public void DrawUiObject(Sprite sprite, string name, string desc)
    {
        if (icon == null || text == null) return;

        icon.sprite = sprite;
        //todo �������� ���⶧���� ���д�
        icon.gameObject.SetActive(false);

        // �޴°� Ű�ڵ�� ������ �������Ŵ������� ������û�� �غ��� .
        if(LanguageManager.Instance == null)
        {
            // ���ٸ� �׳� Ű�ڵ�ä�� ��Ȱ���ѳ��� 
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
