using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Inventory 인게임에서 보여주는 UI를 담는 클래스 
public class InventoryUI : TabManual
{
    public static bool INVETORY_ACTVATIED = false; // 인벤토리가 활성화 되어 있는지 에 대한 값

    public bool isOpenInventory = false;

    private int selectedPage;

    public int maxSlotCount;        // 최대 공간 

    // 필요한 컴포넌트 및 클래스 
    [SerializeField] InvenSlot invenSlot = null;
    [SerializeField] GameObject contentPage = null;
    [SerializeField] List<InvenSlot> invenSlots = new List<InvenSlot>();   // 인벤토리 슬롯 

    [SerializeField] List<Item> totalItems = new List<Item>();

    List<Item> itemList; 
    void Start()
    {
        totalItems.Clear();
        // 인벤토리에서 현재 가지고 있는 아이템 데이터를 가져온다.
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

    // �ִ� ���� �� ��ŭ �κ��丮 ������ �����Ѵ� 
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
    // �κ��丮�� �ֽ�ȭ �Ѵ�
    public void RefreshInventory()
    {
        // ���� ui �� ���� �ִ� �ִ� ������ �κ��丮 �����Ͱ� ���� �ִ� ������ �ٸ��ٸ� �ֽ�ȭ�Ѵ�
        if (maxSlotCount != Inventory.instance.maxSlotCount)
        {
            maxSlotCount = Inventory.instance.maxSlotCount;
            SettingInventorySlot(); 
        }

        TabSetting(selectedPage);
    }

    // �κ��丮�� ������ �ֱ� 
    public void AddItem(Item p_item, int p_Count = 1)
    {
        if (totalItems.Count == invenSlots.Count)
        {
            Debug.Log("�κ��丮�� ��á���ϴ�. ");
            return;
        }
        if (p_item == null) return;

        for (int i = 0; i <= totalItems.Count; i++)
        {
            if (p_item.itemType != ItemType.Equipment && totalItems[i].Equals(p_item) && p_item.itemCount < 100)
            {
                totalItems[i].itemCount += p_Count;
                continue;
            }
            else
            {
                p_item.itemCount = p_Count;
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
                        // �κ��丮 ����Ʈ�� �߰��� ���� - 1�� �ε��������� ���� 
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

  
 
    // ��� ����
    public void TakeOffEquipment(int _idx)
    {
        invenSlots[_idx].TakeOffItemSlot();
    }


    public void SettingSlot(List<Item> p_Items)
    {
        if (p_Items == null) return; 

        if (p_Items.Count != invenSlots.Count)
        {
            Debug.Log("현재 아이템 개수 " + p_Items.Count + " " + invenSlots.Count);
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

        // ���Կ� ������ �ֱ� 
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


    // �������� ���Կ� ��� 
    public void TabSetting(int p_TabNumber)
    {
        SettingSlot(Inventory.instance.GetItemList((InventoryCategory)p_TabNumber));

    }

    // ������ ���� �������� �κ��丮�� �����Ѵ�
    public void OpenInventoryBySeletedTab(int p_TabNumber)
    {
        SoundManager.instance.PlaySE("Confirm_Click");
        
        TabSetting(p_TabNumber);

        TabSlotOpen(contentPage);
    }



}
