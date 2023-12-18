using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    float delayTime;
    bool isDelaying = false; 

    public AttackState(WheelerController context)
    {
        this.owner = context;
    }

    public override void EnterState()
    {
        if (owner == null) return;
        
        delayTime = owner.currentDelayTime;
        if (delayTime > 0)
            isDelaying = true;
        else
            isDelaying = false; 

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

        if (isDelaying == false)
        {
            owner.Attack();
            isDelaying = true;
        }
        else
        {
            delayTime -= Time.deltaTime;

            if (delayTime <= 0)
                isDelaying = false;
        }
    }

   
}
        
