using UnityEngine;

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


    // 필요한 컴포넌트 
    [SerializeField]
    private WeaponController theWeaponController = null;
    [SerializeField] WheelController wheelController = null;
   
    [SerializeField] GameObject go_DashEffect = null;

    public bool IsMove = false;
    private bool canDash;
    public bool isDash = false;
    public bool isTouch = false;

    GameObject dashEffect;


    public enum State
    {
        Idle = 0,
        Walk,
        Attack
    }

    public State currentState = State.Idle;

    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        stateMachine = new StateMachine();
        stateMachine.States.Add(PlayerState.Idle, new IdleState(this));
        stateMachine.States.Add(PlayerState.Move, new MoveState(this));
        stateMachine.States.Add(PlayerState.Attack, new AttackState(this));

        //    myAnimator = GetComponent<Animator>();
        current_Combo_State = ComboState.NONE;
        current_Combo_Timer = default_Combo_Timer;

        //myAnimator.SetFloat("AttackSpeed", 1);
        if (player != null)
        {
            speed = player.MyStat.speed;

            if(theWeaponController != null)
            {
                theWeaponController.SetWeaponOwn(player);
            }
        }

        if(skillAction != null)
        {
            skillAction.skillOwn = player;
        }

        dashTime = startDashTime;
        dashEffect = Instantiate(go_DashEffect, this.transform.position, Quaternion.identity);
        dashEffect.transform.SetParent(this.transform);
        dashEffect.SetActive(false);

        // 스테이터스 컨트롤러 동작 
        //InitStatusConteroller();
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

        direction = direction.normalized * speed * Time.deltaTime;
        m_rigid.MovePosition(this.transform.position + direction);
        //myRigid.position += direction.normalized * speed * Time.deltaTime;
        wheelController.RollingWheel(IsMove, 10);
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
        switch (myState)
        {
            case PlayerState.Idle:
                {
                    myAnimator.SetBool("Moving", false);
                    break;
                }
            case PlayerState.Move:
                {
                    myAnimator.SetBool("Moving", true);
                    break;
                }
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
            // 공격 스테이트로 변경
            ChangeState(PlayerState.Attack);

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

            if (player != null)
            {
                theWeaponController.SetDamageAndCrit(damageRate, 
                    player.MyTotalAttack, player.MyStat.totalCritRate, player.MyStat.totalCritDmg);
            }

            theWeaponController.TryFire(current_Combo_State);
        }
    }

    private void GetInput()
    {
        if (isTouch)
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
            //myRigid.MoveRotation(newRotation);
        }
        else
        {
            IsMove = false;
            //currentState = State.Idle;
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
        ChangeState(inputState);
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

        ChangeState(inputState);
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
            currentState = State.Walk;
            dashDir = direction;
            dashEffect.SetActive(true);
            speed = speed * 2;
            wheelController.RollingWheel(isDash, 140);

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
                isAttacking = false; 
            }
        }
    }

    public override void Damage(int _damage, bool isCrit = false)
    {
        base.Damage(_damage, isCrit);
    }




    public override void Think()
    {
        
    }
}
