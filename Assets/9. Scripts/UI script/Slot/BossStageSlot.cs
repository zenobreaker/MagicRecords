using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 이 클래스는 slot의 자식이지만 기본적인 slot의 기능은 사용하지 않은 상태
public class BossStageSlot : Slot
{
    public StageNodeInfo nodeInfo; // 현재 슬롯에 등록된 보스 정보 

    // 슬롯 이미지

    public void SetBossStageSlot(StageNodeInfo  info)
    {
        if (info == null) return;

        nodeInfo = info;

        //itemImage = 
    }

    public StageNodeInfo GetBossStageInfo() { 
        return nodeInfo; 
    }

}
