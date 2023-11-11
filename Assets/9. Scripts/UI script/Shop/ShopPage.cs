using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopPage : TabManual
{
    public static ShopPage instance;

    [SerializeField] ShopSlot shopSlot = null;
    [SerializeField] GameObject parentSlotPage = null;    // 슬롯의 부모 오브젝트
    [SerializeField] ShopDialog saleAlertUI = null;   // 구매 의사 UI

    public ItemDatabase itemDatabase; 
    private List<ShopSlot> shopSlots = new List<ShopSlot>();

    [Header("일반 아이템 상점 목록")]
    [SerializeField] List<Item> weaponItems = new List<Item>();
    [SerializeField] List<Item> armorItems = new List<Item>();
    [SerializeField] List<Item> accesroyItems = new List<Item>();
    [SerializeField] List<Item> usedItems = new List<Item>();
    [SerializeField] List<Item> skillItems = new List<Item>();

    [Header("랜덤 상점 목록")]
    [SerializeField] List<Item> r_weaponItems = new List<Item>();
    [SerializeField] List<Item> r_armorItems = new List<Item>();
    [SerializeField] List<Item> r_accesroyItems = new List<Item>();
    [SerializeField] List<Item> r_usedItems = new List<Item>();
    [SerializeField] List<Item> r_skillItems = new List<Item>();




    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetList();
        SettingShopSlot(weaponItems);
        TabSetting(0);
        //Debug.Log("탭세팅");
        //TabSlotOpen(selectedTab);
    }

    public void BuyItem()
    {
        saleAlertUI.BuyItem();
    }

    public void OpenShopPageUI()
    {
        UIPageManager.instance.OpenClose(go_BaseUI);

        TabSetting(0);
    }

    // 구매의사 UI 켜기
    public void OpenSaleUI(ShopSlot _item)
    {
        saleAlertUI.ShowToolTip(_item);
    }

    // 아이템 상점 페이지를 초기화 
    public void ClearPage()
    {
        for (int i = 0; i < shopSlots.Count; i++)
        {
            shopSlots[i].ClearSlot();
        }
    }

    void SetList()
    {
        if (itemDatabase != null)
        {
            //itemDatabase.InitializeItemList();
            weaponItems = itemDatabase.weaponList;
            armorItems = itemDatabase.armorList;
            accesroyItems = itemDatabase.accessroyList;
        }
    }

    // 정해진 아이템 리스트만큼 슬롯프리팹 생성 후 리스트 정렬 
    public void SettingShopSlot(List<Item> _items)
    {
      
        if (_items.Count != shopSlots.Count)
        {
            if (_items.Count > shopSlots.Count)
            {
                int t_start = shopSlots.Count > 0 ? shopSlots.Count - 1 : 0;

                for (int i = t_start; i < _items.Count; i++)
                {
                    ShopSlot clone = Instantiate(shopSlot, Vector3.zero, Quaternion.identity);
                    clone.transform.SetParent(parentSlotPage.transform, false);
                    shopSlots.Add(clone);
                }
            }
            else if (_items.Count < shopSlots.Count)
            {
                int t_start = _items.Count > 0 ? _items.Count - 1 : 0;

                for (int i = t_start; i < shopSlots.Count; i++)
                {
                    shopSlots[i].ClearSlot();
                    shopSlots[i].gameObject.SetActive(false);
                }
            }

        }
       
        for (int i = 0; i < _items.Count; i++)
        {
            shopSlots[i].gameObject.SetActive(true);
        }
        

    }

    // 초기 상점으로 복구 
    public void InitShopPage()
    {
        SettingShopSlot(weaponItems);
    }

    // 슬롯에 아이템 넣기 
    public void SettingItems(List<Item> _items)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            shopSlots[i].ClearSlot();
            //shopSlots[i].SetSize(parentSlotPage.GetComponent<GridLayoutGroup>().cellSize);
            shopSlots[i].AddItem(_items[i]);
            //if (RLModeController.isRLMode)
            shopSlots[i].SetItemSale();
        }
    }

    public void TabSetting(int _tabNumber)
    {
        SoundManager.instance.PlaySE("Confirm_Click");
        ClearPage();

        if (RLModeController.isRLMode)
        {
            switch (_tabNumber)
            {

                case 0:
                    SettingShopSlot(r_weaponItems);
                    SettingItems(r_weaponItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 1:
                    SettingShopSlot(r_armorItems);
                    SettingItems(r_armorItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 2:
                    SettingShopSlot(r_accesroyItems);
                    SettingItems(r_accesroyItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 3:
                    SettingShopSlot(r_usedItems);
                    SettingItems(r_usedItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 4:
                    SettingShopSlot(r_skillItems);
                    SettingItems(r_skillItems);
                    TabSlotOpen(parentSlotPage);
                    break;
            }
        }
        else
        {
            // 기존 리스트에 아이템을 지우고 해당 탭의 아이템 리스트를 첨부함 
            switch (_tabNumber)
            {

                case 0:
                    SettingShopSlot(weaponItems);
                    SettingItems(weaponItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 1:
                    SettingShopSlot(armorItems);
                    SettingItems(armorItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 2:
                    SettingShopSlot(accesroyItems);
                    SettingItems(accesroyItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 3:
                    SettingShopSlot(usedItems);
                    SettingItems(usedItems);
                    TabSlotOpen(parentSlotPage);
                    break;
                case 4:
                    SettingShopSlot(skillItems);
                    SettingItems(skillItems);
                    TabSlotOpen(parentSlotPage);
                    break;
            }
        }

    }

    public  ItemRank GetRankRandom()
    {
        int  _maRate = 3, _raRate = 6, _uniRate = 8, _legRate = 9;  // 0 1 2 345 7 8 9

        int rate = Random.Range(0, 10);

        if (rate < _maRate)
            return  ItemRank.Common;
        else if (rate >= _maRate && rate < _raRate)
            return  ItemRank.Magic;
        else if (rate >= _raRate && rate < _uniRate)
            return  ItemRank.Rare;
        else if (rate >= _uniRate && rate < _legRate)
            return  ItemRank.Unique;
        else 
            return  ItemRank.Legendary;
    }

    // 아이템 랭크로 리스트 방출 
    public List<Item> GetSelectedRankItem(List<Item> _itemList, ItemRank _seletedRank)
    {
        List<Item> _rankedItemList = new List<Item>(); 

        for (int i = 0; i < _itemList.Count; i++)
        {
            if (_itemList[i].itemRank == _seletedRank)
                _rankedItemList.Add(_itemList[i]);
        }

        return _rankedItemList;
    }

    // 아이템 리스트에서 랜덤으로 아이템을 빼낸다. 
    public List<Item> GetRandomItemData(List<Item> _itemList, int max)
    {
        List<Item> randArray = _itemList;
        List<Item> resultArray = new List<Item>();

        if (max <= 2)
        {
            for (int i = 0; i < max; i++)
            {
                resultArray.Add(randArray[i]);
            }
            return resultArray;
        }

        for (int i = 0; i < max; i++)
        {
            int rand = Random.Range(0, randArray.Count);    // 
            resultArray.Add(randArray[rand]);

            if (randArray.Count > 1)
                randArray.RemoveAt(rand);
        }

        return resultArray;
    }

    public void ExcuteRandonItemLogic(ref List<Item> _itemList, List<Item> _originalList)
    {
        ItemRank selectedRank = GetRankRandom();
        List<Item> tempList = GetSelectedRankItem(_originalList, selectedRank);

        if (tempList.Count > 2)
        {
            int max = Random.Range(1, tempList.Count);
            _itemList = GetRandomItemData(tempList, max);
        }
        else
        {
            //Debug.Log(tempList.Count + "리스트 개수 ");
            _itemList = GetRandomItemData(tempList, tempList.Count);
        }
    }

    public void RandomSettingItems()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    ExcuteRandonItemLogic(ref r_weaponItems, weaponItems);
                    break;
                case 1:
                    ExcuteRandonItemLogic(ref r_armorItems, armorItems);
                    break;
                case 2:
                    ExcuteRandonItemLogic(ref r_accesroyItems, accesroyItems);
                    break;
                case 3:
                    ExcuteRandonItemLogic(ref r_usedItems, usedItems);
                    break;
                case 4:
                    ExcuteRandonItemLogic(ref r_skillItems, skillItems);
                    break;
            }
        }
    }
}
