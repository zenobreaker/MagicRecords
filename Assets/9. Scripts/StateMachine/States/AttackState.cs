using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(WheelerController context)
    {
        this.owner = context;
    }

    public override void EnterState()
    {
        //Debug.Log("���� ������Ʈ ����");
        if (owner == null) return;

        Debug.Log("���� ������Ʈ ���� ��" + owner.isAttacking);
        if (!owner.isAttacking)
        {
            owner.Attack();
        }

    }

    public override void ExitState()
    {
        if (owner == null) return;
        // Debug.Log("���� ������Ʈ Ż��");
        owner.isAttacking = false;
        // ������ ���¸� idle�� �ٲ۴�
        owner.myState = PlayerState.Idle;
    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
       
    }

   
}
        
