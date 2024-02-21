using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BossStageController : UiBase
{
    public TextMeshProUGUI titleText;

    // todo 이전에 탐사 스테이지용으로 만든 클래스를 이용한 리스트 보스 스테이지는 별도로 할지 결정해야할 듯
    public List<StageNodeInfo> stageList = new List<StageNodeInfo>();

    [SerializeField] BossInfoUi bossInfoUI = null;
    /// <summary>
    /// 1. 스테이지가 보인다
    /// 2. 스테이지를 고르면 보스 정보가 나온다
    /// 3. 입장을 누르면 캐릭터 선택 UI가 나온다. 
    /// 4. 캐릭터를 고르면 입장한다.
    /// </summary>

    private void Start()
    {
        // todo 로비에 들어오면 이벤트 하나를 실행한다. 
        // 이벤트는 스테이지 클리어 보상을 받을까 말까 ㄱ결정 
        // 플래그값을 확인하고 보상들을 받는 UI를 그린다.

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

    // 보스 스테이지를 생성하는 메소드 
    public void SetBossStageList()
    {
        stageList.Clear();

        // todo 뭐 테이블 정보를 담는 걸 따로 json화해야할지도..

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

        // todo 보상 리스트 추가 하기 

        stageList.Add(tableClass);


    }

    // UI를 그리는 메소드 
    public void DrawBossRaidUI()
    {
        // UI 슬롯 배치 

        InitScrollviewObject(stageList.Count);

        // 슬롯에 데이터 할당  
        for(int i = 0; i < content.transform.childCount;i++)
        {
            var child = content.transform.GetChild(i);
            if(child.TryGetComponent<BossStageSlot>(out var slot))
            {
                slot.SetBossStageSlot(stageList[i]);
                child.gameObject.SetActive(true);
            }
            
        }

        // 슬롯에 기능 할당 
        SetScrollviewChildObjectsCallback<BossStageSlot>((slot)=>
        {
            slot.SetActionCallback(() =>
            {
                SelectBossRaidStage(slot);
            });
        });
    }


    // 버튼에 할당되는 이벤트 메소드
    public void SelectBossRaidStage(BossStageSlot slot)
    {
        if (slot == null) return;

        bossInfoUI.gameObject.SetActive(true);
        // 보스 정보 창을 연다. 
        bossInfoUI.OpenBossInfoUI(slot.GetBossStageInfo());
    }


   

}
