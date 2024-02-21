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
    public List<StageNodeInfo> stageList = new List<StageNodeInfo>();

    [SerializeField] BossInfoUi bossInfoUI = null;
    /// <summary>
    /// 1. ���������� ���δ�
    /// 2. ���������� ���� ���� ������ ���´�
    /// 3. ������ ������ ĳ���� ���� UI�� ���´�. 
    /// 4. ĳ���͸� ���� �����Ѵ�.
    /// </summary>

    private void Start()
    {
        // todo �κ� ������ �̺�Ʈ �ϳ��� �����Ѵ�. 
        // �̺�Ʈ�� �������� Ŭ���� ������ ������ ���� ������ 
        // �÷��װ��� Ȯ���ϰ� ������� �޴� UI�� �׸���.

    }

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
        stageList.Clear();

        // todo �� ���̺� ������ ��� �� ���� jsonȭ�ؾ�������..

        StageNodeInfo tableClass = new StageNodeInfo();
        
        tableClass.contentType = ContentType.BOSS_RAID;
        tableClass.tableOrder = 1;

        StageAppearInfo appearInfo = new StageAppearInfo();
        appearInfo.stageType = StageType.BATTLE;
        appearInfo.monsterGrade = MonsterGrade.BOSS;
        appearInfo.stageID = 1001;
        appearInfo.mapID = 100;
        appearInfo.maxWave = 1 ;
        appearInfo.appearIDList = new List<int>();
        appearInfo.appearIDList.Add(401);

        tableClass.stageAppearInfos.Add(appearInfo);

        // todo ���� ����Ʈ �߰� �ϱ� 

        stageList.Add(tableClass);


    }

    // UI�� �׸��� �޼ҵ� 
    public void DrawBossRaidUI()
    {
        // UI ���� ��ġ 

        InitScrollviewObject(stageList.Count);

        // ���Կ� ������ �Ҵ�  
        for(int i = 0; i < content.transform.childCount;i++)
        {
            var child = content.transform.GetChild(i);
            if(child.TryGetComponent<BossStageSlot>(out var slot))
            {
                slot.SetBossStageSlot(stageList[i]);
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
