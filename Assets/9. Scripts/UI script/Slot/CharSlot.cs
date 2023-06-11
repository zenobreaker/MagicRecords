using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class CharSlot : MonoBehaviour, IPointerClickHandler
{
    public Character targetPlayer;
    bool isSelected = false;
    public ChoiceAlert choiceAlert;

    public delegate void Callback();

    [SerializeField] Image img_CharIcon;
    private Callback callback;


    public void SetPlayer(Character p_Player)
    {
        targetPlayer = p_Player;
    }

    public Character GetPlayer()
    {
        return targetPlayer;
    }

    public void DrawSlotState()
    {
        SetSelectedSlot(isSelected);
    }

    public void SetSelectedSlot(bool isSelect)
    {
        if (img_CharIcon == null)
        {
            return;
        }
        Color t_color = img_CharIcon.color;

        if (isSelect == true)
            t_color.a = 0.5f;
        else
        {
            t_color.a = 1f;
        }

        isSelected = isSelect;
        img_CharIcon.color = t_color;
    }

    public void SetCallback(Callback call)
    {
        callback += call;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (callback != null)
        {
            callback();
        }

        // 눌러지면 여부에 따라 변하도록 슬롯이 변하도록 한다.
        // todo 이건 이상하다 슬롯은 어떠한 오브젝트의 속해있는 개념으로 접근해야한다.. 
        if(choiceAlert != null)
        {
            choiceAlert.SelectPlayerSlot(targetPlayer);
            choiceAlert.DrawSlotList();
        }
     

        DrawSlotState();

            //if (this.CompareTag("Info"))
            //  InfoManual.MyInstance.SelectPlayer(targetPlayer);
            //else if (CompareTag("SelectAlert"))
            //    InfoManual.MyInstance.CallSelectPlayerFromCA(targetPlayer);
            //else
            //{
            //    SetSelectedSlot(false);
            //    InfoManual.MyInstance.SelectPlayer(null);
            //}
    }
}
