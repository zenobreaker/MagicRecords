using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillQuickSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;
    private Skill skill;
    public bool isChainSkill;


    public bool isUiView = false; 

    [SerializeField] private Image slotImage;
    public Sprite emptyImage; 


    public Skill GetSkill()
    {
        return this.skill;
    }

    public void SetSkill(Skill _skill)
    {

        if (_skill != null)
        {
            slotImage.sprite = _skill.MyIcon;
            skill = _skill;
        }
        else
            ClearSlot();
    }

    public SkillQuickSlot GetQuickSlot()
    {
        return this;
    }

    public void ClearSlot()
    {
        skill = null;
        slotImage.sprite = emptyImage;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SkillManual.instance == null || isUiView == true) return; 

         SkillManual.instance.RegistToQuickSlot(this);
    }


}
