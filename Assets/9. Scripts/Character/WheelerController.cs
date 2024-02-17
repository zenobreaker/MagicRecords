using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.TextCore.Text;
using static UnityEngine.Networking.UnityWebRequest;
using static UnityEngine.UI.GridLayoutGroup;

public enum PlayType
{
    None,
    Playerable,
}

public enum TeamTag
{
    NONE,
    TEAM,
    ENEMY,
}

public enum PlayerState
{
    Idle = 0,
    Move,
    Attack,
    Chase,
    Follow,
}


public abstract class WheelerController : MonoBehaviour, IDamage
{
    protected LayerMask m_LayerMask;
    protected StateMachine stateMachine;    // 상태 변환기 
    public PlayType myPlayType;
    public TeamTag teamTag;
    public bool isLeader = false;   // 어떠한 조직에서 대장인지 값

    public static readonly int DEFAULT_CHAIN_POINT_VALUE = 1; // 체인 포인트 획득시 포인트값

    [SerializeField] protected Rigidbody m_rigid;
    [SerializeField] protected NavMeshAgent m_agent;
    // 필요한 컴포넌트
    [SerializeField] protected Animator anim = null;
    //[SerializeField] protected ConditionController theCondition = null; // 상태 체크용

    public string m_charName;
    public PlayType MyPlayType { get => myPlayType; set => myPlayType = value; }
    public Rigidbody MyRigid { get { return m_rigid; } }
    public NavMeshAgent MyAgent { get { return m_agent; } }
    public FieldOfViewAngle fieldOfView;
    public PlayerState myState; // 자신의 상태 
    public float delayTime;
    public float currentDelayTime;

    public bool isWalking = false; // 걷는지 안 걷는지 판별
    [HideInInspector] public bool isRunning = false; // 뛰는지 판별
    [HideInInspector] public bool isChasing = false; // 추젹 중인지 판별
    public bool isAttacking = false; // 공격중인지 판별
    
    [HideInInspector] public bool isDead = false; // 죽었는 판별
    public float recoveryTime;

    // 상태 변수
    public bool isTest = false;
    // 콤보 관련 변수들 
    public ComboState current_Combo_State; // 콤보 스테이트 
    public ComboState max_Combo_State;  // 최대 콤보값
    public float default_Combo_Timer = 1.0f; // 기본 콤보 초기화 시간
    public float current_Combo_Timer;   // 현재 콤보 시간을 책정
    public bool activateTimerToReset;  // 콤보 시간이 리셋 확인

    [SerializeField] protected SkillAction skillAction = null;

    // 적용중인 버프들
    public List<BuffDebuff> buffDebuffs = new List<BuffDebuff>();

    // 체력 / 마나 재생 관련 코루틴 
    Coroutine recoveryHealth;
    Coroutine recoveryMana;
    WaitForSeconds waitForSecondsRecovery = new  WaitForSeconds(1.0f);

    [Header("캐릭터 수동 및 자동 조작")]
    public bool isAutoFlag = false;

    protected Character player;
    public Character MyPlayer
    {
        get { return player; }
        set
        {
            player = value;
        }
    }

    // 패턴을 결정하는 추상 메소드 
    public abstract void Think();
    public abstract void Attack();

    // 공격 종료 - 관련한 변수를 되돌려놓아준다.
    public void EndOfAttack()
    {
        isAttacking = false;
        myState = PlayerState.Idle;
    }

    public abstract void Search();
    public abstract void Move();
    public abstract void Wait();

    public virtual void InitPattren()
    {

    }

    public virtual void ChangeState(PlayerState playerState)
    {
        myState = playerState;
        if (stateMachine != null)
            stateMachine.ChangeState(stateMachine.States[myState]);
    }

    // 공격한 대상의 정보를 저장하는 함수
    public void SetTargetInfo(Transform target)
    {
        if (fieldOfView == null) return;

        fieldOfView.target = target;
    }

    // 데미지 계산처리 하는 함수 
    public void DealDamage(Character attackOwn, Transform attackTrasnform = null, float damageRate = 1.0f)
    {
        if (attackOwn == null || attackOwn.MyStat == null || MyPlayer.MyStat == null) return;

        // 패시브 효과 적용
        attackOwn.ApplyPassiveSkillEffectsByWC(this);

        float damage = attackOwn.MyStat.totalATK * damageRate;
        float critRate = attackOwn.MyStat.totalCritRate;
        float critDamage = attackOwn.MyStat.totalCritDmg;
        // 공격한 대상의 크리티컬 확률 계산
        float chance = Random.Range(0.0f, 1.0f);
        bool isCrit = false;
        // 크리티컬 확률 
        if (chance <= critRate)
        {
            isCrit = true;
            // 데미지 공식 
            // 공격력 * 계수 * 크리티컬 데미지 
            damage = damage * critDamage;
        }

        // 잃은 체력 비례 데미지 추가
        float additionalDamage = damage * MyPlayer.MyStat.passiveAdditionalLostHealthRate;

        // 데미지로 내 체력을 깎는 로직
        // 방어력으로 상회 
        // 방어력 공식  = 방어상수(100)  / (방어력 + 방어상수{100})
        float damageReduction = 1.0f; // 데미지 감소율 변수
        if (player.MyStat != null)
        {
            float myDefense = player.MyStat.totalDEF;
            const float DEFENSE = 100;
            damageReduction = DEFENSE / (myDefense + DEFENSE);
        }

        // 총 데미지 계산
        damage = damage * (damageReduction) + additionalDamage;
        MyPlayer.MyStat.passiveAdditionalLostHealthRate = 0; // 계산 후 0 처리

        // 공격자의 CP 증가
        attackOwn.IncreaseCP(DEFAULT_CHAIN_POINT_VALUE);

        Vector3 pos = Vector3.zero;
        if (attackTrasnform != null)
        {
            pos = attackTrasnform.position;
            // 공격자의 정보 세팅 정보전달
            SetTargetInfo(attackTrasnform);
        }

        Damage((int)damage, pos, isCrit);


    }
    public void CheckDead()
    {
        if (player.isDead == true && isTest == false)
        {
            Debug.Log("이 플레이어는 죽었음니다 : " + player.MyID);
            if (GameManager.MyInstance != null)
            {
                // 게임매니저에게 점수 하락을 전달 하자 
                if (teamTag == TeamTag.TEAM)
                {
                    // 훈련장이 아니면 죽음 처리 
                    if (GameManager.MyInstance.isTest == false)
                    {
                        isDead = true;
                        GameManager.MyInstance.ChanagePlayerTeamCount(1);
                        gameObject.SetActive(false);
                        // 조종하는 캐릭터 교체하기
                        if (isLeader == true)
                            GameManager.MyInstance.ChangeControlWheelerByFirstWheeler();
                    }
                }
                else if (teamTag == TeamTag.ENEMY)
                {
                    isDead = true;
                }
            }
        }
    }

    public virtual void Damage(int damage, bool isCrit = false)
    {
        MyPlayer.Damage(damage, isCrit);
        // 데미지 폰트 띄우기 
        if (UIManager.instance != null)
        {
            UIManager.instance.CreateFloatingText(this.gameObject.transform.position,
                damage.ToString(), isCrit);
        }

        Debug.Log("플레이어 방어력 : " + player.MyStat.totalDEF);
        Debug.Log("데미지 입음 현재 체력 : " + player.MyCurrentHP);



    }

    public virtual void Damage(int _damage, Vector3 _targetPos, bool isCrit = false)
    {
        Damage(_damage, isCrit);

        CheckDead();
    }

    public void DotDamage(int damage)
    {
        MyPlayer.Damage(damage, false);
        // 데미지 폰트 띄우기 
        if (UIManager.instance != null)
        {
            UIManager.instance.CreateFloatingText(this.gameObject.transform.position,
                damage.ToString(), false);
        }

        CheckDead();
    }


    public abstract void StateAnimaiton();

    // 공격 속도 관련 변수 애니메이터에 세팅
    public virtual void SetAttackSpeedToAnim()
    {
        if (player == null || anim == null)
            return;

        anim.SetFloat("AttackSpeed", 1.0f * player.MyStat.totalASPD);
    }

    public void UseSkill(ActiveSkill _targetSkill, int power = 0)
    {
        SetAttackSpeedToAnim();

        if (skillAction.ActionSkill(_targetSkill, player, power) == true)
        {
            Debug.Log("스킬 사용 " + _targetSkill.CallSkillName + " = " + player.MyCurrentMP);
            isAttacking = true;
            current_Combo_State = ComboState.Skill1;

            skillAction.SetSkillFinishCallback(() =>
            {
                current_Combo_State = ComboState.NONE;
                isAttacking = false;
                myState = PlayerState.Idle;
                //activateTimerToReset = true;    // 콤보 타이머 활성화
                //current_Combo_Timer = default_Combo_Timer;  
                // 콤보 타이머가 디폴트 값을 대입해서 계산하도록 함.
                //current_Combo_Timer = _targetSkill.MyCastTime;
            });
        }
    }

    public void AddBuffDebuff(BuffDebuff buffDebuff)
    {
        if (buffDebuff == null || buffDebuff.specialOption == null) return;

        // 같은 타입에 버프는 여러 개 넣어지지 않고 지속시간이나 수치만 갱신시킨다.
        var existBuff = buffDebuffs.FirstOrDefault(buff =>
        buff.buffType == buffDebuff.buffType && buff.buffName == buffDebuff.buffName);

        // 해당 버프만 갱신
        if (existBuff != null && existBuff.specialOption != null)
        {
            // 해당 버프 기록 갱신
            if (existBuff.isRefresh == true)
            {
                existBuff.specialOption.coolTime = buffDebuff.specialOption.coolTime;
                existBuff.specialOption.value = buffDebuff.specialOption.value;
                existBuff.buffCount++;
            }
        }
        else
        {
            buffDebuffs.Add(buffDebuff);
            ApplyBuffDebuff(buffDebuff);
        }

    }

    public void ApplyBuffDebuff(BuffDebuff buffDebuff)
    {
        StartCoroutine(ManageBuffTimer(buffDebuff));

    }

    IEnumerator ManageBuffDebuff()
    {
        if (buffDebuffs == null || buffDebuffs.Count <= 0)
            yield return null;

        foreach (var buffDebuff in buffDebuffs)
        {

        }

    }

    IEnumerator ManageBuffTimer(BuffDebuff buffDebuff)
    {
        if (buffDebuff == null || buffDebuff.specialOption == null)
            yield return null;

        // todo 버프 하나에 코루틴 하나를 이용한다. 메모리를 많이 먹을 수 있으니
        // 버프가 늘어나면 리스트를 전부 돌아서 처리하도록 수정하는 방향으로 작업해보기 
        float timer = 0;
        // 버프 실행
        buffDebuff.Activation(this);
        while (buffDebuff.specialOption.coolTime > 0)
        {
            buffDebuff.specialOption.coolTime -= Time.smoothDeltaTime;
            timer += Time.smoothDeltaTime;
            // 버프 기능을 실행하는 경우 체크 
            if (buffDebuff.buffCallFlag == true &&
                buffDebuff.buffCallTime <= timer)
            {
                timer = 0;
                // 버프 기능 발현
                buffDebuff.Excute(this);
            }

            yield return new WaitForFixedUpdate();
        }

        RemoveBuffDebuff(buffDebuff);
    }

    public void RemoveBuffDebuff(BuffDebuff buffDebuff)
    {
        if (buffDebuff == null || buffDebuff.specialOption == null) return;

        buffDebuffs.Remove(buffDebuff);
        buffDebuff.Deactivation(this);
    }



    public void InitRecoveryStat()
    {
        if (player == null || player.MyStat == null)
            return;

        Debug.Log("재생관련 스탯 동작 실시");

        recoveryHealth = StartCoroutine(RecoveryHP());
        recoveryMana = StartCoroutine(RecoveryMP());
    }

    public void EndRecovery()
    {
        StopAllCoroutines();
    }

    // 체력 자동회복
    public IEnumerator RecoveryHP()
    {
        if (player == null) yield return null; 
        
        while(player.isDead == false)
        {
            yield return waitForSecondsRecovery;

            if (player.MyCurrentHP <= player.MyMaxHP)
            {
                player.MyCurrentHP += 1 + (1 * player.MyStat.totalHPR);
            }
        }

    }

    // 마나 자동회복 
    public IEnumerator RecoveryMP()
    {
        if (player == null) yield return null;

        while (player.isDead == false)
        {
            yield return waitForSecondsRecovery;

            if (player.MyCurrentMP <= player.MyStat.totalMP)
            {
                player.MyCurrentMP += 1 + (1 * player.MyStat.totalMPR);
            }
        }
    }

    // 이동 제어 관련

    // 타겟과의 거리가 일정 거리라면 그 자리에서 바로 멈춘다. 
    public void AutoStopPos()
    {
        float dist = Vector3.Distance(transform.position, MyAgent.destination);

        // 대상과 일정 거리라면 멈추도록. 
        if (dist <= 0.5f)
        {
            MyAgent.velocity = Vector3.zero;
            MyAgent.isStopped = true;
        }
        else
        {
            MyAgent.isStopped = false;
        }
    }
}
