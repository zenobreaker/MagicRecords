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
    EndDelegate<Character> endDelegate;    // �˾��� ���� �ÿ� ����Ǵ� ��������Ʈ 

    public enum UISELECT 
    {
        NONE,
        SKILLMANUAL,
        INVENTORY, 
        ENTER_GAME
    };

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
            endDelegate = null;
        }
    }

    public void ActiveAlert(bool isAct)
    {
        //go_BaseUI.SetActive(isAct);
        UIPageManager.instance.OpenClose(go_BaseUI);

        if (isAct == true)
        {
            confirmButton.onClick.RemoveAllListeners(); 

            SetList(InfoManager.instance.GetMyPlayerInfoList());
            ClearSelectSlot();
        }
    }

    public void SelectPlayerSlot(Character p_Target)
    {

        if (p_Target != null)
        {
            selectPlayer = p_Target;
            Debug.Log("���õ� " + selectPlayer.MyID);
        }
    }


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

                    // �ݹ� �Լ� ���� 
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


    public void ConfirmSelect(EndDelegate<Character> del)
    {
        if (confirmButton == null) return;

        isConfirm = true; 
        confirmButton.onClick.AddListener(()=>
        {
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
