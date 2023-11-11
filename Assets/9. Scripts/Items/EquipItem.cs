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
}

public enum EquipType
{
    NONE = 0,
    WEAPON = 1,         // ���� 
    ARMOR,          // ���
    WHEEL,          // ����
    ACCSESORRY_1,   // �Ǽ��縮 1
    ACCSESORRY_2,   // �Ǽ��縮 1
    ACCSESORRY_3,   // �Ǽ��縮 3
    DRONE = 7, 
    RUNE = 8,
}

[System.Serializable]
public class EquipItem : Item
{
    //public Player equipTarget;  // ���� ��� 
    public EquipType equipType; // ��� Ÿ��
    public int itemEnchantRank; // ������ ��ȭ ���
    public bool isEquip;      // ������ ���� ���� 
    public const int MAIN = 0, ADD1 = 0, ADD2 = 1, ADD3 = 2;      // ������ �ɷ� ���� 
    public ItemAbility itemMainAbility; // ������ �ɷ� ��ġ 
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
