using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 공격 판정을 정의하는 클래스
public class AttackArea : AttackObject
{

    public int power;
    public float disableTime;

    [SerializeField]
    private new Collider collider = null;

    [SerializeField] Animation ani = null;

    public void SetPower(int _power)
    {
        power = _power; 
    }

    public void SetOnEnableCollider()
    {
        collider.enabled = true; 
    }
    
    public void SetDisableCollider()
    {
        collider.enabled = false;
    }


    private void OnEnable()
    {
        if (disableTime > 0)
        {
            Invoke("DisableTime", disableTime);
        }
    }

    void DisableTime()
    {
        collider.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.transform.CompareTag("Player"))
        //{
        //    Debug.Log("캐릭터 히트됨!");
        //    other.GetComponent<PlayerControl>().DealDamage(AttackOwn);
        //    collider.enabled = false;
        //}
        
        if(((1<< other.gameObject.layer ) & targetLayer) != 0)
        {
            if(other.TryGetComponent<WheelerController>(out WheelerController wheelerController))
            {
                wheelerController.DealDamage(AttackOwn);
            }
        }

    }


    public void PlayAnim()
    {
        ani.Play();
    }

}
