using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Localization.Platform.Android;
using UnityEngine;



// BT ������� Strategy ������ �ռ��Ͽ� AI ����
public class TreeMonBT : BTBase
{
    // �����ͽ� ���� 
    // 1. ���濡 ���� ��� ��ȯ 
    // 2. ���濡 5�� ���� ��� ��ȯ 
    // 3. ������ 3������ ���� ��� ��ȯ (����Ʈ, ����)
    // 4. 8���� ������� ��ȯ (����) 

    // �� ���Ͽ� ���� ������ �㤷�� Ŭ����
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
    public float attackDelayTime;   // ���� ��, ������ �ð� �� 
    public Coroutine delayCoroutine;    // ������ �ڷ�ƾ
    public Coroutine cooldownCoroutine; // ��Ÿ�� �ڷ�ƾ
    public Coroutine attackCoroutine;   // ���� ���� �ڷ�ƾ


    private AttackPattern selectPattern;
    private void Awake()
    {
        // 
        Debug.Log("bt ��");
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
        // ���� �߰� 
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
        // ���ϵ� ���� ����
        ResetPattern(); 
    }

    public override INode SettingBT()
    {
        base.SettingBT();
        nodes.Add(
            new SelectorNode(
            new List<INode>()
            { 
                // ���� ������ ��� 
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
                // Ž�� �� �̵� ������ ��� 
                new SequenceNode
                (
                    new List<INode>()
                    {
                        new ActionNode(CheckDetectEnemy),
                        new ActionNode(MoveToDetectEnemy),
                    }

                ),
                // ��� ���� ���
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

    // ������Ÿ�ӵ��� ��ٸ��� �ϴ� 
    public INode.ENodeState WaitForDelayTime()
    {
        if (attackDelayTime <= 0) return INode.ENodeState.ENS_Success;

        // ���� ��ȯ - �ٸ� ���·� ���� ��Ű��
        return INode.ENodeState.ENS_Failure;
    }

    // ���� ���� 
    public INode.ENodeState CheckPattern()
    {
        // ���� ����Ʈ �߿��� ��ٿ��� 0�� ���� �����´�. 
        selectPattern = null;
        foreach (var pattern in patterns)
        {
            if (pattern == null) return INode.ENodeState.ENS_Failure; 
            
            if(pattern.isReady == true)
            {
                // �� ������ ����
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

    // ���� �� ��Ÿ�� ������ �ڷ�ƾ
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


    // ��޿� ���� ���� ����
    public INode.ENodeState ExecutePattern()
    {
        if (isAttacking == true || selectPattern == null)
            return INode.ENodeState.ENS_Failure;

        if (attackCoroutine != null && isAttacking == true)
            return INode.ENodeState.ENS_Running;

        Debug.Log("���� ����");
        isAttacking = true;
        selectPattern.isReady = false; // ���� ����� ���� ������ ����Ʈ�� ���� �����
        // �׼� ����
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
        //              ������(�ѷ����� Ŭ ���� ������ ���̱׸�)   *  0~1������ ��(���ϰ���)
        //#. 3�������� ���� ��� ��� 
        Vector3 trForward = transform.forward * 3f;
        Quaternion qAngle1 = Quaternion.Euler(0, 30, 0); // �߾� ����� ���� �־��� ���� (��)
        Quaternion qAngle2 = Quaternion.Euler(0, -30, 0); // �߾� ����� ���� �־��� ���� (��)

        Vector3 myPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // �ڱ� ��ġ
        Vector3 treePos1, treePos2, treePos3;

        treePos1 = myPos + trForward;
        treePos2 = qAngle1 * trForward;  // ���ʹϾ� ���� ���Ͱ��� ���ϸ� ������ŭ ȸ���� �� 
        treePos3 = qAngle2 * trForward;

        //DangerMarkerShoot(0, myPos, treePos1);
        // DangerMarkerShoot(0, myPos, myPos + treePos2);
        // DangerMarkerShoot(0, myPos, myPos + treePos3);

        treePos2 += myPos;  // ���� ��ġ�� �����༭ ���ϴ� ��ġ�� �̵� 
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

            // ������ ��ȯ�� ���� ��ġ + ���� ��ŭ ���� ��ġ �� ���� 
            treePos1 = tree1.transform.localPosition + trForward;
            // �߾� �������� �ٽ� ��ǥ ���� ���� 
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
        float distance = 3.5f;              // ���� ������ �Ÿ� 
        int count = 15;                     // �߻�ü ����
        float intervalAngle = 360 / count;  // �߻�ü ������ ����
        float x, z;
        x = z = 0;

        // #. �� �������� ��� ���� 
        Vector3 t_Pos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // �ڱ� ��ġ

        for (int i = 0; i < count; i++)
        {

            float angle = intervalAngle * i;
            x = t_Pos.x + distance * Mathf.Cos(angle * Mathf.PI / 180);
            z = t_Pos.z + distance * Mathf.Sin(angle * Mathf.PI / 180);

            Vector3 endPos = new Vector3(x, t_Pos.y, z);
            //  DangerMarkerShoot(0, t_Pos, endPos);
        }

        yield return new WaitForSeconds(1.5f);

        // #. �� �������� ���� ����
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
