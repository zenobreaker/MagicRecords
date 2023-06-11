using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 인벤토리 UI과 직접적으로 연관된 클래스 
// Inventory 클래스의 데이터의 상태에 따라 이 UI 클래스가 상태를 보여준다 
public class InventoryUI : TabManual
{
    public static bool inventoryActivated = false; // 인벤토리가 켜져있으면 다른 기능들을 정지시킬 수 있음

    public bool isOpenInventory = false;

    private int selectedPage;

    public int maxSlotCount;        // 최대 인벤 슬롯 개수 

    // 필요한 컴포넌트
    [SerializeField] InvenSlot invenSlot = null;
    [SerializeField] GameObject contentPage = null;
    [SerializeField] List<InvenSlot> invenSlots = new List<InvenSlot>();   // 인벤토리 슬롯

    [SerializeField] List<Item> totalItems = new List<Item>();

    List<Item> itemList; 
    void Start()
    {
        totalItems.Clear();
        // 인벤토리의 아이템 리스트 가져오기
        totalItems = Inventory.instance.GetItems();

        selectedPage = 1;
        SettingInventorySlot();

        TabSetting(selectedPage);
    }
    private void OnEnable()
    {
        selectedPage = 1;
        SettingInventorySlot();

        TabSetting(selectedPage);
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        RefreshInventory(); 
    }

    // 최대 슬롯 수 만큼 인벤토리 슬롯을 설정한다 
    void SettingInventorySlot()
    {
        if (invenSlots.Count < maxSlotCount)
        {
            for (int i = 0; i < maxSlotCount; i++)
            {
                InvenSlot clone = Instantiate(invenSlot, contentPage.transform);
                invenSlots.Add(clone);
            }
        }
    }
    // 인벤토리를 최신화 한다
    public void RefreshInventory()
    {
        // 현재 ui 가 갖고 있는 최대 개수와 인벤토리 데이터가 갖고 있는 개수가 다르다면 최신화한다
        if (maxSlotCount != Inventory.instance.maxSlotCount)
        {
            maxSlotCount = Inventory.instance.maxSlotCount;
            SettingInventorySlot(); 
        }

        TabSetting(selectedPage);
    }

    // 인벤토리에 아이템 넣기 
    public void AddItem(Item p_item, int p_Count = 1)
    {
        if (totalItems.Count == invenSlots.Count)
        {
            Debug.Log("인벤토리가 꽉찼습니다. ");
            return;
        }
        if (p_item == null) return;

        for (int i = 0; i <= totalItems.Count; i++)
        {
            if (p_item.itemType != ItemType.Equipment && totalItems[i].Equals(p_item) && p_item.itemEach < 100)
            {
                totalItems[i].itemEach += p_Count;
                break;
            }
            else
            {
                p_item.itemEach = p_Count;
                totalItems.Add(p_item as EquipItem);

                if ((selectedPage == 4 || selectedPage == 0) && p_item.itemType == ItemType.Used)
                {
                    invenSlots[totalItems.Count - 1].ClearSlot();
                    invenSlots[totalItems.Count - 1].SetSize(new Vector2(200, 200));
                    invenSlots[totalItems.Count - 1].AddItem(totalItems[totalItems.Count - 1], p_Count);
                    break;
                }

                if ((p_item is EquipItem) == true)
                {
                    if (selectedPage == (int)(p_item as EquipItem).equipType || selectedPage == 0)
                    {
                        // 인벤토리 리스트에 추가된 길이 - 1이 인덱스값으로 참조 
                        invenSlots[totalItems.Count - 1].ClearSlot();
                        invenSlots[totalItems.Count - 1].SetSize(new Vector2(200, 200));
                        invenSlots[totalItems.Count - 1].AddItem(totalItems[totalItems.Count - 1]);
                        invenSlots[i].gameObject.SetActive(true);

                    }
                }
                //invenSlots[totalItems.Count - 1].gameObject.SetActive(true);
                break;
            }
        }

        //for (int i = 0; i < totalItems.Count; i++)
        //{
        //    invenSlots[i].ClearSlot();
        //    invenSlots[i].SetSize(new Vector2(200, 200));
        //    invenSlots[i].AddItem(totalItems[i]);
        //    invenSlots[i].gameObject.SetActive(true);
        //}
    }

  
 
    // 장비 해제
    public void TakeOffEquipment(int _idx)
    {
        invenSlots[_idx].TakeOffItemSlot();
    }


    public void SettingSlot(List<Item> p_Items)
    {
        //SplitListWithTotal();

        if (p_Items.Count != invenSlots.Count)
        {
            Debug.Log("둘의 길이 차이 " + p_Items.Count + " " + invenSlots.Count);
            if (p_Items.Count > invenSlots.Count)
            {
                int t_start = invenSlots.Count > 0 ? invenSlots.Count - 1 : 0;

                for (int i = t_start; i < p_Items.Count; i++)
                {
                    InvenSlot clone = Instantiate(invenSlot, contentPage.transform);
                    invenSlots.Add(clone);

                }
            }
            else if (p_Items.Count < invenSlots.Count)
            {
                int t_start = invenSlots.Count > 0 ? (p_Items.Count - 1 < 0 ? 0 : p_Items.Count - 1) : 0;

                for (int i = t_start; i < invenSlots.Count; i++)
                {
                    invenSlots[i].ClearSlot();
                    invenSlots[i].gameObject.SetActive(false);
                }
            }
        }

        // 슬롯에 아이템 넣기 
        for (int i = 0; i < p_Items.Count; i++)
        {
            invenSlots[i].ClearSlot();
            invenSlots[i].SetSize(new Vector2(200, 200));
            invenSlots[i].AddItem(p_Items[i]);
            if (invenSlots[i].isEquiped())
                invenSlots[i].EquipingItemSlot();
            else
                invenSlots[i].TakeOffItemSlot();
            invenSlots[i].gameObject.SetActive(true);
        }
    }


    // 아이템을 슬롯에 등록 
    public void TabSetting(int p_TabNumber)
    {
       

        switch ((InventoryCategory)p_TabNumber)
        {
            //case InventoryCategory.TOTAL:
            //    selectedPage = 0;
            //    SettingSlot(Inventory.instance.GetSlotList());
            //    break;
            case InventoryCategory.WEAPON:
                selectedPage = 1;
                SettingSlot(Inventory.instance.GetWeaponItems());
                break;
            case InventoryCategory.ARMOR:
                selectedPage = 2;
                SettingSlot(Inventory.instance.GetArmorItems());
                break;
            case InventoryCategory.TIRE:
                selectedPage = 3;
                SettingSlot(Inventory.instance.GetTireItems());
                break;
            case InventoryCategory.ACCESORY:
                selectedPage = 4;
                SettingSlot(Inventory.instance.GetAccessoryItems());
                break;
            case InventoryCategory.ETC:
                selectedPage = 5;
                SettingSlot(Inventory.instance.GetUsedItems());
                break;
            case InventoryCategory.DRONE:
                selectedPage = 6;
                SettingSlot(Inventory.instance.GetItemList(InventoryCategory.DRONE));
                break;
            case InventoryCategory.RUNE:
                selectedPage = 7;
                SettingSlot(Inventory.instance.GetItemList(InventoryCategory.RUNE));
                break;
        }
    }

    // 세팅한 탭을 기준으로 인벤토리를 오픈한다
    public void OpenInventoryBySeletedTab(int p_TabNumber)
    {
        SoundManager.instance.PlaySE("Confirm_Click");
        
        TabSetting(p_TabNumber);

        TabSlotOpen(contentPage);
    }



}
