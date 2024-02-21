using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemType
{
    NONE = 0,
    Equipment,
    Used,
    Ingredient,
    ETC,
    Coin,
    Drone,

    POTION_VIEW = 11,
    RECORD_VIEW = 12,
    RELRIC_VIEW = 13,
    RESURRECTION_VIEW = 14,
}

public enum ItemRank
{
    NONE = 0,
    Common,
    Magic,
    Rare,
    Unique,
    Legendary,
}

[System.Serializable]
public class Item : ICloneable
{
    //게임 오브젝트에 붙일 필요가 없는  스크립트 생성 

    // #. 외부 DB 정보
    public int itemUID;      // 아이템의 정보 ID
    public string itemKeycode = "";      // 아이템의 키코드 
    public string itemName = "";     // 아이템 이름
    public ItemType itemType;   // 아이템의 유형
    public ItemRank itemRank;   // 아이템의 등급
    public int itemCount;       // 아이템 개수 
    public int itemValue;       // 아이템 가격 
    [TextArea]
    public string itemDesc;     // 아이템의 설명

    // #. 게임 내에서 리소스로 받아올 정보 
    public int uniqueID = 0;        // 아이템의 고유한 ID값 
    public int userID;        // 소지자 ID
    public bool isSale;         // 아이템 판매 여부 (상점 한)
    public string itemImgPath = "";    // 아이템 이미지 경로
    public string itemSound = "" ;    // 아이템 획득 시, 사운드 
    public Sprite itemImage;    // 아이템 이미지

    public Item()
    {
        uniqueID = 0;
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
        itemCount = _item.itemCount;
        itemImgPath = _item.itemImgPath;
        isSale = _item.isSale;
        uniqueID = _item.uniqueID;
        this.userID = _item.userID;

        if(_item.itemImage != null)
        {
            this.itemImage = _item.itemImage;
        }    
        else
        {
            itemImage = Resources.Load<Sprite>("Image/" + itemImgPath);
        }
    }

    public Item(ItemType itemType, int itemUID, int itemCount)
    {
        this.itemType = itemType;
        this.itemUID = itemUID;
        this.itemCount = itemCount;
    }


    public Item(int id, string name, string desc, string imgPath = "")
    {
        itemType = ItemType.NONE;
        itemCount = 0;
        
        this.itemUID = id;
        itemName = name;
        itemDesc = desc; 
        itemImgPath = imgPath;

        itemImage = Resources.Load<Sprite>("Image/" + itemImgPath);
    }

    public Item(int _itemUID, string _keycode, string _itemName, ItemType _itemTpye, ItemRank _itemRank,
        string _itemDesc, int itemCount, int _itemValue, string itemImgPath, 
        bool _isSale = false,int userID = 0)
    {
        itemUID = _itemUID;
        itemKeycode = _keycode; 
        itemName = _itemName;
        itemDesc = _itemDesc;

        itemType = _itemTpye;
        itemRank = _itemRank;
        itemValue = _itemValue;
        this.itemCount = itemCount;
        this.itemImgPath = itemImgPath;
 
        isSale = _isSale;
        this.userID = userID;

        itemImage = Resources.Load<Sprite>("Image/" + itemImgPath);
    }


    public void SetItemImageForFullPath(string fullPath)
    {
        itemImage = Resources.Load(fullPath, typeof(Sprite)) as Sprite;
    }

    public virtual object Clone()
    {
        Item item = new(itemUID, itemKeycode, itemName, itemType, 
            itemRank, itemDesc, itemCount, itemValue, itemImgPath, false, userID);

        return item;
    }

    public Item MyItem
    {
        get { return this; }
    }
       

}

