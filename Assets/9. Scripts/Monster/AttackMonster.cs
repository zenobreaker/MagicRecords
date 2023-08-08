using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


[System.Serializable]
public class MonsterAttackStat
{
    public bool attackPattern;
    public float attackCoolTime;
}

public class AttackMonster : MonsterBase
{
    [Header("몬스터 공격범위 및 이펙트 ")]
    public AttackArea[] attackRanges;
    public ParticleSystem[] attackEffect;
    public float addRange;   // 랭크의 따른 추가 범위

    [Header("몬스터 패턴별 공격속도 파라미터값")]
    public float[] pattren_parameter_attackspeed_array;

    [SerializeField] float chaseTime = 3f;
    protected bool isComplete = false;

    [SerializeField]
    protected MonsterAttackStat[] monsterAttackStat;
    protected int currentPattern = 0;
    public int MaxPattern;
    public float delayTime; 
    protected override void Start()
    {
        base.Start();
        
        // 공격범위 초기화 
        for (int i = 0; i < attackRanges.Length; i++)
        {
            attackRanges[i].SetDisableCollider();
        }

        if(player.MyStat == null)
        {
            Debug.Log("스탯 데이터 없음");
            return; 
        }

        switch(player.MyStat.myGrade)
        {
            case MonsterGrade.NORMAL:
                addRange = 1f;
                break;
            case MonsterGrade.ELITE:
                addRange = 1.5f;
                break;
            case MonsterGrade.BOSS:
                addRange = 2f;
                break;
        }
    }

    protected override void Update()
    {
        base.Update();
        //if (!isDead && theCondition.curCondition == ConditionController.Condition.NONE)
        //{
        //    if (isAction)
        //    {
               
        //        switch (currentState)
        //        {
        //            case state.CHASE:
        //                Chase();
        //                break;
        //            case state.ATTACK:
        //                Attack();
        //                break; 
        //        }
        //    }

        //    if (!isComplete)
        //        RandomPattern();
                
        //    Think();
        //}

        
    }
    protected override void ChangeState()
    {
        base.ChangeState();
    }

    protected virtual void RandomPattern()
    {
        if(MaxPattern == 0)
        {
            MaxPattern = monsterAttackStat.Length;
            pattren_parameter_attackspeed_array = new float[MaxPattern];
        }

        currentPattern = Random.Range(0, MaxPattern);
        Debug.Log("랜덤에서 나온 공격 패턴 : " + currentPattern);

    }

    public override void Think()
    {
        if(isComplete == false)
            RandomPattern();

        if (!monsterAttackStat[currentPattern].attackPattern)
        {
            isComplete = true;
        }

        /*if (theViewAngle.View() || theViewAngle.target != null)
        {
            destination = theViewAngle.target.position;
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= rangeOfEnemy)
            {
                //if (theViewAngle.target != null)
                    
                //else
                //    destination = theViewAngle.GetTargetPos();

                currentState = state.CHASE;

                if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= baseAttackRange && !isAttacking)
                {
                    currentState = state.ATTACK;
                }
            }
        }
        else if (!theViewAngle.View() && isChasing)
        {
            ResetBehaviour();
            currentState = state.IDLE;
        }
       */
    }

    public void SetTarget(Transform p_Target)
    {
        fieldOfView.target = p_Target;
    }

  
    protected void Chase()
    {
        if (!isChasing)
        {
            isChasing = true;
            //isAttacking = false;
            //currentTime = chaseTime;

            if(Vector3.Distance(transform.position, fieldOfView.GetTargetPos())  > baseAttackRange)
                isWalking = true;
            else
                isWalking = false;

            //anim.SetBool("Walking", isWalking);
           // Debug.Log("추격");
       
        }
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            nav.isStopped = false;
            nav.velocity = Vector3.zero;

            // 공격할 수단이 있는지 검사
            bool isExistPattern = false; 
            foreach( var attackPattern in monsterAttackStat)
            {
                if(attackPattern.attackPattern == false)
                {

                    isExistPattern = true; 
                    break;
                }

            }
            
            // 하나도 없다면 스테이트를 바꿔 대기시킨다.
            if (isExistPattern == false)
            {
                myState = PlayerState.Idle;
                return;
            }

            // 공격 코루틴 실행
            if (monsterAttackStat[currentPattern].attackPattern == false)
            {
                isAttacking = true;
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    public override void Move()
    {
        base.Move();
    }

    protected IEnumerator AttackCoroutine()
    {
        Debug.Log("공격");
       
        nav.ResetPath();
        //currentChaseTime = ChaseTime;
        //yield return new WaitForSeconds(0.5f);

        transform.LookAt(fieldOfView.target); // 플레이어를 바라보게 함

        if (monsterAttackStat[currentPattern] != null)
        {
            monsterAttackStat[currentPattern].attackPattern = true;
        }
        SetAction(currentPattern);

        StartCoroutine(CoolDownAttackDelay(currentPattern));
        yield return new WaitForSeconds(player.MyStat.totalASPD);
        isAttacking = false;
        isComplete = false;

        // 행위가 끝났다면 다음 진행을 위한 스테이트 변경 
        myState = PlayerState.Idle;
        //isAction = false;
    }

    // 스테이트별 움직임 
    public override void StateAnimaiton()
    {

        if (myState == PlayerState.Move || myState == PlayerState.Chase)
        {
            anim.SetBool("Walking", true);
        }
        else if (myState == PlayerState.Idle)
        {
            anim.SetBool("Walking", false);
        }
        else
        {
            anim.SetBool("Walking", false);
        }

    }

    protected virtual void SetAction(int p_currentPattern) 
    {
        Debug.Log("패턴 수 :  " + p_currentPattern + "만들어졋나 : " + attackRanges.Length);
        if (attackRanges.Length > 0 &&
            p_currentPattern < attackRanges.Length &&
            attackRanges[p_currentPattern] != null)
        {
            attackRanges[p_currentPattern].attackOwn = player;
            attackRanges[p_currentPattern].SetPower(player.MyTotalAttack);
        }

        anim.SetFloat("AttackSpeed", 1.0f);
        if (pattren_parameter_attackspeed_array.Length > 0 &&
            pattren_parameter_attackspeed_array.Length < p_currentPattern &&
            p_currentPattern > 0)
        {

            int currentIndex = p_currentPattern - 1; 
            var value = pattren_parameter_attackspeed_array[currentIndex];
            anim.SetFloat("AttackSpeed", value);
        }

    }

    protected IEnumerator CoolDownAttackDelay(int p_PatternNum)
    {
        Debug.Log("쿨타임 시작" + p_PatternNum);
        yield return new WaitForSeconds(monsterAttackStat[p_PatternNum].attackCoolTime);
        Debug.Log("쿨타임 끝" + p_PatternNum);
        monsterAttackStat[p_PatternNum].attackPattern = false;
    }


    public override void Damaged(int _dmg, Vector3 _targetPos)
    {
        base.Damaged(_dmg, _targetPos);


        if (!isDead)
        {
            MyRigid.velocity = Vector3.zero;
            ResetBehaviour();
            myState = PlayerState.Chase;
            stateMachine.ChangeState(stateMachine.States[myState]);
            //theViewAngle.target.position = _targetPos;
            //destination = _targetPos;
            transform.localPosition -= (_targetPos - transform.position).normalized;
            transform.LookAt(_targetPos - transform.position.normalized);
            // PlaySE(sound_Hurt);
            anim.SetTrigger("Hurt");
        }
    }


    protected void DangerMarkerShoot(int p_MakerNum, Vector3 startPos, Quaternion rotate, float distance)
    {

        //dangerLine[p_MakerNum].GetComponent<DangerLine>().EndPosition = endPosition;// 경고선 컴포넌트 
        if(dangerLine.Length <= 0 && 
            p_MakerNum < dangerLine.Length &&
            dangerLine[p_MakerNum] == null)
        {
            return; 
        }
        var dl = Instantiate(dangerLine[p_MakerNum], startPos, rotate);
        dl.gameObject.SetActive(true); 
        dl.CreateGuideSinlgeLine(startPos, rotate, distance);
    }

    protected void DangerMarkerShoot(int p_MakerNum, Vector3 startPos, Vector3 endPos)
    {
        Vector3 dir = endPos - startPos; 

        Quaternion rot = Quaternion.LookRotation(dir);

        float distance = Vector3.Distance(endPos, startPos);

        DangerMarkerShoot(p_MakerNum, startPos, rot, distance);
    }


    // 몬스터가 특정패턴을 진행하면 나타나는 가이드라인을 만드는 함수 
    public void CreateGuideLine(int p_MakerNum, float _destTime, float p_distance)
    {
        Vector3 startPosition = new Vector3(transform.position.x, 0.1f, transform.position.z);
        Vector3 endPosition = startPosition + (transform.forward * p_distance);
    }

    // 공격 오브젝트 활성화 
    public virtual void AttackEableObject(bool isOn)
    {
        
    }

}
