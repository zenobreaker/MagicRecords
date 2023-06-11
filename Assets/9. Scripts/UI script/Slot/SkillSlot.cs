using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, IPointerClickHandler
{
    // 스킬 매뉴얼에서 정보를 전달 받아 보여줌 
    public Skill skill; // 슬롯에 등록되어 있는 스킬 
    public bool isUsed; 
    // 필요한 컴포넌트 
    public Text text_SkillName = null;
    public Image img_SkillImage = null;
    public Sprite sprite_NonSlot;
    public Text text_SkillDesc = null;
    public Text text_SkillLevel = null;

    [SerializeField] GameObject go_usedSlotUI = null;
    [SerializeField] SkillToolTip skillToolTip = null;


    private void Start()
    {
        text_SkillDesc.text = skill.skillDesc;
    }

    public void SetSlot(Skill p_Skill)
    {
        if (p_Skill != null)
        {
            skill = p_Skill;
            img_SkillImage.sprite = p_Skill.MyIcon;
            text_SkillName.text = p_Skill.CallSkillName;
        }else
        {
            img_SkillImage.sprite = sprite_NonSlot;
            text_SkillName.text = "";
            text_SkillLevel.text = "";
            text_SkillDesc.text = "";
            skill = null;
        }
    }

    public void IsUsedSlot(bool p_isUsed ,bool isChainSlkill=false)
    {
        isUsed = p_isUsed;

        if (p_isUsed)
        {
            this.go_usedSlotUI.SetActive(true);

            if (isChainSlkill)
                this.go_usedSlotUI.GetComponentInChildren<Text>().text = "체인스킬";
            else
                this.go_usedSlotUI.GetComponentInChildren<Text>().text = "장착됨";
        }
        else
            this.go_usedSlotUI.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        /* if(skill.MyName != "")
             HandScript.MyInstance.TakeMoveable(skill);*/
        if (skill != null)
        {
            skillToolTip.ShowToolTip(skill);
            SkillManual.instance.SelectSkillSlot(this);
            skillToolTip.ApplyBtnHandle(isUsed);
        }
    }

    public void UpdateTooltip(Skill p_skill)
    {
        if (p_skill.MySkillLevel < 5)
        {
            text_SkillLevel.text = "Lv. " + p_skill.MySkillLevel.ToString();
            text_SkillDesc.text = skill.upgradeCost[skill.MySkillLevel - 1].ToString() + "->" + skill.upgradeCost[skill.MySkillLevel].ToString();
        }
        else
        {
            text_SkillLevel.text = "Lv. M";
            text_SkillDesc.text = "스킬 레벨이 최대입니다. ";
        }
    }

}
