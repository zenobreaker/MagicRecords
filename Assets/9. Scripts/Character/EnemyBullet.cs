using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void Awake()
    {
        base.Awake();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyAttack"), LayerMask.NameToLayer("Enemy"));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyAttack"), LayerMask.NameToLayer("Enemy"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyAttack"), LayerMask.NameToLayer("EnemyAttack"), true);
    }

    private void OnTriggerEnter(Collider other)
    {
        int effectNumber = EffectChoose();
        Bounds boundsPoint = transform.GetComponent<Collider>().bounds; // 충돌한 객체의 접촉면에 대한 정보가 담긴 클래스 
                                                                    //충돌한 객체의 접촉면 정보가 collision.contacts[0]에 저장되어 있다.(가장 가까운 접촉면) 
      
        SoundManager.instance.PlaySE(sound_Ricochet);
        var clone = Instantiate(go_RicochetEffect, boundsPoint.max, Quaternion.LookRotation(-transform.position));
        //point 대상의 좌표 LootRotation = 특정 방향을 바라보게 하는 메소드 normal 부딪힌 객체의 접촉면 방향 

        Debug.Log(other.transform.name);
        if (((1 << other.gameObject.layer) & targetLayer) != 0) //닿은 대상에 태그가 "Monster"라면
        {
            if(other.TryGetComponent(out PlayerControl playerControl))
            {
                //todo 임시 소드 
                if(AttackOwn != null)
                {
                    playerControl.DealDamage(AttackOwn, attackOwnTransform, 1.0f); // 해당 transform에 들어있는 Object컴포넌트에 Damgaed메소드를 호출하여 damge값 전달
                }
                else
                {
                    playerControl.Damage(MyDamage);
                }
                
            }
            Destroy(clone, 0.5f);
            //Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
        Destroy(clone, 0.5f);
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
    }
}
