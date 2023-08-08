using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    // todo skillattackarea와 개념이 비슷한거같으니 공용으로 쓸 수 있도록 합치는 작업이 필요할거같다. 
    // 적들이 쓰는 공격 판정 클래스.. 
    public int power;
    public float disableTime;
    public Character attackOwn; 

    [SerializeField]
    private Status status = null;

    [SerializeField]
    private new Collider collider = null;

    [SerializeField] Animation ani = null;

    public BuffStock buff;


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
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("캐릭터 히트됨!");
            other.GetComponent<PlayerControl>().DealDamage(attackOwn);
            collider.enabled = false;

            // 디버프가 있다면 던진다. 
            if(other.TryGetComponent<CharacterController>(out var chararcter))
            {
                if (buff == null) return; 

                if (buff.myBuff != Buff.NONE ||
                    buff.myDebuff != Debuff.NONE)
                {
                    chararcter.SetBuff(buff);
                }
            }

        }
    }


    public void PlayAnim()
    {
        ani.Play();
    }

}
