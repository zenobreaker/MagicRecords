using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToastMessage : MonoBehaviour
{
    // �佺Ʈ �޼��� Ŭ����

    [SerializeField]
    TextMeshProUGUI mainText;

    public float fadeDuration;      // ������� �ð�     


    // �佺Ʈ �޼����� ����� �ִ� �Լ� 
    public void CreateToastMessage(string context)
    {
        // 1. �ؽ�Ʈ ������ �����ش� 
        DrawText(context);

        // 2. ���� ������Ʈ�� Ȱ��ȭ�Ѵ�.
        this.gameObject.SetActive(true);
    }

    // �佺Ʈ ���� �� �佺Ʈ�� ���Ե� �ؽ�Ʈ UI�� �Ű������� ���� ������ �ۼ��Ѵ�.
    public void DrawText(string context)
    {
        if (mainText == null) return; 

        mainText.text = context;
    }

    // �佺Ʈ�� ���� ��Ű�� �żҵ�
    public void AppearMessage()
    {
        if (mainText == null)
            return;

        // ������� �޼ҵ�� �ؽ�Ʈ�� ���İ� 0���� ���������Ƿ� �÷��ֱ� 
        mainText.DOFade(1.0f, 0.0f);

        // todo ���尡 �ִٸ� ���� ���� �߰��ϱ� 
    }

    // �佺Ʈ�� ����� �� ȣ��ȴ�. 
    // �佺Ʈ �޼����� ������� �Ѵ�. 
    public void DisappearMessage()
    {
        if (mainText == null)
            return;
        
        mainText.DOFade(0.0f, fadeDuration);
    }

    // �佺Ʈ ������Ʈ�� ��Ȱ��ȭ �����ش�. 
    public void EndToastMessage()
    {
        // �佺Ʈ �޼��� ������Ʈ�� �̺�Ʈ�� ������ ������ �̺�Ʈ�� �߰��Ѵ�.
        this.gameObject.SetActive(false);
    }
    
}
