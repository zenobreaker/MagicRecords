using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �� Ŭ������ slot�� �ڽ������� �⺻���� slot�� ����� ������� ���� ����
public class BossStageSlot : Slot
{
    public StageNodeInfo nodeInfo; // ���� ���Կ� ��ϵ� ���� ���� 

    // ���� �̹���

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
