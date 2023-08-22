using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnchantManual : MonoBehaviour
{
    public GameObject go_BaseUI = null; // 베이스 UI
    //[SerializeField] GameObject go_Tooltip = null;  // 툴팁 UI; 

    [SerializeField] Image img_ItemIcon = null; // 아이템 아이콘 

    //Item selectedItem;  // 강화할 아이템 대상 
    public EquipItem selectedItem;
    private int[] itemEffects;
    private string itemName;
    private int maxEnchantCount = 12;

    [SerializeField] Text txt_ItemName = null;  // 아이템 명

    // 필요한 컴포넌트 
    // [SerializeField] ItemEffectDatabase theitemEffects = null;  // 아이템 효과 데이터베이스 

    // 아이템 설명란 
    [Header("기존 옵션 설명")]
    [SerializeField] Text txt_ItemAbility = null;   // 아이템 능력치 
    [SerializeField] Text[] txt_ItemAddedAb = null;   // 아이템 추가 능력치

    // 아이템 업그레이드 후 수치 설명
    [Header("업그레이드 결과")]
    [SerializeField] Text txt_IncreasedAbility = null;  // 상승된 능력치
    [SerializeField] Text[] txt_IncreaseAddedAb = null;   // 상승된 추가 능력치 


    // UI 보이기
    public void ShowUI()
    {
        if (!go_BaseUI.activeSelf)
            UIPageManager.instance.OpenClose(go_BaseUI);
    }

    // 아이템 등록 
    public void SetItem(Item p_item)
    {
        //selectedItem = p_item.itemEquipment;    // 아이템에 등록된 장비 정보를 받아낸다. 
        img_ItemIcon.sprite = p_item.itemImage;
        itemName = p_item.itemName;
        txt_ItemName.text = "+" + ((EquipItem)selectedItem).itemEnchantRank + " " + itemName;

        //SetItemEffects();

    }

    void ClearView()
    {
        txt_ItemName.text = "";

        txt_ItemAbility.text = "";
        txt_IncreasedAbility.text = "";

        for (int i = 0; i < txt_IncreaseAddedAb.Length; i++)
        {
            txt_IncreaseAddedAb[i].text = "";
            txt_ItemAddedAb[i].text = "";
        }
    }

    void ClearIncreaseText()
    {
        txt_IncreasedAbility.text = "";

        for (int i = 0; i < txt_IncreaseAddedAb.Length; i++)
        {
            txt_IncreaseAddedAb[i].text = "";
        }
    }

    public void SetEquip(EquipItem p_item)
    {
        ClearView();
        ClearIncreaseText();
        img_ItemIcon.sprite = p_item.itemImage;
        selectedItem = p_item;
        txt_ItemName.text = "+" + (selectedItem).itemEnchantRank + " " + (selectedItem).itemName;
        EnrollItemInfo(p_item);
    }


    // 아이템 정보 등록
    public void EnrollItemInfo(EquipItem p_EquipItem)
    {
        txt_ItemAbility.text = WritingItemAbility(p_EquipItem.itemMainAbility, true);
        
        for (int i = 0; i < selectedItem.itemAbilities.Length; i++)
        {
            txt_ItemAddedAb[i].text = (selectedItem).itemAbilities[i].power > 0 ? WritingItemAbility((selectedItem).itemAbilities[i], false) : " ";
        }

        //if(((EquipItem)selectedItem).((EquipItem)selectedItem) + 1<10)
        //    ShowIncreaseEffects();
    }

    public string WritingItemAbility(ItemAbility p_Ability, bool p_isMain)
    {
        string strText = "";

        switch(p_Ability.abilityType)
        {
            case AbilityType.ATK:
                strText = "공격력 + " + p_Ability.power.ToString();
                break;
            case AbilityType.ASPD:
                strText = "공격속도 + " + p_Ability.power.ToString();
                break;
            case AbilityType.DEF:
                strText = "방어력 + " + p_Ability.power.ToString();
                break;
            case AbilityType.HP:
                strText = "체력 + " + p_Ability.power.ToString();
                break;
            case AbilityType.HPR:
                strText = "체력 재생 + " + p_Ability.power.ToString();
                break;
            case AbilityType.MP:
                strText = "마나 + " + p_Ability.power.ToString();
                break;
            case AbilityType.MPR:
                strText = "마나 재생+ " + p_Ability.power.ToString();
                break;
            case AbilityType.SPD:
                strText = "이동속도 + " + p_Ability.power.ToString();
                break;

        }
        if (p_isMain)
            return strText;
        else
            return strText + "%";
    }

    public string GetItemMainAbility(int _power)
    {
        if (selectedItem.equipType == EquipType.WEAPON)
            return "공격력 + " + _power.ToString();
        if (selectedItem.equipType == EquipType.ARMOR)
            return "방어력 + " + _power.ToString();
        if (selectedItem.equipType == EquipType.ACCSESORRY_1
            || selectedItem.equipType == EquipType.ACCSESORRY_2
             || selectedItem.equipType == EquipType.ACCSESORRY_3)   
            return "특정 옵션 + " + _power.ToString();

        return "";
    }

    // 아이템 다음 강화 정보 보여주기
    void ShowIncreaseEffects()
    {
        int t_EnchantValue;

        t_EnchantValue = ((selectedItem).itemEnchantRank +1) + (selectedItem).itemMainAbility.power + Mathf.RoundToInt((selectedItem).itemMainAbility.power * 0.1f);

        txt_IncreasedAbility.text = GetItemMainAbility(t_EnchantValue);

        for (int i = 0; i < (selectedItem).itemAbilities.Length; i++)
        {
            txt_IncreaseAddedAb[i].text = (selectedItem).itemAbilities[i].power > 0 ? WritingItemAbility((selectedItem).itemAbilities[i], false) : " ";

            if (((selectedItem).itemEnchantRank+1) % 3 == 0 && ((selectedItem).itemEnchantRank+1) != 0)
            {
                int t_idx = selectedItem.itemEnchantRank+1 <= 9 ? (selectedItem.itemEnchantRank+1) / 3 - 1 : (selectedItem.itemEnchantRank+1) / 3 - 2;
                if (selectedItem.itemAbilities[t_idx].abilityType ==AbilityType.NONE)
                    txt_IncreaseAddedAb[t_idx].text = "능력치 개방!";
                else
                {
                    ItemAbility t_itemAbility = new ItemAbility();
                    t_itemAbility.power = selectedItem.itemAbilities[t_idx].power + Mathf.RoundToInt((float)selectedItem.itemAbilities[t_idx].power * 0.1f);
                    txt_IncreaseAddedAb[t_idx].text = WritingItemAbility(t_itemAbility, false);
                    
                }
            }

        }
    }

    public void EchantEquipment()
    {
        int t_enchantCost;

        if ((selectedItem).itemEnchantRank < maxEnchantCount)
        {
            t_enchantCost = ((selectedItem).itemEnchantRank * 100) + 10;  // 강화 가격 상승 

            if (LobbyManager.coin < t_enchantCost)
            {
                Debug.Log("강화 불가능! 코인 부족");
            }
            else if (LobbyManager.coin >= t_enchantCost)
            {
                LobbyManager.coin -= t_enchantCost;

                if ((selectedItem).isEquip)
                {
                    Debug.Log("장착된 장비 옵션 변경 루틴 ");
                    (selectedItem).itemEnchantRank += 1;

                    IncreaseAbility(ref selectedItem.itemMainAbility);
                    if ((selectedItem).itemEnchantRank % 3 == 0)
                    {
                        int t_idx = selectedItem.itemEnchantRank <= 9 ? selectedItem.itemEnchantRank / 3 - 1 : selectedItem.itemEnchantRank / 3 - 2;
                        IncreaseAbility(ref selectedItem.itemAbilities[t_idx]);
                    }

                    UpdateView();
                }
                else
                {
                    (selectedItem).itemEnchantRank += 1;
                    IncreaseAbility(ref selectedItem.itemMainAbility);
                    if ((selectedItem).itemEnchantRank % 3 == 0)
                    {
                        
                        int t_idx = selectedItem.itemEnchantRank <= 9 ? selectedItem.itemEnchantRank / 3 - 1 : selectedItem.itemEnchantRank / 3 - 2;
                        IncreaseAbility(ref selectedItem.itemAbilities[t_idx]);
                    }
                    UpdateView();
                }
               
            }

        }
        else if ((selectedItem).itemEnchantRank >= 10)
        {
            Debug.Log("강화 수치가 최종단계입니다. 더 이상 강화가 불가능합니다.");
            ClearIncreaseText();
        }
    }


    // 아이템 능력치 증가 
    void IncreaseMainAbility()
    {
        (selectedItem).itemMainAbility.power += (selectedItem).itemEnchantRank + Mathf.RoundToInt((selectedItem).itemMainAbility.power * 0.1f) ;
    }

    void IncreaseAbility(ref ItemAbility p_Ability)
    {
        if (p_Ability.abilityType != AbilityType.NONE)
        {
            p_Ability.power += Mathf.RoundToInt((float)p_Ability.power * 0.1f);
        }
        else
        {
            RandomSubAbility(ref p_Ability);
        }

    }

    void RandomSubAbility(ref ItemAbility p_Ability)
    {
        int idx = UnityEngine.Random.Range(1, Enum.GetNames(typeof(AbilityType)).Length);
        int power;

        switch (idx)
        {
            case 1:
                p_Ability.abilityType = AbilityType.ATK;
                break;
            case 2:
                p_Ability.abilityType = AbilityType.ASPD;
                break;
            case 3:
                p_Ability.abilityType = AbilityType.DEF;
                break;
            case 4:
                p_Ability.abilityType = AbilityType.SPD;
                break;
            case 5:
                p_Ability.abilityType = AbilityType.HP;
                break;
            case 6:
                p_Ability.abilityType = AbilityType.HPR;
                break;
            case 7:
                p_Ability.abilityType = AbilityType.MP;
                break;
            case 8:
                p_Ability.abilityType = AbilityType.MPR;
                break;
        }
        p_Ability.isPercent = true;

        power = UnityEngine.Random.Range(1, 4) * 10; // 1Rank = 10%.. 
        p_Ability.power = power; 
          //  ((EquipItem)selectedItem).itemAbilities[((EquipItem)selectedItem).itemEnchantRank / 3 - 1].type = types[idx];
          //  ((EquipItem)selectedItem).itemAbilities[((EquipItem)selectedItem).itemEnchantRank / 3 - 1].isPercent = isPercent;
    }


    // 강화가 될 때마다 보여주는 표기들을 최신화
    void UpdateView()
    {
        // 아이템 이름 및 강화 수치 
        txt_ItemName.text = "+" + (selectedItem).itemEnchantRank + " " + selectedItem.itemName;

        // 장비 아이템이 가지고 있는 주요 수치와 부가 수치 
        //txt_ItemAbility.text = selectedItem.itemMainAbility.ToString();
        txt_ItemAbility.text = WritingItemAbility(selectedItem.itemMainAbility, true);
        
        for (int i = 0; i < (selectedItem).itemAbilities.Length; i++)
        {
            txt_ItemAddedAb[i].text =  WritingItemAbility(selectedItem.itemAbilities[i], false);
        }

        // 다음 강화에 대한 수치표기 
        if ((selectedItem).itemEnchantRank < maxEnchantCount)
            ShowIncreaseEffects();
    }

}
