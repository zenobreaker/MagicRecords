using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Localization.Platform.Android;
using UnityEngine;



// BT 기반으로 Strategy 패턴을 합성하여 AI 제작
public class TreeMonBT : BTBase
{
    // 나무귀신 패턴 
    // 1. 전방에 나무 기둥 소환 
    // 2. 전방에 5개 나무 기둥 소환 
    // 3. 전방향 3갈래로 나무 기둥 소환 (엘리트, 보스)
    // 4. 8방향 나무기둥 소환 (보스) 

    // 각 패턴에 대한 정보를 담ㅇ은 클래스
    [System.Serializable]
    public class AttackPattern
    {
        public string name;
        public float attackRange;
        public float delayTime;
        public float coolDown = 0;
        public bool isReady = false;
        public Func<IEnumerator> action; 
    }

    public List<AttackPattern> patterns = new List<AttackPattern>();
    public float attackDelayTime;   // 공격 후, 딜레이 시간 값 
    public Coroutine delayCoroutine;    // 딜레이 코루틴
    public Coroutine cooldownCoroutine; // 쿨타임 코루틴
    public Coroutine attackCoroutine;   // 공격 중인 코루틴


    private AttackPattern selectPattern;
    private void Awake()
    {
        // 
        Debug.Log("bt 온");
        InitPattern();
        btRunner = new BehaviourTreeRunner(SettingBT());
        
    }

    private void Update()
    {
        if (btRunner != null && isTest == false)
            btRunner.Operate();
    }

    public void InitPattern()
    {
        // 패턴 추가 
        patterns.Clear(); 

        switch(grade)
        {
            case MonsterGrade.NORMAL:
                //patterns.Add(new AttackPattern { name = "NormalAttack", attackRange = 3.5f, delayTime = 1.5f, action = NormalAttack});
                patterns.Add(new AttackPattern { name = "SummonTree", attackRange = 3.5f, delayTime = 1.5f, action = SummonTree });
                patterns.Add(new AttackPattern { name = "TreeWave", attackRange = 7.0f, delayTime = 1.5f, action = TreeWave });
                break;
            case MonsterGrade.ELITE:
                patterns.Add(new AttackPattern { name = "TreeTriWave", attackRange = 7.0f, delayTime = 3.0f, action = TreeTriWave });
                break;
            case MonsterGrade.BOSS:
                patterns.Add(new AttackPattern { name = "TreeAroundWave", attackRange = 7.0f, delayTime = 3.0f, action = TreeAroundWave });
                break;
            default:
                break; 
        }
        // 패턴들 전부 리셋
        ResetPattern(); 
    }

    public override INode SettingBT()
    {
        base.SettingBT();
        nodes.Add(
            new SelectorNode(
            new List<INode>()
            { 
                // 공격 시퀀스 노드 
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(WaitForDelayTime),
                        new ActionNode(SetPatternCoolDown),
                        new ActionNode(CheckPattern),
                        new ActionNode(CheckAttacking),
                        new ActionNode(CheckEnemyWithAttackRange),
                        new ActionNode(ExecutePattern),
                    }
                ),
                // 탐색 및 이동 시퀀스 노드 
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDetectEnemy),
                        new ActionNode(MoveToDetectEnemy),
                    }

                ),
                // 대기 선택 노드
                new SelectorNode
                (
                    new List<INode>()
                    {
                        new ActionNode(DoIdle),
                        new ActionNode(DoPatrol),
                    }
                ),
            }
        )); 

        return rootNode;
    }

    // 딜레이타임동안 기다리게 하는 
    public INode.ENodeState WaitForDelayTime()
    {
        if (attackDelayTime <= 0) return INode.ENodeState.ENS_Success;

        // 실패 반환 - 다른 상태로 전이 시키기
        return INode.ENodeState.ENS_Failure;
    }

    // 패턴 결정 
    public INode.ENodeState CheckPattern()
    {
        // 패턴 리스트 중에서 쿨다운이 0인 패턴 가져온다. 
        selectPattern = null;
        foreach (var pattern in patterns)
        {
            if (pattern == null) return INode.ENodeState.ENS_Failure; 
            
            if(pattern.isReady == true)
            {
                // 이 패턴을 선택
                selectPattern = pattern;
              
                attackRange = selectPattern.attackRange;
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }

    public void ResetPattern()
    {
        foreach (var pattern in patterns)
        {
            if (pattern == null)
                continue;

            pattern.coolDown = pattern.delayTime;
            pattern.isReady = false; 
        }
    }

    public INode.ENodeState SetPatternCoolDown()
    {
        if (cooldownCoroutine == null)
            cooldownCoroutine = StartCoroutine(PatternCoolDown());

        return INode.ENodeState.ENS_Success;
    }

    // 패턴 별 쿨타임 돌리는 코루틴
    IEnumerator PatternCoolDown()
    {
        while (true)
        {
            foreach (var pattern in patterns)
            {
                if (pattern == null)
                    continue;

                if (pattern.coolDown > 0 && pattern.isReady == false)
                {
                    pattern.coolDown -= Time.deltaTime;
                }
                else if(pattern.coolDown <= 0)
                {
                    pattern.isReady = true;
                    pattern.coolDown = pattern.delayTime;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public IEnumerator DownDelaytime()
    {
        while(attackDelayTime >0)
        {
            attackDelayTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }


    // 등급에 따라 패턴 실행
    public INode.ENodeState ExecutePattern()
    {
        if (isAttacking == true || selectPattern == null)
            return INode.ENodeState.ENS_Failure;

        if (attackCoroutine != null && isAttacking == true)
            return INode.ENodeState.ENS_Running;

        Debug.Log("패턴 실행");
        isAttacking = true;
        selectPattern.isReady = false; // 얕은 복사로 인한 참조로 리스트의 값도 변경됨
        // 액션 실행
        attackCoroutine = StartCoroutine(selectPattern.action());
        attackDelayTime = selectPattern.delayTime;

        delayCoroutine = StartCoroutine(DownDelaytime());

        return INode.ENodeState.ENS_Success;
    }

    


    #region Treemon Pattern
    IEnumerator NormalAttack()
    {
        yield return new WaitForSeconds(1.2f);

        Vector3 t_Pos = transform.localPosition + (transform.forward * 1.5f + transform.up * 1.5f);
        //var clone = Instantiate(attackEffect[0], t_Pos, transform.localRotation);
        //clone.Play();
        yield return new WaitForSeconds(0.8f);
        //attackRange.GetComponent<BoxCollider>().enabled = false;
        //AttackEableObject(false);
        //Destroy(clone.gameObject);
        yield return null;
    }

    IEnumerator SummonTree()
    {
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        //DangerMarkerShoot(1, 1f, 1f);
        yield return new WaitForSeconds(1f);

        //var tree = Instantiate(go_TreeAttack, t_Pos + transform.forward*3f, Quaternion.identity);
        var tree = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", t_Pos + transform.forward * 3f);
        tree.SetDisableTimer(2.0f);
        //tree.GetComponentInChildren<AttackArea>().power = Mathf.RoundToInt(player.MyTotalAttack* 1.5f);
        var attackArea = tree.GetComponentInChildren<AttackArea>();
        if (attackArea != null)
        {
            attackArea.SetLayer(targetLayerMask);
            attackArea.SetAttackInfo(player, transform, 1.5f);
        }

        //tree.GetComponent<AttackArea>().PlayAnim();
        //Destroy(tree, 1f);
        isAttacking = false;
    }

    IEnumerator TreeWave()
    {
        Vector3 trForward = transform.forward * 3f;
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        // DangerMarkerShoot(0, t_Pos, t_Pos + trForward);
        yield return new WaitForSeconds(1.5f);

        Vector3 treePos = t_Pos;
        int count = 0;

        while (count < 5)
        {
            var tree = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", treePos + trForward);
            tree.SetDisableTimer(2.0f);
            //var tree = Instantiate(go_TreeAttack, treePos + trForward, Quaternion.identity);
            //tree.GetComponentInChildren<AttackArea>().power = Mathf.RoundToInt(player.MyTotalAttack * 1.7f);
            var attackArea = tree.GetComponentInChildren<AttackArea>();
            if (attackArea != null)
            {
                attackArea.SetLayer(targetLayerMask);
                attackArea.SetAttackInfo(player, transform, 1.5f);
            }

            //tree.GetComponent<AttackArea>().PlayAnim();
            treePos = tree.transform.localPosition;
            count++;
            //Destroy(tree, 4.5f);
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        //yield return null;
    }


    IEnumerator TreeTriWave()
    {
        //Mathf.Cos(Mathf.PI * 2 * curPatternCount / maxPatternCount[patternIndex])
        //              원주율(둘레값이 클 수록 파형을 많이그림)   *  0~1까지의 수(패턴개수)
        //#. 3방향으로 보낼 경고선 계산 
        Vector3 trForward = transform.forward * 3f;
        Quaternion qAngle1 = Quaternion.Euler(0, 30, 0); // 중앙 기술로 부터 멀어질 각도 (우)
        Quaternion qAngle2 = Quaternion.Euler(0, -30, 0); // 중앙 기술로 부터 멀어질 각도 (좌)

        Vector3 myPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // 자기 위치
        Vector3 treePos1, treePos2, treePos3;

        treePos1 = myPos + trForward;
        treePos2 = qAngle1 * trForward;  // 쿼터니언 값에 벡터값을 곱하면 각도만큼 회전한 값 
        treePos3 = qAngle2 * trForward;

        //DangerMarkerShoot(0, myPos, treePos1);
        // DangerMarkerShoot(0, myPos, myPos + treePos2);
        // DangerMarkerShoot(0, myPos, myPos + treePos3);

        treePos2 += myPos;  // 원래 위치를 더해줘서 원하는 위치로 이동 
        treePos3 += myPos;

        yield return new WaitForSeconds(1.5f);

        int count = 0;

        while (count < 5)
        {

            var tree1 = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", treePos1);
            tree1.SetDisableTimer(2.0f);
            var aa1 = tree1.GetComponentInChildren<AttackArea>();
            if (aa1 != null)
            {
                aa1.SetLayer(targetLayerMask);
                aa1.SetAttackInfo(player, transform, 1.7f);
            }
            var tree2 = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", treePos2);
            tree2.SetDisableTimer(2.0f);
            var aa2 = tree2.GetComponentInChildren<AttackArea>();
            if (aa2 != null)
            {
                aa2.SetLayer(targetLayerMask);
                aa2.SetAttackInfo(player, transform, 1.7f);
            }
            var tree3 = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", treePos3);
            tree3.SetDisableTimer(2.0f);
            var aa3 = tree3.GetComponentInChildren<AttackArea>();
            if (aa3 != null)
            {
                aa3.SetLayer(targetLayerMask);
                aa3.SetAttackInfo(player, transform, 1.7f);
            }

            // 이전에 소환한 나무 위치 + 차이 만큼 다음 위치 값 세팅 
            treePos1 = tree1.transform.localPosition + trForward;
            // 중앙 기준으로 다시 목표 방향 세팅 
            Vector3 tVec = treePos1 - myPos;
            treePos2 = qAngle1 * tVec;
            treePos3 = qAngle2 * tVec;
            treePos2 += myPos;
            treePos3 += myPos;

            count++;
            yield return new WaitForSeconds(0.5f);
        }
        isAttacking = false;
    }

    IEnumerator TreeAroundWave()
    {
        int roopCount = 5;
        float distance = 3.5f;              // 나무 생성간 거리 
        int count = 15;                     // 발사체 개수
        float intervalAngle = 360 / count;  // 발사체 사이의 각도
        float x, z;
        x = z = 0;

        // #. 전 방향으로 경고선 방출 
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // 자기 위치

        for (int i = 0; i < count; i++)
        {

            float angle = intervalAngle * i;
            x = t_Pos.x + distance * Mathf.Cos(angle * Mathf.PI / 180);
            z = t_Pos.z + distance * Mathf.Sin(angle * Mathf.PI / 180);

            Vector3 endPos = new Vector3(x, t_Pos.y, z);
            //  DangerMarkerShoot(0, t_Pos, endPos);
        }

        yield return new WaitForSeconds(1.5f);

        // #. 전 방향으로 나무 생성
        while (roopCount > 0)
        {
            for (int i = 0; i < count; i++)
            {

                float angle = intervalAngle * i;
                x = Mathf.Cos(angle * Mathf.PI / 180);
                z = Mathf.Sin(angle * Mathf.PI / 180);

                t_Pos.x = distance * x + transform.localPosition.x;
                t_Pos.z = distance * z + transform.localPosition.z;

                //var tree = Instantiate(go_TreeAttack, t_Pos, Quaternion.identity);
                var tree = ObjectPooler.SpawnFromPool<DisableObjectEvent>("TreeAttack", t_Pos);
                tree.SetDisableTimer(2.0f);
                var aa = tree.GetComponentInChildren<AttackArea>();
                if (aa != null)
                {
                    aa.SetLayer(targetLayerMask);
                    aa.SetAttackInfo(player, transform, 1.7f);
                }
                //  Destroy(tree, 3.5f);
            }
            distance += 3;
            roopCount -= 1;
            yield return new WaitForSeconds(0.7f);
        }

        isAttacking = false;
        yield return null;
    }

    #endregion


}
