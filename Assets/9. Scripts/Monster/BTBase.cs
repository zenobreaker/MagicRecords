using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BTBase : WheelerController
{
    public MonsterGrade grade;
    public float attackRange;   // ���� ����
    public float detectRange;   // �߰� ����
    public float speed;

    Transform enemyTransform;
    public float patrolRadius; // ��ȸ ����
    Vector3 originPos;
    Vector3 patrolDestination;

    public BehaviourTreeRunner btRunner;
    protected SelectorNode rootNode; // �ֻ������� �������� ��� ���� ��忡 �ڽĵ��� �߰��� ��� �ִ� �������� ����
    protected List<INode> nodes = new List<INode>(); // �� ��忡 �־��� �ڽ� ������ ��� ����Ʈ 

    public LayerMask targetLayerMask;   // ���� ����� ���̾� ����ũ

    public Vector3 destination;    // ������  

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void Search()
    {
        throw new System.NotImplementedException();
    }

    public override void StateAnimaiton()
    {
        throw new System.NotImplementedException();
    }

    public override void Think()
    {
        throw new System.NotImplementedException();
    }

    public override void Wait()
    {
        throw new System.NotImplementedException();
    }

    public virtual INode SettingBT()
    {
        rootNode = new SelectorNode(nodes);
        return rootNode;
    }

    // �ִϸ��̼��� ���� ������ �˻� 
    bool isAnimationRunning(string stateName)
    {
        if (anim == null) return false;

        if ((anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)))
        {
            var normalizeedTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

            return normalizeedTime != 0 && normalizeedTime < 1f;
        }

        return false;
    }

    void RotateToTarget(Vector3 targetVec)
    {
        Vector3 direction = (targetVec - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }


    #region Move & Detect Node

    // ���� �߰��ߴ��� �˻�
    protected INode.ENodeState CheckDetectEnemy()
    {
        var overlapColliders = Physics.OverlapSphere(transform.position, detectRange, LayerMask.GetMask("Player"));

        if (overlapColliders != null && overlapColliders.Length > 0)
        {
            enemyTransform = overlapColliders[0].transform;
            destination = enemyTransform.position;
            return INode.ENodeState.ENS_Success;
        }

        enemyTransform = null;

        return INode.ENodeState.ENS_Failure;
    }

    // ���� ���� �ٰ�����
    protected INode.ENodeState MoveToDetectEnemy()
    {
        Debug.Log("move to enemy");
        if (enemyTransform != null)
        {

            if (Vector3.SqrMagnitude(enemyTransform.position - transform.position) < (attackRange * attackRange))
            {
                anim.SetBool("Walking", false);
                return INode.ENodeState.ENS_Success;
            }

            RotateToTarget(enemyTransform.position);
            transform.position = Vector3.MoveTowards(transform.position, enemyTransform.position, Time.deltaTime * speed);
            if(!isAnimationRunning("Walking"))
                anim.SetBool("Walking", true);
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    // ���� ������ Ȯ���ϴ� �޼ҵ�
    protected INode.ENodeState CheckAttacking()
    {
        if (isAttacking == true)
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // ���� ���� �ȿ� �ִ��� �˻�
    protected INode.ENodeState CheckEnemyWithAttackRange()
    {
        if (enemyTransform != null)
        {

            if (Vector3.SqrMagnitude(enemyTransform.position - transform.position) < (attackRange * attackRange))
            {
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }


    #region Idle & Patrol Node
    // ����ϱ�
    protected INode.ENodeState DoIdle()
    {
        Debug.Log("do idle");
        if (isAnimationRunning("Idle"))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // �� ������ ������ ��ġ ��ȯ�ϴ� �Լ�
    protected Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        Vector3 randomPoint = center + new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        return randomPoint;
    }

    // ��ȸ�ϱ�
    protected INode.ENodeState DoPatrol()
    {
        Debug.Log("do patrol");
        // �⺻ ��ġ���� ���� �Ÿ� ����� ���ο� ������ ����
        if (Vector3.SqrMagnitude(originPos - transform.position) >= patrolRadius * patrolRadius)
        {
            destination = GetRandomPointInCircle(originPos, patrolRadius);
        }

        // �������� �̵�
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);

        // �̵� ���߿��� ��� Running ���� ����
        return INode.ENodeState.ENS_Running;
    }

    #endregion
}
