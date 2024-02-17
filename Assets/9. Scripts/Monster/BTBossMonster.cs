using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BTBossMonster : MonsterBase
{

    public GameObject muzzleObject; 

    public float attackRange;   // 공격 범위
    public float detectRange;   // 발견 범위
    public float speed; 

    Transform enemyTransform;
    public float patrolRadius; // 배회 범위
    Vector3 originPos;
    Vector3 patrolDestination;
    public int attackSlashCount = 0; // 슬래쉬 공격 횟수 제한 변수 
    int tripleSlashCount  =  0; // 트리플 슬래쉬 관련 변수 
    public float spiritBombCoolTime = 30.0f;
    bool isSpiritBombReady = false; 

    public float currentSpiritBomobCoolTime = 0.0f;
    public float trapBulletCreateCooltime = 5.0f;
    public float currentCreateTrapBulletTime = 0.0f;

    int checkIndex = 0; 
    int[] checkHealthPointArr = new int[3];

    bool isPattern = false; // 패턴 중일 때 제어하는 변수 
    BehaviourTreeRunner btRunner;
    Coroutine attackCoroutine;
    Coroutine bombCoolTimeCoroutine; 


    private void Awake()
    {
        Debug.Log("bt 테스트");
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
                Debug.Log(" 피 감소 : " + player.MyCurrentHP + " / " + player.MyMaxHP);
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

    // 애니메이션을 진행 중인지 검사 
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

    // 공격할 패턴을 결정할 메소드
    INode.ENodeState DecideAttackPattern()
    {
        // 패턴 선택 
        // 피가 50% 이하인가?
        if(player != null && player.MyCurrentHP <= player.MyMaxHP *0.5f)
        {
        }

        // 원기옥 조건이 달성됐는가? 

        // 시계 방향으로 날린 조건이 되었는가? 

        // 트리플 슬래쉬 

        // 슬래쉬 


            return INode.ENodeState.ENS_Success;
    }

    // 공격 중인지 확인하는 메소드
    INode.ENodeState CheckAttaking()
    {
        if(isAttacking == true || isPattern == true)
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // 공격 범위 안에 있는지 검사
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


    // 슬래쉬 - 일직선으로 검기 발사
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

        Debug.Log("슬래쉬 코루틴 종료");
        isPattern = false;
        yield return null; 
    }

    INode.ENodeState AttackSlash()
    {
        if( anim == null || isAnimationRunning("Boss_Slash") == true || attackSlashCount >= 3)
            return INode.ENodeState.ENS_Failure;

        // 검기 생성 
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
     
        // 일정한 구역의 랜덤한 장소로 이동하기 
        // 이동할 위치에서 일정한 거리에 랜덤한 오프셋을 설정하여 몬스터 이동
        Vector3 randomOffset = Random.insideUnitSphere * attackRange;
        Vector3 destination = this.transform.position + randomOffset;

        bool isEnable = false;

        Collider[] colliders = Physics.OverlapSphere(destination, 1.0f, LayerMask.NameToLayer("SpecialArea"));

        // 주변에 오브젝트가 있는지 확인
        if (colliders.Length > 0)
        {
            Debug.Log("이동할 위치에 오브젝트가 있습니다.");
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
        // 이동할 위치에서 일정한 거리에 랜덤한 오프셋을 설정하여 몬스터 이동
        Vector3 randomOffset = Random.insideUnitSphere * attackRange;
        Vector3 destination = this.transform.position + randomOffset;
        destination.y = 0;
        isAttacking = true;
        isPattern = true;
        bool isEnable = false;

        Collider[] colliders = Physics.OverlapSphere(destination, 2.0f);

        // 주변에 오브젝트가 있는지 확인
        bool isObstackle = false; 
        if (colliders.Length > 0)
        {
            for(int i = 0; i < colliders.Length;i++)
            {
                // 땅이 아니고 벽이나 기타 장애물들이 있다면? 
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
            Debug.Log("이동할 위치에 오브젝트가 있습니다.");
        }


        // 해당 위치로 빠르게 이동한다. 
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

        // 적을 향해 바라본다. 
        RotateToTarget(enemyTransform.position);
        var targetPos = transform.forward * 3f; 
        yield return new WaitForSeconds(0.5f);

        // 경고선 발사 후 
        DangerMarkerShoot(0, transform.position, transform.localRotation, 3f);

        yield return new WaitForSeconds(1.0f);
        // 슬래쉬 공격
        StartCoroutine(CreateSlash());
        yield return null;
    }


    // 무작위 위치로 빠르게 이동 후 슬래쉬 
    INode.ENodeState TeleportSlash()
    {
        // 기본  슬래쉬를 3번 이상하지않았따면 본 패턴은 실행 안함
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
        // 적을 향해 바라본다. 
        RotateToTarget(enemyTransform.position);

        // 경고선 사출
        //#. 3방향으로 보낼 경고선 계산 
        Vector3 trForward = transform.forward * 3f;
        Quaternion qAngle1 = Quaternion.Euler(0, 30, 0); // 중앙 기술로 부터 멀어질 각도 (우)
        Quaternion qAngle2 = Quaternion.Euler(0, -30, 0); // 중앙 기술로 부터 멀어질 각도 (좌)

        Vector3 myPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);   // 자기 위치
        Vector3 treePos1, treePos2, treePos3;

        treePos1 = myPos + trForward;
        treePos2 = qAngle1 * trForward;  // 쿼터니언 값에 벡터값을 곱하면 각도만큼 회전한 값 
        treePos3 = qAngle2 * trForward;

        DangerMarkerShoot(0, myPos, treePos1);
        DangerMarkerShoot(0, myPos, myPos + treePos2);
        DangerMarkerShoot(0, myPos, myPos + treePos3);

        // 일정 시간 대기 후
        yield return new WaitForSeconds(1.5f);

        // 3 방향으로 발사 

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


    // 트리플 슬래쉬 - 전방에 3방향으로 검기 발사 
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
        // 맵의 가운데로 이동한다. 
        if (GameManager.MyInstance == null)
            yield return null;
        isPattern = true;
        var centerPos = GameManager.MyInstance.GetMapCenterPos();

        transform.position = centerPos;

        // 애니메이션 실행
        anim.SetTrigger("CastingDown");

        // 시계 방향으로 빨간선을 날린 후 n초 후 검기를 발사
        int count = 0;
        while(count < 12 )
        {
            Vector3 trForward = transform.forward * 10.0f;
            Quaternion angle = Quaternion.Euler(0, 30 * count, 0); // 중앙 기술로 부터 멀어질 각도 (우)

            Vector3 myPos = new Vector3(transform.position.x,0, transform.position.z);   // 자기 위치
            var finalPos =  angle  * trForward ;

            DangerMarkerShoot(0, myPos, finalPos);
            yield return new WaitForSeconds(0.5f);

            // 검기 발사 
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

    // 시계 방향으로 슬래쉬 
    INode.ENodeState ArroundSlash()
    {
        // 피가 70%, 40% 10% 일 때만 사용 
        if (anim == null || player == null || 
            checkIndex >= checkHealthPointArr.Length ||
            player.MyCurrentHP > checkHealthPointArr[checkIndex])
            return INode.ENodeState.ENS_Failure;
        else if (isPattern == true)
            return INode.ENodeState.ENS_Running;
        
        // 인덱스값을 늘린다.
        checkIndex++;
        
        isAttacking = true;
        attackCoroutine = StartCoroutine(IEArroundSlash());

        return INode.ENodeState.ENS_Success;

    }

    IEnumerator IESpiritBomob()
    {
        if (anim == null)
            yield return null;
        // todo 원형 위험선 발사 
        isPattern = true;
        // 구체 생성 
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
        // 일정 시간 후 구체 투하 
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

    // 원기옥 - 원기옥을 모은 후 일정 시간 후에 전방에 투척
    INode.ENodeState SpiritBomb()
    {
        if (anim == null || currentSpiritBomobCoolTime > 0)
            return INode.ENodeState.ENS_Failure;
        else if (isPattern == true )
            return INode.ENodeState.ENS_Running;
        // 보스 조우 후, 30초 마다 발사 
        // todo 원형 위험선 발사 
        if (isSpiritBombReady == false && bombCoolTimeCoroutine == null)
        {
            currentSpiritBomobCoolTime = spiritBombCoolTime;
            // 쿨타임 
            bombCoolTimeCoroutine = StartCoroutine(IESpiritBombCoolTimer());
        }
        if(isSpiritBombReady == true)
        {
            bombCoolTimeCoroutine = null; 
            // 액션 실행
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

    // 피 50% 이하 패턴 
    // 맵에 무작위로 지나가는 구체를 소환
    INode.ENodeState SummonUltimateBomb()
    {
        // 피가 50퍼 미만인지 검사
        if(player == null || player.MyStat == null || TrapContoller.instance == null ||
            player.MyCurrentHP > player.MyMaxHP * 0.5f)
            return INode.ENodeState.ENS_Failure;

        // 이 패턴은 별도의 동작없이 작동하므로 다른 동작을 싱행하기 위해 실패를 반환
        if (currentCreateTrapBulletTime > 0.0f)
            return INode.ENodeState.ENS_Failure;


        // todo 게임 화면에 어떤 문구가 뜨면 좋을 것 같다.
        // 아주 느리게 날아가지만 닿으면 아플거야
        StartCoroutine(IECreateUlitmateBomb()); // 특정 시간 마다 호출
        TrapContoller.instance.CreateTrapByMoveType(5, enemyTransform.position,
            10);

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState AfterPatternIncrease()
    {
        tripleSlashCount++;

        return INode.ENodeState.ENS_Success; 
    }

    // 공격 노드들을 반환한다.
    List<INode> GetAttackNodeList()
    {
        // 공격 세 번 후 텔레포트 슬래쉬 
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

    // 공격 실행
    INode.ENodeState DoAttack()
    {
        if (enemyTransform == null)
            return INode.ENodeState.ENS_Failure;
        Debug.Log("do attack");
        // 공격 실행
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

    // 적을 발견했는지 검사
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

    // 적을 향해 다가가기
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
    // 대기하기
    INode.ENodeState DoIdle()
    {
        Debug.Log("do idle");
        if(isAnimationRunning("Idle"))
        {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    // 원 내에서 랜덤한 위치 반환하는 함수
    Vector3 GetRandomPointInCircle(Vector3 center, float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius;

        Vector3 randomPoint = center + new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
        return randomPoint;
    }

    // 배회하기
    INode.ENodeState DoPatrol()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }

}
