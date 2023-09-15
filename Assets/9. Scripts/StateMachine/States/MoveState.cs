using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{

    // 여기선 움직임을 관리한다.
    // 도착 관련 변수인 destination을 설정하는 스테이트이기도 함 

    public MoveState(WheelerController context)
    {
        this.owner = context;
    }

    void SetDestination(WheelerController sm)
    {
        destination = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
        destination = sm.transform.localPosition + destination;
        RaycastHit hit;

        if (Physics.Raycast(destination, destination + Vector3.down, out hit, 100f))
        {
            //Debug.Log(hit.collider.name + "충돌체");
            if (!hit.collider.CompareTag("land"))
            {
             //   Debug.Log("정해진 땅이 없음 도착지점 재설정 ");
              //  SetDestination(sm);
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

        if (owner.myPlayType == PlayType.Playerable)
            return;

        if (owner.myState == PlayerState.Move)
        {
        //    Debug.Log("무브 스테이트 진입");
            SetDestination(owner);
        }
        else if (owner.myState == PlayerState.Chase)
        {
           // Debug.Log("추적스테이트 진입");
            SetDestination(owner.fieldOfView.GetTargetPos());
        }
    }

    public override void ExitState()
    {
    //    Debug.Log("무브 스테이트 탈출");
        destination = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
        if (owner == null) return; 

        switch (owner.MyPlayType)
        {
            case PlayType.None:
                {
                   // Debug.Log("무브 스테이트 실행 중");
                    if (owner.MyAgent != null)
                    {

                        // 목적지까지 도착햇는지 검사 
                        if (CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance))
                        {

                            if (owner.fieldOfView.View())
                                owner.myState = PlayerState.Chase;
                            if (owner.fieldOfView.MeeleAttackRangeView())
                                owner.myState = PlayerState.Attack;
                            // 도착했다면 IDLE로 
                            owner.myState = PlayerState.Idle;
                        }

                        //  Debug.Log("무브 스테이트 에이전트 검사 ");
                        owner.MyAgent.SetDestination(destination);
                    }
                }
                break;
            case PlayType.Playerable:
                // 주인 객체의 Move 메소드를 호출한다. 
                owner.Move();
                break;
        }

    }

    public override void UpdateState()
    {
        if (owner == null) return; 

        // 플레이어블 타입이면 동작하지 않도록
        if (owner.myPlayType == PlayType.Playerable)
            return;

        // 23. 02. 15타겟이 이미 설정되어 있다면 추적 스테이트로 변경하도록 추가 
        // 타겟이 시야에 들어왔다면 추적 스테이트로 
        if (owner.fieldOfView.View() == true || owner.fieldOfView.GetTargetPos() != Vector3.zero)
            owner.myState = PlayerState.Chase;
        // 타겟이 근접 범위까지 왔다면 공격 스테이트로 변환 
        if (owner.fieldOfView.MeeleAttackRangeView())
        {
            owner.MyAgent.ResetPath();
            owner.myState = PlayerState.Attack;
        }
    }

}
