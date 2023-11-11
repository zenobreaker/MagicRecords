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

    public void ClearSlot()
    {
        img_SkillImage.sprite = sprite_NonSlot;
        text_SkillName.text = "";
        text_SkillLevel.text = "";
        text_SkillDesc.text = "";
        this.skill = null;
        isUsed = false; 
    }

    public void SetSlot(PageSkill pageSkill, bool isPassive = false )
    {
        if (pageSkill == null) return;

        SetSlot(pageSkill.skill);
        
        IsUsedSlot(pageSkill.isUsed, pageSkill.isChain);
    }

    public void SetSlot(Skill skill)
    {
        if (skill != null)
        {
            this.skill = skill;
            img_SkillImage.sprite = skill.MyIcon;
            text_SkillName.text = skill.MyName;

            //스킬 레벨 텍스트 
            text_SkillLevel.text = "Lv" + skill.MySkillLevel;
            
            // 스킬 레벨 체크 
            // 다음 레벨 : upgrade 
            if (skill.MySkillLevel < skill.MySkillMaxLevel)
            {
                text_SkillDesc.text =
                    "Next Level : "
                    + (skill.MySkillLevel+1).ToString() +"-> Coin : "+ skill.upgradeCost;
            }
            else
            {
                text_SkillDesc.text = "스킬 레벨을 더 이상 올릴 수 없습니다.";
            }
        }
        else
        {
            img_SkillImage.sprite = sprite_NonSlot;
            text_SkillName.text = "";
            text_SkillLevel.text = "";
            text_SkillDesc.text = "";
            this.skill = null;
        }
    }

    public void IsUsedSlot(bool p_isUsed ,bool isChainSlkill=false, bool isPassive = false)
    {
        isUsed = p_isUsed;

        if (p_isUsed)
        {
            this.go_usedSlotUI.SetActive(true);

            if (isChainSlkill)
                this.go_usedSlotUI.GetComponentInChildren<Text>().text = "체인스킬";
            else
            {
                if (isPassive == false)
                {
                    this.go_usedSlotUI.GetComponentInChildren<Text>().text = "장착됨";
                }
                else
                {
                    this.go_usedSlotUI.GetComponentInChildren<Text>().text = "";
                }
            }
        }
        else
            this.go_usedSlotUI.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (skill != null)
        {
            skillToolTip.ShowToolTip(skill);
            SkillManual.instance.SelectSkillSlot(this);
            skillToolTip.ApplyBtnHandle(isUsed);
        }
    }

    public void UpdateTooltip(Skill p_skill)
    {
        if (p_skill.MySkillLevel < p_skill.MySkillMaxLevel)
        {
            text_SkillLevel.text = "Lv. " + p_skill.MySkillLevel.ToString();
            // 다음 레벨 : upgrade 
            if (skill.MySkillLevel < skill.MySkillMaxLevel)
            {
                text_SkillDesc.text =
                    "Next Level : "
                    + (skill.MySkillLevel + 1).ToString() + "-> Coin : " + skill.upgradeCost;
            }
            else
            {
                text_SkillDesc.text = "스킬 레벨을 더 이상 올릴 수 없습니다.";
            }
        }
        else
        {
            text_SkillLevel.text = "Lv. M";
            text_SkillDesc.text = "스킬 레벨이 최대입니다. ";
        }
    }

}
