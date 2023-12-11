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
        if (owner == null) return;
        
        //owner.Attack();
    }

    public override void ExitState()
    {
        if (owner == null) return;

    }

    public override void FixedUpdateState()
    {
    }

    public override void UpdateState()
    {
        if (owner == null) return;

        owner.Attack();
    }

   
}
        
