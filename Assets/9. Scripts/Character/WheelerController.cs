using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using static UnityEngine.Networking.UnityWebRequest;

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
    Chase
}


public abstract class WheelerController : MonoBehaviour, IDamage
{
    protected LayerMask m_LayerMask;
    protected StateMachine stateMachine;    // 상태 변환기 
    public PlayType myPlayType;
    public TeamTag teamTag;

    [SerializeField] protected Rigidbody m_rigid;
    [SerializeField] protected NavMeshAgent m_agent;
    [SerializeField] protected ConditionController theCondition = null; // 상태 체크용

    public string m_charName;
    public PlayType MyPlayType{get => myPlayType;}
    public Rigidbody MyRigid { get { return m_rigid; } }
    public NavMeshAgent MyAgent { get { return m_agent; } }
    public FieldOfViewAngle fieldOfView; 
    public PlayerState myState; // 자신의 상태 
    public float idleTime;

    public bool isWalking = false; // 걷는지 안 걷는지 판별
    [HideInInspector] public bool isRunning = false; // 뛰는지 판별
    [HideInInspector] public bool isChasing = false; // 추젹 중인지 판별
    [HideInInspector] public bool isAttacking = false; // 공격중인지 판별
    [HideInInspector] public bool isDead = false; // 죽었는지 판별


    // 콤보 관련 변수들 
    public ComboState current_Combo_State; // 콤보 스테이트 
    public float default_Combo_Timer = 0.5f; // 기본 콤보 초기화 시간
    public float current_Combo_Timer;   // 현재 콤보 시간을 책정
    public bool activateTimerToReset;  // 콤보 시간이 리셋 확인

    [SerializeField] protected SkillAction skillAction = null;

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
    public abstract void Move();

    public abstract void Wait();


    // 공격한 대상의 정보를 저장하는 함수
    public void SetTargetInfo(Transform target)
    {
        if (fieldOfView == null) return;

        fieldOfView.target = target;
    }

    // 데미지 계산처리 하는 함수 
    public void DealDamage(Character attackOwn, Transform attackTrasnform = null, float damageRate = 1.0f)
    {
        if (attackOwn == null || attackOwn.MyStat == null) return;
        
        // 패시브 효과 적용
        attackOwn.ApplyPassiveSkillEffects(MyPlayer);

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

        damage = damage * (damageReduction);
        // 총계산

        Vector3 pos = Vector3.zero;
        if (attackTrasnform != null)
        {
            pos = attackTrasnform.position;
            // 공격자의 정보 세팅 정보전달
            SetTargetInfo(attackTrasnform);
        }
        
        Damage((int)damage, pos, isCrit);

       
    }

    public virtual void Damage(int damage, bool isCrit = false)
    {
      
        // 컨디션 상태에 따라 효과 관리
        if (theCondition != null)
            theCondition.AbnormalCondition();
        // 데미지에 따른 체력 감소해보기 
        player.MyCurrentHP -=  damage;

        // 데미지 폰트 띄우기 
        if (UIManager.instance != null)
        {
            UIManager.instance.CreateFloatingText(this.gameObject.transform.position,
                damage.ToString(), isCrit);
        }

        Debug.Log("플레이어 방어력 : " + player.MyStat.totalDEF);
        Debug.Log("데미지 입음 현재 체력 : " + player.MyCurrentHP);
        if (player.MyCurrentHP <= 0)
        {

            Debug.Log("이 플레이어는 죽었음니다 : " + player.MyID);
            isDead = true;
            if (GameManager.MyInstance != null)
            {
                // 게임매니저에게 점수 하락을 전달 하자 
                if (teamTag == TeamTag.TEAM)
                {
                    GameManager.MyInstance.ChanagePlayerTeamCount(1);
                }
                else if(teamTag == TeamTag.ENEMY)
                {

                }
            }
        }
        
        
    }

    public virtual void Damage(int _damage, Vector3 _targetPos, bool isCrit = false)
    {
        Damage(_damage, isCrit);       
    }

    public virtual void SetBuff(BuffStock _buffStcok)
    {
        if(theCondition != null && _buffStcok != null)
        {
            theCondition.AddBuff(_buffStcok);
        }
    }

    public abstract void StateAnimaiton();

    public void UseSkill(ActiveSkill _targetSkill, int power = 0)
    {
        if (skillAction.ActionSkill(_targetSkill, player, power) == true)
        {
            Debug.Log("스킬 사용 " + _targetSkill.CallSkillName + " = " + player.MyCurrentMP);
            isAttacking = true;
            current_Combo_State = ComboState.Skill1;

            skillAction.SetSkillFinishCallback(() =>
            {
                current_Combo_State = ComboState.NONE;
                isAttacking = false;
                //activateTimerToReset = true;    // 콤보 타이머 활성화
                //current_Combo_Timer = default_Combo_Timer;  
                // 콤보 타이머가 디폴트 값을 대입해서 계산하도록 함.
                //current_Combo_Timer = _targetSkill.MyCastTime;
            });
        }
    }
  
}
