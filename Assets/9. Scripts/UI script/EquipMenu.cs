using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenu : TabManual
{
    public static EquipMenu instance;

    [SerializeField] GameObject SlotBase = null;
    
    //[SerializeField] GameObject equipSign = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        selectedTab = SlotBase;
        TabSetting(0);
        //Debug.Log("탭세팅");
        //TabSlotOpen(selectedTab);
    }




    public void TabSetting(int _tabNumber)
    {
        SoundManager.instance.PlaySE("Confirm_Click");

        switch (_tabNumber)
        {
            
            case 0:
                TabSlotOpen(SlotBase);
              //  Inventory.instance.SetApplyList(_tabNumber);
              //  Debug.Log("무기슬롯");
                break;
            case 1:
                TabSlotOpen(SlotBase);
              //  Inventory.instance.SetApplyList(_tabNumber);
              //  Debug.Log("방어구슬롯");
                break;
            case 2:
                TabSlotOpen(SlotBase);
            //    Inventory.instance.SetApplyList(_tabNumber);
                break;
            case 3:
                TabSlotOpen(SlotBase);
              //  Inventory.instance.SetApplyList(_tabNumber);
                break;
        }
        
    }

}
