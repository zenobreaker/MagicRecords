using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AbilityType
{
    NONE = 0,
    ATK = 1,
    ASPD = 2,
    DEF = 3,
    SPD = 4,
    HP = 5,
    HPR = 6,
    MP = 7,
    MPR = 8,
    CRITRATE = 9,
    CRITDMG = 10,
    MAX_ABILITY = CRITDMG,
    BREAK_AROMR = 100,        // �� �ı� (���� ����)
    BREAK_WEAPON,       // ���� �ı� (���ݷ� ����) 
    DOWN_ATTACK_SPEED,  // ���ݼӵ� ����

    BLEED,              // ����  - ���� ������ + ���ݷ� ���� 
    BURN,               // ȭ�� - ���� ������ + ���� ����
    CURSE,              // ���� (��� �������ͽ� ����)
    HOLD,               // �ӹ� - �̵��Ұ� 
    SLOW,               // ��ȭ - ���ݼӵ� / �̵��ӵ� ����
    STURN,              // ���� - ����/�̵� �Ұ� 
    ICE,                // ���� - ������ ����

    // Ư�� �����̻�
    CURSE_HATED = 404,  // ������ ���� - �������� ���ݷ� ���� ��ŭ �ʸ��� ����
}

[System.Serializable]
public struct ItemAbility
{
    public bool isPercent;
    public AbilityType abilityType;
    public int power;

    public static explicit operator ItemAbility(int v)
    {
        throw new NotImplementedException();
    }

    public ItemAbility Copy()
    {
        ItemAbility ability = new ItemAbility();
        ability.isPercent = this.isPercent;
        ability.abilityType = this.abilityType;
        ability.power = this.power;

        return ability;
    }
}

public enum EquipType
{
    NONE = 0,
    WEAPON = 1,     // 무기
    ARMOR,          // 방어구
    WHEEL,          // 바퀴
    ACCSESORRY_1,   // 악세사리 1
    ACCSESORRY_2,   // 악세사리 2
    ACCSESORRY_3,   // 악세사리 3
    DRONE = 7, 
    RUNE = 8,
}

[System.Serializable]
public class EquipItem : Item
{
    //public Player equipTarget;  
    public EquipType equipType; // 장비타입
    public int itemEnchantRank; // 장비 강화 수치
    public bool isEquip;      // 장착 여부 
    public const int MAIN = 0, ADD1 = 0, ADD2 = 1, ADD3 = 2;      // 추가 능력치 
    public ItemAbility itemMainAbility; // 장비의 주요 능력치
    public ItemAbility[] itemAbilities;

    public EquipItem(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye,
        ItemRank _itemRank, string _itemDesc, int _itemEach, int _itemValue,
        string _itemIMG, EquipType equipType,
                     int itemEnchantRank,
                     bool isEquiped,
                     ItemAbility itemMainAbility,
                     bool _isSale = false) :
        base(_itemUID, _keycode, _itemName, _itemTpye, _itemRank, _itemDesc, 
                         _itemEach, _itemValue, _itemIMG, _isSale)
    {
        this.equipType = equipType;
        this.itemEnchantRank = itemEnchantRank;
        this.isEquip = isEquiped;
        this.itemMainAbility = itemMainAbility;
        //this.itemAbilities = itemAbilities;

        this.itemImage = Resources.Load("ItemImage/" + _itemIMG.ToString(), typeof(Sprite)) as Sprite;
    }

    public EquipItem(Item _item, EquipType equipType,
                     int itemEnchantRank,
                     bool isEquiped,
                     ItemAbility itemMainAbility,
                     ItemAbility[] itemAbilities) : base(_item)
    {
        this.equipType = equipType;
        this.itemEnchantRank = itemEnchantRank;
        this.isEquip = isEquiped;
        this.itemMainAbility = itemMainAbility.Copy();
        this.itemImage = _item.itemImage;
        
        SetSubAbility(itemAbilities);
        SetRandomSubAbility();
    }

    public EquipItem(Item _item, EquipItem _equipItem) : base(_item)
    {
        this.equipType = _equipItem.equipType;
        this.itemEnchantRank = _equipItem.itemEnchantRank;
        this.isEquip = _equipItem.isEquip;
        this.itemMainAbility = _equipItem.itemMainAbility.Copy();
        //this.itemAbilities = _equipItem.itemAbilities;
        SetSubAbility(_equipItem.itemAbilities);

        this.itemImage = _item.itemImage;
    }

  
    public void SetSubAbility(ItemAbility[] itemAbilities)
    {
        this.itemAbilities = new ItemAbility[itemAbilities.Length];

        if (itemAbilities != null)
        {

            for (int i = 0; i < itemAbilities.Length; i++)
            {
                this.itemAbilities[i].abilityType = itemAbilities[i].abilityType;
                this.itemAbilities[i].isPercent = itemAbilities[i].isPercent;
                this.itemAbilities[i].power = itemAbilities[i].power;
            }
        }
        else
        {
            for (int i = 0; i < itemAbilities.Length; i++)
            {
                this.itemAbilities[i].abilityType = AbilityType.NONE;
                this.itemAbilities[i].isPercent = true;
                this.itemAbilities[i].power = 0;
            }
        }
        
    }


    // 아이템에 등급에 따라 랜덤한 옵션이 부여된다.
    public void SetRandomSubAbility()
    {
        if (itemAbilities.Length <= 0)
            return;

        int optionCount = 0;
        int minPower = 0;
        int maxPower = 0;
        // 커먼 : 0 매직 : 1 레어 : 1 유니크 : 2 전설 : 3
        switch(itemRank)
        {
            case ItemRank.Common:
                optionCount = 0;
                minPower = 1;
                maxPower = 7;
                break;
            case ItemRank.Magic:
            case ItemRank.Rare:
                optionCount = 1;
                minPower = 1;
                maxPower = 11;
                break;
            case ItemRank.Unique:
                optionCount = 2;
                minPower = 5;
                maxPower = 16;
                break;
            case ItemRank.Legendary:
                optionCount = 3;
                minPower = 7;
                maxPower = 21;
                break;
        }

        // 옵션 개수 만큼 서브 옵션 추가 
        for (int i = 0; i < optionCount; i++)
        {
            if(itemAbilities[i].abilityType == AbilityType.NONE)
            {
                int idx = Random.Range(1, (int)AbilityType.MAX_ABILITY + 1);
                int power = Random.Range(minPower, maxPower); 

                itemAbilities[i].abilityType = (AbilityType)idx;
                itemAbilities[i].power = power;
                itemAbilities[i].isPercent = true;

            }
        }

    }

    public void SetItemPower(ItemAbility _itemAbility, int _num)
    {
        this.itemAbilities[_num] = _itemAbility;
    }

    public void SetItemPowers(ItemAbility[] _itemAbilities)
    {
        this.itemAbilities = _itemAbilities;
    }

    // 장비가 갖고 있는 해금되지 않은 가장 가까운 서브아이템 인덱스값 반환
    // 없다면 -1을 반환한다. 
    public int GetEmptySubItemAbilityIndex()
    {
        int index = -1;
        int count = 0; 
        foreach(var ability in itemAbilities)
        {
            if(ability.abilityType == AbilityType.NONE)
            {
                index = count;
                break; 
            }
            count++; 
        }

        return index; 
    }


    public override object Clone()
    {
        Item item = (Item)base.Clone(); 

        EquipItem equipItem = new(item, equipType, itemEnchantRank, isEquip, itemMainAbility,
            itemAbilities);


        return equipItem;
    }

    //public void SetEquipItem( EquipType _equipType,
    //                 int _itemEnchantRank,
    //                 bool _isEquiped,
    //                 int _itemMainAbility,
    //                 ItemAbility[] _itemAbilities)
    //{

    //    this.equipType = _equipType;
    //    this.itemEnchantRank = _itemEnchantRank;
    //    this.isEquiped = _isEquiped;
    //    this.itemMainAbility = _itemMainAbility;
    //    this.itemAbilities = _itemAbilities;

    //}
}
