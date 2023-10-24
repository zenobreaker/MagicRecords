using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : MonoBehaviour
{

    public static bool isOpen = false; 

    [Header("UI")]
    [SerializeField] private Text txt_SkillName = null;
    [SerializeField] private Text txt_SkillDesc = null;
    [SerializeField] private Image image_SkillImage = null;
    [SerializeField] private Sprite image_emptyImage = null;
    [SerializeField] Button registBtn = null;
    [SerializeField] Button cancelBtn = null;
    [SerializeField] Button upgradeBtn = null;

    [Space(10)]
    [Header("필요한 컴포넌트")]
    [SerializeField] GameObject go_Base = null;

    private Skill selectedSkill; 


    public void ShowToolTip(Skill skill)
    {
        if (!go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);
        
        // 
        selectedSkill = skill;

        // 스킬 이름 설정 
        txt_SkillName.text = skill.MyName;
        if (SkillDataBase.instance != null)
        {
            // 스킬 설명 설정
            txt_SkillDesc.text = SkillDataBase.instance.GetSkillDesc(skill);
        }

        // 스킬 아이콘 
        if (skill.MyIcon != null)
            image_SkillImage.sprite = skill.MyIcon;
        else
            image_SkillImage.sprite = image_emptyImage;


        // 버튼 그리기 
        // 강화 버튼 
        if (skill.upgradeCost <= InfoManager.coin)
        {
            upgradeBtn.interactable = true; 
        }
        else
        {
            upgradeBtn.interactable = false; 
        }

       

    }

    public void HideToolTip()
    {
        isOpen = false;
        if (go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);
        //go_Base.SetActive(false);
    }


    // 툴팁 갱신 함수 
    public void UpdateTooltip(SkillSlot skillSlot)
    {
        if (skillSlot == null) return;

        UpdateTooltip(skillSlot.skill);

        ApplyBtnHandle(skillSlot.isUsed);
    }

    public void UpdateTooltip(Skill skill)
    {
        txt_SkillDesc.text = SkillDataBase.instance.GetSkillDesc(skill); 

        if (skill.upgradeCost <= InfoManager.coin)
        {
            upgradeBtn.interactable = true;
        }
        else
        {
            upgradeBtn.interactable = false;
        }
  
    }


    public void RegistSkill()
    {
        // 장착 버튼 그리기
        if (selectedSkill != null && selectedSkill is PassiveSkill)
        {
            registBtn.gameObject.SetActive(false);
        }
        else
        {
            registBtn.gameObject.SetActive(true);
        }
        cancelBtn.gameObject.SetActive(false);
    }

    public void CancelSkill()
    {

        registBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(true);
    }


    public void ApplyBtnHandle(bool _isCheck)
    {
        if (_isCheck)
        {
            CancelSkill();
        }
        else
        {
            RegistSkill();
        }
    }
}
