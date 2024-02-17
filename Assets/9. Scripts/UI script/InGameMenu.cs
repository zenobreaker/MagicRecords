using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : TabManual
{
    public GameObject soundTab; // 사운드 조정 탭
    public GameObject missionTab; // 임무 탭
    public GameObject gameTab; // 그래픽 조정 탭 

    //void Start()
    //{
    //    if (soundTab.activeSelf)
    //        selectedTab = soundTab;
    //    else if (missionTab.activeSelf)
    //        selectedTab = missionTab;
    //    else
    //        selectedTab = gameTab;
    //}


    public override void OpenUI()
    {
        base.OpenUI();
        TabSetting(tabNumber);
    }


    public void TabSetting(int p_tabNumber)
    {
        
        // 게임 정지 
        StartCoroutine(TimeStopAndStart());

        tabNumber = p_tabNumber;
        SoundManager.instance.PlaySE("Confirm_Click");
        switch (p_tabNumber)
        {
            case 0:
                TabSlotOpen(missionTab);
                break;
            case 1:
                TabSlotOpen(soundTab);
                break;
            case 2:
                TabSlotOpen(gameTab);
                break;
        }
    }

    public void TabSettingToGame(int p_tabNumber)
    {
        SoundManager.instance.PlaySE("Confirm_Click");
        // 게임 정지 
        //StartCoroutine(TimeStopAndStart());
       
        switch (p_tabNumber)
        {
            case 0:
                TabSlotOpen(missionTab);
                break;
            case 1:
                TabSlotOpen(soundTab);
                break;
            case 2:
                TabSlotOpen(gameTab);
                break;
        }
    }

    // 로비에서 세팅
    public void TabSettingToLobby(int _tabNumber)
    {
        tabNumber = _tabNumber;
        SoundManager.instance.PlaySE("Confirm_Click");
        switch (tabNumber)
        {
            case 0:
                TabSlotOpen(soundTab);
                break;
            case 1:
                TabSlotOpen(gameTab);
               
                break;
        }
    }

    IEnumerator TimeStopAndStart()
    {
        Debug.Log("멈춤");
        Time.timeScale = 0;
        yield return new WaitUntil(() =>  (go_BaseUI.activeSelf == false));
        Time.timeScale = 1;
    }

    // 로비로 돌아가기
    public void BackToLobby()
    {
        //씬 변경
        LoadingSceneController.LoadSceneWithCallback("Lobby", LoadSceneMode.Single, ()=>
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1;

            if(StageInfoManager.instance != null)
            {
                StageInfoManager.instance.isTest = false; 
            }
        });
    }

    // 게임 계속 하기 
    public void ContinueTheGame()
    {
        // 심플하게 그냥 UI 종료 
        go_BaseUI.SetActive(false);
    }

}
