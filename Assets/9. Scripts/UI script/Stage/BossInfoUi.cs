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



    // PRIVATE FUNCTION : ��������� ������ �����Ѵ�.
    private void SetRewardData(StageNodeInfo nodeInfo)
    {
        if (nodeInfo == null || nodeInfo.stageAppearInfos == null ||
            nodeInfo.stageAppearInfos.Count <= 0)
            return; 

        var appearInfo = nodeInfo.stageAppearInfos[0];
        if (appearInfo == null)
            return;

        // ������ ������ ����Ʈ
        var stageRewardData = RewardDatabase.instance.GetStageRewardData(appearInfo.stageID);
        if (stageRewardData == null) return;


        viewItemList = stageRewardData.viewItemList;
    }

    // ���� ���� UI �� ��Ÿ���� �޼ҵ� ������ ���ԵǾ� �ִ�. 
    public void OpenBossInfoUI(StageNodeInfo info)
    {
        if (info == null) return;

        // ���� ���� 
        this.nodeInfo = info;

        SetRewardData(nodeInfo);

        // ���� �� ��ŭ �ڽ� ������Ʈ ����� 
        InitScrollviewObject(viewItemList.Count);

        // UI �׸��� 
        DrawInfoUI(); 
    }

    // ���� UI�� �׸��� �޼ҵ�
    public void DrawInfoUI()
    {
        if (nodeInfo == null)
                return;
        // Ÿ��Ʋ �׸��� 
        if(titleText != null)
        {
            titleText.text = nodeInfo.stageName;
        }
        
        
        // �ش� �������� ���� ��ȭ ����Ʈ �׸��� 
        DrawRewardItemList();
    }


    // ����� ��ȭ�� �׸���.
    public void DrawRewardItemList()
    {
        if (nodeInfo == null || ItemDatabase.instance == null)
            return;

        var appearInfo = nodeInfo.stageAppearInfos.First();
        if (appearInfo == null) return;

        // todo ���ٱ�.. 
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


    // ĳ���� ����
    void SetStageCharacters(List<Character> selectPlayers)
    {
        // �̳༮�� ������ �������� ���Ѵ�. 
        if (StageInfoManager.instance == null || InfoManager.instance == null) return;


        var appearInfo = nodeInfo.stageAppearInfos.First();
        if (appearInfo == null) return;


        // ������ ĳ���� ���� ���� 
        List<int> idList = new List<int>();

        for (int i = 0; i < selectPlayers.Count; i++)
        {
            idList.Add(selectPlayers[i].MyID);
        }

        InfoManager.instance.SetSelectPlayers(idList.ToArray(), false);

        // ������ ���������� �ִ��� �˻� �� ���� �ű��. 
        // ������ ĳ���Ͱ� ������ �� �ű�� 
        //var stageAppearInfo = StageInfoManager.instance.GetStageAppearInfoByCurrentStageNode();
        if (InfoManager.instance.GetSelectPlayerList().Count > 0 &&
            appearInfo != null)
        {
            // �������� ���°� �����ΰ� 
            if (appearInfo.stageType == StageType.BATTLE)
            {
                //�� ����
                LoadingSceneController.LoadScene("GameScene");
            }
            // �������� ���°� �̺�Ʈ�� �����̸� �ش��ϴ� UI�� �����ش�. 
            else if (appearInfo.stageType == StageType.EVENT)
            {

            }
            else if (appearInfo.stageType == StageType.SHOP)
            {

            }

        }
        // ������ ĳ���͸� ����� �˸� �޼��� ����ϱ� 
        else
        {
            // todo 
            ToastMessageContorller.CreateToastMessage("�÷����� ĳ���͵��� �������ּ���.");
        }
    }

    // �ش� �������� ���� 
    public void EnterBossStage()
    {
        if(StageInfoManager.instance != null)
        {
            // todo .. test������ ó���� �浵�� �ʿ��ϴ�.
            StageInfoManager.instance.isTest = false; 
            StageInfoManager.instance.SetStageInfo(nodeInfo);
        }

        // ĳ���� ���� UI ȣ�� 
        // ĳ���� ���� UI�� ����.
        choiceAlert.ActiveAlert(true, STAGE_MAX_PLAYER_COUNT);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        // Ȯ�ι�ư ��ɿ� ��� �Ҵ� 
        choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
    }

  


}
