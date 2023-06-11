using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TabManual : UiBase
{
    // 상태변수 
   // private bool isActivated = false;   // UI 상태 변수 

    public GameObject go_BaseUI; // 기본 베이스 UI 
    public int tabNumber = 0;
    protected int slectedSlotNumber;
    protected GameObject selectedTab; // 선택된 탭

    // 탭페이지 초기화 
    private void ClearSlot()
    { 
        if(selectedTab != null)
            selectedTab.SetActive(false);
    }

    public virtual void OpenUI()
    {
        UIPageManager.instance.OpenClose(go_BaseUI);
    }

    // 탭 선택 시 해당 페이지 열기
    public void TabSlotOpen(GameObject _tab)
    {
        if(_tab == null)
        {
            return;
        }
        ClearSlot();
        selectedTab = _tab;
        selectedTab.SetActive(true);
    }

}
