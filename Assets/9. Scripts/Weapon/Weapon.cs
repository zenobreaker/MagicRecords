using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("타겟 레이어")]
    public LayerMask targetLayer;
    
    [Header("공격 범위")]
    public AttackArea attackArea;

    [Header("총구")]
    public GameObject go_Muzzle;

    [Header("총 연사속도 조정")]
    public float fireRate;

    [Header("총구 섬광")]
    public ParticleSystem ps_MuzzleFlash;

    [Header("총알 프리팹")]
    public GameObject go_Bullet_Prefab;

    [Header("총알 스피드")]
    public float speed;

    [Header("총알 발사 사운드")]
    public string sound_Fire;

    [Header("총알 발사 사운드2")]
    public string sound_Fire2;


    public void SetTargetLayer(LayerMask layer)
    {
        targetLayer = layer;
        if(attackArea != null)
            attackArea.SetLayer(targetLayer);
    }

}
