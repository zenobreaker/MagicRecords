using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BTBossMonster : MonsterBase
{

    public GameObject muzzleObject; 

    public float attackRange;   // ���� ����
    public float detectRange;   // �߰� ����
    public float speed; 

    Transform enemyTransform;
    public float patrolRadius; // ��ȸ ����
    Vector3 originPos;
    Vector3 patrolDestination;
    public int attackSlashCount = 0; // ������ ���� Ƚ�� ���� ���� 
    int tripleSlashCount  =  0; // Ʈ���� ������ ���� ���� 
    public float spiritBombCoolTime = 30.0f;
    bool isSpiritBombReady = false; 

    public float currentSpiritBomobCoolTime = 0.0f;
    public float trapBulletCreateCooltime = 5.0f;
    public float currentCreateTrapBulletTime = 0.0f;

    int checkIndex = 0; 
    int[] checkHealthPointArr = new int[3];

    bool isPattern = false; // ���� ���� �� �����ϴ� ���� 
    BehaviourTreeRunner btRunner;
    Coroutine attackCoroutine;
    Coroutine bombCoolTimeCoroutine; 


    private void Awake()
    {
        Debug.Log("bt �׽�Ʈ");
        btRunner = new BehaviourTreeRunner(SettingBT());
    }

    private void OnEnable()
    {
        if (player != null && player.MyStat != null)
        {
            speed = player.MyStat.speed;

            checkHealthPointArr[0] = (int)(player.MyMaxHP * 0.7f);
            checkHealthPointArr[1] = (int)(player.MyMaxHP * 0.4f);
            checkHealthPointArr[2] = (int)(player.MyMaxHP * 0.3f);
        }

        checkIndex = 0;

        if (TrapContoller.instance != null)
        {
            TrapContoller.instance.SetSpawnObject();
        }
    }

    private new void Update()
    {
        if(Input.GetKey(KeyCode.L))
        {
            if (player != null && player.MyStat != null)
            {
                player.MyCurrentHP = (int)(player.MyCurrentHP * 0.5);
                Debug.Log(" �� ���� : " + player.MyCurrentHP + " / " + player.MyMaxHP);
            }
        }

        if (btRunner != null)
            btRunner.Operate();
    }

    INode SettingBT()
    {
        return new SelectorNode
            (
                new List<INode>()
                {
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckAttaking),
                            new ActionNode(CheckEnemyWithAttackRange),
                            new SelectorNode(GetAttackNodeList()),
                            new ActionNode(AfterPatternIncrease),
                        }
                    ),
                    new SequenceNode
                    (
                        new List<INode>()
                        {
                            new ActionNode(CheckDetectEnemy),
                            new ActionNode(MoveToDetectEnemy),
                        }

                    ),
                    new SelectorNode
                    (
                        new List<INode>()
                        {
                          new ActionNode(DoIdle),
                          new ActionNode(DoPatrol),
                        }
                    ),

                }
            );
    }

    // �ִϸ��̼��� ���� ������ �˻� 
    bool isAnimationRunning(string stateName)
    {
        if (anim == null) return false; 

        if((anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)))
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

    #region Attack Node

    // ������ ������ ������ �޼ҵ�
    INode.ENodeState DecideAttackPattern()
    {
        // ���� ���� 
        // �ǰ� 50% �����ΰ�?
        if(player != null && player.MyCurrentHP <= player.MyMaxHP *0.5f)
        {
        }

        // ����� ������ �޼��ƴ°�? 

        // �ð� �������� ���� ������ �Ǿ��°�? 

        // Ʈ���� ������ 

        // ������ 


            return INode.ENodeState.ENS_Success;
    }

    // ���� ������ Ȯ���ϴ� �޼ҵ�
    INode.ENodeState CheckAttaking()
    {
        if(isAttacking == true || isPattern == true)
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // ���� ���� �ȿ� �ִ��� �˻�
    INode.ENodeState CheckEnemyWithAttackRange()
    {
        if(enemyTransform != null)
        {

            if (Vector3.SqrMagnitude(enemyTransform.position - transform.position) < (attackRange * attackRange))
            {
                return INode.ENodeState.ENS_Success;
            }
        }

        return INode.ENodeState.ENS_Failure;
    }


    // ������ - ���������� �˱� �߻�
    IEnumerator CreateSlash()
    {
        if (anim == null || muzzleObject == null )
            yield return null;

        anim.SetTrigger("Slash");
        
        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                CreateSlashObject();
                break; 
            }
            yield return new WaitForSeconds(0.1f); 
        }

        Debug.Log("������ �ڷ�ƾ ����");
        isPattern = false;
        yield return null; 
    }

    INode.ENodeState AttackSlash()
    {
        if( anim == null || isAnimationRunning("Boss_Slash") == true || attackSlashCount >= 3)
            return INode.ENodeState.ENS_Failure;

        // �˱� ���� 
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        isPattern = true;
        isAttacking = true;
        attackSlashCount++;
        attackCoroutine = StartCoroutine(CreateSlash());
        return INode.ENodeState.ENS_Success;
    }

    void Teleport(string targetName)
    {
     
        // ������ ������ ������ ��ҷ� �̵��ϱ� 
        // �̵��� ��ġ���� ������ �Ÿ��� ������ �������� �����Ͽ� ���� �̵�
        Vector3 randomOffset = Random.insideUnitSphere * attackRange;
        Vector3 destination = this.transform.position + randomOffset;

        bool isEnable = false;

        Collider[] colliders = Physics.OverlapSphere(destination, 1.0f, LayerMask.NameToLayer("SpecialArea"));

        // �ֺ��� ������Ʈ�� �ִ��� Ȯ��
        if (colliders.Length > 0)
        {
            Debug.Log("�̵��� ��ġ�� ������Ʈ�� �ֽ��ϴ�.");
        }
        else
        {
            isEnable = true; 
        }

        if(isEnable == true)
        {
            transform.position = destination;
        }
    }


    IEnumerator MoveAndSlah()
    {
        // �̵��� ��ġ���� ������ �Ÿ��� ������ �������� �����Ͽ� ���� �̵�
        Vector3 randomOffset = Random.insideUnitSphere * attackRange;
        Vector3 destination = this.transform.position + randomOffset;
        destination.y = 0;
        isAttacking = true;
        isPattern = true;
        bool isEnable = false;

        Collider[] colliders = Physics.OverlapSphere(destination, 2.0f);

        // �ֺ��� ������Ʈ�� �ִ��� Ȯ��
        bool isObstackle = false; 
        if (colliders.Length > 0)
        {
            for(int i = 0; i < colliders.Length;i++)
            {
                // ���� �ƴϰ� ���̳� ��Ÿ ��ֹ����� �ִٸ�? 
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Land") ||
                    colliders[i].gameObject.layer == LayerMask.NameToLayer("SpecialArea"))
                {
                    continue;
                }
                else
                {
                    isObstackle = true;
                    break;
                }
            }
        }

        if(isObstackle == false)
        {
            isEnable = true;
            anim.SetBool("Walking", true);
        }
        else
        {
            Debug.Log("�̵��� ��ġ�� ������Ʈ�� �ֽ��ϴ�.");
        }


        // �ش� ��ġ�� ������ �̵��Ѵ�. 
        while(isEnable)
        {
            var dir = destination - transform.position;
            dir.y = 0; 
            RotateToTarget(destination);
            //MyRigid.MovePosition(transform.position + dir.normalized * Time.deltaTime * speed);
            transform.position = 
            Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            var sqr = Vector3.SqrMagnitude(destination - transform.position);
            if (sqr <= 0.1f* 0.1f)
            {
                anim.SetBool("Walking", false);
                break; 
            }

            yield return null; 
        }

        // ���� ���� �ٶ󺻴�. 
        RotateToTarget(enemyTransform.position);
        var targetPos = transform.forward * 3f; 
        yield return new WaitForSeconds(0.5f);

        // ��� �߻� �� 
        DangerMarkerShoot(0, transform.position, transform.localRotation, 3f);

        yield return new WaitForSeconds(1.0f);
        // ������ ����
        StartCoroutine(CreateSlash());
        yield return null;
    }


    // ������ ��ġ�� ������ �̵� �� ������ 
    INode.ENodeState TeleportSlash()
    {
        // �⺻  �������� 3�� �̻������ʾҵ��� �� ������ ���� ����
        if(attackSlashCount < 3 || isAttacking == true)
            return INode.ENodeState.ENS_Failure;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
            return INode.ENodeState.ENS_Running;
        }

        isAttacking = true;
        attackCoroutine = StartCoroutine(MoveAndSlah());
        attackSlashCount = 0; 

        return INode.ENodeState.ENS_Success;
    }


    IEnumerator IETripleSlash()
    {
        isPattern = true;
        // ���� ���� �ٶ󺻴�. 
        RotateToTarget(enemyTransform.position);

        // ��� ����
        //#. 3�������� ���� ��� ��� 
        Vector3 trForward = transform.forward * 3f;
        Quaternion qAngle1 = Quaternion.Euler(0, 30, 0); // �߾� ����� ���� �־��� ���� (��)
        Quaternion qAngle2 = Quaternion.Euler(0, -30, 0); // �߾� ����� ���� �־��� ���� (��)

        Vector3 myPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // �ڱ� ��ġ
        Vector3 treePos1, treePos2, treePos3;

        treePos1 = myPos + trForward;
        treePos2 = qAngle1 * trForward;  // ���ʹϾ� ���� ���Ͱ��� ���ϸ� ������ŭ ȸ���� �� 
        treePos3 = qAngle2 * trForward;

        DangerMarkerShoot(0, myPos, treePos1);
        DangerMarkerShoot(0, myPos, myPos + treePos2);
        DangerMarkerShoot(0, myPos, myPos + treePos3);

        // ���� �ð� ��� ��
        yield return new WaitForSeconds(1.5f);

        // 3 �������� �߻� 

        anim.SetTrigger("Slash");

        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                CreateSlashObject();
                CreateSlashObject(this.transform.rotation * qAngle1);
                CreateSlashObject(this.transform.rotation * qAngle2);
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        tripleSlashCount = 0;
        isPattern = false; 
        yield return null;
    }


    // Ʈ���� ������ - ���濡 3�������� �˱� �߻� 
    INode.ENodeState AttackTripleSlash()
    {
        if (anim == null || tripleSlashCount < 5 )
        {
            return INode.ENodeState.ENS_Failure;
        }

        if (attackCoroutine != null && isPattern == true && isAttacking == true && 
            tripleSlashCount >= 5)
        {
            return INode.ENodeState.ENS_Running;
        }
        
        isAttacking = true;
        attackCoroutine = StartCoroutine(IETripleSlash());

        return INode.ENodeState.ENS_Success;
    }

    IEnumerator IEArroundSlash()
    {
        // ���� ����� �̵��Ѵ�. 
        if (GameManager.MyInstance == null)
            yield return null;
        isPattern = true;
        var centerPos = GameManager.MyInstance.GetMapCenterPos();

        transform.position = centerPos;

        // �ִϸ��̼� ����
        anim.SetTrigger("CastingDown");

        // �ð� �������� �������� ���� �� n�� �� �˱⸦ �߻�
        int count = 0;
        while(count < 12 )
        {
            Vector3 trForward = transform.forward * 10.0f;
            Quaternion angle = Quaternion.Euler(0, 30 * count, 0); // �߾� ����� ���� �־��� ���� (��)

            Vector3 myPos = new Vector3(transform.position.x,0, transform.position.z);   // �ڱ� ��ġ
            var finalPos =  angle  * trForward ;

            DangerMarkerShoot(0, myPos, finalPos);
            yield return new WaitForSeconds(0.5f);

            // �˱� �߻� 
            CreateSlashObject(this.transform.rotation * angle);
            //RotateToTarget(finalPos);
            
            //while (true)
            //{
            //    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            //    {
            //        break; 
            //    }

            //    yield return new WaitForSeconds(0.1f);
            //}

            count++; 
        }

        isAttacking = false;
        isPattern = false; 
        yield return null; 
    }

    // �ð� �������� ������ 
    INode.ENodeState ArroundSlash()
    {
        // �ǰ� 70%, 40% 10% �� ���� ��� 
        if (anim == null || player == null || 
            checkIndex >= checkHealthPointArr.Length ||
            player.MyCurrentHP > checkHealthPointArr[checkIndex])
            return INode.ENodeState.ENS_Failure;
        else if (isPattern == true)
            return INode.ENodeState.ENS_Running;
        
        // �ε������� �ø���.
        checkIndex++;
        
        isAttacking = true;
        attackCoroutine = StartCoroutine(IEArroundSlash());

        return INode.ENodeState.ENS_Success;

    }

    IEnumerator IESpiritBomob()
    {
        if (anim == null)
            yield return null;
        // todo ���� ���輱 �߻� 
        isPattern = true;
        // ��ü ���� 
        var upSpwan = transform.position + new Vector3(0, 15.0f, 0);
        var meteor = ObjectPooler.SpawnFromPool<Meteor>("MeteorObject", upSpwan,
               transform.rotation);
        if (meteor != null)
        {
            meteor.SetOwn(player, transform);
            meteor.targetLayer = 1 <<LayerMask.NameToLayer("Player");
            meteor.transform.position = upSpwan;
            meteor.scaleUpDelayTime = 5.0f;
            meteor.dropSpeed = 100.0f;
            meteor.maximumlScaleSize = 10.0f;
            meteor.scaleUpDelayTime = 5.0f;
            meteor.StartMeteor();
            var dropPoint = transform.forward * 5.0f;
            meteor.dropPoint = dropPoint;
        }
        anim.SetTrigger("Casting");
        // ���� �ð� �� ��ü ���� 
        yield return new WaitForSeconds(5.0f);
        anim.SetTrigger("CastingDown");

        yield return new WaitForSeconds(3.0f);
        isSpiritBombReady = false; 
        isPattern = false;
    }

    IEnumerator IESpiritBombCoolTimer()
    {
        while(currentSpiritBomobCoolTime >= 0.0f)
        {
            currentSpiritBomobCoolTime -= Time.deltaTime;
            yield return null; 
        }
        currentSpiritBomobCoolTime = 0.0f;
        isSpiritBombReady = true; 
    }

    // ����� - ������� ���� �� ���� �ð� �Ŀ� ���濡 ��ô
    INode.ENodeState SpiritBomb()
    {
        if (anim == null || currentSpiritBomobCoolTime > 0)
            return INode.ENodeState.ENS_Failure;
        else if (isPattern == true )
            return INode.ENodeState.ENS_Running;
        // ���� ���� ��, 30�� ���� �߻� 
        // todo ���� ���輱 �߻� 
        if (isSpiritBombReady == false && bombCoolTimeCoroutine == null)
        {
            currentSpiritBomobCoolTime = spiritBombCoolTime;
            // ��Ÿ�� 
            bombCoolTimeCoroutine = StartCoroutine(IESpiritBombCoolTimer());
        }
        if(isSpiritBombReady == true)
        {
            bombCoolTimeCoroutine = null; 
            // �׼� ����
            attackCoroutine = StartCoroutine(IESpiritBomob());
        }

        return INode.ENodeState.ENS_Success;
    }


    IEnumerator IECreateUlitmateBomb()
    {
        currentCreateTrapBulletTime = trapBulletCreateCooltime;
        while (currentCreateTrapBulletTime > 0.0f)
        {
            currentCreateTrapBulletTime -= Time.deltaTime;
            yield return null; 
        }
    }

    // �� 50% ���� ���� 
    // �ʿ� �������� �������� ��ü�� ��ȯ
    INode.ENodeState SummonUltimateBomb()
    {
        // �ǰ� 50�� �̸����� �˻�
        if(player == null || player.MyStat == null || TrapContoller.instance == null ||
            player.MyCurrentHP > player.MyMaxHP * 0.5f)
            return INode.ENodeState.ENS_Failure;

        // �� ������ ������ ���۾��� �۵��ϹǷ� �ٸ� ������ �����ϱ� ���� ���и� ��ȯ
        if (currentCreateTrapBulletTime > 0.0f)
            return INode.ENodeState.ENS_Failure;


        // todo ���� ȭ�鿡 � ������ �߸� ���� �� ����.
        // ���� ������ ���ư����� ������ ���ðž�
        StartCoroutine(IECreateUlitmateBomb()); // Ư�� �ð� ���� ȣ��
        TrapContoller.instance.CreateTrapByMoveType(5, enemyTransform.position,
            10);

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState AfterPatternIncrease()
    {
        tripleSlashCount++;

        return INode.ENodeState.ENS_Success; 
    }

    // ���� ������ ��ȯ�Ѵ�.
    List<INode> GetAttackNodeList()
    {
        // ���� �� �� �� �ڷ���Ʈ ������ 
        // 
        return new List<INode>()
        {
            new ActionNode(SummonUltimateBomb),
            new ActionNode(SpiritBomb),
            new ActionNode(ArroundSlash),
            new ActionNode(AttackTripleSlash),
            new ActionNode(TeleportSlash),
            new ActionNode(AttackSlash),
        };
    }

    void CreateSlashObject(Vector3 vec, Quaternion quaternion)
    {
        var so = ObjectPooler.SpawnFromPool("SlashObject", vec,
               quaternion);
        if (so == null)
            return; 

        if(so.TryGetComponent(out EnemyBullet enemyBullet))
        {
            enemyBullet.SetAttackInfo(player, transform);
        }
    }
    
    void CreateSlashObject(Quaternion quaternion)
    {
        CreateSlashObject(muzzleObject.transform.position, quaternion);
    }

    void CreateSlashObject()
    {
        CreateSlashObject(muzzleObject.transform.position, transform.rotation);
    }

    // ���� ����
    INode.ENodeState DoAttack()
    {
        if (enemyTransform == null)
            return INode.ENodeState.ENS_Failure;
        Debug.Log("do attack");
        // ���� ����
        RotateToTarget(enemyTransform.position);
        List<INode> list = GetAttackNodeList();

        if(list.Count == 0)
            return INode.ENodeState.ENS_Failure;

      
        foreach(INode node in list)
        {
            return node.Evaluate();
        }

        return INode.ENodeState.ENS_Failure;

    }
    #endregion

    #region Move & Detect Node

    // ���� �߰��ߴ��� �˻�
    INode.ENodeState CheckDetectEnemy()
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
    INode.ENodeState MoveToDetectEnemy()
    {
        Debug.Log("move to enemy");
        if (enemyTransform != null)
        {

            if (Vector3.SqrMagnitude(enemyTransform.position - transform.position) < (attackRange * attackRange))
            {
                return INode.ENodeState.ENS_Success;
            }

            RotateToTarget(enemyTransform.position);
            transform.position = Vector3.MoveTowards(transform.position, enemyTransform.position, Time.deltaTime * speed);

            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Failure;
    }

    #endregion

    #region Idle & Patrol Node
    // ����ϱ�
    INode.ENodeState DoIdle()
    {
        Debug.Log("do idle");
        if(isAnimationRunning("Idle"))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // �� ������ ������ ��ġ ��ȯ�ϴ� �Լ�
    Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        Vector3 randomPoint = center + new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        return randomPoint;
    }

    // ��ȸ�ϱ�
    INode.ENodeState DoPatrol()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }

}
