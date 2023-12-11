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


        owner.Search();

        //// 대기 중일 때 시간 감소시키기
        //if (idleTime > 0 && owner.myState == PlayerState.Idle)
        //{
        //    idleTime -= Time.deltaTime;
        //}
        //if (idleTime <= 0)
        //{
        //    owner.myState = PlayerState.Move;
        //}
    }

    public void ExecuteEnemyIdle()
    {
        if (owner.fieldOfView != null)
        {
            // 지정한 곳에 도착했는지 체크
            var isAlive = CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance);

            // 타겟을 감지한 상태이고 도착하지않았다면 계속 추격상태
            if ((owner.fieldOfView.View() && isAlive == false) &&
                owner.fieldOfView.MeeleAttackRangeView(owner.MyAgent) == false)
            //|| owner.fieldOfView.GetTargetTransform() != null)
            {
                owner.myState = PlayerState.Chase;
                return;
            }
            else if (owner.fieldOfView.MeeleAttackRangeView(owner.MyAgent))
            {
                owner.myState = PlayerState.Attack;
                return;
            }
            // 목표가 없으면 장시간 대기
            else if (owner.fieldOfView.View() == false)
            {
                owner.myState = PlayerState.Idle;
                return;
            }

            // todo 타입별 가만히 있을 때 동작하는 함수를 저마다 정의해서 호출

            // 아군인 경우, 일정 거리를 리더가 움직이면 그 주변을 따라가도록 스테이트를 변경
            // 타겟 설정하는 클래스에선 아군 타겟 변수를 추가해서 적이 없다면 아군을 따라가도록 설정하기

         
        }

    }


    public void IdleAction()
    {
        // todo 이내용을 휠러컨트롤러에게 정의하게 하고 
        // think함수에서 동작하도록해보자.
        // think함수는 update에서 동작하도록 수정한다.
        // 아래 내용은 플레이어컨트롤러/ 위 함수의 내용은 몬스터들이 하도록한다.
        if (GameManager.MyInstance == null) return; 

        // 대상이 어떤 타입인지에 따라 행동하는게 다르다
        // 아군일 경우 
        if(owner.teamTag == TeamTag.TEAM)
        {
            // 팀 중에서 대장인지 검사한다.
            if(owner.isLeader == true)
            {

            }
            // 리더와의 거리를 체크한다.
            else
            {
                //todo 타겟을 감지했는지 검사
                var pos = GameManager.MyInstance.GetLeaderPosition();
                float dist = Vector3.Distance(owner.transform.position, pos);
             
                // 리더와의 거리가 일정 거리보다 크다면 리더 쪽으로 오게 한다. 
            }
        }
        else
        {

        }

        
    }
}
