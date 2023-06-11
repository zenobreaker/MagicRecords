using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] SkillDescDic skillDescDic = null;
    [SerializeField] SkillManual skillManual = null;

    public void ShowToolTip(Skill _skill)
    {
        if (!go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);

        txt_SkillName.text = _skill.CallSkillName;
        txt_SkillDesc.text = skillDescDic.SetSkillDesc(_skill);

        if (_skill.MyIcon != null)
            image_SkillImage.sprite = _skill.MyIcon;
        else
            image_SkillImage.sprite = image_emptyImage;
    }

    public void HideToolTip()
    {
        isOpen = false;
        if (go_Base.activeSelf)
            UIPageManager.instance.OpenClose(go_Base);
        //go_Base.SetActive(false);
    }

    public void UpdateTooltip(Skill p_skill)
    {
        txt_SkillDesc.text = skillDescDic.SetSkillDesc(p_skill);
    }


    public void RegistSkill()
    {
        registBtn.gameObject.SetActive(true);
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
