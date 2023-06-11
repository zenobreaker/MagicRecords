using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class InfoManual : UiBase
{
    public static InfoManual MyInstance;
    [SerializeField] GameObject go_Base = null;

    [Header("캐릭터의 모든 정보를 담는 그룹")]
    [SerializeField] GameObject go_charInfoBase = null; 

    [SerializeField] SkillQuickSlot[] skillQuickSlots = null;
    [SerializeField] EquipSlot[] equipSlots = null;


    [SerializeField] Text txt_Level = null;
    [SerializeField] Text txt_Attack = null;
    [SerializeField] Text txt_Deffence = null;
    [SerializeField] Text txt_Speed = null;
    [SerializeField] Text txt_HP = null;
    [SerializeField] Text txt_MP = null;
    [SerializeField] Text txt_Exp = null;
    [SerializeField] private Gauge expGauge = null;


    [SerializeField] GameObject go_SkillBase = null;
    [SerializeField] ChoiceAlert choiceAlert = null;

    Character selectedPlayer;

    [SerializeField] GameObject tabObject = null; 

    [SerializeField] GameObject charSlot = null;
    List<GameObject> charSlotList = new List<GameObject>();

    private void Awake()
    {
        if (MyInstance == null)
            MyInstance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }


    private void OnEnable()
    {
        // 캐릭터 수 만큼 슬롯 버튼 만들기 
        CreateCharSlot();

        // UI가 켜지면 기본적으로 캐릭터 리스트 중 맨 앞 캐릭터를 먼저 고른다. 
        // 캐릭터 리스트를 가져온다 .
        var charList = InfoManager.instance.GetMyPlayerInfoList();
        if (charList == null || charList.Count == 0)
        {
            return;
        }

        // 맨 앞 캐릭터 가져오기
        var firstChar = charList.FirstOrDefault();
        if (firstChar == null)
        {
            return;
        }

        // 캐릭터를 세팅해놓는다. 
        selectedPlayer = firstChar;
        // UI를 그린다. 
        // 슬롯 리스트 초기화 
        ResetSlostList();

        // UI 켜기 
        SetEnableCharInfoBase();

        // 플레이어 설정 
        SetEquipSlot();
        SetStatText(selectedPlayer);
        SetSkillSlot();
    }

    private void OnDisable()
    {
        // 선택한 대상 초기화 
        selectedPlayer = null;

        // UI들 초기화 시키기 
        SetEquipSlot();
        SetStatText(selectedPlayer);
        SetSkillSlot();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        if (selectedPlayer != null)
        {
            SetEquipSlot();
            SetStatText(selectedPlayer);
            SetSkillSlot();
        }
    }

    // 캐릭터 수 만큼 슬롯 버튼 만들기 
    void CreateCharSlot()
    {
        var dic_MyPlayerList = InfoManager.instance.GetMyPlayerInfoList();
        if (dic_MyPlayerList.Count <= 0) return;

        int maxCount = 0; 
        // 이미 캐릭터 슬롯 개수랑 가져온 정보의 개수가 다르다면 만들도록 
        if (charSlotList.Count == dic_MyPlayerList.Count)
        {
            return;
        }
        else
        {
            maxCount = dic_MyPlayerList.Count - charSlotList.Count;
        }


        int count = 0; 
        foreach(var player in dic_MyPlayerList)
        {
            if (player == null) continue;
            // 추가로 만들 개수 만큼 만들었다면 탈출 
            if (maxCount <= count) break; 

            var slot = Instantiate(charSlot);
            slot.gameObject.SetActive(true);
            charSlotList.Add(slot);
            if (slot.TryGetComponent<CharSlot> (out var _charSlot))
            {
                _charSlot.gameObject.transform.SetParent(tabObject.transform);
                // 플레이어 정보 세팅
                _charSlot.SetPlayer(player);
                // 콜백 함수 연결 
                _charSlot.SetCallback(() =>
                {
                    // 인포 UI들 켜주거나 끈다.
                    SetEnableCharInfoBase(); 
                    ResetSlostList();
                    _charSlot.SetSelectedSlot(true);
                    SelectPlayer(player);
                });
            }

            count++; 
            
        }
    }
    public void ResetSlostList()
    {
        for (int i = 0; i < charSlotList.Count; i++)
        {
            if (charSlotList[i].TryGetComponent<CharSlot>(out var _charSlot))
            {
                _charSlot.SetSelectedSlot(false);
            }
        }
    }

    public void OpenSkillSetter()
    {
        if (SkillManual.MyInstacne != null)
        {
            SkillManual.MyInstacne.OpenBaseUI(selectedPlayer);
        }

        else if(UIPageManager.instance != null)
        {
            UIPageManager.instance.OpenSkillManual(selectedPlayer);
        }
        
    }


    public void SelectPlayer(Character p_Target)
    {
        if(p_Target == null)
        {
            Debug.Log("없다");
            selectedPlayer = null;
            return;
        }

        // 정보 매니저에서 자신이 소지한 캐릭터 리스트 가져온다 
        var info = InfoManager.instance.GetMyPlayerInfo(p_Target.MyID);
        if (info != null)
        {
       
            selectedPlayer = p_Target;
            SetEquipSlot();
            SetStatText(selectedPlayer);
            SetSkillSlot();
         
        }
        
    }

    public void CallSelectPlayerFromCA(Character p_Target)
    {
        selectedPlayer = p_Target;
        //choiceAlert.SelectPlayer(p_Target);
    }


    public Character GetSelectedPlayer()
    {
        return selectedPlayer;
    }


    // 캐릭터 아이콘을 누르면 캐릭터 info group 이 켜진다. 
    public void SetEnableCharInfoBase()
    {
        if (go_charInfoBase == null) return;

        // 해당 오브젝트의 켜진 상태값을 받아낸다 
        bool isActive = false;

        // 캐릭터가 선택된게 없으면 항상 꺼진다.
        if(selectedPlayer == null)
        {
            // 세팅엔 반대로 세팅하므로 true 
            isActive = true; 
        }

        // 받아 낸 값의 반대로 세팅 
        go_charInfoBase.SetActive(!isActive); 
    }


    #region SKILL_SETTING 
    public void SetSkillSlot()
    {
        if (selectedPlayer == null)
        {
            for (int i = 0; i < skillQuickSlots.Length; i++)
            {
                skillQuickSlots[i].ClearSlot();
                return;
            }
        }

        for (int i = 0; i < skillQuickSlots.Length; i++)
        {
            if (selectedPlayer.MySkills[i] != null)
            {
                Debug.Log(selectedPlayer.MySkills[i].CallSkillName);
                skillQuickSlots[i].SetSkill(selectedPlayer.MySkills[i]);
            }
            else
            {
                skillQuickSlots[i].ClearSlot();
            }
        }
    }


    public void SetSkillToSelcetedPlayer(Skill[] p_Skills, Skill[] p_Chains)
    {
        if (selectedPlayer != null)
            selectedPlayer.SetSkills(p_Skills, p_Chains);
        else
            return;
    }

    #endregion


    // 장비 슬롯창에 캐릭터의 장착한 아이템을 그린다. 

    public void SetEquipSlot()
    {
        if (selectedPlayer == null)
        {
            for (int i = 0; i < equipSlots.Length; i++)
            {

                equipSlots[i].ClearEquipSlot();
            }

            return;
        }

        for (int i = 0; i < equipSlots.Length; i++)
        {

            var slot = selectedPlayer.GetEquipItemSlot((EquipType)i+1);
            if(slot != null)
            {
                equipSlots[i].EquipItem(slot);
            }
            else
            {
                equipSlots[i].ClearEquipSlot();
            }
        }
    }


    public void SetStatText(Character targetPlayer)
    {
        if (targetPlayer == null)
        {
            // todo 여기에 텍스트들을 언젠가 다른 말로도 바꿀수 있도록 수정 
            txt_Level.text = "Level : ";
            txt_HP.text = "체력 : " ;
            txt_MP.text = "마나 : " ;
            txt_Attack.text = "공격력 : " ;
            txt_Deffence.text = "방어력 : " ;
            txt_Speed.text = "이동 속도 : " ;
            txt_Exp.text = "경험치 : " ;
            expGauge.MyMaxValue = 0;
            expGauge.MyCurrentValue = 0;

            return; 
        }

        //targetPlayer.MyStat.ApplyOption();

        txt_Level.text = "Level : " + targetPlayer.MyStat.level; 
        txt_HP.text = "체력 : " + targetPlayer.MyStat.totalHP; 
        txt_MP.text = "마나 : " + targetPlayer.MyStat.totalMP; 
        txt_Attack.text = "공격력 : " + targetPlayer.MyStat.totalATK;
        txt_Deffence.text = "방어력 : " + targetPlayer.MyStat.totalDEF;
        txt_Speed.text = "이동 속도 : " + targetPlayer.MyStat.totalSPD;
        txt_Exp.text = "경험치 : " + targetPlayer.MyStat.maxExp;
        expGauge.MyMaxValue =  targetPlayer.MyStat.maxExp;
        expGauge.MyCurrentValue = targetPlayer.MyStat.exp;
    }
}
