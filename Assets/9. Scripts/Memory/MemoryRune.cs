using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 가이드 메모리 드론에 추가적 효과를 부여하는 부착물
// 룬도 아이템 클래스를 상속받는다 
[System.Serializable]
public class MemoryRune : EquipItem
{
    public MemoryRune(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye, ItemRank _itemRank,
        string _itemDesc, int _itemEach, int _itemValue, string _itemIMG, EquipType equipType, 
        int itemEnchantRank, bool isEquiped, ItemAbility itemMainAbility, bool _isSale = false) 
        : base(_itemUID, _keycode, _itemName, _itemTpye, _itemRank, _itemDesc, _itemEach, 
            _itemValue, _itemIMG, equipType, itemEnchantRank, 
        isEquiped, itemMainAbility, _isSale)
    {

    }

    public new MemoryRune Clone()
    {
        MemoryRune mr = new MemoryRune(this.itemUID, itemKeycode, itemName, itemType, itemRank,
            itemDesc, itemCount, itemValue, itemImgID, equipType, itemEnchantRank, isEquip, itemMainAbility);


        return mr; 
    }
}
