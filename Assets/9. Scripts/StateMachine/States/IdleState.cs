using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    float idleTime;

    public IdleState(WheelerController context)
    {
        this.owner = context;
    }

    public override void EnterState()
    {
        if (owner == null) return;

        //  Debug.Log("���̵� ������Ʈ ����");
        idleTime = owner.idleTime;
        // owner�� wait �Լ��� ȣ���Ѵ�. 
        owner.Wait();

        owner.Think();
    }

 
    public override void ExitState()
    {
       // Debug.Log("���̵� ������Ʈ Ż��");
    }


    public override void FixedUpdateState()
    {
    }


    public override void UpdateState()
    {
        if (owner == null) return;

        if (owner.myPlayType == PlayType.Playerable && owner.isAutoFlag == false)
            return; 

        if (owner.fieldOfView != null)
        {
            // 지정한 곳에 도착했는지 체크
            var isAlive = CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance);

            // 타겟을 감지한 상태이고 도착하지않았다면 계속 추격상태
            if ((owner.fieldOfView.View() && isAlive == false) )
                //|| owner.fieldOfView.GetTargetTransform() != null)
            {
                owner.myState = PlayerState.Chase;
                return; 
            }
            else if(owner.fieldOfView.MeeleAttackRangeView(owner.MyAgent))
            {
                owner.myState = PlayerState.Attack;
                return; 
            }
        }

        // 대기 중일 때 시간 감소시키기
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
