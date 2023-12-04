using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public enum ComboState
{
    NONE,
    ATTACK_1,
    ATTACK_2,
    ATTACK_3,
    ATTACK_4,
    Skill1,
    Skill2,
}

public class PlayerControl : WheelerController
{
    [Header("속도 관련 변수")]
    public float speed;

    private float dashSpeed;
    private float dashTime;
    public float startDashTime;

    private Vector3 dashDir;
    private Vector3 direction;

    //private Rigidbody myRigid;
    public Animator myAnimator;
    public float TargetRotation;    // 캐릭터 회전값

    private Vector3 targetPos;  // 자동시 움직일 정보

    // 필요한 컴포넌트 
    [SerializeField]
    private WeaponController theWeaponController = null;
    [SerializeField] WheelController wheelController = null;
   
    [SerializeField] GameObject go_DashEffect = null;

    // 제어 관련 변수
    public bool IsMove = false;
    private bool canDash;
    public bool isDash = false;
    public bool isTouch = false;

    // 대쉬에 나타나는 이펙트 
    GameObject dashEffect;

    // 자동 공격 상태일 때, 자신이 사용했던 스킬 슬롯 값
    public SkillSlotNumber usedSkillSlotNumber;

    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        if (fieldOfView != null)
        {
            m_agent.stoppingDistance = fieldOfView.meeleAttackDistance;
        }
        // 상태머신 생성 및 초기화 
        stateMachine = new StateMachine();
        stateMachine.States.Add(PlayerState.Idle, new IdleState(this));
        stateMachine.States.Add(PlayerState.Move, new MoveState(this));
        stateMachine.States.Add(PlayerState.Attack, new AttackState(this));
        stateMachine.States.Add(PlayerState.Chase, new MoveState(this));

        // 콤보값 관련 변수들 초기화 
        current_Combo_State = ComboState.NONE;
        current_Combo_Timer = default_Combo_Timer;

        if (player != null)
        {
            speed = player.MyStat.speed;

            if(theWeaponController != null)
            {
                theWeaponController.SetWeaponOwn(player);
            }
        }

        // 스킬 동작 관련 클래스에 주인 변수 할당 시키기 
        if(skillAction != null)
        {
            skillAction.skillOwn = player;
        }

        // 대쉬 관련 변수 초기화 
        dashTime = startDashTime;
        dashEffect = Instantiate(go_DashEffect, this.transform.position, Quaternion.identity);
        dashEffect.transform.SetParent(this.transform);
        dashEffect.SetActive(false);

        // 재생 관련 스탯 동작 
        InitRecoveryStat();
        
        idleTime = 3.0f; 
        // 초기 스테이트 변경
        ChangeState(PlayerState.Idle);
    }

    void FixedRotation()
    {
        m_rigid.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        stateMachine.FixedOperateState(); 
        FixedRotation();
       // Move(); 스테이트머신에서 호출하므로 주석 
    }

     void Update()
    {
        GetInput();
       
        StateAnimaiton();
        DashMove();
        ResetComboState();

        ChangeState(myState);
        stateMachine.OperateState();
        
    }


    public  override void Move()
    {
        if (isDead == true) return;

        if(skillAction != null)
        {
            if (skillAction.isAction == true)
                return; 
        }

        if (isAttacking == true)
            return;

        // 자동 상태일 경우 다른 로직을 쓰도록 한다. 
        if(isAutoFlag == true)
        {
            AutoMove();
            return; 
        }

        direction = direction.normalized * speed * Time.deltaTime;
        m_rigid.MovePosition(this.transform.position + direction);
        //myRigid.position += direction.normalized * speed * Time.deltaTime;
    }

    public override void Wait()
    {
        
    }

    // private void Move()
    // {
    //  this.transform.position += ((direction.normalized * CharStat.instance.speed) * Time.deltatime);
    //  transform.translate(direction.normalized * CharStat.instance.speed * Time.deltatime, space.world);
    //  myRigid.velocity = direction.normalized * CharStat.instance.speed;
    // if (isDash)
    //     direction = dashDir;

    //direction = direction.normalized * speed * Time.deltaTime;
    //myRigid.MovePosition(this.transform.position + direction );
    ////myRigid.position += direction.normalized * speed * Time.deltaTime;
    //wheelController.RollingWheel(IsMove, 10);
    //if (wheelRolling != null)
    //    wheelRolling.RollingWheel(10);

    //  }

    public override void StateAnimaiton()
    {
        bool isWheel = false; 
        switch (myState)
        {
            case PlayerState.Idle:
                {
                    myAnimator.SetBool("Walking", false);
                    isWheel = false; 
                    break;
                }
            case PlayerState.Move:
            case PlayerState.Chase:
                {
                    myAnimator.SetBool("Walking", true);
                    isWheel = true; 
                    break;
                }
        }

        if (wheelController != null)
        {
            wheelController.RollingWheel(isWheel, 10);
        }
    }

    // 공격하기 
    public override void Attack()
    {
        if (theWeaponController == null)
        {
            return;
        }

        // todo 임시 변수 나중에 따로 빼야함
        float damageRate = 1.0f; 

        if (theWeaponController.currentFireRate <= 0)
        {
            SearchtoAttack();

            // 오토 동작시 동작할 위치
            if(isAutoFlag == true)
            {
                bool isSkillAction = AutoAttack();
                if (isSkillAction == true)
                {
                    activateTimerToReset = true;
                    return; 
                }
            }

            // 공격 스테이트로 변경
            //ChangeState(PlayerState.Attack);
            myState = PlayerState.Attack;

            if (current_Combo_State == ComboState.ATTACK_4 || current_Combo_State == ComboState.Skill1
                || current_Combo_State == ComboState.Skill2 || skillAction.isAction == true)
                return;

            current_Combo_State++;  // 콤보 스테이트 증가 
            activateTimerToReset = true;    // 콤보 타이머 활성화
            current_Combo_Timer = default_Combo_Timer;  // 콤보 타이머가 디폴트 값을 대입해서 계산하도록 함.

            switch (current_Combo_State)
            {
                case ComboState.ATTACK_1:
                    myAnimator.SetTrigger("Attack");
                    break;
                case ComboState.ATTACK_2:
                    myAnimator.SetTrigger("Attack");
                    break;
                case ComboState.ATTACK_3:
                    myAnimator.SetTrigger("Attack");
                    break;
                case ComboState.ATTACK_4:
                    damageRate = 1.2f;
                    myAnimator.SetTrigger("FinalAttack");
                    break;
            }

            if (player != null && theWeaponController != null)
            {
                theWeaponController.SetDamageAndCrit(damageRate, 
                    player.MyTotalAttack, player.MyStat.totalCritRate, player.MyStat.totalCritDmg);
            }

            theWeaponController.TryFire(current_Combo_State);
        }
    }

    private void GetInput()
    {
        // 키보드 입력 관련 함수이므로 터치하거나 자동 모드일 경우엔 키지 않음
        if (isTouch || isAutoFlag)
            return;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        PlayerState inputState = PlayerState.Idle;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            IsMove = true;
            //currentState = State.Walk;
            inputState = PlayerState.Move;
            direction = new Vector3(h, 0, v);
            Quaternion newRotation = Quaternion.LookRotation(direction);

            if (skillAction != null)
            {
                if (skillAction.isAction == true)
                    return;
            }

            if (isAttacking == true) return; 

            m_rigid.rotation = Quaternion.Slerp(m_rigid.rotation, newRotation, 50*Time.deltaTime);
        }
        else
        {
            IsMove = false;
            inputState = PlayerState.Idle;
            direction = Vector3.zero;
        }

        isDash = Input.GetKeyDown(KeyCode.C);   // 대쉬키 누름

        // 공격키를 눌렀을 때 처리 
        if (Input.GetKeyDown(KeyCode.X))
        {
            // 공격에 대한 플래그값 변경 
            // 공격 시, 값을 초기화 이 플래그 값이 켜져 있으면 추가로 공격 못하게 할려고 해놓은 것.
            isAttacking = false;
            // 공격 스테이트로 변경
            inputState = PlayerState.Attack;
        }

        // keybindManager.에 설정된 버튼으로 스킬 사용
        if (KeybindManager.MyInstance != null)
        {
            foreach (string action in KeybindManager.MyInstance.ActionBinds.Keys)
            {
                if (Input.GetKeyDown(KeybindManager.MyInstance.ActionBinds[action]))
                {
                    UIManager.instance.ClickActionButton(action);
                }
            }
        }

        // 상태 머신 변경
        // ChangeState(inputState);
        myState = inputState;
    }

    public void InputJoyStick(Vector2 p_joy)
    {

        direction = new Vector3(p_joy.x, 0, p_joy.y);
        //Debug.Log("조이스틱 : " + direction);
        PlayerState inputState = PlayerState.Idle;  

        if (p_joy != Vector2.zero)
        {
            IsMove = true;
            m_rigid.MoveRotation(Quaternion.LookRotation(direction.normalized));
            inputState = PlayerState.Move;
        }


        if (!isTouch)
        {
            IsMove = false;
            inputState = PlayerState.Idle;
            direction = Vector3.zero;
        }

        //ChangeState(inputState);
        myState = inputState;
    }

    // 가장 가까운 적을 향해 방향 전환 
    public void SearchtoAttack()
    {
        Collider[] collis = Physics.OverlapSphere(transform.position, 20, 1 << LayerMask.NameToLayer("Enemy"));
        float shortTemp = 1000f;
        float temp = 0;
        GameObject t_Target = null;

        Debug.Log("검사:)" + collis.Length);

        for (int i = 0; i < collis.Length; i++)
        {
            temp = Vector3.Distance(collis[i].transform.position, transform.position);
            if (temp < shortTemp)
            {
                t_Target = collis[i].gameObject;
                shortTemp = temp;
            }
        }

        if (t_Target != null)
        {
            Vector3 finalTarget = t_Target.transform.position;
            finalTarget.y = transform.position.y;
            transform.LookAt(finalTarget);
        }
    }

    // 대쉬
    public void DashMove()
    {
        if (direction != Vector3.zero && isDash && !canDash)
        {
            // 대쉬 (급속 이동) 
            canDash = true;
            dashDir = direction;
            dashEffect.SetActive(true);
            speed = speed * 2;
            if(wheelController!= null)
            {
                wheelController.RollingWheel(isDash, 140);
            }

            Invoke("DashOut", 0.4f);
        } 
        
        // 대쉬 (텔레포트형)
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    /* float dashDistance =10f;

        //     isDash = true;
        //     currentState = State.Walk;
        //     Debug.Log("대쉬!");
        //     myRigid.MovePosition(this.transform.position + direction.normalized * dashDistance);
        //     wheelController.RollingWheel(isDash, (int)dashDistance);
        //     */

        //}

    }

    void DashOut()
    {
        speed = player.MyStat.speed;
        direction = Vector3.zero;
        canDash = false;
        isDash = false;
        dashEffect.SetActive(false);
    }


    void ResetComboState()
    {
        if (activateTimerToReset)
        {
            current_Combo_Timer -= Time.deltaTime;

            if (current_Combo_Timer <= 0f)
            {
                current_Combo_State = ComboState.NONE;

                activateTimerToReset = false;
                current_Combo_Timer = default_Combo_Timer;

                // 일정 시간이 지나도 원래대로 초기화 되도록
                EndOfAttack();
            }
        }
    }

    public override void Damage(int _damage, bool isCrit = false)
    {
        base.Damage(_damage, isCrit);
    }




    public override void Think()
    {
        // 오토 모드인지 검사
        if (isAutoFlag == false)
            return; 

        // 자동 조작 상태라면 AI처럼 행동
        // 1. 타겟 감지(1. 보스 -> 엘리트 -> 일반 순으로 가장 가까운 적)
        if(GameManager.MyInstance != null)
        {
            var wc = GameManager.MyInstance.GetNearEnemyForEnemyTeam(transform.position);
            if( wc != null)
            {
                targetPos = wc.transform.position;
                fieldOfView.target = wc.transform;
            }
        }

        // 2. 타겟 감지하면 거리 계산
        //todo 사용할 스킬별 범위 값만큼 타겟 까지 이동하기 

        // 3. 거리 계산 후 거리에 따른 행동 지시 
    }


    // 수동 상태일 때 이동로직 
    public void AutoMove()
    {
        if (fieldOfView == null)
            return; 

        MyAgent.speed = player.MyStat.speed;
        // 목적지 값에 도달했다면 초기화
        if (Vector3.Distance(transform.position, targetPos) <= 1.0f)
        {
            myState = PlayerState.Idle;
            targetPos = Vector3.zero;
        }

        // 타겟이 설정되어 있다면? 
        if (fieldOfView.target != null && fieldOfView.MeeleAttackRangeView(MyAgent) == false)
        {
            // 타겟 위치를 목표로 설정하고 움직인다. 
            MyAgent.SetDestination(fieldOfView.target.position);
        }
        // 타겟이 사거리 내에 있다면 공격 스테이트로 
        else if(fieldOfView.target != null && fieldOfView.MeeleAttackRangeView(MyAgent) == true)
        {
            isAttacking = false;
            myState = PlayerState.Attack;// ChangeState(PlayerState.Move);
        }
        // 타겟이 없다면 주위에 랜덤한 곳으로 이동시켜본다.
        else if(targetPos == Vector3.zero)
        {
            var destination = new Vector3(Random.Range(-5, 6), 0, Random.Range(-5, 6));
            targetPos = transform.localPosition + destination;
            MyAgent.SetDestination(targetPos);
        }
    }

    // 자동 상태일 때 공격 기능 
    public bool AutoAttack()
    {
        if (MyPlayer == null || skillAction == null) return false;

        // 자신의 스킬들을 검사해서 사용가능한 스킬 부터 사용 
        var skillSlotNumber = MyPlayer.GetSkillSlotNumber();
        // 스킬을 사용한게 있나?
        if(skillSlotNumber == SkillSlotNumber.NONE)
        {
            // 일반 공격을 사용하거나 다른 스테이트로 넘긴다.
            return false;
        }
        else if (skillSlotNumber != SkillSlotNumber.NONE)
        {
            // 다음 스킬은 사용하지 않은 스킬 슬롯의 스킬
            // 쿨타임이 적고 마나를 소비 하지 않은 스킬만 쓸 수 있음.
            if(usedSkillSlotNumber == SkillSlotNumber.NONE)
            {
                usedSkillSlotNumber = skillSlotNumber;
            }
           
            // 찾아낸 스킬 값을 이용하여 스킬 사용
            UseSkill(MyPlayer.GetEquipedSkill(usedSkillSlotNumber));
            
            // 사용 후 다음 스킬 슬롯으로 설정
            usedSkillSlotNumber = MyPlayer.GetAnotherSkillSlotNumber(usedSkillSlotNumber);
            return true;
        }

        return false;
    }
}
