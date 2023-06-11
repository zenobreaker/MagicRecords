using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceAlert : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUI = null;

    [SerializeField] CharSlot charSlot = null;
    [SerializeField] GameObject slotPannel = null;
    [SerializeField] Button confirmButton = null;

    public Dictionary<int, CharSlot> charSlots = new Dictionary<int, CharSlot>();


    bool isConfirm = false;
    Character selectPlayer;
    Slot targetSlot;

    public delegate void EndDelegate<T>(T t);
    EndDelegate<Character> endDelegate;    // 팝업이 종료 시에 실행되는 델리게이트 

    public enum UISELECT {NONE,SKILLMANUAL, INVENTORY, ENTER_GAME};

    public UISELECT uiSELECT;


    private void OnEnable()
    {
        isConfirm = false;

        endDelegate = null; 
    }

    private void OnDisable()
    {
        if(endDelegate != null && isConfirm == true)
        {
            endDelegate.Invoke(selectPlayer);
            // ui가 꺼지면 기능을 전부 제거 
            endDelegate = null;
        }
    }

    public void ActiveAlert(bool isAct)
    {
        //go_BaseUI.SetActive(isAct);
        UIPageManager.instance.OpenClose(go_BaseUI);

        if (isAct == true)
        {
            // 버튼의 있는 기능 전부 제거 
            confirmButton.onClick.RemoveAllListeners(); 

            SetList(InfoManager.instance.GetMyPlayerInfoList());
            ClearSelectSlot();
        }
    }

    public void SelectPlayerSlot(Character p_Target)
    {

        // 선택한 대상은 변수에 담고 같은 대상을 고르거나 다른 대상을 고르면 변수값을 초기화한다.
        // 선택한 캐릭터 정보를 
        // 변수값 상태에 따라 슬롯은 선택 상태 여부를 그린다ㅣ. 
        if (p_Target != null)
        {
            selectPlayer = p_Target;
            Debug.Log("선택됨 " + selectPlayer.MyID);
        }
    }


    // 캐릭터 슬롯 만들기위한 리스트 설정
    void SetList(List<Character> p_List)
    {

        if (charSlots.Count != p_List.Count)
        {
            
            if (charSlots.Count < p_List.Count)
            {
                for (int i = charSlots.Count; i < p_List.Count; i++)
                {
                    CharSlot clone = Instantiate(charSlot, slotPannel.transform);
                    clone.SetPlayer(p_List[i]);
                    clone.tag = "SelectAlert";
                    clone.choiceAlert = this;

                    // 콜백 함수 연결 
                    clone.SetCallback(() =>
                    {
                        ClearSelectSlot();
                        clone.SetSelectedSlot(true);
                    });
                    charSlots.Add(i, clone);

                    
                }
            }
            else if (charSlots.Count > p_List.Count)
            {
                for (int i = p_List.Count; i < charSlots.Count; i++)
                {
                    charSlots[i].gameObject.SetActive(false);
                }
            }
        }

        for (int i = 0; i < charSlots.Count; i++)
        {
            charSlots[i].gameObject.SetActive(true);
        }

    }

   

    public void DrawSlotList()
    {
        for (int i = 0; i < charSlots.Count; i++)
        {
            charSlots[i].DrawSlotState();
        }
    }

    void ClearSelectSlot()
    {
        for (int i = 0; i < charSlots.Count; i++)
        {
            charSlots[i].SetSelectedSlot(false);
        }
    }

    public void SetSlot(Slot _Target)
    {
        targetSlot = _Target;
    }


    // 확인 버튼을 누르면
    // 해당 캐릭터를 선택해서 스테이지로 들어가거나 UI를 연다 
    public void ConfirmSelect(EndDelegate<Character> del)
    {
        if (confirmButton == null) return;

        isConfirm = true; 
        confirmButton.onClick.AddListener(()=>
        {
            // 선택된게 없으면 아무것도 하지 않는다.
            if (selectPlayer == null)
            {
                return; 
            }

            endDelegate += del;
            ActiveAlert(false);
        });
   
    }



    public void CancelSelectChar()
    {
        ClearSelectSlot();
        
        ActiveAlert(false);

        if (InfoManual.MyInstance != null)
        {
            InfoManual.MyInstance.SelectPlayer(null);
        }

        endDelegate = null; 
    }

    
    // 
    public Character GetSelectedCharacter()
    {
        return selectPlayer;
    }
}
