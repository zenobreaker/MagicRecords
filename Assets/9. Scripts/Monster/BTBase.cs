using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BTBase : WheelerController
{
    public MonsterGrade grade;
    public float attackRange;   // 공격 범위
    public float detectRange;   // 발견 범위
    public float speed;

    Transform enemyTransform;
    public float patrolRadius; // 배회 범위
    Vector3 originPos;
    Vector3 patrolDestination;

    public BehaviourTreeRunner btRunner;
    protected SelectorNode rootNode; // 최상위에서 결정짓는 노드 여기 노드에 자식들을 추가로 쥐어 주는 형식으로 구상
    protected List<INode> nodes = new List<INode>(); // 위 노드에 넣어질 자식 노드들이 담긴 리스트 

    public LayerMask targetLayerMask;   // 공격 대상의 레이어 마스크

    public Vector3 destination;    // 목적지  

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

    // 애니메이션을 진행 중인지 검사 
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

    // 적을 발견했는지 검사
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

    // 적을 향해 다가가기
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

    // 공격 중인지 확인하는 메소드
    protected INode.ENodeState CheckAttacking()
    {
        if (isAttacking == true)
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // 공격 범위 안에 있는지 검사
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
    // 대기하기
    protected INode.ENodeState DoIdle()
    {
        Debug.Log("do idle");
        if (isAnimationRunning("Idle"))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // 원 내에서 랜덤한 위치 반환하는 함수
    protected Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        Vector3 randomPoint = center + new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        return randomPoint;
    }

    // 배회하기
    protected INode.ENodeState DoPatrol()
    {
        Debug.Log("do patrol");
        // 기본 위치에서 일정 거리 벗어나면 새로운 목적지 선택
        if (Vector3.SqrMagnitude(originPos - transform.position) >= patrolRadius * patrolRadius)
        {
            destination = GetRandomPointInCircle(originPos, patrolRadius);
        }

        // 목적지로 이동
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);

        // 이동 도중에는 계속 Running 상태 유지
        return INode.ENodeState.ENS_Running;
    }

    #endregion
}
