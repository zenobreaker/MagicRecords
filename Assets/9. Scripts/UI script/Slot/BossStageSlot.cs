using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;

// �� Ŭ������ slot�� �ڽ������� �⺻���� slot�� ����� ������� ���� ����
public class BossStageSlot : Slot
{
    public StageAppearInfo appearInfo; // ���� ���Կ� ��ϵ� ���� ���� 

    // ���� �̹���

    public void SetBossStageSlot(StageAppearInfo  info)
    {
        if (info == null) return;

        appearInfo = info;

        //itemImage = 
    }

    public StageAppearInfo GetBossStageInfo() { 
        return appearInfo; 
    }

}
