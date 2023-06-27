using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelecter : MonoBehaviour
{
    public bool isStageScreenOpen = false; // 스테이지 선택창
    public bool isSignOpen = false; // 선택 안내문 

    bool isRLMode;
    private int currentStageNum;

    // 필요한 컴포넌트 
    [SerializeField] private StagePosController theSPC = null;
    
    [Header("스테이지 화면")]
    [SerializeField] private GameObject go_GModeScreen = null;
    [SerializeField] private Button btn_RMode = null;

    [SerializeField] private GameObject go_StageScreen = null; // 스테이지 선택 화면
                                                               //   [SerializeField]
                                                               //   private Text text_StageTitle = null; // 큰 스테이지 번호
                                                               //   [SerializeField]
                                                               //    private Image[] img_StageSlots = null; // 작은 스테이지 버튼들 
    [Header("알림창")]
    [SerializeField] private GameObject go_SignBase = null;
    [SerializeField] private Text text_SignText = null;        // 스테이지 진행 안내 텍스트 
    [Header("버튼")]
    [SerializeField] private Button btn_EnrollToStory = null;
    [SerializeField] private Button btn_EnrollToRL = null;
    [SerializeField] private Button btn_Cancel = null;



    public void OpenModeSelectScreen()
    {
        UIPageManager.instance.OpenClose(go_GModeScreen);
    }

    // RLMode 시작 
    public void StartStageSelect()
    {
        if (go_SignBase.activeSelf)
            go_SignBase.SetActive(false);

        Debug.Log("현재 사용중?");
        theSPC.cur_MainChpaterNum = 1;
        //theSPC.CreateStage();
        theSPC.UpgradeCreateStage();
        UIPageManager.instance.OpenClose(go_StageScreen);
        //UIPageManager.instance.ChangeButtonAtoB(LobbyManager.MyInstance.btn_ModeSelect, LobbyManager.MyInstance.btn_IntotheStage);
     
        //RLModeController.instance.InitGameMode();
        //theSPC.LockedAllStage();
        //theSPC.OpenAlert();
        //LobbyManager.MyInstance.OpenClose(go_StageScreen);
    }

    public void EndToRLMode()
    {
        if(go_StageScreen.activeSelf)
            UIPageManager.instance.OpenClose(go_StageScreen);
        UIPageManager.instance.ChangeButtonAtoB(LobbyManager.MyInstance.btn_IntotheStage, LobbyManager.MyInstance.btn_ModeSelect);
    }

    public void OpenStageScreen()
    {
        UIPageManager.instance.OpenClose(go_StageScreen);
        isStageScreenOpen = true;
    }

    public void CloseStageScreen()
    {
        if (isStageScreenOpen)
        {
            isStageScreenOpen = false;
            go_StageScreen.SetActive(false);
        }
    }

    // 스테이지 선택
    public void SelectStage(string _stageName)
    {
        if (!isSignOpen)
        {
            SignOpen(_stageName);
        }
    }

    // 안내창 열기 
    public void SignOpen(string _stageName)
    {
        isSignOpen = true;
        text_SignText.text = _stageName + "\n"
            + "스테이지에 입장하시겠습니까?";
        go_SignBase.SetActive(true);
        StageChannel.stageName = _stageName;
    }

    //public void OpenRLModeUI()
    //{
    //    text_SignText.text =  "로그라이트 모드 실행하겠습니까?";

    //    btn_EnrollToRL.gameObject.SetActive(true);

    //    go_SignBase.SetActive(true);
       
    //}

    public void OpenTest()
    {
        
    }

    public void Cancel()
    {
        isSignOpen = false;
        go_SignBase.SetActive(false);
        //StageChannel.stageName = "";
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("LoadingScene");
    }
}
