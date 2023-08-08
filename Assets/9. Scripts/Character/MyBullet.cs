using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : Bullet
{
    enum Type {
        NORMAL,
        ICE, 
        SPIRAL, 
        BURST, 
        RAIN,  
    };
    protected float tempSpeed;

    [SerializeField] Type myType = Type.NORMAL;

    protected override void OnEnable()
    {
        base.OnEnable();
        if(myType == Type.SPIRAL && speed == 1)
            speed = tempSpeed;
        if(myType == Type.RAIN)
        {
            direction.z = 0;
            direction.y = transform.position.y * Vector3.down.y;
        }
    }



    //피격 관리 
    void OnTriggerEnter(Collider other)
    {
        int effdfectNumber = EffectChoose();
        Bounds boundsPoint = transform.GetComponent<Collider>().bounds; // 충돌한 객체의 접촉면에 대한 정보가 담긴 클래스 
        //충돌한 객체의 접촉면 정보가 collision.contacts[0]에 저장되어 있다.(가장 가까운 접촉면) 
        SoundManager.instance.PlaySE(sound_Ricochet);

        if (go_RicochetEffect != null)
        {
            var recochet = Instantiate(go_RicochetEffect, boundsPoint.max, Quaternion.LookRotation(-transform.position));

            //point 대상의 좌표 LootRotation = 특정 방향을 바라보게 하는 메소드 normal 부딪힌 객체의 접촉면 방향 

            switch (myType)
            {
                case Type.SPIRAL:
                    tempSpeed = speed;
                    speed = 1;
                    myRigid.velocity = Vector3.zero;
                    break;

                case Type.NORMAL:
                case Type.ICE:
                    if (other.transform.CompareTag("Monster")) //닿은 대상에 태그가 "Monster"라면
                    {
                        IncreaseCP();
                        if (other.transform.TryGetComponent<CharacterController>(out CharacterController component))
                        {
                            component.DealDamage(AttackOwn, attackOwnTransform);
                        }
                        // 해당 transform에 들어있는 Object컴포넌트에 Damgaed메소드를 호출하여 damge값 전달
                    }
                    if (Type.ICE == myType)
                    {
                       
                    }

                    Destroy(recochet, 0.5f);
                    this.gameObject.SetActive(false);

                    break;

                case Type.BURST:
                    Destroy(recochet, 3f);
                    this.gameObject.SetActive(false);
                    break;

                case Type.RAIN:
                    if (other.transform.CompareTag("Land") || other.transform.CompareTag("land") )
                    {
                        // 폭발 이펙트 생성 
                        var boomEffect = ObjectPooler.SpawnFromPool(
                            "BulletRain_BoomEffect", this.transform.position);
                        
                        // 스킬에 데미지값 대입
                        if(boomEffect.TryGetComponent<SkillAttackArea>(out var skillArea))
                        {

                            skillArea.enabled = true;
                            skillArea.damage = MyDamage;
                            // 콜라이더를 찾아서 콜라이더 기능을 켠다.
                            if(boomEffect.TryGetComponent<SphereCollider>(out var collider))
                            {
                                collider.enabled = true;
                            }
                        }
                        Destroy(recochet, 0.5f);
                        this.gameObject.SetActive(false);
                    }
                    break;

            }
        }

        if (other.TryGetComponent<CharacterController>(out var chararcter))
        {
            if (buff == null) return;

            if (buff.myBuff != Buff.NONE ||
                buff.myDebuff != Debuff.NONE)
            {
                chararcter.SetBuff(buff);
            }
        }
    }

    void IncreaseCP()
    {
        //if (CharStat.instance != null)
        //{
        //    CharStat.instance.currentMP += 1;
        //    if (CharStat.instance.currentCP < CharStat.instance.cp)
        //        CharStat.instance.currentCP += 1;
        //}

    }

                                                                                                                                              
}
