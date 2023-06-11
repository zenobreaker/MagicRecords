using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager MyInstance;

    public bool isStageScreenOpen = false;      // 스테이지 선택창
    private bool isSignOpen = false;            // 선택 안내문 

    public static int coin;                     // 사용자가 사용하는 돈

    // 필요한 컴포넌트 
    [SerializeField]
    private GameObject go_StageScreen = null;   // 스테이지 선택 화면
    [SerializeField]
    private GameObject go_SignBase = null;
    [SerializeField]
    private Text text_SignText = null;          // 스테이지 진행 안내 텍스트 
    [SerializeField] Text text_Coin = null;     // 코인 표시 
    [SerializeField] Text text_Level = null;    // 레벨 표시

    [Header("UI")]
    [SerializeField] GameObject lobbyUI = null;  // 로비 UI 모음
    //[SerializeField] GameObject gameUI = null;  // 게임 UI 모음
    [SerializeField] Image img_BackGround = null;  // 배경 이미지 
    [SerializeField] Sprite[] spt_StageBg = null;   // 배경 스프라이트

    [Header("Button")]
    public Button btn_ModeSelect = null;
    public Button btn_IntotheStage = null;
   

    [Header("Key Set")]
    private GameObject[] keybindButtons;

    [Header("Menu")]
    [SerializeField] EquipMenu equipMenu = null;
    //[SerializeField] Infomation theInfo = null;
    [SerializeField] GameObject btn_Back = null; // 뒤로가기 버튼 
    //[SerializeField] CharStat charStat = null;

    public void Awake()
    {
        if (MyInstance == null)
        {
            MyInstance = this;
        }
        else if (MyInstance != null)
        {
            Destroy(gameObject);
        }

        ///DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Tag가 keybind로 설정된 게임 오브젝트를 찾는다.
        keybindButtons = GameObject.FindGameObjectsWithTag("keybind");
        equipMenu = FindObjectOfType<EquipMenu>();
        //theInfo = FindObjectOfType<Infomation>();
        //if (charStat == null)
          //  charStat = FindObjectOfType<CharStat>();

    }

    private void LateUpdate()
    {
        if(text_Coin != null)
        { 
            text_Coin.text = "코인 : " + coin.ToString();
        }
       // text_Level.text = "레벨 : " + charStat.level;
    }

    public void ChangeBackground(int _idx = 0)
    {
        img_BackGround.sprite = spt_StageBg[_idx];
    }


    public void ApearLobbyUI()
    {
        lobbyUI.gameObject.SetActive(true);
    }

    public void HideLobbyUI()
    {
        lobbyUI.gameObject.SetActive(false);
    }

    public void UpdateKeyText(string key, KeyCode code)
    {
        Text tmp = Array.Find(keybindButtons, x => x.name == key).GetComponentInChildren<Text>();
        tmp.text = code.ToString();
    }


    public void OpenStagePage()
    {
        if (!isStageScreenOpen)
        {
            isStageScreenOpen = true;
            go_StageScreen.SetActive(true);
            SoundManager.instance.PlaySE("Confirm_Click");
        }
    }

    public void CloseStagePage()
    {
        if (isStageScreenOpen)
        {
            go_StageScreen.SetActive(false);
            isStageScreenOpen = false;
            SoundManager.instance.PlaySE("Escape_UI");
        }
    }

    // 스테이지 선택
    public void SelectStage(string _stageName)
    {
        if (!isSignOpen)
        {
            SignOpen(_stageName);
            SoundManager.instance.PlaySE("Confirm_Click");
            StageChannel.stageName = _stageName;
        }
    }

    // 스테이지에 입장 
    public void JoinStage()
    {
        // 로딩씬 추가 
        HideLobbyUI();
        GameManager.MyInstance.StageInit();
    }

    public void JoinTestStage()
    {
        HideLobbyUI();
        GameManager.MyInstance.TestStage();
    }

    // 안내창 열기 
    public void SignOpen(string _stageName)
    {
        isSignOpen = true;
        text_SignText.text = _stageName + "\n"
            + "스테이지에 입장하시겠습니까?";
        go_SignBase.SetActive(true);
        //OpenClose(go_SignBase);
    }


    public void SceneChange()
    {
        // StageChannel.stageCategory = "GameScene";
        SceneManager.LoadScene("LoadingScene");
        SoundManager.instance.PlaySE("Confirm_Click");
    }

    public void TestingCoinUp()
    {
        StartCoroutine(CoIncreaseCoin(1000));
        //GetComponent<GameObject>().SetActive(true);
    }

    public void TestingCoinDown()
    {
        StartCoroutine(CoIncreaseCoin(-1000));
    }

    public void IncreseCoin(int _num)
    {
        StartCoroutine(CoIncreaseCoin(_num));
    }

    IEnumerator CoIncreaseCoin(int _num)
    {
        float time = Time.deltaTime * 2;
        int lastValue = 0;

        lastValue = coin + _num;

        while (coin != lastValue)
        {

            coin = (int)Mathf.SmoothStep(coin, lastValue, 0.5f);
            Debug.Log("코인 증가 : " + coin);


            if (_num > 0)
            {
                if (Mathf.Abs(coin) >= Mathf.Abs(lastValue) * 99 / 100)
                {
                    coin = lastValue;
                    break;
                }
            }
            else if(_num <= 0)
            {
                if (Mathf.Abs(lastValue) >= Mathf.Abs(coin) * 99 / 100)
                {
                    coin = lastValue;
                    break;
                }
            }

            yield return null;
        }
    }
}
