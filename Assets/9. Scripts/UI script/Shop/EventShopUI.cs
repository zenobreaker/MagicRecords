using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    // ��ް� �⺻���� ������ �������� �Ǹŵ� ���� ��ȯ 
    private int CalcRecordItemValue(int grade, int baseValue)
    {
        int maxGrade = 3;
        int result = (maxGrade - grade) * 100 + baseValue; 
        
        return result; 
    }

    private void Awake()
    {
        InitEventShopItem();

        //DrawEventShopGroup();
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

        // �����̸� ������ ����� 
        // ���ڵ�� �����̸� �ּ� 0~3���� ��ġ�ϵ��� �Ѵ�. 

        foreach(var shopItem in eventShopItemAllData.eventShopItems)
        {
            if (shopItem == null) continue;

            // ���ڵ峪 �����ΰ���? 
            if ((ItemType)shopItem.itemType == ItemType.RECORD_VIEW)
            {
                // �׷��� �������� 0~3�� ���� �������մϴ�.
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
                // todo : ������ �����Ƿ� ������ ���� �ѱ��. 
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

    public void OpenEventShopUI()
    {
        UIPageManager.instance.OpenClose(gameObject);
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

    // ���ڵ� ������ ������ �ش� ������ �������� �����. 
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


    // �̺�Ʈ ������ �����۵� ����ϱ� 
    public void DrawEventShopGroup()
    {
        if (content == null || shopItems == null) return;
        int count = shopItems.Count;
       
        // ��ũ�� ������Ʈ ����
        InitScrollviewObject(count);

        for(int i = 0; i < count; i++)
        {
            var childObject = content.transform.GetChild(i);
            if (childObject == null)
                continue; 

            // ������Ʈ�� ������ �߰� 
            if(childObject.TryGetComponent<ShopSlot>(out var slot))
            {
                slot.AddItem(shopItems[i]);
                slot.gameObject.SetActive(true);
            }
        }

        // ��ü ��ũ�� ������Ʈ�� �ݹ� �߰� 
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

    // 아이템 관련 작업 시 콜백 
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

    // 개별 회복 포션 구매 
    public void BuyUseEachHealthPotion()
    {

        // 선택한 캐릭터를 회복 시킨다.
        UIPageManager.instance.OpenSelectCharacter((characters) =>
        {
            var target = characters.First();
            if (target == null)
                return; 

            int currentHP = target.MyCurrentHP; 

            var maxHP = target.MyMaxHP;
            target.MyCurrentHP += (int)(maxHP * 0.3f);

            int afterHP = target.MyCurrentHP;
            Debug.Log("캐릭터 회복 : " + currentHP + " " + afterHP);
        });
        
    }
    // 전체 대상을 회복 시키는 포션 구매
    public void BuyUseMultiHealthPotion()
    {
        // 선택한 플레이어들을 가져온다. 
        var playerList = InfoManager.instance.GetSelectPlayers();

        if (playerList == null) return; 

        // 각 캐릭터별로 회복 시킨다. 
        foreach (var player in playerList)
        {
            if (player.Value == null) continue;
            // 대상이 죽어있다면 넘긴다.
            if (player.Value.isDead == true)
                continue; 

            int maxHP = player.Value.MyMaxMP;
            player.Value.MyCurrentHP += (int)(maxHP * 0.1f);
        }

    }

    // 레코드 구매 
    public void BuyRecordItem(Item item)
    {
        if(item== null) return;

        //todo . 

        //
        RecordManager.instance.SelectRecord(item.itemUID);
    }


    // 유물 구매
    public void BuyRelric()
    {

    }

    // 부활 포션 구매
    public void BuyResurrectionPotion(Character character)
    {
        if (character == null || character.isDead == false) return;

        // 캐릭터의 사망 표시 해제 
        character.isDead = false;

        // 최대 HP의 30퍼를 회복시켜놓는다.
        int maxHp = character.MyMaxHP;
        character.MyCurrentHP = (int)(maxHp * 0.3f);

    }

}
