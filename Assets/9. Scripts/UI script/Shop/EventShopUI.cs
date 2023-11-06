using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;


   

[System.Serializable]
public class EventShopItemData
{
    public int id;
    public string itemName;
    public int itemValue;
    public int targetID;
    public int itemType;
    public string itemSprite;
}

[System.Serializable]
public class EventShopItemAllData
{
    public EventShopItemData[] eventShopItems;
}

public class EventShopUI : UiBase
{
    public TextAsset eventShopItemTextAsset;

    EventShopItemAllData eventShopItemAllData;

    public List<Item> shopItems;

    public EventShopConfirmPage confirmPage;

    public ChoiceAlert choiceAlert;


    // 등급과 기본값을 받으면 상점에서 판매될 값을 반환 
    private int CalcRecordItemValue(int grade, int baseValue)
    {
        int maxGrade = 3;
        int result = (maxGrade - grade) * 100 + baseValue; 
        
        return result; 
    }

    private void Awake()
    {
        InitEventShopItem();

    }

    private void Start()
    {
        DrawEventShopGroup();
        
    }

    public override void RefreshUI()
    {
        DrawEventShopGroup();
    }

    private void InitEventShopItem()
    {
        eventShopItemAllData = JsonUtility.FromJson<EventShopItemAllData>(eventShopItemTextAsset.text);

        // 물약이면 물약을 만들고 
        // 레코드랑 유물이면 최소 0~3개를 배치하도록 한다. 

        foreach(var shopItem in eventShopItemAllData.eventShopItems)
        {
            if (shopItem == null) continue;

            // 레코드나 유물인가요? 
            if ((ItemType)shopItem.itemType == ItemType.RECORD_VIEW)
            {
                // 그러면 아이템을 0~3개 정도 만들어야합니다.
                int randCount = Random.Range(0, 4);
                if(RecordManager.instance != null)
                {
                    var recordList = RecordManager.instance.GetRandomRecordByRandRange(0, 4);
                    foreach(var id in recordList )
                    {
                        var record = RecordManager.instance.GetRecordInfoByID(id);

                        int value = CalcRecordItemValue(record.grade, shopItem.itemValue); 
                        
                        var item = CreateEventShopItemByRecord(record, value);
                        item.itemType = ItemType.RECORD_VIEW;
                        shopItems.Add(item);
                    }
                }
            }
            else if((ItemType)shopItem.itemType == ItemType.RELRIC_VIEW)
            {
                // todo : 유물이 없으므로 정보가 오면 넘긴다. 
                continue; 
            }
            else
            {
                var item = CreateEventShopItem(shopItem);
                item.itemType = ItemType.POTION_VIEW;
                shopItems.Add(item);
            }
        }

    }

    public Item CreateEventShopItem(EventShopItemData shopItem)
    {
        if (shopItem == null) return null; 

        Item item = new Item();

        item.itemUID = shopItem.id;
        item.uniqueID = shopItem.targetID;
        item.itemName = shopItem.itemName;
        item.itemValue = shopItem.itemValue;
        string path = "Image/" + shopItem.itemSprite;
        Sprite sprite = Resources.Load<Sprite>(path);
        item.itemImage = sprite;

        return item; 
    }

    // 레코드 정보를 받으면 해당 정보로 아이템을 만든다. 
    public Item CreateEventShopItemByRecord(RecordInfo record, int value)
    {
        if (record == null) return null;  

        Item item = new Item();
        item.itemUID = record.id;
        Sprite sprite;
        sprite = Resources.Load<Sprite>(record.spritePath);
        item.itemImage = sprite;
        item.itemValue = value; 
        return item; 
    }


    // 이벤트 상점에 아이템들 등록하기 
    public void DrawEventShopGroup()
    {
        if (content == null || shopItems == null) return;
        int count = shopItems.Count;
       
        // 스크롤 오브젝트 생성
        InitScrollviewObject(count);

        for(int i = 0; i < count; i++)
        {
            var childObject = content.transform.GetChild(i);
            if (childObject == null)
                continue; 

            // 오브젝트에 아이템 추가 
            if(childObject.TryGetComponent<ShopSlot>(out var slot))
            {
                slot.AddItem(shopItems[i]);
            }
        }

        // 전체 스크롤 오브젝트에 콜백 추가 
        SetScrollviewChildObjectsCallack<ShopSlot>((component) =>
        {
            component.SetActionCallback(() =>
            {
                if (confirmPage != null)
                {
                    confirmPage.OpenConfirmBuyItemUI(component.MyItem);
                    confirmPage.SetCallback(() =>
                    {
                        CallbackToItemPopup(component.MyItem);
                    });
                    UIPageManager.instance.OpenClose(confirmPage.gameObject);
                }
            });

        });
    }

    // 아이템에 따른 콜백 호출
    void CallbackToItemPopup(Item item)
    {
        if (item == null) return; 

        var itemType = item.itemType;

        if(itemType == ItemType.POTION_VIEW)
        {
            if(item.itemUID == 1)
            {
                BuyUseEachHealthPotion();
            }
            else if(item.itemUID == 2)
            {
                BuyUseMultiHealthPotion();
            }
        }
        else if(itemType == ItemType.RECORD_VIEW)
        {

        }
        else if(itemType == ItemType.RELRIC_VIEW)
        {
            BuyRecordItem(item);
        }
        else if(itemType == ItemType.RESURRECTION_VIEW)
        {

        }
    }

    // 회복 아이템 구매시 
    public void BuyUseEachHealthPotion()
    {
        // 코스트 감소

        // 캐릭터 선택 ui 켜주기 
        UIPageManager.instance.OpenSelectCharacter((character) =>
        {
            //todo 
            int currentHP = character.MyCurrentHP; 

            var maxHP = character.MyMaxHP;
            character.MyCurrentHP += (int)(maxHP * 0.3f);

            int afterHP = character.MyCurrentHP;
            Debug.Log("해당 캐릭터 체력 회복 : " + currentHP + " " + afterHP);
        });
        
    }
    public void BuyUseMultiHealthPotion()
    {
        // 탐사 중인 캐릭터들 회복 
        var playerList = InfoManager.instance.GetSelectPlayers();

        if (playerList == null) return; 

        // 캐릭터들 전부 회복
        foreach (var player in playerList)
        {
            if (player.Value == null) continue;
            // 쓰러진 캐릭터는 회복 불가
            if (player.Value.isDead == true)
                continue; 

            int maxHP = player.Value.MyMaxMP;
            player.Value.MyCurrentHP += (int)(maxHP * 0.1f);
        }

    }

    // 레코드 아이템 구매 시
    public void BuyRecordItem(Item item)
    {
        if(item== null) return;

        //todo . 구매 후 연출 

        // 레코드 매니저에 구매한 레코드 추가하기 
        RecordManager.instance.SelectRecord(item.itemUID);
    }


    // 유물 아이템 구매시 
    public void BuyRelric()
    {

    }

    // 부활제 구매시 
    public void BuyResurrectionPotion(Character character)
    {
        if (character == null || character.isDead == false) return;

        // 죽은 캐릭터 부활 
        character.isDead = true;

        // 체력은 30퍼로 채워놓는다.
        int maxHp = character.MyMaxHP;
        character.MyCurrentHP = (int)(maxHp * 0.3f);

    }

}
