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

    // �����ۿ� ���� �ݹ� ȣ��
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

    // ȸ�� ������ ���Ž� 
    public void BuyUseEachHealthPotion()
    {
        // �ڽ�Ʈ ����

        // ĳ���� ���� ui ���ֱ� 
        UIPageManager.instance.OpenSelectCharacter((character) =>
        {
            //todo 
            int currentHP = character.MyCurrentHP; 

            var maxHP = character.MyMaxHP;
            character.MyCurrentHP += (int)(maxHP * 0.3f);

            int afterHP = character.MyCurrentHP;
            Debug.Log("�ش� ĳ���� ü�� ȸ�� : " + currentHP + " " + afterHP);
        });
        
    }
    public void BuyUseMultiHealthPotion()
    {
        // Ž�� ���� ĳ���͵� ȸ�� 
        var playerList = InfoManager.instance.GetSelectPlayers();

        if (playerList == null) return; 

        // ĳ���͵� ���� ȸ��
        foreach (var player in playerList)
        {
            if (player.Value == null) continue;
            // ������ ĳ���ʹ� ȸ�� �Ұ�
            if (player.Value.isDead == true)
                continue; 

            int maxHP = player.Value.MyMaxMP;
            player.Value.MyCurrentHP += (int)(maxHP * 0.1f);
        }

    }

    // ���ڵ� ������ ���� ��
    public void BuyRecordItem(Item item)
    {
        if(item== null) return;

        //todo . ���� �� ���� 

        // ���ڵ� �Ŵ����� ������ ���ڵ� �߰��ϱ� 
        RecordManager.instance.SelectRecord(item.itemUID);
    }


    // ���� ������ ���Ž� 
    public void BuyRelric()
    {

    }

    // ��Ȱ�� ���Ž� 
    public void BuyResurrectionPotion(Character character)
    {
        if (character == null || character.isDead == false) return;

        // ���� ĳ���� ��Ȱ 
        character.isDead = true;

        // ü���� 30�۷� ä�����´�.
        int maxHp = character.MyMaxHP;
        character.MyCurrentHP = (int)(maxHp * 0.3f);

    }

}
