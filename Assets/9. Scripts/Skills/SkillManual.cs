using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * 페이지에 등록되어 보여줄 스킬 
 */
[System.Serializable]
public class PageSkill
{
    public Skill skill;
    public bool isUsed;
    public bool isChain;

    public PageSkill(Skill _skill, bool _isUsed = false,bool _isChain = false)
    {
        skill = _skill;
        isUsed = _isUsed;
        isChain = _isChain;
    }
}

public class SkillManual : MonoBehaviour
{

    public static SkillManual instance;
    public static SkillManual MyInstacne
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SkillManual>();
            }

            return instance;
        }
    }


    // [SerializeField]
    // private GameObject go_BaseUI = null;

    private int tabNumber = 0;  // 탭 수
    private int page = 1;       // 페이지 수
    private int finalPage;      // 최종 페이지 수 
    private PageSkill[] selectedPageSkills; // 선택된 스킬 탭 


    private PageSkill[] activeSkills = null;    // 액티브 스킬들
    private PageSkill[] passiveSkills = null;
    //private PageSkill[] applySkills = null;
    
    // [SerializeField] GameObject go_Base = null;
    [SerializeField] SkillToolTip skillToolTip = null;
    [SerializeField] GameObject go_BaseUI = null; 

    [Header("페이지 표시")]
    [SerializeField] SkillSlot[] skillSlots = null;             // 배울 스킬슬롯들 
    [SerializeField] Text txt_pageText = null;                  // 페이지 표시할 텍스트 
    [SerializeField] SkillDataBase skillDataBase = null;        // 데이터베이스 
    [SerializeField] Sprite emptyImage = null;                  // 빈슬롯 이미지 
    [SerializeField] Image skillSelector = null;                // 스킬 선택자 

    Skill selectedSkill;                                        // 선택한 스킬 
    SkillQuickSlot selectedSlot;                                // 선택한 퀵슬롯 
    SkillSlot selectedSkillSlot;                                // 선택한 스킬데이터
 
    [Header("퀵슬롯 선택자")]
    [SerializeField] Image quickSlotSelector = null;
    [SerializeField] GameObject btnGroup = null;                // 버튼을 모아놓은 그룹 오브젝트
    [SerializeField] Button btn_RgstChain = null;                 // 장착 버튼 
    [SerializeField] Button cancelBtn = null;                   // 해제 버튼 

    [Header("스킬슬롯")]
    //[SerializeField] ActionButton[] actionButtons = null;
    [SerializeField] SkillQuickSlot[] quickSlots = null;
    [SerializeField] SkillQuickSlot[] chainSkillSlots = null;   // 체인스킬슬롯
    //public Skill[] chainSkills = new Skill[4];

    [Header("체인스킬 UI")]
    [SerializeField] GameObject chainSkillBtn = null;           // 체인스킬 설정버튼 
    [SerializeField] GameObject chainSkillUI = null;            // 체인스킬 UI
    [SerializeField] Text[] txt_chainSkillNames = null;
    [SerializeField] Image img_ChainSkill = null;

    // 알림 UI 컴포넌트 
    [Header("알림 UI")]
    [SerializeField] ChoiceAlert choiceAlert = null;
    [SerializeField] GameObject alertUIBase = null;
    [SerializeField] Button al_confirmBtn = null;
    [SerializeField] Button al_upgradeBtn = null;
    [SerializeField] Button al_cancelBtn = null;
    [SerializeField] Text al_Text = null;

    Character selectedPlayer;  // 선택한 캐릭터 정보 
    
     Skill[] selectedPlayersSkills;
     Skill[] selectedPlayersChainSkills;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        if (instance == null)
            instance = this;
        //skillDataBase = FindObjectOfType<SkillDataBase>();
        selectedPlayer = null; 
        SetActiveSkills();
        TabSetting(1);
    }


    private void OnEnable()
    {
        
    }

    #region UI
    // 캐릭터 선택 팝업을 열게 한다. 
    public void OpenCharacterSelectPopup()
    {
        if (choiceAlert == null) return; 

        choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.SKILLMANUAL;
        choiceAlert.ConfirmSelect(player => OpenBaseUI(player));
    } 

    public void OpenBaseUI(Character _selectPlayer)
    {
        if (_selectPlayer == null) return; 
        // 데이터 초기화 
        selectedPlayer = _selectPlayer;
        selectedPlayersSkills = null;
        selectedPlayersChainSkills = null;

        // 스킬창이 열릴 때 선택한 캐릭터 정보가 없다면 열리지 않도록
        ConnectSelectedPlayerSkillsData(_selectPlayer);
        if (selectedPlayer == null)
            return;

        UIPageManager.instance.OpenClose(go_BaseUI);

        TabSetting(1);
    }
    // 선택한 캐릭터 스킬셋 연결하기 
    public void ConnectSelectedPlayerSkillsData(Character _selectPlayer)
    {
        // 인포매뉴얼이 아니라.. 뭐더라..?
        selectedPlayer = _selectPlayer;
        if (selectedPlayer == null)
            return;

        selectedPlayersSkills = selectedPlayer.MySkills;
        selectedPlayersChainSkills = selectedPlayer.MyChains;
        Debug.Log("스킬 셋 할당 " + selectedPlayersSkills.Length + " " + selectedPlayersChainSkills.Length);
    }


    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;
        //SkillManager.instance.ClearAllSkills();
    
        // 스킬페이지 초기화 
        ClearSlot();
        // 퀵슬롯 초기화 
        ClearQuickSlots();
        switch (tabNumber)
        {
            case 1:
                //applySkills = activeSkills;
                ClearPageSkill(activeSkills);
                SetActiveSkillSlotFromInfo();
                TabPageSetting(activeSkills);
                break;
            case 2:
               
                if (passiveSkills == null || passiveSkills.Length <= 0 )
                {
                   
                    SetPassiveSkills();
                }
                ClearPageSkill(passiveSkills);
                TabPageSetting(passiveSkills);
                //applySkills = passiveSkills;
                break;
        }
    }
    
    // 스킬데이터베이스에 등록된 스킬들을 불러와 저장(액티브)
    private void SetActiveSkills()
    {
        Skill[] skills = skillDataBase.GetActiveSkills();
        activeSkills = new PageSkill[skills.Length];

        for (int i = 0; i < skills.Length; i++)
        {
            activeSkills[i] = new PageSkill(skills[i]);
        }
    }

    private void SetPassiveSkills()
    {
        Skill[] skills = skillDataBase.GetPassiveSkills();
        passiveSkills = new PageSkill[skills.Length];

        for (int i = 0; i < skills.Length; i++)
        {
            passiveSkills[i] = new PageSkill(skills[i]);
        }
    }

    private void ClearSlot()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].img_SkillImage.sprite = emptyImage;
            skillSlots[i].text_SkillLevel.text = "";
            skillSlots[i].text_SkillName.text = "";
            skillSlots[i].text_SkillDesc.text = "";
            skillSlots[i].skill = null;
            skillSlots[i].isUsed = false;
        }
    }

    void ClearPageSkill(PageSkill[] p_TargetSkills)
    {
        if (p_TargetSkills == null) return; 

        for (int i = 0; i < p_TargetSkills.Length; i++)
        {
            p_TargetSkills[i].isUsed = false;
            p_TargetSkills[i].isChain = false;
        }
    }

    // 스킬 페이지에 장착되어 있는지 보여줌
    void ApplySkillSlot(bool isChain = false)
    {
        if (activeSkills == null) return; 

        for (int i = 0; i < skillSlots.Length; i++)
        {
            for (int j = 0; j < activeSkills.Length; j++)
            {
                if (skillSlots[i].skill == activeSkills[j].skill)
                {
                    skillSlots[i].IsUsedSlot(activeSkills[j].isUsed,isChain);
                    break;
                }
            }
        }
        //InfoManual.MyInstance.SetSkillToSelcetedPlayer(SkillManager.instance.GetSkills(), SkillManager.instance.GetChainSkills());
        if (selectedPlayer != null)
        {
            // 데이터 넣기
            InfoManager.instance.ApplySkillDataSelcetedPlayer(selectedPlayer.MyID);
        }
    }

    // 페이지 넘기는 버튼 조작 
    public void RightPageSetting()
    {
        if (page < (selectedPageSkills.Length / skillSlots.Length) + 1)
            page++;
        else
            page = 1;
        
        txt_pageText.text = page.ToString() + "/" + finalPage.ToString();
        TabPageSetting(selectedPageSkills);
    }

    public void LeftPageSetting()
    {
        if (page != 1)
            page--;
        else
            page = (selectedPageSkills.Length / skillSlots.Length) + 1;

        txt_pageText.text = page.ToString() + "/" + finalPage.ToString();
        TabPageSetting(selectedPageSkills);
    }

    void TabPageSetting(PageSkill[] _pageSkills)
    {
        if (_pageSkills == null) return; 

        selectedPageSkills = _pageSkills;

        int startPageNumber = (page - 1) * skillSlots.Length;
        finalPage = Mathf.RoundToInt((selectedPageSkills.Length / skillSlots.Length)+1);

        for (int i = 0; i < skillSlots.Length; i++)
        {
            //if (i == page * skillSlots.Length) // 슬롯 개수만큼 반복문 결정 
            //    break;

            int pageNum = i + startPageNumber;
            skillSlots[i].gameObject.SetActive(true);

            if (pageNum < selectedPageSkills.Length)
            {
                skillSlots[i].SetSlot(_pageSkills[pageNum].skill);
                skillSlots[i].IsUsedSlot(_pageSkills[pageNum].isUsed);

                if (skillSlots[i].skill.MySkillLevel < skillSlots[i].skill.MySkillMaxLevel
                    && skillSlots[i].skill.upgradeCost.Length > 0)
                {
                    al_upgradeBtn.interactable = true;
                    skillSlots[i].text_SkillDesc.text =
                        _pageSkills[i].skill.upgradeCost[skillSlots[i].skill.MySkillLevel - 1].ToString() + "->"
                        + _pageSkills[i].skill.upgradeCost[skillSlots[i].skill.MySkillLevel].ToString();
                }
                else
                {
                    skillSlots[i].text_SkillDesc.text = "스킬 레벨을 더 이상 올릴 수 없습니다.";
                    al_upgradeBtn.interactable = false;
                }
                skillSlots[i].text_SkillLevel.text = "Lv" + _pageSkills[i].skill.MySkillLevel;
                skillSlots[i].skill.skillDesc = _pageSkills[i].skill.skillDesc;
            }else
            {
                skillSlots[i].SetSlot(null);
            }
        }
        ApplySkillSlot();
        txt_pageText.text = page.ToString() + "/" + finalPage.ToString();
    }

    #endregion


    //  스킬 정보를 받아서 슬롯들에게 정리함 
    private void SetActiveSkillSlotFromInfo()
    {
        if (selectedPlayer== null)
            return;

        ClearQuickSlots();
        
        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i].SetSkill(selectedPlayersSkills[i]);
            TakeONOFFSkill(selectedPlayersSkills[i]);
        }

        for (int i = 0; i < chainSkillSlots.Length; i++)
        {
            chainSkillSlots[i].SetSkill(selectedPlayersChainSkills[i]);
            TakeONOFFSkill(chainSkillSlots[i].GetSkill(), chainSkillSlots[i].GetSkill() != null,true);
        }

        ApplySkillSlot();
    }

    // 스킬 업그레이드 
    public void SkillUpgrade()
    {
        selectedSkill = selectedSkillSlot.skill;

        if (selectedSkill == null)
            return;

        if (selectedSkill.upgradeCost[selectedSkill.MySkillLevel - 1] <= InfoManager.coin
            && selectedSkill.MySkillLevel < selectedSkill.MySkillMaxLevel)
        {
            if (RLModeController.isRLMode)
            {
                RLModeController.instance.DownBHPoint();
            }

            InfoManager.coin -= selectedSkill.upgradeCost[selectedSkill.MySkillLevel - 1];
            selectedSkill.UpgradeSkill();

            selectedSkillSlot.UpdateTooltip(selectedSkill);
            skillToolTip.UpdateTooltip(selectedSkill);
          
        }
        else if (selectedSkill.upgradeCost[selectedSkill.MySkillLevel - 1] > InfoManager.coin)
        {
            Debug.Log("코인이 부족해요!");
        }
        else if (selectedSkill.MySkillLevel == 5)
            Debug.Log("스킬이 최대 레벨입니다!");
    }

    public void SelectSkillSlot(SkillSlot _skillSlot)
    {
         selectedSkillSlot = _skillSlot;
    }

    // 사용하려는 스킬의 등재 변경 
    void TakeONOFFSkill(Skill p_TargetSkill, bool p_Use = true,  bool isChain = false)
    {
        if (p_TargetSkill == null)
            return;

        for (int i = 0; i < activeSkills.Length; i++)
        {
            if (p_TargetSkill.MyName.Equals(activeSkills[i].skill.MyName))
            {
                activeSkills[i].isUsed = p_Use;
                activeSkills[i].isChain = isChain;
                break;
            }
        }
    }

    // 스킬 퀵슬롯에 등록시키기 버튼 이벤트
    public void RegistSkill()
    {
        if (selectedSkillSlot != null)
        {
            selectedSkill = selectedSkillSlot.skill;
            // 장착 버튼 누르면 호출 
            Debug.Log(selectedSkill.MyName);
        }
        // 슬롯 선택하라는 알림 출력
        skillToolTip.HideToolTip();
    }

    // 퀵슬롯 선택시 스킬 등록
    public void RegistToQuickSlot(SkillQuickSlot _quickSlot)
    {
        AppearSelecter(_quickSlot);

        if (selectedSkill == null)
            return;
        else
        {
            CheckToSlotIsUsed(_quickSlot);
            _quickSlot.SetSkill(selectedSkill);
            TakeONOFFSkill(selectedSkill);  // 새로 오는 스킬 사용 

            //SkillManager.instance.GetSkills()[_quickSlot.slotNumber >= 0 ? _quickSlot.slotNumber : 0] = selectedSkill;
            Skill t_Skill;
            t_Skill = selectedSkill.DeepCopy();

            if (!_quickSlot.isChainSkill)
            {
                Debug.Log("선택한 스킬 슬롯 번호 " + _quickSlot.slotNumber);
                //SkillManager.instance.SetSkill(t_Skill, _quickSlot.slotNumber >= 0 ? _quickSlot.slotNumber : 0);
                selectedPlayersSkills[_quickSlot.slotNumber >= 0 ? _quickSlot.slotNumber : 0] = t_Skill;
            }
            else
            {
                //SkillManager.instance.SetChainSkill(t_Skill, _quickSlot.slotNumber >= 0 ? _quickSlot.slotNumber : 0);
                selectedPlayersChainSkills[_quickSlot.slotNumber >= 0 ? _quickSlot.slotNumber : 0] = t_Skill;
            }

            selectedSkill = null;
            ApplySkillSlot(_quickSlot.isChainSkill);    // 체인 스킬 여부에 따라 표기 변경 
            //InfoManual.MyInstance.SetSkillSlot();
        }
    }


    // 등록 취소
    //public void CancelSkill()
    //{
    //    Debug.Log("이건 언제오는데? ");
    //    if (selectedSkill == null)
    //        return;

    //    selectedSkill = selectedSkillSlot.skill;

    //    for (int i = 0; i < quickSlots.Length; i++)
    //    {
    //        if (quickSlots[i].GetSkill() == selectedSkill)
    //        {
    //            TakeONOFFSkill(quickSlots[i].GetSkill());
    //            quickSlots[i].CleaerSlot();
    //            //selectedSkillSlot.IsUnUsedSlot();
    //            ApplySkillSlot();
    //            selectedSkillSlot = null;
    //            selectedSkill = null;
    //            break;
    //        }
    //    }
    //     skillToolTip.HideToolTip();
    //}



    // 퀵슬롯 초기화 
    public void ClearQuickSlots()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            quickSlots[i].SetSkill(null);
        }
        quickSlotSelector.gameObject.SetActive(false);
    }

    // 퀵슬롯에 든 스킬 해제 버튼 이벤트
    public void CancelSkillToQuickSlot()
    {
        Debug.Log("퀵슬롯  스킬 해제 ");
        
        if (selectedSlot.transform.CompareTag("NormalSkill"))
        {
            if (selectedSlot.isChainSkill)
            {
                Debug.Log("퀵슬롯  스킬 이하 전체 해제  ");
                img_ChainSkill.gameObject.SetActive(false);
                selectedSlot.isChainSkill = false;
                selectedPlayersSkills[selectedSlot.slotNumber] = null;
                //SkillManager.instance.ClearChianSkillForTarget();
                //InfoManual.MyInstance.GetSelectedPlayer().ClearChains();
                ClearChainSkill();
            }
            else
                //SkillManager.instance.GetSkills()[selectedSlot.slotNumber] = null;
                selectedPlayersSkills[selectedSlot.slotNumber] = null;
        }

        
        TakeONOFFSkill(selectedSlot.GetSkill(), false);
        selectedSlot.SetSkill(null);
        ApplySkillSlot();
        
        
        selectedSlot = null;
        btnGroup.SetActive(false);
    }

    // 스킬북에서 선택한 (장착되어 있는) 스킬 해제 
    public void CancelSlotToQuickSlot()
    {
        if (selectedSkillSlot == null)
            return;

        Debug.Log("스킬북  스킬 해제 ");


        for (int i = 0; i < activeSkills.Length; i++)
        {
            if(selectedSkillSlot.skill == activeSkills[i].skill)
            {
                if (activeSkills[i].isChain)
                {
                    for (int k = 0; k < quickSlots.Length; k++)
                    {
                        if (quickSlots[k].GetSkill() == selectedSkillSlot.skill)
                        {
                            if (quickSlots[k].isChainSkill)
                            {
                                Debug.Log("체인 스킬 전체 해제 ");
                                img_ChainSkill.gameObject.SetActive(false);
                                //chainSkillSlots[k].isChainSkill = false;
                                //SkillManager.instance.ClearChianSkillForTarget();
                                //InfoManual.MyInstance.GetSelectedPlayer().ClearChains();
                                ClearChainSkill();
                                TakeONOFFSkill(quickSlots[k].GetSkill(), false, quickSlots[k].isChainSkill);
                                quickSlots[k].SetSkill(null);
                                ApplySkillSlot();
                                //SkillManager.instance.GetSkills()[k] = null;
                                selectedPlayersSkills[k] = null;
                                return;
                            }
                            else break;
                            //skillToolTip.ApplyBtnHandle(selectedSkillSlot.isUsed);
                        }
                    }

                    for (int j = 0; j < chainSkillSlots.Length; j++)
                    {
                        if(chainSkillSlots[j].GetSkill() == activeSkills[i].skill)
                        {
                            Debug.Log("체인 스킬 전체 해제 2");
                            //SkillManager.instance.GetChainSkills()[chainSkillSlots[j].slotNumber] = null;
                            selectedPlayersChainSkills[j] = null;
                            TakeONOFFSkill(selectedSkillSlot.skill,false, activeSkills[i].isChain);
                            //skillToolTip.ApplyBtnHandle(selectedSkillSlot.isUsed);
                            chainSkillSlots[j].SetSkill(null);
                            ApplySkillSlot();
                            break;
                        }
                    }
                }else
                {
                    for (int k = 0; k < quickSlots.Length; k++)
                    {
                        if (quickSlots[k].GetSkill() == selectedSkillSlot.skill)
                        {
                            TakeONOFFSkill(quickSlots[k].GetSkill(),false);
                            quickSlots[k].SetSkill(null);
                            ApplySkillSlot();
                            skillToolTip.ApplyBtnHandle(selectedSkillSlot.isUsed);
                            //selectedSkillSlot = null;
                            //SkillManager.instance.GetSkills()[k] = null;
                            selectedPlayersSkills[k] = null;
                            return;
                        }
                    }
                }

            }
        }

        //for (int i = 0; i < quickSlots.Length; i++)
        //{
        //    if(quickSlots[i].GetSkill() == selectedSkillSlot.skill)
        //    {
        //        TakeONOFFSkill(quickSlots[i].GetSkill());
        //        quickSlots[i].SetSkill(null);
        //        ApplySkillSlot();
        //        skillToolTip.ApplyBtnHandle(selectedSkillSlot.isUsed);
        //        selectedSkillSlot = null;
        //        SkillManager.skills[i] = null;

        //        InfoManual.MyInstance.SetSkillSlot();
        //        actionButtons[i].SetSkill(quickSlots[i].GetSkill());
        //        return;
        //    }

        //}
    }

    // 사용하려는 슬롯에 스킬이 있는 경우 해결하는 메소드
    void CheckToSlotIsUsed(SkillQuickSlot p_TargetSlot)
    {
        if (p_TargetSlot.GetSkill() == null)
            return;
        else if(p_TargetSlot.GetSkill() != null)
        {
            TakeONOFFSkill(p_TargetSlot.GetSkill(), false);  // 기존 스킬 사용 유무 해제 
            p_TargetSlot.ClearSlot();
        }
    }



    #region ChainSkill UI

    // 체인 스킬 버튼 이벤트 
    public void SetChiainSkill()
    {
        if (selectedSlot == null || selectedSkillSlot == null)
            return;

        if(selectedSlot.transform.CompareTag("NormalSkill"))
        {
            selectedSlot.isChainSkill = true;
            selectedPlayersSkills[selectedSlot.slotNumber].IsChain = true;
            Debug.Log("스킬 체인!" + selectedSlot.GetSkill().MyName);

            ApplySkillSlot(selectedSlot.isChainSkill);
            TakeONOFFSkill(selectedSlot.GetSkill(), true, true);

            img_ChainSkill.transform.position = selectedSlot.transform.position;
            img_ChainSkill.gameObject.SetActive(true);
            
            btnGroup.SetActive(false);
            OpenCloseChainSkillUI();
        }
    }

    public void ClearChainSkill()
    {
        if (selectedSlot == null || selectedSkillSlot == null)
            return;


        for (int i = 0; i < chainSkillSlots.Length; i++)
        {
            TakeONOFFSkill(selectedPlayersChainSkills[i], false, false);
            chainSkillSlots[i].SetSkill(null);
        }

        //SkillManager.instance.ClearChianSkillForTarget();
        selectedPlayer.ClearChains();

        //ApplySkillSlot();
    }


    // 체인스킬 UI 열기 
    public void OpenCloseChainSkillUI()
    {
        UIPageManager.instance.OpenClose(chainSkillUI);
    }



    #endregion


    // 퀵슬롯 선택 시, 선택자 출현
    public void AppearSelecter(SkillQuickSlot _quickSlot)
    {
        if (!quickSlotSelector.IsActive() && 
            (_quickSlot.transform.CompareTag("NormalSkill") || _quickSlot.transform.CompareTag("ChainSkill")))
            quickSlotSelector.gameObject.SetActive(true);

        if (selectedSlot == _quickSlot)
        {
            quickSlotSelector.gameObject.SetActive(false);
            selectedSlot = null;
            return;
        }

        quickSlotSelector.transform.position = _quickSlot.transform.position;
        selectedSlot = _quickSlot;

        if (_quickSlot.GetSkill() != null)
        {
            btnGroup.SetActive(true);
            btn_RgstChain.gameObject.SetActive(_quickSlot.transform.CompareTag("NormalSkill"));
        }
        else
            btnGroup.SetActive(false);
    }
}
