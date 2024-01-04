using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossInfoUi : UiBase
{
    public TextMeshProUGUI titleText;

    int STAGE_MAX_PLAYER_COUNT = 3;

    StageAppearInfo appearInfo;

    [SerializeField] ChoiceAlert choiceAlert = null;

    // ���� ���� UI �� ��Ÿ���� �޼ҵ� ������ ���ԵǾ� �ִ�. 
    public void OpenBossInfoUI(StageAppearInfo info)
    {
        if (info == null) return; 

        // ���� ���� 
        this.appearInfo = info;

        // UI �׸��� 
        DrawInfoUI(); 
    }

    // ���� UI�� �׸��� �޼ҵ�
    public void DrawInfoUI()
    {
        if (appearInfo == null)
                return;
        // Ÿ��Ʋ �׸��� 
        if(titleText != null)
        {
            titleText.text = appearInfo.stageName;
        }

        // ���� �� ��ŭ �ڽ� ������Ʈ ����� 
        InitScrollviewObject(appearInfo.rewardIDList.Count);
        
        // �ش� �������� ���� ��ȭ ����Ʈ �׸��� 
        DrawRewardItemList();
    }


    // ����� ��ȭ�� �׸���.
    public void DrawRewardItemList()
    {
        if (appearInfo == null || ItemDatabase.instance == null)
            return; 

        // todo ���ٱ�.. 
        for(int i = 0; i < content.transform.childCount; i++)
        {
            Item item = ItemDatabase.instance.GetItemByUID(appearInfo.rewardIDList[i]);
            var child = content.transform.GetChild(i); 
            if(child.TryGetComponent<Slot>(out Slot slot))
            {
                slot.AddItem(item); 
            }
        }
    }


    // ĳ���� ����
    void SetStageCharacters(List<Character> selectPlayers)
    {
        // �̳༮�� ������ �������� ���Ѵ�. 
        if (StageInfoManager.instance == null || InfoManager.instance == null) return;

        // ������ ĳ���� ���� ���� 
        List<int> idList = new List<int>();

        for (int i = 0; i < selectPlayers.Count; i++)
        {
            idList.Add(selectPlayers[i].MyID);
        }

        InfoManager.instance.SetSelectPlayers(idList.ToArray());

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
            //ToastMessageContorller.CreateToastMessage("�÷����� ĳ���͵��� �������ּ���.");
        }
    }

    // �ش� �������� ���� 
    public void EnterBossStage()
    {
        // �������� ���� ���� 
        StageNodeInfo stageNodeInfo = new StageNodeInfo();
        stageNodeInfo.stageAppearInfos.Add(appearInfo);
        if(StageInfoManager.instance != null)
        {
            StageInfoManager.instance.SetStageInfo(stageNodeInfo);
        }

        // ĳ���� ���� UI ȣ�� 
        // ĳ���� ���� UI�� ����.
        choiceAlert.ActiveAlert(true, STAGE_MAX_PLAYER_COUNT);
        choiceAlert.uiSELECT = ChoiceAlert.UISELECT.ENTER_GAME;
        // Ȯ�ι�ư ��ɿ� ��� �Ҵ� 
        choiceAlert.ConfirmSelect(selectPlayer => SetStageCharacters(selectPlayer));
    }

  


}
