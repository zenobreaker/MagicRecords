using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillAttackArea : MonoBehaviour
{
    public Transform MasterTR { get; set; }

    public int skillDamage;
    public int hitCount;
    
    public bool isContiued; // 연속공격인가?
    
    public float recoveryTime;
    private bool isAction = false;
    private int curHitCount = 0;

    public delegate void Callback();
    private Callback callback;

    [SerializeField] private ParticleSystem ps_SkillMainEffect;

    public BuffStock buff;

    private void Start()
    {
        if (ps_SkillMainEffect != null)
        {
            var main = ps_SkillMainEffect.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }
    }

    private void OnEnable()
    {
        isAction = true;
       
    }

    private void OnParticleSystemStopped()
    {
        this.gameObject.SetActive(false);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("여기 검사 pt :" + this.gameObject.name);
    }


    protected void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public void SetFinishCallback(Callback _callback)
    {
        callback = _callback;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("여기 검사  :" + this.gameObject.name);
        
            Debug.Log("대미지 줌");
            other.transform.GetComponentInParent<MonsterBase>().Damage(skillDamage, this.transform.position);
            // 디버프가 있다면 던진다. 
            if (other.TryGetComponent<CharacterController>(out var chararcter))
            {
                if (buff != null)
                {
                    if (buff.myBuff != Buff.NONE ||
                        buff.myDebuff != Debuff.NONE)
                    {
                        chararcter.SetBuff(buff);
                    }
                }
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isContiued == false) return; 

        if (other.CompareTag("Monster"))
        {
            // 1회
            if (isAction)
            {
                Debug.Log("여기 검사  :" + this.gameObject.name + "연속자 : " + isContiued);
                isAction = false;

                Debug.Log("대미지 줌");
                other.transform.GetComponentInParent<MonsterBase>().Damage(skillDamage, this.transform.position);
                // 디버프가 있다면 던진다. 
                if (other.TryGetComponent<CharacterController>(out var chararcter))
                {
                    if (buff != null)
                    {
                        if (buff.myBuff != Buff.NONE ||
                            buff.myDebuff != Debuff.NONE)
                        {
                            chararcter.SetBuff(buff);
                        }
                    }
                }
              
                // 일정 주기마다 대미지를 줄 수 있도록
                StartCoroutine(AttackAreaRecovery(recoveryTime, other));
            }
        }
    }

    public IEnumerator AttackAreaRecovery(float _rTime, Collider other)
    {
        Debug.Log("콜링");
        yield return new WaitForSeconds(_rTime);
        isAction = true;
        
    }
}
