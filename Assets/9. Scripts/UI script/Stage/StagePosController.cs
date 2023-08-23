using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Scene = UnityEngine.SceneManagement.Scene;

public class StagePosController : MonoBehaviour
{
    [SerializeField] StageMenuSelectUI stageMenuUI = null;

    [Header("몬스터 스테이지 테마 이미지")]
    [SerializeField] Image img_StageTheme = null;  // 배경 이미지 
    [Header("몬스터 스테이지 테마 이미지 모음")]
    [SerializeField] Sprite[] spt_StageBg = null;   // 배경 스프라이트

    [Header("몬스터 스테이지 아이콘")]
    [SerializeField] Sprite[] spt_stageMIcon = null;

    [Header("몬스터 이미지")]
    [SerializeField] Image[] img_MonsterIcon = null;

    [Header("스테이지")]
    [SerializeField] Text txt_Chpater = null;

    [SerializeField] GameObject scrollview = null;
    [SerializeField] GameObject contentObject = null; 
    [SerializeField] StageSelectSlot stageSlot = null; 

    [SerializeField] ChoiceAlert choiceAlert = null;
    [SerializeField] GameObject go_Alert;

    [SerializeField] List<StageTableClass> stageTables = null;

    [SerializeField] RewardController rewardController = null;

    // 변수
    public int finalChapterNum;         // 마지막 챕터 번호

    public int curMainChpaterNum;      // 메인 챕터 번호 
    public int selectEventSlotNumber;       // 선택한 이벤트 ID
    int curSelectStageNum;             // 현재 선택한 스테이지


    private void Start()
    {
        // 저장한게 없다면 챕터는 1로 초기화시켜놓는다
        curMainChpaterNum = StageInfoManager.instance.currentChapter;
    }

    private void OnEnable()
    {
        // 스테이지 인포 매니저에서 등록된 정보를 가져온다. 
        StageInfoManager.instance.GetLocatedStageInfoListByCurrenChapter(out stageTables);

        // 오브젝트가 켜질 때마다 스테이지 정보대로 그려준다.
        DrawStageButtonByScrollview();

        // 첫 진입 플래그가 켜져있다면. 
        if (StageInfoManager.initJoinPlayGameModeFlag == true)
        {
            // RewardController에서 첫 메모리 선택 UI를 보여준다.
            Debug.Log("여기서 호출함니다????");

            if(rewardController == null)
            {
                return; 
            }

            // todo 임시로 3개를 줘본다.
            rewardController.SetRecordRewardList(3);

            rewardController.DrawRewardCards();
        }

     
    }


    // 스테이지에 맵에서 스테이지를 선택하면 뜨는 팝업 
    // 몬스터 스테이지 메뉴 열기 
    public void OpenMonsterStageMenu(int stageNum)
    {
        Debug.Log("스테이지 번호 : " + stageNum);

        curSelectStageNum = stageNum;

        selectEventSlotNumber = -1;

        if (stageTables == null || stageTables.Count <= 0)
        {
            Debug.Log("stage table info list size 0 or null");
            return;
        }

        if (UIPageManager.instance != null && stageMenuUI != null)
        {
            var stageTableInfo = stageTables[stageNum - 1];
            stageMenuUI.DeploySelectEventSlot(ref stageTableInfo);
            UIPageManager.instance.OpenClose(stageMenuUI.gameObject);
        }
    } 

    public void CloseMonsterMenu()
    {
        //cur_SelectStageNum = -1;
        if (stageMenuUI != null)
        {
            UIPageManager.instance.OpenClose(stageMenuUI.gameObject);
        }
    }


    // 스테이지 팝업에서 선택 후 입장 버튼 기능 
    // 진행할 대상 선택
    public void SelectEventStage()
    {
        selectEventSlotNumber = stageMenuUI.GetSelectEvent();

        // 이벤트 선택 메뉴를 닫는다.
        CloseMonsterMenu();
        // 캐릭터 선텍 UI를 연다. 
        choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        // 확인버튼 기능에 기능 할당 
        choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
     
    }

    // 캐릭터 세팅
    void SetStageCharacters(Character selectPlayer)
    {
        // 이녀석이 없으면 실행하지 못한다. 
        if (StageInfoManager.instance == null || InfoManager.instance == null) return; 

        // 선택한 캐릭터 정보 저장 
        List<int> idList = new List<int>
                    {
                        selectPlayer.MyID
                    };
        InfoManager.instance.SetSelectPlayers(idList.ToArray());

        // 스테이지 선택한 정보로 저장
        StageInfoManager.instance.ChoiceStageInfoForPlaying(curMainChpaterNum, curSelectStageNum, selectEventSlotNumber);
        
        // 지정한 스테이지가 있는지 검사 후 씬을 옮긴다. 
        // 선택한 캐릭터가 있으면 씬 옮기기 
        if (InfoManager.instance.GetSelectPlayerList().Count > 0 && 
            StageInfoManager.instance.GetStageEventClass() != null)
        {
            
            //씬 변경
            LoadingSceneController.LoadScene("GameScene");
        }
        // 없으면 캐릭터를 고르라고 알림 메세지 출력하기 
        else
        {
            // todo 
            Debug.Log("Please Choose the character for play");
        }
    }

    // 연습장 
    public void GototheTestStage()
    {
        //CloseMonsterMenu();
        //GameManager.MyInstance.
        choiceAlert.ActiveAlert(true);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        //LobbyManager.MyInstance.JoinTestStage();
    }

    // 챕터 텍스트 변경 
    void SetChpaterText()
    {
        txt_Chpater.text = "Stage " + curMainChpaterNum;
    }


    // 모든 스테이지 잠금 처리 (1스테이지 제외)
    // 특정 스테이지 클리어 여부에 따라 세팅 
    public void LockedAllStage()
    {
        if (stageTables == null) return; 

        for (int i = 1; i < stageTables.Count; i++)
        {
            if (stageTables[i] == null) continue;
            stageTables[i].isLocked = true; 
        }
    }



    // 다음 챕터로 변경 
    public void SetNextChapter()
    {
        if (curMainChpaterNum < finalChapterNum)
        {
            img_StageTheme.sprite = spt_StageBg[curMainChpaterNum];
            //CreateStage();     // 스테이지 재정렬
        }
        else if (curMainChpaterNum >= finalChapterNum)
        {
            Debug.Log("이번 챕터가 마지막입니다.");
            RLModeController.instance.EndGameMode(true);

        }
    }

    public string GetStageName()
    {
        return ""; // stageTables[cur_SelectStageNum].stageName;
    }

    public bool GetIsBoss()
    {
        Debug.Log("보스 스테이지" + stageTables[curSelectStageNum].isBossStage);
        return stageTables[curSelectStageNum].isBossStage;
    }



    //#  스크롤뷰 관련 함수들 

    public void DrawStageButtonByScrollview()
    {
        if (stageTables == null || stageSlot == null ||
            contentObject == null)
        {
            return;
        }

        var cotentRect = contentObject.GetComponent<RectTransform>();
        if (cotentRect == null)
        {
            return;
        }

        //  스테이지 만들기 전 검사
        int stageCount = stageTables.Count;
        int slotCount = contentObject.transform.childCount;

        if (slotCount < stageCount)
        {
            for (int i = 0; i < stageCount; i++)
            {
                var slot = Instantiate(stageSlot, contentObject.transform);
            }
        }
        else if (slotCount > stageCount)
        {
            int diff = slotCount - stageCount;
            if (diff > 0)
            {
                for (int i = diff + 1; i < stageCount; i++)
                {
                    var slot = contentObject.transform.GetChild(i);
                    if (slot == null) continue;
                    slot.gameObject.SetActive(false);
                }
            }
        }

        // 스테이지를 배치한다. 
        for(int i = 0; i < stageTables.Count; i++)
        {
            // 스테이지 버튼에 이름 그리기 
            var slot = contentObject.transform.GetChild(i);
            if (!slot.TryGetComponent<StageSelectSlot>(out var stageSelectSlot)) continue; 

            slot.gameObject.SetActive(true);

            string stageName = curMainChpaterNum.ToString() + "-" + (i + 1).ToString();
            stageSelectSlot.SetSlotText(stageName);
            int temp = i + 1; 
            if (stageSelectSlot.GetButtonEventCount() == 0)
            {
                // 슬롯에 기능 할당 
                stageSelectSlot.SetButtonOnlyOneEventLisetener(() => OpenMonsterStageMenu(temp));
            }

            // 스테이지가 잠겨있으면 버튼 기능을 잠근다. 
            stageSelectSlot.SetButtonInteractive(!stageTables[i].isLocked);
        }

        // 스크롤 기능 설정 
        if (scrollview == null) return;

        var scrollRect = scrollview.GetComponent<ScrollRect>();
        if (scrollRect == null) return;

        // 컨텐츠 그룹보다 작으면 스크롤 기능 제거 
        var contentLegnth = cotentRect.sizeDelta.x; 
        if (contentLegnth > Screen.width)
        {
            scrollRect.horizontal = true; 
        }
        else
        {
            scrollRect.horizontal = false;
        }
    }

    // 스테이지에 맞는 몬스터 이미지 세팅
    void DrawStageIcon()
    {
    //    for (int i = 0; i < MAX_STAGE_COUNT; i++)
    //    {
    //        for (int j = 0; j < stageTables.Count; j++)
    //        {
    //            string stageName = curMainChpaterNum.ToString() + "-" + (i + 1).ToString();
    //            //btn_MonsterSlots[i].SetSlotText(stageName);
    //            // if (stageTables[i].stageType == StageType.MONSTER)
    //            {
    //                //switch (stageTables[i].monsterType)
    //                //{

    //                //    case MonsterGrade.NORMAL:
    //                //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[0]);
    //                //        break;
    //                //    case MonsterGrade.ELITE:
    //                //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[1]);
    //                //        break;
    //                //    case MonsterGrade.BOSS:
    //                //        btn_MonsterSlots[i].SetSpriteImage(spt_stageMIcon[2]);
    //                //        break;
    //                //}
    //            }
    //        }

    //    }
    }
}
