using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum SkillCategory
{
    NONE,
    PRIVATE_ACTIVE,
    PRIVATE_PASSIVE,
    PUBLIC_ACTIVE,
    PUBLIC_PASSIVE,
}

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

// 선택한 캐릭터의 스킬들을 보여주고 스킬들을 장착하는 기능을 한다. 
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
    private int currentPage = 1;
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

    Skill selectedSkill;                                        // 선택한 스킬 
    SkillQuickSlot selectedSlot;                                // 선택한 퀵슬롯 
    SkillSlot selectedSkillSlot;                                // 선택한 스킬데이터

    [Header("스킬슬롯")]
    //[SerializeField] ActionButton[] actionButtons = null;
    [SerializeField] SkillQuickSlot[] quickSlots = null;
    [SerializeField] SkillQuickSlot[] chainSkillSlots = null;   // 체인스킬슬롯
    //public Skill[] chainSkills = new Skill[4];

    [Header("체인스킬 UI")]
    [SerializeField] Image img_ChainSkill = null;
    [SerializeField] GameObject chainSkillUI = null;
    // 알림 UI 컴포넌트 
    [Header("알림 UI")]
    [SerializeField] ChoiceAlert choiceAlert = null;


    Character selectedPlayer;  // 선택한 캐릭터 정보 

    private bool isEquipFlag = false; // 장착버튼을 누르면 변경

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
        {
            instance = this;
        }
        else
        {

        }

        TabSetting(1);
    }

    #region UI
    // 스킬 매뉴얼 UI를 그리는 함수
    public void DrawSkillManual()
    {
        // todo 현재 액티브 전용만 갱신하고 있으니 패시브랑 일반 스킬들도 따로 추가해야한다.
        // 스킬 정보 갱신
        RefreshEquippedSkills();

        // 퀵슬롯 그리기 
        DrawQuickSlot();

        // 페이지 그리기 
        TabSetting(currentPage);
    }

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
        // 스킬창이 열릴 때 선택한 캐릭터 정보가 없다면 열리지 않도록
        if (_selectPlayer == null) return; 
        // 데이터 초기화 
        selectedPlayer = _selectPlayer;

        UIPageManager.instance.OpenClose(go_BaseUI);
        
        // 액티브 스킬 정보 세팅
        SetActiveSkills();

        // 패시브 스킬 정보 세팅 
        SetPassiveSkills(); 

        // 퀵슬롯 그리는 형태
        if (quickSlots != null)
        {
            foreach (var slot in quickSlots)
            {
                if (slot == null) continue;
                slot.InitSkillSlot();
                // 등록해제
                slot.SetCancelButtonAction(
                    () =>
                    {
                        CancelSkillToQuickSlot();
                    });
                // 체인
                slot.SetChaiSkillButton(
                    () =>
                    {
                        SetChiainSkill();
                    });

            }
        }

        // 스킬 매뉴얼을 그린다. 
        DrawSkillManual(); 
    }

    public void TabSetting(int _tabNumber)
    {
        tabNumber = _tabNumber;
        page = 1;
        //SkillManager.instance.ClearAllSkills();
    
        // 스킬페이지 초기화 
        ClearSlot();
 
        switch (tabNumber)
        {
            case 1:
                TabPageSetting(activeSkills);
                break;
            case 2:
                TabPageSetting(passiveSkills);
                break;
        }
    }
    
    // 스킬데이터베이스에 등록된 스킬들을 불러와 저장(액티브)
    private void SetActiveSkills()
    {
        if (selectedPlayer == null) return; 

        // 선택한 캐릭터의 acitve 스킬들 가져온다.
        List<Skill> skills = SkillDataBase.instance.GetActiveSkillListFromID(selectedPlayer.MyID);
        activeSkills = new PageSkill[skills.Count];

        string chainSkillKeycode = selectedPlayer.GetFirstChainSkillID();
        for (int i = 0; i < skills.Count; i++)
        {
            bool isEquipped = false;
            bool isChain = false; 
            if (selectedPlayer.skills.ContainsValue(skills[i]) == true)
            {
                isEquipped = true;
            }

            if(chainSkillKeycode != "" && chainSkillKeycode == skills[i].keycode)
            {
                isChain = true; 
            }


            activeSkills[i] = new PageSkill(skills[i], isEquipped, isChain);
        }
    }

    private void SetPassiveSkills()
    {

        // 선택한 캐릭터의 passive 스킬들 가져온다.
        List<Skill> skills = SkillDataBase.instance.GetPassiveSkillListFromID(selectedPlayer.MyID);
        passiveSkills = new PageSkill[skills.Count];

        // 패시브 스킬은 레벨이 1이상이면 자동으로 활성화되고 적용된다. 

        for (int i = 0; i < skills.Count; i++)
        {
            bool isEnabled = false;
            //if (skills[i].MySkillLevel > 0)
            //{
            //    isEnabled = true;
            //}

            passiveSkills[i] = new PageSkill(skills[i], isEnabled);
        }
    }

    private void ClearSlot()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].ClearSlot();
        }
    }


    // 페이지 넘기는 버튼 조작 
    public void RightPageSetting()
    {
        if (selectedPageSkills == null) return;

        if (page < (selectedPageSkills.Length / skillSlots.Length) + 1)
            page++;
        else
            page = 1;
        
        txt_pageText.text = page.ToString() + "/" + finalPage.ToString();
        TabPageSetting(selectedPageSkills);
    }

    public void LeftPageSetting()
    {
        if (selectedPageSkills == null) return;

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
        finalPage = Mathf.RoundToInt((_pageSkills.Length / skillSlots.Length)+1);

        for (int i = 0; i < skillSlots.Length; i++)
        {
            int pageNum = i + startPageNumber;
            skillSlots[i].gameObject.SetActive(true);

            if (pageNum < selectedPageSkills.Length)
            { 
                skillSlots[i].SetSlot(_pageSkills[pageNum]);
            }
            else
            {
                skillSlots[i].ClearSlot();
            }
        }

        //ApplySkillSlot();
        txt_pageText.text = page.ToString() + "/" + finalPage.ToString();
    }

    #endregion


    
    // 스킬 업그레이드 
    public void SkillUpgrade()
    {
        selectedSkill = selectedSkillSlot.skill;

        if (selectedSkill == null)
            return;

        if (selectedSkill.upgradeCost <= InfoManager.coin
            && selectedSkill.MySkillLevel < selectedSkill.MySkillMaxLevel)
        {
            InfoManager.coin -= selectedSkill.upgradeCost;
            selectedSkill.UpgradeSkill();
            selectedSkillSlot.UpdateTooltip(selectedSkill);
            if(selectedSkill.skillType == SkillType.PASSIVE)
            {
                if(selectedSkill is PassiveSkill)
                    (selectedSkill as PassiveSkill).isUnlocked = true;
                // 효과 적용 시킨다. ...
                if (selectedPlayer != null)
                {
                    selectedPlayer.SetPassiveSkill(selectedSkill as PassiveSkill);
                }
            }

            skillToolTip.UpdateTooltip(selectedSkill);
          
        }
        else if (selectedSkill.upgradeCost> InfoManager.coin)
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

    
    // PRIVATE 장착한 스킬 여부 체크 후 값 갱신 
    private void RefreshEquippedSkills()
    {
        if (selectedPlayer == null)
            return;

        // 해당 탭에 해당하는 스킬만 갱신
        switch(tabNumber)
        {
            case 1:
                foreach (PageSkill pageSkill in activeSkills)
                {
                    if (pageSkill == null || pageSkill.skill == null) continue;

                    // 장착했는지 검사 
                    bool isEquipped = selectedPlayer.CheckEquppiedSkillBySkillKeycode(pageSkill.skill.keycode);
                    bool isChainEquipped = selectedPlayer.CheckEquippedChainSkillBySkillKeycode(pageSkill.skill.keycode);

                    // 값 변경 
                    pageSkill.isUsed = isEquipped || isChainEquipped;
                    pageSkill.isChain = isChainEquipped;
                }
                break;
            case 2: break;
            case 3: break;
            case 4: break; 
        }
        
    }

    

    ///////////////////////
    // 퀵슬롯 관련 
    ////////////////////////////

    //  현재 선택한 캐릭터의 스킬 정보를 바탕으로 퀵슬롯을 그린다. 
    private void DrawQuickSlot()
    {
        if (selectedPlayer == null)
            return;

        ClearQuickSlots();

        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == null)
                continue; 
            quickSlots[i].SetSkill(selectedPlayer.skills[(SkillSlotNumber)i]);
            // .. 체인 이미지가 별도로 배치되어 있으므로 여기서 조절한다.
            if (selectedPlayer.GetSlotisChain(quickSlots[i].slot) == true)
            {
                // 하르키니아 르 살레브아르스 
                img_ChainSkill.transform.position = quickSlots[i].transform.position;
                img_ChainSkill.gameObject.SetActive(true);
            }

        }

        for (int i = 0; i < chainSkillSlots.Length; i++)
        {
            SkillSlotNumber index = SkillSlotNumber.CHAIN1 + i;
            chainSkillSlots[i].SetSkill(selectedPlayer.chainsSkills[index]);
        }

    }


    // 스킬 퀵슬롯에 등록시키기 버튼 이벤트
    public void RegistSkill()
    {
        if (selectedSkillSlot != null)
        {
            selectedSkill = selectedSkillSlot.skill;
            if(selectedSkill.MySkillLevel <= 0)
            {
                // 레벨이 없으므로 장착할 수 없다.
                Debug.Log("You Can regist Skill because not enough skill level");
                selectedSkill = null;
                return; 
            }
            // 장착 버튼 누르면 호출 
            Debug.Log(selectedSkill.MyName);
        }
        isEquipFlag = true; 
        // 슬롯 선택하라는 알림 출력
        skillToolTip.HideToolTip();
    }

    // 퀵슬롯 선택시 스킬 등록
    public void RegistToQuickSlot(SkillQuickSlot _quickSlot)
    {
        if (selectedPlayer == null || isEquipFlag == false)
            return;

        // 캐릭터에게 장착시키기 
        selectedPlayer.EquipSkill(_quickSlot.slot, selectedSkill);
       
        // 선택자 UI 보여주기 
        //AppearSelecter(_quickSlot);
        
        // 화면 다시그린다.
        DrawSkillManual();

        selectedSkill = null;

        selectedSlot = null;

        isEquipFlag = false; // 플래그를 돌려서 스킬 슬롯을 눌러도 장착되지않도록 
    }



    // 퀵슬롯 초기화 
    public void ClearQuickSlots()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (quickSlots[i] == null) continue; 
            quickSlots[i].SetSkill(null);
        }
        //quickSlotSelector.gameObject.SetActive(false);
    }


    // 스킬 해제 
    public void UnequipSkill()
    {
        if (selectedSkill != null)
        {
            // 스킬 해제 
            selectedPlayer.UnequipSkill(selectedSkill);

            // 화면 다시그린다.
            DrawSkillManual();

            // 스킬 툴팁 다시 그리기
            skillToolTip.UpdateTooltip(selectedSkill); 
        }
    }


    // 퀵슬롯에 든 스킬 해제 버튼 이벤트
    public void CancelSkillToQuickSlot()
    {
        if (selectedSlot == null) return;

        selectedSkill = selectedSlot.GetSkill();
        
        // 스킬 해제 함수 호출 
        UnequipSkill(); 

    }

    // 스킬북에서 선택한 (장착되어 있는) 스킬 해제 
    public void CancelSlotToQuickSlot()
    {
        if (selectedSkillSlot == null)
            return;

        Debug.Log("스킬북  스킬 해제 ");

        // 캐릭터에게 장착 해제 
        selectedPlayer.UnequipSkill(selectedSkillSlot.skill);

        // 화면 다시그린다.
        DrawSkillManual();

        // 스킬 툴립도 갱신해준다
        if( skillToolTip != null)
        {
            // 여기선 슬롯을 던져 준다. 툴팁창이 꺼지지 않기 때문
            skillToolTip.UpdateTooltip(selectedSkillSlot);
        }
    }


    #region ChainSkill UI

    // 등록한 체인 스킬을 해제하는 버튼
    public void CloseChainSkill()
    {
        if (selectedSlot == null || selectedPlayer == null)
            return;

        // 0. 정말 체인 스킬 정보를 없앨 것인지 여부 묻는 팝업 

        // 확인 했을 경우 

        // 1. 등록된 체인 해제 



        // 아닐 경우 
    }

    // 체인 스킬 버튼 이벤트 
    public void SetChiainSkill()
    {
        if (selectedSlot == null || selectedPlayer == null)
            return;

        // 해당 슬롯에서 체인스킬 버튼을 누르면 해당 스킬을 체인 스킬화 한다.

        // 1. 선택한 슬롯에서 스킬을 체인 시킨다. 
        // 1-1 슬롯번호를 통해서 캐릭터의 해당 슬롯번호에 해당하는 스킬을 체인시킨다.
        selectedPlayer.SetChainSkillByNormalSlot(selectedSlot.slot);

        // 2. UI들을 다시 그린다. 
        DrawSkillManual();

        // 3. 체인 스킬 UI를 열어준다.
        //OpenCloseChainSkillUI();

    }

    public void ClearChainSkill()
    {
        if (selectedSlot == null || selectedSkillSlot == null)
            return;

        for (int i = 0; i < chainSkillSlots.Length; i++)
        {
           // TakeONOFFSkill(selectedPlayersChainSkills[i], false, false);
            chainSkillSlots[i].SetSkill(null);
        }

        selectedPlayer.ClearChains();
    }


    // 체인스킬 UI 열기 
    public void OpenCloseChainSkillUI()
    {
        UIPageManager.instance.OpenClose(chainSkillUI);
        //
    }



    #endregion


    // 퀵슬롯 선택 정보 저장
    public void SelectSkillSlot(SkillQuickSlot slot)
    {
        if (slot == null) return;
        
        // 1. 이전에 선택한 정보가 있다면 선택한 정보가 그리는 걸 끈다 
        if(selectedSlot != slot && selectedSlot != null)
        {
            selectedSlot.DrawSelectUIGroup();
        }

        // 2. 이전에 선택한 정보랑 같다면 취소
        if(selectedSlot == slot)
        {
            selectedSlot = null;
            return; 
        }

        selectedSlot = slot; 
    }
   
}
