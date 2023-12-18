using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public int selectMaxCount; // 최대로 선택할 수 있는 개수
    public Dictionary<int, CharSlot> charSlots = new Dictionary<int, CharSlot>();


    bool isConfirm = false;
    List<Character> selectPlayers = new List<Character>();
    Slot targetSlot;

    public delegate void EndDelegate<T>(T t);
    EndDelegate<List<Character>> endDelegate;    // 팝업 종료시 실행할 델리게이트  

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
        selectPlayers.Clear();
        DrawSlotList();
    }

    private void OnDisable()
    {
        if(endDelegate != null && isConfirm == true)
        {
            endDelegate.Invoke(selectPlayers);
            endDelegate = null;
        }
    }

    public void ActiveAlert(bool isAct, int selectCount = 1)
    {
        UIPageManager.instance.OpenClose(go_BaseUI);

        selectMaxCount = selectCount;

        if (isAct == true)
        {
            confirmButton.onClick.RemoveAllListeners();

            CreateSlotByList(InfoManager.instance.GetMyPlayerInfoList());
            ClearSelectSlot();
        }
    }

    // 인자로 오는 캐릭터를 선택한 캐릭터 리스트에 추가해준다.
    public void SelectPlayerSlot(Character target)
    {
        if (target == null)
            return;

        var findTarget = selectPlayers.Find(x => x.MyID == target.MyID);
        if(findTarget == null)
        {
            selectPlayers.Add(target);
            if(selectMaxCount < selectPlayers.Count)
            {
                selectPlayers.Remove(selectPlayers.First());
            }
        }
        else
        {
            selectPlayers.Remove(findTarget);
        }

        Debug.Log("선택된 캐릭터" + target.MyID);
    }

    // 팝업에 등장하는 캐릭터 리스트 슬롯을 만들어 주는 함수 
    void CreateSlotByList(List<Character> characters)
    {

        if (charSlots.Count != characters.Count)
        {
            
            if (charSlots.Count < characters.Count)
            {
                for (int i = charSlots.Count; i < characters.Count; i++)
                {
                    CharSlot clone = Instantiate(charSlot, slotPannel.transform);
                    clone.SetPlayer(characters[i]);
                    clone.tag = "SelectAlert";
                    clone.choiceAlert = this;
                    
                    clone.SetCallback(() =>
                    {
                        SelectPlayerSlot(clone.GetPlayer());
                        DrawSlotList();
                    });
                    charSlots.Add(i, clone);

                    
                }
            }
            else if (charSlots.Count > characters.Count)
            {
                for (int i = characters.Count; i < charSlots.Count; i++)
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
            var slot = charSlots[i];
            var slotPlayer = slot.GetPlayer();
            if (slotPlayer == null)
                continue;
            
            var targetPlayer = selectPlayers.Find(x => x.MyID == slotPlayer.MyID);
            if (targetPlayer == null)
                charSlots[i].SetSelectedSlot(false);
            else
                charSlots[i].SetSelectedSlot(true);

            //charSlots[i].DrawSlotState();
        }
    }

    // 최대 선택할 수 있는 수에 따라 꺼주는 함수 호출 
    public void ClearSelectSlot()
    {
        //if(selectmaxcount== 1)
        //{
        //    clearallselectslot();
        //}
        //else
        //{
        //
         ClearPrevAtFristSelectSlot();
        //}

    }

    // 이전에 선택한 슬롯들을 UI를 꺼준다.
    void ClearAllSelectSlot()
    {
        for (int i = 0; i < charSlots.Count; i++)
        {
            charSlots[i].SetSelectedSlot(false);
        }
    }

    // 이전에 선택한 것 중 가장 먼저 선택한 슬롯 UI 꺼준다.
    public void ClearPrevAtFristSelectSlot()
    {
        // 최대로 선택 가능한 수보다 더 많이 선택되었다면 
        // 맨 처음 선택한 대상을 제거한다.
        if(selectPlayers.Count >= selectMaxCount)
        {
            var removeTarget = selectPlayers.First();

            for(int i = 0; i < charSlots.Count;i++)
            {
                var slot = charSlots[i];
                var slotPlayer = slot.GetPlayer();
                if (slotPlayer == null) 
                    continue;

                // 삭제할 대상 정보가 담긴 슬롯의 UI를 찾아 끈다
                if (removeTarget.MyID == slotPlayer.MyID)
                {
                    slot.SetSelectedSlot(false);
                    break;
                }
            }

            // 대상의 정보 제거
            selectPlayers.Remove(removeTarget);
        }
    }

    public void SetSlot(Slot _Target)
    {
        targetSlot = _Target;
    }


    public void ConfirmSelect(EndDelegate<List<Character>> del)
    {
        if (confirmButton == null) return;

        isConfirm = true; 
        confirmButton.onClick.AddListener(()=>
        {
            if (selectPlayers == null)
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
    public Character GetSelectedCharacter(int i)
    {
        if (selectPlayers[i] != null)
            return selectPlayers[i];
        else
            return null;
    }
}
