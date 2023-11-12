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

        // �����ϴ� ����� �ƴϸ� ������Ű�� 
        if (owner.myPlayType == PlayType.None)
        {
            owner.Think();
        }
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

        //   Debug.Log("���̵� ������Ʈ ���� ��");
        if (owner.fieldOfView != null)
        {
            // ���������� �����޴��� �˻� 
            var isAlive = CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance);

            // ����� �ν� ������ �����ٸ� ���� ������Ʈ�� ���� �� Ż�� 
            if ((owner.fieldOfView.View() && isAlive == false) 
                || owner.fieldOfView.GetTargetTransform() != null)
            {
                owner.myState = PlayerState.Chase;
                return; 
            }
        }

        // ���� �ð��� ������ �����̰� �Ѵ�. 
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
