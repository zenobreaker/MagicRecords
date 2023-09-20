using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillQuickSlot : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;
    public SkillSlotNumber slot;
    private Skill skill;
    public bool isChainSkillSlot;   // 이 슬롯이 체인 스킬 전용 슬롯인지 여부 확인용 

    public GameObject chainImage;
    public GameObject selectUIGroup; // 선택자 UI 

    public Button cancelButton;
    public Button chainSkillButton; 

    public bool isUiView = false;
    [SerializeField] private Text skillKeyText;
    [SerializeField] private Image slotImage;
    [SerializeField] private Text skillNameText; 
    public Sprite emptyImage;

    delegate void Callback();
    Callback cancelCallback;
    Callback chainCallback;

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

        if (skill == null && skill is ActiveSkill)
        {
            SkillManual.instance.RegistToQuickSlot(this);
        }
        else
        {
            SkillManual.instance.SelectSkillSlot(this);
            
            // 스킬이 있다면 UI를 그린다. 
            DrawSelectUIGroup(); 
        }
        
        if (isChainSkillSlot == true)
        {
            ChainSkillSetting.instance.CheckChainSkillCount();
        }
    }

    public void InitSkillSlot()
    {
        if(selectUIGroup != null)
        {
            selectUIGroup.SetActive(false);
        }
    }

    public void DrawSelectUIGroup()
    {
        if (selectUIGroup == null) return; 


        if(selectUIGroup.activeSelf == true)
        {
            selectUIGroup.SetActive(false);
        }
        else
        {
            selectUIGroup.SetActive(true);

            // 켜질 때 스킬이 있다면 체인 스킬 버튼을 활성화 한다.
            // todo 이후 추가 조건이 필요하면 여기 수정 
            if(skill != null &&
                chainSkillButton != null )
            {
                chainSkillButton.gameObject.SetActive(true);
            }
            else
            {
                chainSkillButton.gameObject.SetActive(false);
            }
        }

        if(skillNameText != null)
        {
            if (skill != null)
            {
                skillNameText.text = skill.MyName;
            }
            else
            {
                skillNameText.text = ""; 
            }
        }
    }

    public void DrawChainUi(bool isChain )
    {
        if (chainImage == null) return; 
     
        chainImage.SetActive(isChain);
    }

    public void SetCancelButtonAction(UnityAction call)
    {
        if (cancelButton == null) return;
        if (call != null)
        {

            cancelButton.onClick.AddListener(() =>
            {
                DrawSelectUIGroup();
                call.Invoke();
            });
        }
    }


    public void SetChaiSkillButton(UnityAction call)
    {
        if (chainSkillButton == null) return; 
        if(call != null)
        {
            chainSkillButton.onClick.AddListener(() =>
            {
                DrawSelectUIGroup();
                call.Invoke();
            });
        }
    }

}
