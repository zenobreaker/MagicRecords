using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class ActionButton : MonoBehaviour//, IPointerClickHandler
{
    int index = 0;
    public bool isChain;
    public bool isChainReady;

    public PlayerControl playerControl;
    Queue<Skill> chainSkillQ; 
    public Skill selectedSkill = null;  // 슬롯에 저장된 스킬 데이터 
    public Button MyButton { get; private set; }
    public Image MyIcon { get; set; }

    [SerializeField]
    private Image skillFillter = null;  // 쿨타임 이미지
    [SerializeField]
    private Sprite img_nonSkill = null; // 빈 슬롯 이미지 스프라이트
    [SerializeField]
    private Text text_SkillLevel = null; // 스킬레벨 표시할 텍스트
    [SerializeField] Image chainIcon = null;

    public Coroutine chainRoutine;
    public Coroutine chainCoolDownRoutine;

    private void Awake()
    {
        if(MyButton == null)
            MyButton = GetComponent<Button>();
        MyIcon = GetComponent<Image>();
        
            // 클릭 이벤트를 MyButton에 등록한다.
        MyButton.onClick.AddListener(OnClick);
        if(skillFillter!=null)
            skillFillter.fillAmount = 0;
    }

    public void SetSkill(int idx)
    {
        selectedSkill = playerControl.MyPlayer.skills[(SkillSlotNumber)idx];
        if (selectedSkill != null)
        {
            // 쿨타임 초기화 
            selectedSkill.CoolTimeReset();
        }
        index = idx;
        UpdateVisual();
    }
    public Skill GetSkill()
    {
        return selectedSkill;
    }

    public void RemoveSkill()
    {
        selectedSkill = null;
    }

    // 클릭 발생하면 실행
    public void OnClick()
    {
        if (selectedSkill == null)
        {
            Debug.Log("스킬 없음!");
            return;
        }

        if (selectedSkill.MyCoolDown)
        {
            Debug.Log("나의 스킬 " + selectedSkill.MyName);
            selectedSkill.Use(playerControl);
            //playerControl.UseSkill(selectedSkill);

            if (isChainReady)
            {
                //SkillAction.MyInstance.ChianAction();
                chainRoutine = StartCoroutine(ChangeNextSkill());
                return;
            }

            if (skillFillter != null)
            {
                skillFillter.fillAmount = 0;
                StartCoroutine(CoolTime());
            }
        
        }
    }

    //클릭이 발생했는지 감지
    // IPointerClickHandler에 명시된 함수이다. 
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if(eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        if(HandScript.MyInstance.MyMoveable != null)
    //        {
    //            // IUseable로 변환할수 있는지 확인
    //            if(HandScript.MyInstance.MyMoveable is Skill)
    //            {
    //                SetUseable(HandScript.MyInstance.MyMoveable as Skill);
    //                //ActionButton의 이미지를 변경한다.
    //                MyIcon.sprite = HandScript.MyInstance.Put().MyIcon;
    //            }
    //        }
    //    }
    //    else if(eventData.button == PointerEventData.InputButton.Right)
    //    {
    //        SetUseable(null);
    //    }
    //}

    public void SetUseable(Skill _useable)
    {
        // MyUseable.Use()는 버튼이 클릭되었을때 호출된다.
        // MyUseable은 인터페이스로 Spell에서 상속받고 있다.
        this.selectedSkill = _useable;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (selectedSkill != null)
        {
            MyIcon.sprite = selectedSkill.MyIcon;
            MyIcon.color = Color.white;
            text_SkillLevel.gameObject.SetActive(true);
            text_SkillLevel.text = "Lv. " + selectedSkill.MySkillLevel;
        }
        else
        {
            MyIcon.sprite = img_nonSkill;
            MyIcon.color = Color.white;
            text_SkillLevel.gameObject.SetActive(false);
            text_SkillLevel.text = "";
        }
    }

    public void ResetSkillCoolTime()
    {
        selectedSkill.CoolTimeReset();
        Debug.Log("스킬 초기화합니다?" + selectedSkill.MyCoolDown);
    }

    IEnumerator CoolTime()
    {
        while(skillFillter.fillAmount < 1 )
        {
            skillFillter.fillAmount += 1 * Time.smoothDeltaTime / selectedSkill.MyCoolTime;
            selectedSkill.MyCoolDown = false;
            yield return null;
        }

        skillFillter.fillAmount = 0;
        selectedSkill.MyCoolDown = true;
        yield break;
    }

    // 체인스킬 다음 스킬로 변형
    IEnumerator ChangeNextSkill()
    {
       
        if (chainSkillQ.Count > 0)
        {   
            
            selectedSkill = chainSkillQ.Dequeue();
            Debug.Log("스킬 이미지 변환!" + selectedSkill.CallSkillName);
            
            if (chainCoolDownRoutine == null)
                chainCoolDownRoutine = StartCoroutine(CoolDownChainSkill());
            else
            {
                StopCoroutine(chainCoolDownRoutine);
                chainCoolDownRoutine = StartCoroutine(CoolDownChainSkill());
            }
        }
        else if (chainSkillQ.Count == 0)
        {
            Debug.Log("스킬 이미지 변환 완료");
            selectedSkill = playerControl.MyPlayer.skills[(SkillSlotNumber)index];
            isChainReady = false;
            chainIcon.gameObject.SetActive(false);
            skillFillter.fillAmount = 0;
            StartCoroutine(CoolTime());
        }

        UpdateVisual();
        yield return new WaitForSeconds(0.1f);
    }

    
  

    public void ActiveChainIcon()
    {
        chainIcon.transform.position = this.transform.position;
        isChainReady = true;
        chainIcon.gameObject.SetActive(true);

        if(chainSkillQ == null)
            chainSkillQ = new Queue<Skill>();

        for (int i = 0; i < playerControl.MyPlayer.chainsSkills.Count; i++)
        {
            if (playerControl.MyPlayer.chainsSkills[(SkillSlotNumber)i]!= null)
            {
                chainSkillQ.Enqueue(playerControl.MyPlayer.chainsSkills[(SkillSlotNumber)i]);
            }
        }
    }


    // 스킬 이미지 기존으로 되돌림 
    public void ChangeEvent()
    {
        if (chainRoutine != null)
        {
            StopCoroutine(chainRoutine);
            selectedSkill = playerControl.MyPlayer.skills[(SkillSlotNumber)index];
            UpdateVisual();
            skillFillter.fillAmount = 0;
            StartCoroutine(CoolTime());
            isChainReady = false;
        }
    }

    IEnumerator CoolDownChainSkill()
    {
        chainIcon.fillAmount = 1;
        while (chainIcon.fillAmount > 0)
        {
            chainIcon.fillAmount -= Time.smoothDeltaTime / 3.0f;
            yield return null;
        }

        if (chainIcon.fillAmount == 0)
        {
            ChangeEvent();
            yield return null;
        }
    }

}
