using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MoveState : BaseState
{

    public MoveState(WheelerController context)
    {
        this.owner = context;
    }

    void SetRandomDestination(WheelerController sm)
    {
        destination = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
        destination = sm.transform.localPosition + destination;
        RaycastHit hit;

        if (Physics.Raycast(destination, destination + Vector3.down, out hit, 100f))
        {
            if (!hit.collider.CompareTag("land"))
            {
                // 지형이 다른 지형이므로 다시 랜덤하게 돌린다.
                //SetRandomDestination(sm);
                owner.myState = PlayerState.Idle;
            }
        }
    }

    void SetDestination(Vector3 pos)
    {
        destination = pos;
    }

    public override void EnterState()
    {
        if (owner == null) return; 

        if (/*owner.myPlayType == PlayType.Playerable &&*/ owner.isAutoFlag == false)
            return;

        owner.fieldOfView.target = null;

        if (owner.myState == PlayerState.Move)
        {
            // 도착 지점을 랜덤으로 설정한다. 
            SetRandomDestination(owner);
        }
        else if (owner.myState == PlayerState.Chase)
        {
            // 추격 중이라면 지정한 위치로 이동한다. 
            owner.MyAgent.stoppingDistance = owner.fieldOfView.meeleAttackDistance;
            SetDestination(owner.fieldOfView.GetTargetPos());
        }
        else if(owner.myState == PlayerState.Follow)
        {
            // 리더를 따라다닌다.
            owner.MyAgent.stoppingDistance = PlayerControl.LEADER_DISTANCE;
        }
    }

    public override void ExitState()
    {
        owner.MyAgent.ResetPath();
        owner.MyAgent.velocity = Vector3.zero;
        destination = Vector3.zero;

      
    }

    public override void FixedUpdateState()
    {
        if (owner == null) return;


        owner.Move();
        //switch (owner.MyPlayType)
        //{
        //    case PlayType.None:
        //        {
        //            if (owner.MyAgent != null)
        //            {
        //                // 도착 했는지 검사한다. 
        //                bool isAlive = CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance);
        //                // 사거리 내에 있는지 검사 
        //                bool isMeleeRange = owner.fieldOfView.MeeleAttackRangeView(owner.MyAgent);

        //                if (owner.fieldOfView.View() && isAlive == false && isMeleeRange == false)
        //                {
        //                    owner.Move();
        //                }
        //                else if (isMeleeRange == true)
        //                {
        //                    owner.myState = PlayerState.Attack;
        //                }
        //                else if(owner.fieldOfView.View() == false)
        //                {
        //                    owner.myState = PlayerState.Idle;
        //                }
        //            }
        //        }
        //        break;
        //    case PlayType.Playerable:
        //        owner.Move();
        //        break;
        //}

    }

    public override void UpdateState()
    {
        if (owner == null) return; 

        if (owner.myPlayType == PlayType.Playerable)
            return;

        owner.Search();
        //if (owner.fieldOfView.View() == true || owner.fieldOfView.GetTargetPos() != Vector3.zero)
        //    owner.myState = PlayerState.Chase;
        //if (owner.fieldOfView.MeeleAttackRangeView(owner.MyAgent))
        //{
        //    owner.myState = PlayerState.Attack;
        //}
    }

}
