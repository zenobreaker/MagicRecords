using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : AttackMonster
{

    protected override void RandomPattern()
    {
        base.RandomPattern();

        switch (currentPattern)
        {
            case 0:
                baseAttackRange = 3.5f * addRange;
                break;
            case 1:
                baseAttackRange = 5f * addRange;
                break;
        }
    }

    protected override void SetAction(int p_currentPattern)
    {
        base.SetAction(p_currentPattern);
        
        switch (p_currentPattern)
        {
            case 0:
                StartCoroutine(NormalAttack());
                break;
            case 1:
                StartCoroutine(JellyJump());
                break;
        }
    }

  
    IEnumerator NormalAttack()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1.2f);  
         Vector3 t_Pos = transform.localPosition + (transform.forward * 1.5f );
        var clone = Instantiate(attackEffect[0], t_Pos, transform.localRotation);
        
        clone.Play();
        yield return new WaitForSeconds(0.8f);
        
        
        anim.SetFloat("AttackSpeed", 1.0f);
        Destroy(clone.gameObject);
        yield return null;
    }
    // 공격 오브젝트 활성화 
    public override void AttackEableObject(bool isOn)
    {
        if (attackRanges.Length > 0 &&
           MaxPattern != 0 &&
           attackRanges[currentPattern] != null && 
           currentPattern <= attackRanges.Length)
        {
            // AttackMonster에서 호출하니 여긴 주석
            //attackRange.GetComponent<AttackArea>().power = status.MyAttack;
            if(attackRanges[currentPattern] != null && isOn == true)
            {
                attackRanges[currentPattern].SetOnEnableCollider();         
            }
            else if(attackRanges[currentPattern] != null  && isOn  == false)
            {
                attackRanges[currentPattern].SetDisableCollider();
            }

        }
    }


    IEnumerator JellyJump()
    {
        Vector3 t_Pos = transform.localPosition + transform.forward*2;
        //anim.SetFloat("AttackSpeed", 2.0f);
        anim.SetTrigger("Attack2");

        if(attackRanges[currentPattern] != null)
        {
            attackRanges[currentPattern].SetPower( Mathf.RoundToInt(player.MyTotalAttack * 1.3f));
        }

        //rigid.AddForce( Vector3.up * 5f,ForceMode.Impulse);

        yield return new WaitForSeconds(1.1f);

        while (Vector3.SqrMagnitude(t_Pos - transform.localPosition) > 0.9 )
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, t_Pos, Time.deltaTime *8);

            // normalizedTime은 진행율이고 0과 1사이값 반복은 1에서 더해진다
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75 &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.83)
            {
                
            }
            yield return null;
        }

        anim.SetFloat("AttackSpeed", 1.0f);
        transform.localPosition = t_Pos;
        yield return null;
    }
}
