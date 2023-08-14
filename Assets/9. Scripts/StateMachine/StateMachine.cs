using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    Dictionary<PlayerState, BaseState> stateDic = new Dictionary<PlayerState, BaseState>();


    // TODO.  상태 변화는 기술에 따라 상태를 해당 상태로 멈추도록 하던지 별도의 기술 바람

    public BaseState CurrentState { get; set; }
    public Dictionary<PlayerState, BaseState> States
    {
        get { return stateDic; }
        set { stateDic = value; }
    }

 
    public void ChangeState(BaseState nextState)
    {
        if (nextState == null)
            return;
        if (CurrentState == nextState)
            return;
        if (CurrentState == null)
            CurrentState = nextState;

        CurrentState.ExitState();
        CurrentState = nextState;
        CurrentState.EnterState();
    }

    public void ChangeState(PlayerState state)
    {
        ChangeState(States[state]);
    }


    public void OperateState()
    {
        if(CurrentState!= null)
            CurrentState.UpdateState();
    }

    public void FixedOperateState()
    {
        if(CurrentState != null)
            CurrentState.FixedUpdateState();
    }
}
