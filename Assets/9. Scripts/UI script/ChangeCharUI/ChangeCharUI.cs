using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


// 게임 상에서 플레이 중인 캐릭터를 변경하는 기능을 해주는 UI 
public class ChangeCharUI : MonoBehaviour
{
    // 캐릭터 변경 기능은 최대 3개 까지 가져온 팀 구성에서
    // 플레이 중인 캐릭터를 제외한 오토로 동작하는 캐릭터가 배치되어진다. 
    // 배치된 아이콘을 누르면 해당 캐릭터로 화면 전환 및 캐릭터를 조작할 수 잇게 되고
    // 이전에 플레이한 캐릭터는 오토로 동작하게 변경된다.

    // 변경될 캐릭터가 표기되는 슬롯들 
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

        // 체인지 UI 버튼에 기능 추가하기 
        SetCharacterListUI(); 

        // UI 그려주기 
        DrawCharacaterListUI(); 
    }

    // 대상 캐릭터로 캐릭터 변경
    public void ChangeTargetWheeler(Character target)
    {
        if (target == null || GameManager.MyInstance == null)
            return;

        // 게임매니저에게 정보 전달
        GameManager.MyInstance.ChangeControlWheeler(target);

        charList = GameManager.MyInstance.GetMyTeamCharList();

        // 리스트 다시 그리기 
        DrawCharacaterListUI();

        // 슬롯에 기능을 다시 설정한다.
        SetCharacterListUI();

        DrawMainUI();
    }


    // 체인징 UI에 그려질 내용을 그려준다
    public void DrawCharacaterListUI()
    {
        // 리스트 오브젝트 전부 비활성화하기 
        for (int i = 0; i < charSlots.Length; i++)
        {
            charSlots[i].gameObject.SetActive(false);
        }

        int count = 0; 
        // 데이터가 있는 경우에 오브젝트 정보 넣어준다. 
        for (int i = 0; i < charList.Count; i++)
        {
            var character = charList[i]; 
            // 맨 처음 정보는 플레이어블이므로 넘긴다. 
            if (i == 0)
                continue;

            CharSlot charSlot = charSlots[count];
            charSlot.SetPlayer(character);
            // 두 번째 캐릭터 정보는 첫 번째 캐릭터 슬롯에 그려놓는다.
            charSlots[count].gameObject.SetActive(true);
            count++;
        }

    }

    // 체인지 UI 버튼에 기능 추가하기 
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
                Debug.Log("초기화 중 :");
                ChangeTargetWheeler(character);
            });
            count++; 
        }
    }

    // 메인 UI들 변경 (HP, 스킬슬롯)
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
