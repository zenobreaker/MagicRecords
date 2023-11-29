using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class EnchantManual : MonoBehaviour
{
    public GameObject go_BaseUI = null; // 베이스 UI

    [SerializeField] Image img_ItemIcon = null; // 아이템 아이콘 

    private EquipItem preveItem;    // 강화 이전에 정보를 가진 아이템 클래스
    public EquipItem selectedItem; // 강화할 아이템 대상 
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
    //[Header("업그레이드 결과")]
    //[SerializeField] Text txt_IncreasedAbility = null;  // 상승된 능력치
    //[SerializeField] Text[] txt_IncreaseAddedAb = null;   // 상승된 추가 능력치 


 
    // - 다음 옵션이 뜰 자리에 옵션 개방 문구 띄우기 
    // PRIVATE : 강화된 아이템 수치 구분을 해주는 함수
    void ShowIncreaseAbilityValue()
    {
        if (selectedItem == null && preveItem == null)
            return; 
            

        bool isUpgrade = false;
        if (selectedItem.itemEnchantRank > preveItem.itemEnchantRank)
            isUpgrade = true; 


        int count = 0;
        foreach (var ability in selectedItem.itemAbilities)
        {
            // 능력치가 있다면 
            if (ability.abilityType != AbilityType.NONE)
            {
                string currentValue = WritingItemAbility(ability, false);
                int margin = ability.power - preveItem.itemAbilities[count].power;
                bool isNew = false; // 새롭게 등장하는 옵션인지 체크한다.
                isNew = preveItem.itemAbilities[count].abilityType == AbilityType.NONE;
                Debug.Log("강화 전 " + preveItem.itemEnchantRank + " " + preveItem.itemAbilities[count].power);
                Debug.Log("강화 후 " + selectedItem.itemEnchantRank + " " + selectedItem.itemAbilities[count].power);
                if (isUpgrade && margin > 0 && isNew == false)
                {
                    currentValue = currentValue + "(" + "<color=orange>" + "+" + margin + "</color>" +")";
                }
                txt_ItemAddedAb[count].text = currentValue;
            }
            // 능력치가 없다면 
            else
            {
                if (CheckCanOpenAbility(selectedItem, count) == true)
                    txt_ItemAddedAb[count].text = "능력치 개방!";
                else
                {
                    txt_ItemAddedAb[count].text = "";
                }
            }

            count++;
        }

    }

    // 장비 업그레이드 함수
    private void UpgradeEquipment(EquipItem equipItem)
    {
        if (equipItem == null) return;
        int nextOptionViewItemRank = 9;
        int subViewOrUpgradeItemRank = 3;

        // 0 강화 되기 전 정보를 저장해놓는다.
        preveItem = (EquipItem)equipItem.Clone();

        // 1. 아이템 강화 수 상승
        (equipItem).itemEnchantRank += 1;

        // 2. 메인 능력치 값 상승 
        IncreaseAbility(ref equipItem.itemMainAbility);

        // 3. 강화 수치가 일정 수치일 경우 서브 능력치 추가 하거나 강화한다.
        if ((equipItem).itemEnchantRank % subViewOrUpgradeItemRank == 0)
        {
            bool isNoneAbility = false;
            foreach (var ability in equipItem.itemAbilities)
            {
                if (ability.abilityType == AbilityType.NONE)
                {
                    isNoneAbility = true;
                    break;
                }
            }

            // 3.1 서브 능력치가 전부 활성화된 상태일 경우 
            if (isNoneAbility == false)
            {
                // 서브 능력치 중 랜덤한 부위 강화
                int randIdx = Random.Range(0, selectedItem.itemAbilities.Length);
                IncreaseAbility(ref selectedItem.itemAbilities[randIdx]);
            }
            // 3.2 서브 능력치가 비어 있는 공간이 있는 경우
            else
            {
                // 랜덤한 서브 능력치를 추가한다. 
                for (int i = 0; i < selectedItem.itemAbilities.Length; i++)
                {
                    if (selectedItem.itemAbilities[i].abilityType == AbilityType.NONE)
                    {
                        AddRandomSubAbility(ref selectedItem.itemAbilities[i]);
                        break;
                    }
                }
            }
        }
    }


    // 능력치 증가
    void IncreaseAbility(ref ItemAbility p_Ability)
    {
        if (p_Ability.abilityType != AbilityType.NONE)
        {
            // todo. 능력치별 / 강화 수치별 강화 증가량 변경해야할 것 같다.
            p_Ability.power += Mathf.RoundToInt((float)p_Ability.power * 0.1f);
        }
        // 능력치가 없다면 새로운 능력치를 할당시킨다.
        else
        {
            AddRandomSubAbility(ref p_Ability);
        }

    }


    // 랜덤한 서브 능력치를 추가한다.
    void AddRandomSubAbility(ref ItemAbility p_Ability)
    {
        // 
        int idx = Random.Range(1, (int)AbilityType.MAX_ABILITY + 1);
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
            case 9:
                p_Ability.abilityType = AbilityType.CRITRATE;
                break;
            case 10:
                p_Ability.abilityType = AbilityType.CRITDMG;
                break;
        }
        p_Ability.isPercent = true;

        power = Random.Range(1, 4) * 10; // 1Rank = 10%.. 
        p_Ability.power = power;
    }


    // 강화가 될 때마다 보여주는 표기들을 최신화
    void UpdateView()
    {
        // 아이템 이름 및 강화 수치 
        DrawItemEnchantAndName(selectedItem);

        // 메인 옵션
        // 장비 아이템이 가지고 있는 주요 수치와 부가 수치 
        txt_ItemAbility.text = WritingItemAbility(selectedItem.itemMainAbility, true);

        // 서브 옵션 
        //float prevOptionValue = 0.0f;
        //for (int i = 0; i < (selectedItem).itemAbilities.Length; i++)
        //{
        //    txt_ItemAddedAb[i].text = WritingItemAbility(selectedItem.itemAbilities[i], false);
        //}
        ShowIncreaseAbilityValue();
    }


    // PRIVATE : 화면을 정리한다.
    void ClearView()
    {
        txt_ItemName.text = "";

        txt_ItemAbility.text = "";

        for (int i = 0; i < txt_ItemAddedAb.Length; i++)
        {
            txt_ItemAddedAb[i].text = "";
        }
    }

    // PRIVATE : 아이템이 다음 강화에서 능력치가 개방되는 형태인지 반환
    bool CheckCanOpenAbility(EquipItem equipItem,int count)
    {
        if (equipItem == null) return false;

        // 다음 강화되는 슬롯이 없다면 false
        int targetIdx = equipItem.GetEmptySubItemAbilityIndex();
        if (targetIdx == -1)
            return false;

        // 다음이 강화해서 열리는 능력치 칸인지 검사 
        if (targetIdx == count && (selectedItem.itemEnchantRank +1 ) / 3 == count + 1)
            return true;

        return false;
    }



    // UI 보이기
    public void ShowUI()
    {
        if (!go_BaseUI.activeSelf)
            UIPageManager.instance.OpenClose(go_BaseUI);
    }



    // 아이템 강화 수치와 이름 그리기 
    public void DrawItemEnchantAndName(EquipItem equipItem)
    {
        if (equipItem == null) return;

        txt_ItemName.text = "+" + equipItem.itemEnchantRank + " " + equipItem.itemName;
    }

    // 아이템 옵션 그리기 (강화 후 모습을 그리는 게 아니다)
    public void DrawInitItemAbility(EquipItem equipItem)
    {
        if (equipItem == null) return;

        txt_ItemAbility.text = WritingItemAbility(equipItem.itemMainAbility, true);

        for (int i = 0; i < equipItem.itemAbilities.Length; i++)
        {
            if (equipItem.itemAbilities[i].abilityType == AbilityType.NONE)
            {
                txt_ItemAddedAb[i].text = "";
            }
            else
            {
                txt_ItemAddedAb[i].text = WritingItemAbility((equipItem).itemAbilities[i], false);
            }
        }
    }


    // 아이템 정보를 세팅한다. 
    public void SetEquipItemInfo(EquipItem p_item)
    {
        if (p_item == null) return; 

        ClearView();
        img_ItemIcon.sprite = p_item.itemImage;
        selectedItem = p_item;
        preveItem = (EquipItem)selectedItem.Clone();
        DrawItemEnchantAndName(p_item);
        DrawInitItemAbility(p_item);
    }


    // 아이템 어빌리티에 따른 이름을 그린다. 
    public string WritingItemAbility(ItemAbility p_Ability, bool p_isMain)
    {
        string strText = "";

        switch(p_Ability.abilityType)
        {
            case AbilityType.ATK:
                strText = "공격력 +" + p_Ability.power.ToString();
                break;
            case AbilityType.ASPD:
                strText = "공격속도 +" + p_Ability.power.ToString();
                break;
            case AbilityType.DEF:
                strText = "방어력 +" + p_Ability.power.ToString();
                break;
            case AbilityType.HP:
                strText = "체력 +" + p_Ability.power.ToString();
                break;
            case AbilityType.HPR:
                strText = "체력 재생 +" + p_Ability.power.ToString();
                break;
            case AbilityType.MP:
                strText = "마나 +" + p_Ability.power.ToString();
                break;
            case AbilityType.MPR:
                strText = "마나 재생 +" + p_Ability.power.ToString();
                break;
            case AbilityType.SPD:
                strText = "이동속도 +" + p_Ability.power.ToString();
                break;
            case AbilityType.CRITRATE:
                strText = "치명확률 +" + p_Ability.power.ToString();
                break;
            case AbilityType.CRITDMG:
                strText = "치명피해 +" + p_Ability.power.ToString();
                break;

        }
        if (p_Ability.isPercent == false)
            return strText;
        else
            return strText + "%";
    }


 
    // 강화 버튼을 누르면 발동하는 메소드
    public void EchantEquipment()
    {
        int t_enchantCost;

        if ((selectedItem).itemEnchantRank < maxEnchantCount)
        {
            t_enchantCost = ((selectedItem).itemEnchantRank * 100) + 10;  // 강화 가격 상승 

            if (InfoManager.coin < t_enchantCost)
            {
                Debug.Log("강화 불가능! 코인 부족");
                // 토스트 메시지 
                ToastMessageContorller.CreateToastMessage("코인이 부족하여 강화가 불가능합니다.");
            }
            // 강화 가능
            else if (InfoManager.coin >= t_enchantCost)
            {
                InfoManager.coin -= t_enchantCost;

                // 장착된 아이템인지 검사 
                if ((selectedItem).isEquip)
                {
                    Debug.Log("장착된 장비 옵션 변경 루틴 ");
                    // 장착한 대상에게서 장비를 때넨다. 
                    var user = InfoManager.instance.GetMyPlayerInfo(selectedItem.userID);
                    if (user != null)
                    {
                        // 기존에 장착된 슬롯에게서 해제 
                         user.RemoveEquipment(selectedItem.equipType);
                        // 장비 업그레이드 
                        UpgradeEquipment(selectedItem);
                        // 강화된 아이템으로 다시 장착 
                        user.EquipItem(selectedItem); 
                    }
                }
                else
                {
                    // 장비 업그레이드 
                    UpgradeEquipment(selectedItem);
                }
                UpdateView();
            }

        }
        else
        {
            Debug.Log("강화 수치가 최종단계입니다. 더 이상 강화가 불가능합니다.");
            ToastMessageContorller.CreateToastMessage("강화 수치가 최종단계입니다. 더 이상 강화가 불가능합니다.");
        }
    }



}
