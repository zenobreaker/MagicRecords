using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Equipment,
    Used,
    Ingredient,
    ETC,
    Coin,
    Drone,
}

public enum ItemRank
{
    NONE,
    Common,
    Magic,
    Rare,
    Unique,
    Legendary,
}

[System.Serializable]
public class Item
{
    //게임 오브젝트에 붙일 필요가 없는  스크립트 생성 

    // #. 외부 DB 정보
    public int itemUID;      // 아이템의 고유 ID
    public string itemKeycode;      // 아이템의 키코드 
    public string itemName;     // 아이템 이름
    public ItemType itemType;   // 아이템의 유형
    public ItemRank itemRank;   // 아이템의 등급
    public int itemEach;       // 아이템 개수 
    public int itemValue;       // 아이템 가격 
    [TextArea]
    public string itemDesc;     // 아이템의 설명

    // #. 게임 내에서 리소스로 받아올 정보 
    public int uniqueID;        // 소지자 ID
    public bool isSale;         // 아이템 판매 여부 (상점 한)
    public string itemImgID;    // 아이템 이미지 아이디 
    public string itemSound;    // 아이템 획득 시, 사운드 
    public Sprite itemImage;    // 아이템 이미지

    public Item()
    {

    }

    public Item(Item _item)
    {
        itemUID = _item.itemUID;
        itemKeycode = _item.itemKeycode; 
        itemName = _item.itemName;
        itemDesc = _item.itemDesc;

        itemType = _item.itemType;
        itemRank = _item.itemRank;
        itemValue = _item.itemValue;
        itemEach = _item.itemEach;
        itemImgID = _item.itemImgID;
        isSale = _item.isSale;
        uniqueID = _item.uniqueID;
        // equipType = _item.equipType;
        //   itemEnchantRank = _item.itemEnchantRank;
        //   isEquiped = _item.isEquiped;
        //itemMainAbility = _item.itemMainAbility;
        //itemAbilities[ADD1] = _item.itemAbilities[ADD1];
        //itemAbilities[ADD2] = _item.itemAbilities[ADD2];
        //itemAbilities[ADD3] = _item.itemAbilities[ADD3];
        

        itemImage = Resources.Load("ItemImage/" + _item.itemImgID.ToString(), typeof(Sprite)) as Sprite;
    }

    public Item(ItemType _itemType, int _itemUID, int _itemEach)
    {
        this.itemType = _itemType;
        this.itemUID = _itemUID;
        this.itemEach = _itemEach;
    }

    public Item(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye, ItemRank _itemRank,
        string _itemDesc, int _itemEach, int _itemValue, string _itemIMG, 
        bool _isSale = false,int _uniqueID = 0)
    {
        itemUID = _itemUID;
        itemKeycode = _keycode; 
        itemName = _itemName;
        itemDesc = _itemDesc;

        itemType = _itemTpye;
        itemRank = _itemRank;
        itemValue = _itemValue;
        itemEach = _itemEach;
        itemImgID = _itemIMG;
        // equipType = _equipType;

        //        itemMainAbility = _mainAbil;

        //      itemValue = _itemValue;
        //    itemEnchantRank = _echantRank;
        //  isEquiped = _isEquiped;
        isSale = _isSale;
        uniqueID = _uniqueID;


        //itemAbilities[ADD1] = _add1;
        //itemAbilities[ADD2] = _add2;
        //itemAbilities[ADD3] = _add3;

        itemImage = Resources.Load("ItemImage/" + _itemIMG.ToString(), typeof(Sprite)) as Sprite;
    }



    public void SetItem(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye, 
        ItemRank _itemRank, string _itemIMG, string _itemDesc,
        bool _isSale = false, int _uniqueID = 0)
    {
        itemUID = _itemUID;
        itemKeycode= _keycode;
        itemType = _itemTpye;
        itemRank = _itemRank;
        itemName = _itemName;
        itemDesc = _itemDesc;
        itemImgID = _itemIMG;

        isSale = _isSale;
        uniqueID = _uniqueID;
    }

    public Item MyItem
    {
        get { return this; }
    }
       

}
