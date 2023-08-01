using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    [SerializeField] public string MonsterName = ""; // 동물의 이름

    public int itemNumber = 0; // 아이템 획득 개수

    [SerializeField] protected bool isBoss = false;

    [SerializeField]
    protected float ChaseTime = 0f; // 총 추격 시간
    protected float currentChaseTime; // 계산
    [SerializeField]
    protected float chaseDelayTime = 0f;// 추격 딜레이


    [SerializeField]
    protected LayerMask targetMask; // 타겟 마스크

    protected GameObject player;
    public Vector3 destination; // 목적지

    // 상태 변수
    protected bool isAction = false; // 행동중인지 아닌지 판별
    protected bool isWalking = false; // 걷는지 안 걷는지 판별
    protected bool isRunning = false; // 뛰는지 판별
    protected bool isChasing = false; // 추젹 중인지 판별
    protected bool isAttacking = false; // 공격중인지 판별
    public bool isDead = false; // 죽었는지 판별

    public enum state { WALK, IDLE, CHASE, ATTACK };
    public state currentState;

    [SerializeField] protected float walkTime = 0f; // 걷기 시간
    [SerializeField] protected float waitTime = 0f; // 대기 시간
    [SerializeField] protected float runTime = 0f; // 뛰기 시간
    protected float currentTime;

    public float baseAttackRange;
    public GameObject dangerLine;
    public LayerMask layerMask;

    // 필요한 컴포넌트
    [SerializeField] protected Status status = null;

    [SerializeField] protected Animator anim = null;
    [SerializeField] protected Rigidbody rigid = null;
    [SerializeField] protected BoxCollider boxCol = null;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;

    [SerializeField] protected AudioClip[] sound_Normal = null;
    [SerializeField] protected AudioClip sound_Hurt = null;
    [SerializeField] protected AudioClip sound_Dead = null;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
    
    private void Start()
    {
        theViewAngle = GetComponent<FieldOfViewAngle>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        currentState = state.IDLE;
        isAction = true;
        nav = GetComponent<NavMeshAgent>();

    }

    protected virtual void Update()
    {
        /*
        if (!isDead)
        {
            Move();
            ElapseTime();
        }
        */
        if (!isChasing )
        {
            switch (currentState)
            {
                case state.IDLE:
                    Wait();
                    break;
                case state.WALK:
                    Walk();
                    break;
                case state.CHASE:
                    Chase();
                    break;
                case state.ATTACK:
                    Attack();
                    break;

            }
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
        {
            // rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));
            //rigid.velocity =  (transform.forward * walkSpeed * Time.deltaTime);
            nav.SetDestination(transform.position + destination);
        }
    }

    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0 && !isChasing && !isAttacking)
                ReSet();// 다음 랜덤 행동 개시
        }
    }

    protected virtual void ReSet()
    {
        Debug.Log("리셋");
        isWalking = false; isRunning = false; isAction = true;
        nav.speed = status.MyWalkSpeed;
        nav.ResetPath(); // 목적지를 제거함으로써 목적지의 충돌 현상 제거
                         //anim.SetBool("Walking", isWalking); // anim.SetBool("Running", isRunning);
                         //anim.SetBool("Running", isRunning);
                         // direction.Set(0f, Random.Range(0f, 360f), 0f);
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-1f, 1f));
    }

    // 몬스터가 멈춰서 대기 중인 상태 
    protected void Wait()
    {
        rigid.velocity = Vector3.zero;
        nav.ResetPath();
        isWalking = false;
        anim.SetTrigger("Idle");

        Debug.Log("대기" + currentState.ToString());

        if (theViewAngle.View()) // 걷다가 적을 발견한 경우 
        {
            currentState = state.CHASE;
            Debug.Log("적발견" + currentState.ToString());
            Chase();
        }

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = Random.Range(currentTime, waitTime * 2.0f);

                Vector3 randDest = new Vector3(transform.position.x + Random.Range(-5f, 5f), 0f,
                  transform.position.z + Random.Range(-5f, 5f));
                destination.Set(randDest.x, randDest.y, randDest.z); // 시간이 다되면 목적지를 설정한다.

                currentState = state.WALK;
               
            }
        }
        else
            currentTime = waitTime;

    }

    // 몬스터가 걷는 행위를 취하면서 적을 탐색
    protected void Walk()
    {

        Debug.Log("걷기" + currentState.ToString() + nav.isStopped);
        nav.speed = status.MyWalkSpeed;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        nav.speed = status.MyWalkSpeed;
        Move();

        if (theViewAngle.View()) // 걷다가 적을 발견한 경우 
        {
            currentState = state.CHASE;
            Debug.Log("적발견" + currentState.ToString());
            Chase();
        }

        if (IsArrived())  // 목적지에 도착한다면 시간을 초기화하고 대기 상태로 돌입 
        {
            Debug.Log("도착!");
            currentTime = waitTime;
            isWalking = false;
            anim.SetBool("Walking", isWalking);
            currentState = state.IDLE;
        }

    }

    protected bool IsArrived()
    {
        float distance = Vector3.Distance(transform.position, destination);
   
        if (nav.remainingDistance <= 0.1f) 
            return true;
        else
            return false;
    }

    protected void Chase()
    {
        if (theViewAngle.View() && !isAttacking && !isDead)
        {
            currentState = state.CHASE;
            nav.SetDestination(theViewAngle.GetTargetPos());
            nav.speed = status.MyWalkSpeed;
            isChasing = true;
            Move();
            StartCoroutine(ChaseTimeCalc());
        }
    }

    protected IEnumerator ChaseTimeCalc()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Debug.Log("체이스시작");
            // 충분히 가까이 있고, 
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= baseAttackRange)
            {
                if (theViewAngle.View()) // 눈 앞에 있을 경우 
                {
                    Debug.Log("플레이어 공격 시도");
                    currentState = state.ATTACK;
                }
                else
                {
                    currentState = state.CHASE;
                }
            }
            yield return new WaitForSeconds(chaseDelayTime);
            currentChaseTime += chaseDelayTime;
        }

        isChasing = false;
        currentState = state.IDLE;
        currentTime = waitTime;
        nav.ResetPath();
    }

    protected void TryWalk()
    {
        currentTime = walkTime;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        nav.speed = status.MyWalkSpeed;
    }

    // 공격 루틴 
    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        //direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;
        destination = _targetPos;
        //isRunning = true;
        isWalking = true;
        nav.speed = status.MyWalkSpeed;
        nav.SetDestination(destination);
        //anim.SetBool("Running", isRunning);
        anim.SetBool("Walking", isWalking);

    }

    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            Debug.Log("체이스시작");
            // 충분히 가까이 있고, 

            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= baseAttackRange)
            {
                if (theViewAngle.View()) // 눈 앞에 있을 경우 
                {
                    Debug.Log("플레이어 공격 시도");
                    StartCoroutine(AttackCoroutine());
                }
            }
            yield return new WaitForSeconds(chaseDelayTime);
            currentChaseTime += chaseDelayTime;
        }

        isChasing = false;
        nav.ResetPath();
    }

    protected void Attack()
    {
        isAttacking = true;
        Debug.Log("플레이어 공격 시도");
        StopAllCoroutines();
        StartCoroutine(AttackCoroutine());
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        isWalking = false;
        anim.SetBool("Walking", isWalking);
        nav.ResetPath();
        currentChaseTime = ChaseTime;
        yield return new WaitForSeconds(0.5f);

        transform.LookAt(theViewAngle.GetTargetPos()); // 플레이어를 바라보게 함
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.5f);

        RaycastHit _hit;
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out _hit, 5, targetMask))
        {
            Debug.Log("플레이어 적중!");
            //  thePlayerStatus.DecreaseHP(attackDamage);
         //   CharStat.MyInstance.Damaged(status.MyAttack);
        }
        else
        {
            Debug.Log("플레이어 빗나감");
        }

        yield return new WaitForSeconds(status.MyAttackDelay);
        isAttacking = false;
        //StartCoroutine(ChaseTargetCoroutine());
    }

    public virtual void Damaged(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            Debug.Log("쳐맞네 이걸 ");
            status.MyHP -= _dmg;
            //UIManager.instance.ShowHealthBar(player);

            transform.localPosition -= (_targetPos - transform.position).normalized;
            transform.LookAt(_targetPos - transform.position.normalized);
            isChasing = true;

            if (status.MyHP <= 0)
            {
                Dead();
                return;
            }

            //   PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
        }
    }

    protected void Dead()
    {
        // PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isChasing = false;
        isAttacking = false;
        isDead = true;
        isAction = false;
        nav.ResetPath();
        Debug.Log(MonsterName + "가 죽었음!");
        anim.SetTrigger("Dead");

        UIManager.instance.HideHealthBar();
        this.gameObject.tag = "Untagged";

        //player.GetComponent<StatusController>().IncreaseExp(status.MyEXP);
        //Debug.Log(status.MyEXP + "경험치 획득!");
        //DropItems();
        Destroy(gameObject, 0.5f);

    }

  
}
