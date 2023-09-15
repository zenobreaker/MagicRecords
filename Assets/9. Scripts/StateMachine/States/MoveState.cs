using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState
{

    // ���⼱ �������� �����Ѵ�.
    // ���� ���� ������ destination�� �����ϴ� ������Ʈ�̱⵵ �� 

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
            //Debug.Log(hit.collider.name + "�浹ü");
            if (!hit.collider.CompareTag("land"))
            {
             //   Debug.Log("������ ���� ���� �������� �缳�� ");
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
        //    Debug.Log("���� ������Ʈ ����");
            SetDestination(owner);
        }
        else if (owner.myState == PlayerState.Chase)
        {
           // Debug.Log("����������Ʈ ����");
            SetDestination(owner.fieldOfView.GetTargetPos());
        }
    }

    public override void ExitState()
    {
    //    Debug.Log("���� ������Ʈ Ż��");
        destination = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
        if (owner == null) return; 

        switch (owner.MyPlayType)
        {
            case PlayType.None:
                {
                   // Debug.Log("���� ������Ʈ ���� ��");
                    if (owner.MyAgent != null)
                    {

                        // ���������� �����޴��� �˻� 
                        if (CheckAlived(owner.transform.position, destination, owner.MyAgent.stoppingDistance))
                        {

                            if (owner.fieldOfView.View())
                                owner.myState = PlayerState.Chase;
                            if (owner.fieldOfView.MeeleAttackRangeView())
                                owner.myState = PlayerState.Attack;
                            // �����ߴٸ� IDLE�� 
                            owner.myState = PlayerState.Idle;
                        }

                        //  Debug.Log("���� ������Ʈ ������Ʈ �˻� ");
                        owner.MyAgent.SetDestination(destination);
                    }
                }
                break;
            case PlayType.Playerable:
                // ���� ��ü�� Move �޼ҵ带 ȣ���Ѵ�. 
                owner.Move();
                break;
        }

    }

    public override void UpdateState()
    {
        if (owner == null) return; 

        // �÷��̾�� Ÿ���̸� �������� �ʵ���
        if (owner.myPlayType == PlayType.Playerable)
            return;

        // 23. 02. 15Ÿ���� �̹� �����Ǿ� �ִٸ� ���� ������Ʈ�� �����ϵ��� �߰� 
        // Ÿ���� �þ߿� ���Դٸ� ���� ������Ʈ�� 
        if (owner.fieldOfView.View() == true || owner.fieldOfView.GetTargetPos() != Vector3.zero)
            owner.myState = PlayerState.Chase;
        // Ÿ���� ���� �������� �Դٸ� ���� ������Ʈ�� ��ȯ 
        if (owner.fieldOfView.MeeleAttackRangeView())
        {
            owner.MyAgent.ResetPath();
            owner.myState = PlayerState.Attack;
        }
    }

}
