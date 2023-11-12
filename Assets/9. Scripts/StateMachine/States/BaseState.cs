using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    protected WheelerController owner;    // �� ������Ʈ�� ������ ���
    public Vector3 destination; // ���� ���� ����

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();


    // ���� �������� �����ߴ��� �˻��ϴ� �Լ� 
    public bool CheckAlived(Vector3 tr, Vector3 destination, float stopDist)
    {
        var result = Vector3.Distance(tr, destination);
        if ( result <= stopDist)
        {
            return true;
        }


        return false;
    }
}

