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



    private void Start()
    {
        // 씬이 호출될 때마다 호출되는 함수이므로 프로세싱 관련 함수 호출    
        ProcessingAdventureMode();
    }

    public void OpenModeSelectScreen()
    {
        UIPageManager.instance.OpenClose(go_GModeScreen);
    }

    // 탐사 UI 켜주는 것부터 시작해주는 함수 
    public void StartStageSelect()
    {
        if (go_SignBase.activeSelf)
            go_SignBase.SetActive(false);

        if (StageInfoManager.instance != null)
        {
            // 챕터를 다 클리어하면 열지 않는다. 
            if (StageInfoManager.instance.currentChapter <=
                StageInfoManager.instance.maxChapter)
            {
                StageInfoManager.instance.CreateStageTableList();
                UIPageManager.instance.OpenClose(go_StageScreen);
            }
        }
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


    // 탐사 안내창 열기 
    public void OpenPopupTryAdventurePopup()
    {
        // 플래그가 켜져 있으면 여부 상관 없이 진행한 화면을 표출한다. 
        if(StageInfoManager.FLAG_ADVENTURE_MODE == true)
        {
            // 탐사 UI 켜주기
            StartStageSelect();
        }
        //플래그가 꺼져 있다면 탐사할 것인지 팝업을 띄워 여부를 확인하게 한다. 
        else
        {
            // 팝업을 키고 특정 컴포넌트를 가지고 있는지 검사 
            UIPageManager.instance.OpenClose(go_SignBase);
            if (go_SignBase.TryGetComponent<NoticePopup>(out var noticePopup))
            {
                if (noticePopup.confirmButton != null)
                {
                    //확인버튼 기능 할당 
                    noticePopup.Confirm(() =>
                    {
                        // 팝업을 끈다 
                        UIPageManager.instance.OpenClose(go_SignBase);

                        // 변수 플래그 변경
                        StageInfoManager.FLAG_ADVENTURE_MODE = true;

                        // 탐사 UI 켜주기
                        StartStageSelect();
                    });
                }
            }
        }
    }


    // 탐사 모드 진행 모듈 프로세스 함수 
    public void ProcessingAdventureMode()
    {
        // 이 함수는 게임씬에서 로비씬으로 왔을 때 호출 된다. 
        // 플래그가 켜져 있으면 다시 탐사 UI를 켜줘서 게임을 진행하도록 한다. 
        if (StageInfoManager.FLAG_ADVENTURE_MODE == false)
        {
            return;
        }

        // 게임 진행 관련 UI를 먼저 켜준다. 
        OpenModeSelectScreen();

        // 탐사 UI 켜주기
        StartStageSelect();
    }


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
