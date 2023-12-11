using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class MonsterBase : WheelerController
{
    [SerializeField] public string MonsterName = ""; // 동물의 이름

    [SerializeField] GameObject item_Coin = null; // 동전 아이템.
    public Item[] item_prefabs = null;   // 아이템. 
    public int itemNumber = 0;  // 아이템 획득 개수

    [SerializeField]
    protected float ChaseTime = 0f; // 총 추격 시간
    protected float currentChaseTime; // 계산
    [SerializeField]
    protected float chaseDelayTime = 0f;// 추격 딜레이

    [SerializeField]
    protected LayerMask targetMask; // 타겟 마스크

    public Vector3 destination; // 목적지


    [SerializeField] protected bool isBoss = false;

    protected bool isAction = false; // 행동중인지 아닌지 판별

    [Header("패턴 오브젝트")]
    public DangerLine[] dangerLine;
    public float rangeOfEnemy;
    public float baseAttackRange;

    // 필요한 컴포넌트
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected BoxCollider boxCol = null;

    protected AudioSource theAudio;

    [SerializeField] protected AudioClip[] sound_Normal = null;
    [SerializeField] protected AudioClip sound_Hurt = null;
    [SerializeField] protected AudioClip sound_Dead = null;

 

    protected virtual void Start()
    {
     
        stateMachine = new StateMachine();
        stateMachine.States.Add(PlayerState.Idle, new IdleState(this));
        stateMachine.States.Add(PlayerState.Move, new MoveState(this));
        stateMachine.States.Add(PlayerState.Attack, new AttackState(this));
        stateMachine.States.Add(PlayerState.Chase, new MoveState(this));

        fieldOfView = GetComponent<FieldOfViewAngle>();
        if(fieldOfView != null)
            fieldOfView.viewDistance = rangeOfEnemy;
        theAudio = GetComponent<AudioSource>();
        //currentTime = waitTime;
        
        myState = PlayerState.Idle;
        //stateMachine
        isAction = true;
        
    }

    void FixedVelocity()
    {
        // 움직이는 상태거나 추적 상태면 속도값을 고정으로 줘서 부딪혀도 이상하게 보이지 않도록 
        if (myState == PlayerState.Move || myState == PlayerState.Chase)
        {
            MyRigid.velocity = Vector3.zero;
            MyRigid.angularVelocity = Vector3.zero;
        }

    }

    protected virtual void Update()
    {
        // 테스트 모드면 넘긴다.
        if (isTest)
        {
            myState = PlayerState.Idle;
            return;
        }

        if (stateMachine != null)
        {
            //if (theCondition != null)
            //{
            //    // 컨디션이 움직일 수 있는 컨디션이 아니라면 상태를 움직이지 않는 Idle 상태로 전환한다.
            //    if (theCondition.myCondition != Condition.MOVE_CONDITION)
            //    {
            //        myState = PlayerState.Idle;
            //        ResetBehaviour();
            //    }
            //}

            // 상태 변경
            ChangeState(myState);
            //stateMachine.ChangeState(stateMachine.States[myState]);
            // 상태에 따른 실행 
            stateMachine.OperateState();
        }

        StateAnimaiton();
    }

    protected void FixedUpdate()
    {
        if (isTest)
            return;

        FixedVelocity();
        if (stateMachine != null)
            stateMachine.FixedOperateState();
    }

    protected void ResetBehaviour()
    {
        // currentTime = waitTime;
        isAttacking = false;
        isChasing = false;
        isWalking = false;
        isAction = true;
    }

    public override void Attack()
    {

    }

    public override void Search()
    {
        if (fieldOfView != null)
        {
            // 타겟을 발견 했지만 공격 사정 거리 내에 있지 않는다면 
            if (fieldOfView.View()  &&
                fieldOfView.MeeleAttackRangeView(MyAgent) == false)
            {
                myState = PlayerState.Chase;
            }
            // 사정 거리 내에 왔다면 공격 
            else if (fieldOfView.MeeleAttackRangeView(MyAgent))
            {
                myState = PlayerState.Attack;
            }
        }
    }

    public override void Move()
    {
        MyAgent.speed = player.MyStat.speed;
        isWalking = true;
        if (fieldOfView != null)
        {
            MyAgent.SetDestination(fieldOfView.GetTargetPos());
        }
    }

  
    public override void StateAnimaiton()
    {
        isAction = true;

        switch (myState)
        {
            case PlayerState.Idle:
                anim.SetBool("Walking", isWalking);
                break;
            case PlayerState.Move:
                anim.SetBool("Walking", isWalking);
                break;
        }
    }


    // idle state에 진입 시 동작할 함수 
    // 몬스터가 멈춰서 대기 중인 상태 
    public override void Wait()
    {
        MyRigid.velocity = Vector3.zero;
        MyAgent.velocity = Vector3.zero;
        MyAgent.isStopped = true;
        MyAgent.ResetPath();

        isWalking = false;

    }

    public override void Damage(int _damage, Vector3 _targetPos, bool isCrit = false)
    {

        if (!isDead)
        {
            base.Damage(_damage, _targetPos, isCrit);

         
            //   PlaySE(sound_Hurt);
            if (anim != null)
            {
                anim.SetTrigger("Hurt");
            }
        }
    }

    public override void Damage(int damage, bool isCrit = false)
    {
        base.Damage(damage, isCrit);

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowHealthBar(this);
        }

        if (GameManager.MyInstance != null)
        {
            GameManager.MyInstance.IncreaseCombo();
        }

        if (player.MyCurrentHP <= 0 && !isTest)
        {
            Dead();
            return;
        }

    }


    public virtual void Damaged(int _dmg, Vector3 _targetPos)
    {
        if (!isDead && player != null)
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.CreateFloatingText(this.gameObject.transform.position, _dmg.ToString());
                UIManager.instance.ShowHealthBar(this);

                // 그냥 일반적인 공격에 해당 루틴이 돌면서 컨디션을 NONE으로 변경해버린다
                // 일반 공격은 타이머값을 던져주지않기때문에 0초로 들어와서 바로 NONE화되버린다.
                //if(theCondition != null)
                //    theCondition.AbnormalCondition();
                GameManager.MyInstance.IncreaseCombo();
            }
         //   transform.localPosition -= (_targetPos - transform.position).normalized;
         //   transform.LookAt(_targetPos - transform.position.normalized);
         //     currentState = state.CHASE;

            if (player.MyCurrentHP <= 0 && !isTest)
            {
                Dead();
                return;
            }

            //   PlaySE(sound_Hurt);
            if(anim != null)
                anim.SetTrigger("Hurt");
        }
    }


    protected virtual void Dead()
    {
        // PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false; 
        isChasing = false;
        isAttacking = false;
        isDead = true;
        isAction = false;
        MyAgent.ResetPath();
        Debug.Log(MonsterName + "가 죽었음!");
        anim.SetTrigger("Dead");

        UIManager.instance.HideHealthBar();
        this.gameObject.tag = "Untagged";

        // 게임 매니저 기능 함수 호출 
        if (GameManager.MyInstance != null)
        {
            GameManager.MyInstance.ChangeGameScore(1, player.GetExp);
        }

        // ㅅtodo 경험치 기능 및 레벨업 기능 수정 
        Debug.Log(player.GetExp + "경험치 획득!");
        DropItem();
        Destroy(gameObject, 0.5f);

    }

    protected void RandomSound()
    {
        int _random = Random.Range(0, 3); // 일상 사운드 세 개
                                          //   PlaySE(sound_Normal[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    protected void DropItem()
    {
       // item_Coin.GetComponent<ItemPickUp>().item.itemValue = Random.Range(1, 10); 
       // Instantiate(item_Coin, transform.position, Quaternion.identity);

       // for (int i = 0; i < item_prefabs.Length; i++)
       // {
       ////     Instantiate(item_prefabs[i].itemPrefab, transform.position, Quaternion.identity);
       // }
    }
    public override void Think()
    {
        if (MyPlayer == null) return;

        foreach (var buff in buffDebuffs)
        {
            if (buff == null || buff.specialOption == null) continue;

            if (buff.specialOption.optionType == OptionType.DEBUFF)
            {
                switch (buff.specialOption.abilityType)
                {
                    // todo 이동 제약 관련 디버프면 일단 상태를 멈춘다.
                    case AbilityType.HOLD:
                        myState = PlayerState.Idle;
                        break;
                    case AbilityType.STURN:
                        myState = PlayerState.Idle;
                        break;
                    case AbilityType.ICE:
                        myState = PlayerState.Idle;
                        break;
                }
            }
        }
    }
}


