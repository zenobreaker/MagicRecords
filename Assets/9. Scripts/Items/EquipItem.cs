using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

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

    BREAK_AROMR = 100,        // 방어구 파괴 (방어력 감소)
    BREAK_WEAPON,       // 무기 파괴 (공격력 감소) 
    DOWN_ATTACK_SPEED,  // 공격속도 감소

    BLEED,              // 출혈  - 지속 데미지 + 공격력 감소 
    BURN,               // 화상 - 지속 데미지 + 방어력 감소
    CURSE,              // 저주 (모든 스테이터스 감소)
    HOLD,               // 속박 - 이동불가 
    SLOW,               // 둔화 - 공격속도 / 이동속도 감소
    STURN,              // 기절 - 공격/이동 불가 
    ICE,                // 빙결 - 기절과 같음
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
}

public enum EquipType
{
    NONE = 0,
    WEAPON = 1,         // 무기 
    ARMOR,          // 장비
    WHEEL,          // 바퀴
    ACCSESORRY_1,   // 악세사리 1
    ACCSESORRY_2,   // 악세사리 1
    ACCSESORRY_3,   // 악세사리 3
    DRONE = 7, 
    RUNE = 8,
}

[System.Serializable]
public class EquipItem : Item
{
    //public Player equipTarget;  // 장착 대상 
    public EquipType equipType; // 장비 타입
    public int itemEnchantRank; // 아이템 강화 등급
    public bool isEquip;      // 아이템 장착 여부 
    public const int MAIN = 0, ADD1 = 0, ADD2 = 1, ADD3 = 2;      // 아이템 능력 순서 
    public ItemAbility itemMainAbility; // 아이템 능력 수치 
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
        this.itemMainAbility = itemMainAbility;
        this.itemAbilities = itemAbilities;
        this.itemImage = _item.itemImage;
    }

    public EquipItem(Item _item, EquipItem _equipItem) : base(_item)
    {
        this.equipType = _equipItem.equipType;
        this.itemEnchantRank = _equipItem.itemEnchantRank;
        this.isEquip = _equipItem.isEquip;
        this.itemMainAbility = _equipItem.itemMainAbility;
        this.itemAbilities = _equipItem.itemAbilities;
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

    public void SetItemPower(ItemAbility _itemAbility, int _num)
    {
        this.itemAbilities[_num] = _itemAbility;
    }

    public void SetItemPowers(ItemAbility[] _itemAbilities)
    {
        this.itemAbilities = _itemAbilities;
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
