using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RLModeController : MonoBehaviour
{
    /*
     * RogueLite Mode Controller 
     */

    public static RLModeController instance;


    public int behaviourPoint;          // 행위 포인트 
    public int behaviourMaxPnt;
    public int[] arr_itemRank = new int[4];
    public int resultItemRank;

    [Header("알림창")]
    public GameObject ui_RMCAlert;  // 

    public static bool isRLMode; 

    public Button Btn_Shop;
    public Button Btn_UpgradeEquip;
    public Button Btn_UpgradeSkill;
    public Button Btn_EndMode; 

    [SerializeField] GameObject go_BHBase  = null;
    [SerializeField] Text txt_Notice = null; 

    public Text txt_BehaviourPnt;

    [SerializeField] StageSelecter stageSelecter = null;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else 
        {
            Destroy(this.gameObject);
        }
    }

    private void LateUpdate()
    {
        if(isRLMode)
        {
            txt_BehaviourPnt.text = behaviourPoint.ToString() + "/" + behaviourMaxPnt.ToString();
        }
    }

    void SetNotice(int p_Select)
    {
        switch (p_Select)
        {
            case 0:
                txt_Notice.text = "행동 포인트가 모두 소진되었습니다. \n 스테이지를 클리어하여 행동 포인트를 획득하세요.";
                break;
            case 1:
                txt_Notice.text = "모든 챕터를 클리어하셨습니다!";
                break;
            case 2:
                txt_Notice.text = "모드를 취소하셨습니다.";
                break;
            default:
                txt_Notice.text = "행동 포인트가 모두 소진되었습니다. \n 스테이지를 클리어하여 행동 포인트를 획득하세요.";
                break;
        }
        
    }

    public void OpenCloseRMUI()
    {
        UIPageManager.instance.OpenClose(ui_RMCAlert);
    }


    // 게임 난이도 체크 
    public void CheckToGameLevel()
    {
        // 캐릭터 장비 착용 등급 별 (전설 + 3 유니크 +2 레어 +1 언커먼 + 1 커먼x 2이상  +1)

        // 스킬 등급별 난이도 증가 
    }

    //
    public void AnalyzingEuqip()
    {
        for (int i = 0; i < 6; i++)
        {
            //switch (EquipManager.instance.equipedItem[i].itemRank)
            //{
            //    case ItemRank.Common:
            //    case ItemRank.Magic:
            //        arr_itemRank[0] += 1;
            //        break;
            //    case ItemRank.Rare:
            //        arr_itemRank[1] += 1;
            //        break;
            //    case ItemRank.Unique:
            //        arr_itemRank[2] += 1;
            //        break;
            //    case ItemRank.Legendary:
            //        arr_itemRank[3] += 1;
            //        break;
            //}
        }

        resultItemRank += (arr_itemRank[0] * 1) + (arr_itemRank[1] * 2) + (arr_itemRank[2] * 3)
            + (arr_itemRank[2] * 4);
        
    }

    // RL모드 현재 상태로 초기화 
    public void InitGameMode()
    {
        if (!isRLMode)
        {
            isRLMode = true;
            go_BHBase.SetActive(true);
            Btn_EndMode.gameObject.SetActive(true);
            LobbyManager.MyInstance.ChangeBackground(1);
        }

        behaviourPoint = behaviourMaxPnt;
        
        txt_BehaviourPnt.text = behaviourPoint.ToString() + "/" + behaviourMaxPnt.ToString();
        ShopPage.instance.RandomSettingItems();
        OpenContents();
    }
    
    // RL모드 종료 전 상태로 초기화 
    public void EndGameMode(bool isClear)
    {
        isRLMode = false;
        
        go_BHBase.SetActive(false);
        Btn_EndMode.gameObject.SetActive(false);
        LobbyManager.MyInstance.ChangeBackground(0);
        ShopPage.instance.InitShopPage();
        stageSelecter.EndToRLMode();

        if (isClear)
        {
            SetNotice(1);
            OpenCloseRMUI();
        }
        else
        {
            SetNotice(2);
            OpenCloseRMUI();
        }
    }

    public void OutGameMode()
    {
        go_BHBase.SetActive(false);
    }

    // 행위 포인트 감소 
    public void DownBHPoint()
    {
        if(behaviourPoint > 0)
            behaviourPoint--;

        if(behaviourPoint == 0)
        {
            // 알림창 출력
            SetNotice(0);
            OpenCloseRMUI();

            CloseContents();
        }
    }

    void CloseContents()
    {
        Btn_Shop.interactable = false;
        Btn_UpgradeEquip.interactable = false;
        Btn_UpgradeSkill.interactable = false;

    }

    void OpenContents()
    {
        Btn_Shop.interactable = true;
        Btn_UpgradeEquip.interactable = true;
        Btn_UpgradeSkill.interactable = true;
    }
    
    public void RestrictionOnUse()
    {
        Btn_Shop.enabled = false;
       // equipBtn.enabled = false;
       // skillBtn.enabled = false;
    }

}
