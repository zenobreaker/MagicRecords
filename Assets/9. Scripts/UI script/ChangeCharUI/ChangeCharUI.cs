using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


// ���� �󿡼� �÷��� ���� ĳ���͸� �����ϴ� ����� ���ִ� UI 
public class ChangeCharUI : MonoBehaviour
{
    // ĳ���� ���� ����� �ִ� 3�� ���� ������ �� ��������
    // �÷��� ���� ĳ���͸� ������ ����� �����ϴ� ĳ���Ͱ� ��ġ�Ǿ�����. 
    // ��ġ�� �������� ������ �ش� ĳ���ͷ� ȭ�� ��ȯ �� ĳ���͸� ������ �� �հ� �ǰ�
    // ������ �÷����� ĳ���ʹ� ����� �����ϰ� ����ȴ�.

    // ����� ĳ���Ͱ� ǥ��Ǵ� ���Ե� 
    public CharSlot[] charSlots;
    public List<Character> charList = new List<Character>();

    public Image portraitImage; 

    private void Update()
    {
        if(Input.GetKeyDown("1") && charList.Count >= 2)
        {
            ChangeTargetWheeler(charList[1]);
        }
        else if(Input.GetKeyDown("2") && charList.Count >= 3)
        {
            
            ChangeTargetWheeler(charList[2]);
        }
    }
    public void InitChangeUI()
    {
        if (GameManager.MyInstance == null)
            return; 

        charList = GameManager.MyInstance.GetMyTeamCharList();

        // ü���� UI ��ư�� ��� �߰��ϱ� 
        SetCharacterListUI(); 

        // UI �׷��ֱ� 
        DrawCharacaterListUI(); 
    }

    // ��� ĳ���ͷ� ĳ���� ����
    public void ChangeTargetWheeler(Character target)
    {
        if (target == null || GameManager.MyInstance == null)
            return;

        // ���ӸŴ������� ���� ����
        GameManager.MyInstance.ChangeControlWheeler(target);

        charList = GameManager.MyInstance.GetMyTeamCharList();

        // ����Ʈ �ٽ� �׸��� 
        DrawCharacaterListUI();

        // ���Կ� ����� �ٽ� �����Ѵ�.
        SetCharacterListUI();

        DrawMainUI();
    }


    // ü��¡ UI�� �׷��� ������ �׷��ش�
    public void DrawCharacaterListUI()
    {
        // ����Ʈ ������Ʈ ���� ��Ȱ��ȭ�ϱ� 
        for (int i = 0; i < charSlots.Length; i++)
        {
            charSlots[i].gameObject.SetActive(false);
        }

        int count = 0; 
        // �����Ͱ� �ִ� ��쿡 ������Ʈ ���� �־��ش�. 
        for (int i = 0; i < charList.Count; i++)
        {
            var character = charList[i]; 
            // �� ó�� ������ �÷��̾���̹Ƿ� �ѱ��. 
            if (i == 0)
                continue;

            CharSlot charSlot = charSlots[count];
            charSlot.SetPlayer(character);
            // �� ��° ĳ���� ������ ù ��° ĳ���� ���Կ� �׷����´�.
            charSlots[count].gameObject.SetActive(true);
            count++;
        }

    }

    // ü���� UI ��ư�� ��� �߰��ϱ� 
    public void SetCharacterListUI()
    {
        int count = 0; 
        for (int i = 0; i < charList.Count; i++)
        {
            var character = charList[i];
            if (i == 0) continue; 

            CharSlot charSlot = charSlots[count];
            charSlot.SetCallback(() =>
            {
                Debug.Log("�ʱ�ȭ �� :");
                ChangeTargetWheeler(character);
            });
            count++; 
        }
    }

    // ���� UI�� ���� (HP, ��ų����)
    public void DrawMainUI()
    {
        if (portraitImage == null || 
            PlayerDatabase.instance == null || 
            charList.Count <= 0)
            return;
        
        var data = PlayerDatabase.instance.GetCharacterData(charList[0].MyID);
        if (data == null) return;

        portraitImage.sprite = data.portrait;
    }



}
