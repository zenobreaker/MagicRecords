using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(CharacterController context)
    {
        this.owner = context;
    }

    public override void EnterState()
    {
        //Debug.Log("���� ������Ʈ ����");

  
    }

    public override void ExitState()
    {
        if (owner == null) return;
        // Debug.Log("���� ������Ʈ Ż��");
        owner.isAttacking = false;
    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
        if (owner == null) return;

        Debug.Log("���� ������Ʈ ���� ��" + owner.isAttacking);
        if (!owner.isAttacking)
        {
            owner.Attack();
        }
    }

   
}
        