using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    float idleTime;

    public IdleState(CharacterController context)
    {
        this.owner = context;
    }

    public override void EnterState()
    {
        if (owner == null) return;

        //  Debug.Log("아이들 스테이트 진입");
        idleTime = owner.idleTime;
        // owner의 wait 함수를 호출한다. 
        owner.Wait();

        // 조종하는 대상이 아니면 생각시키기 
        if (owner.myPlayType == PlayType.None)
        {
            owner.Think();
        }
    }

 
    public override void ExitState()
    {
       // Debug.Log("아이들 스테이트 탈출");
    }


    public override void FixedUpdateState()
    {
    }


    public override void UpdateState()
    {
        if (owner == null) return; 

        //   Debug.Log("아이들 스테이트 실행 중");
        if (owner.fieldOfView != null)
        {
            // 목적지까지 도착햇는지 검사 
            var isAlive = CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance);

            // 대상이 인식 범위에 보였다면 추적 스테이트로 변경 후 탈출 
            if ((owner.fieldOfView.View() && isAlive == false) 
                || owner.fieldOfView.GetTargetTransform() != null)
            {
                owner.myState = PlayerState.Chase;
                return; 
            }
        }

        // 일정 시간이 지나면 움직이게 한다. 
        if (idleTime > 0 && owner.myState == PlayerState.Idle)
        {
            idleTime -= Time.deltaTime;
        }
        if (idleTime <= 0)
        {
            owner.myState = PlayerState.Move;
        }
    }
}
