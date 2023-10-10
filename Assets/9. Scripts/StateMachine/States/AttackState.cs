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
        //Debug.Log("어택 스테이트 진입");
        if (owner == null) return;

        Debug.Log("어택 스테이트 실행 중" + owner.isAttacking);
        if (!owner.isAttacking)
        {
            owner.Attack();
        }

    }

    public override void ExitState()
    {
        if (owner == null) return;
        // Debug.Log("어택 스테이트 탈출");
        owner.isAttacking = false;
        // 공격후 상태를 idle로 바꾼다
        owner.myState = PlayerState.Idle;
    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
       
    }

   
}
        