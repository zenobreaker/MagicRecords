using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlayType
{
    None,
    Playerable,
}

public enum PlayerState
{
    Idle = 0,
    Move,
    Attack,
    Chase
}


public abstract class CharacterController : MonoBehaviour, IDamage
{
    protected LayerMask m_LayerMask;
    protected StateMachine stateMachine;    // 상태 변환기 
    public PlayType myPlayType;

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

    public virtual void Damage(int damage)
    {
       // DamageObjectPooler.instance.GetObject(this.transform.position, damage.ToString());
    }

    public virtual void Damage(int _damage, Vector3 _targetPos)
    {
        Damage(_damage);       
    }

    public virtual void SetBuff(BuffStock _buffStcok)
    {
        if(theCondition != null && _buffStcok != null)
        {
            theCondition.AddBuff(_buffStcok);
        }
    }

    public abstract void StateAnimaiton();
    


    // 버프 관련한 수치를 전달 받아서 상위 클래스에서 재정의하여 대상에 따른 스탯을 올리도록 요구한다. 
    // 공격 관련 스탯 버프 
    public virtual void AddBuffStatAtk(int _value)
    {

    }

    // 방어 관련 스탯 버프 
    public virtual void AddBuffStatDef(int _value)
    {

    }

    // 이동속도 관련 스탯 버프
    public virtual void AddBuffStatMoveSpd(int _value)
    {

    }


    // 공격속도 관련 스탯 버프 
    public virtual void AddBuffStatAtkSpd(int _value)
    {

    }


    // 체력 관련 스탯 버프 
    public virtual void AddBuffStatHp(int _value)
    {

    }

    // 마나 관련 스탯 버프
    public virtual void AddBuffStatMp(int _value)
    {

    }
}
