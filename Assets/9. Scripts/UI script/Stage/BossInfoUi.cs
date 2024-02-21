using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BossInfoUi : UiBase
{
    public TextMeshProUGUI titleText;

    int STAGE_MAX_PLAYER_COUNT = 3;

    StageNodeInfo nodeInfo;

    List<Item> viewItemList = new List<Item>();

    [SerializeField] ChoiceAlert choiceAlert = null;



    // PRIVATE FUNCTION : 보상관련한 값들을 추출한다.
    private void SetRewardData(StageNodeInfo nodeInfo)
    {
        if (nodeInfo == null || nodeInfo.stageAppearInfos == null ||
            nodeInfo.stageAppearInfos.Count <= 0)
            return; 

        var appearInfo = nodeInfo.stageAppearInfos[0];
        if (appearInfo == null)
            return;

        // 보여줄 아이템 리스트
        var stageRewardData = RewardDatabase.instance.GetStageRewardData(appearInfo.stageID);
        if (stageRewardData == null) return;


        viewItemList = stageRewardData.viewItemList;
    }

    // 보스 정보 UI 가 나타나는 메소드 연출이 포함되어 있다. 
    public void OpenBossInfoUI(StageNodeInfo info)
    {
        if (info == null) return;

        // 정보 세팅 
        this.nodeInfo = info;

        SetRewardData(nodeInfo);

        // 보상 수 만큼 자식 오브젝트 만들기 
        InitScrollviewObject(viewItemList.Count);

        // UI 그리기 
        DrawInfoUI(); 
    }

    // 인포 UI를 그리는 메소드
    public void DrawInfoUI()
    {
        if (nodeInfo == null)
                return;
        // 타이틀 그리기 
        if(titleText != null)
        {
            titleText.text = nodeInfo.stageName;
        }
        
        
        // 해당 스테이지 보상 재화 리스트 그리기 
        DrawRewardItemList();
    }


    // 드랍할 재화를 그린다.
    public void DrawRewardItemList()
    {
        if (nodeInfo == null || ItemDatabase.instance == null)
            return;

        var appearInfo = nodeInfo.stageAppearInfos.First();
        if (appearInfo == null) return;

        // todo 뭐줄까.. 
        for (int i = 0; i < viewItemList.Count; i++)
        {
            var item = viewItemList[i];
            var child = content.transform.GetChild(i); 
            if(child.TryGetComponent<Slot>(out Slot slot))
            {
                slot.AddItem(item);
                slot.gameObject.SetActive(true);
            }
        }
    }


    // 캐릭터 세팅
    void SetStageCharacters(List<Character> selectPlayers)
    {
        // 이녀석이 없으면 실행하지 못한다. 
        if (StageInfoManager.instance == null || InfoManager.instance == null) return;


        var appearInfo = nodeInfo.stageAppearInfos.First();
        if (appearInfo == null) return;


        // 선택한 캐릭터 정보 저장 
        List<int> idList = new List<int>();

        for (int i = 0; i < selectPlayers.Count; i++)
        {
            idList.Add(selectPlayers[i].MyID);
        }

        InfoManager.instance.SetSelectPlayers(idList.ToArray(), false);

        // 지정한 스테이지가 있는지 검사 후 씬을 옮긴다. 
        // 선택한 캐릭터가 있으면 씬 옮기기 
        //var stageAppearInfo = StageInfoManager.instance.GetStageAppearInfoByCurrentStageNode();
        if (InfoManager.instance.GetSelectPlayerList().Count > 0 &&
            appearInfo != null)
        {
            // 스테이지 형태가 전투인가 
            if (appearInfo.stageType == StageType.BATTLE)
            {
                //씬 변경
                LoadingSceneController.LoadScene("GameScene");
            }
            // 스테이지 형태가 이벤트나 상점이면 해당하는 UI를 열어준다. 
            else if (appearInfo.stageType == StageType.EVENT)
            {

            }
            else if (appearInfo.stageType == StageType.SHOP)
            {

            }

        }
        // 없으면 캐릭터를 고르라고 알림 메세지 출력하기 
        else
        {
            // todo 
            ToastMessageContorller.CreateToastMessage("플레이할 캐릭터들을 선택해주세요.");
        }
    }

    // 해당 스테이지 입장 
    public void EnterBossStage()
    {
        if(StageInfoManager.instance != null)
        {
            // todo .. test변수를 처리할 방도가 필요하다.
            StageInfoManager.instance.isTest = false; 
            StageInfoManager.instance.SetStageInfo(nodeInfo);
        }

        // 캐릭터 선택 UI 호출 
        // 캐릭터 선텍 UI를 연다.
        choiceAlert.ActiveAlert(true, STAGE_MAX_PLAYER_COUNT);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        // 확인버튼 기능에 기능 할당 
        choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
    }

  


}
