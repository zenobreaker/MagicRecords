using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BossStageController : UiBase
{
    public TextMeshProUGUI titleText;

    // todo ������ Ž�� �������������� ���� Ŭ������ �̿��� ����Ʈ ���� ���������� ������ ���� �����ؾ��� ��
    public List<StageAppearInfo> stageInfoList = new List<StageAppearInfo>();

    [SerializeField] BossInfoUi bossInfoUI = null;
    /// <summary>
    /// 1. ���������� ���δ�
    /// 2. ���������� ���� ���� ������ ���´�
    /// 3. ������ ������ ĳ���� ���� UI�� ���´�. 
    /// 4. ĳ���͸� ���� �����Ѵ�.
    /// </summary>

    private void OnEnable()
    {
        SetBossStageList();

        DrawBossRaidUI();
    }

    private void OnDisable()
    {
        if(bossInfoUI !=null)
            bossInfoUI.gameObject.SetActive(false);
    }

    // ���� ���������� �����ϴ� �޼ҵ� 
    public void SetBossStageList()
    {
        stageInfoList.Clear();

        // todo �� ���̺� ������ ��� �� ���� jsonȭ�ؾ�������..

        StageNodeInfo tableClass = new StageNodeInfo();
        tableClass.Init();

        tableClass.contentType = ContentType.BOSS_RAID;
        
        StageAppearInfo appearInfo = new StageAppearInfo();
        appearInfo.stageType = StageType.BATTLE;
        appearInfo.monsterGrade = MonsterGrade.BOSS;
        appearInfo.mapID = 100;
        appearInfo.maxWave =1 ;
        appearInfo.appearIDList = new List<int>();
        appearInfo.appearIDList.Add(401);
        
        // todo ���� ����Ʈ �߰� �ϱ� 

        stageInfoList.Add(appearInfo);


    }

    // UI�� �׸��� �޼ҵ� 
    public void DrawBossRaidUI()
    {
        // UI ���� ��ġ 

        InitScrollviewObject(stageInfoList.Count);

        // ���Կ� ������ �Ҵ�  
        for(int i = 0; i < content.transform.childCount;i++)
        {
            var child = content.transform.GetChild(i);
            if(child.TryGetComponent<BossStageSlot>(out var slot))
            {
                slot.SetBossStageSlot(stageInfoList[i]);
                child.gameObject.SetActive(true);
            }
            
        }

        // ���Կ� ��� �Ҵ� 
        SetScrollviewChildObjectsCallback<BossStageSlot>((slot)=>
        {
            slot.SetActionCallback(() =>
            {
                SelectBossRaidStage(slot);
            });
        });
    }


    // ��ư�� �Ҵ�Ǵ� �̺�Ʈ �޼ҵ�
    public void SelectBossRaidStage(BossStageSlot slot)
    {
        if (slot == null) return;

        bossInfoUI.gameObject.SetActive(true);
        // ���� ���� â�� ����. 
        bossInfoUI.OpenBossInfoUI(slot.GetBossStageInfo());
    }


   

}
