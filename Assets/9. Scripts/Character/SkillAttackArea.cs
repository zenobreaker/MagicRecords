using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillAttackArea : AttackObject
{
    [SerializeField] private ParticleSystem ps_SkillMainEffect;


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



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("대미지 줌");
            other.transform.GetComponentInParent<WheelerController>().DealDamage(AttackOwn, attackOwnTransform, damageRate);
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
                //other.transform.GetComponentInParent<WheelerController>().Damage(damage, this.transform.position);
                other.transform.GetComponent<WheelerController>().DealDamage(AttackOwn, attackOwnTransform, damageRate);
                
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
