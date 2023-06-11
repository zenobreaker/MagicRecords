using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [Header("총 연사속도 조정")]
    public float fireRate;

    [Header("총구 섬광")]
    public ParticleSystem ps_MuzzleFlash;

    [Header("총알 프리팹")]
    public GameObject go_Bullet_Prefab;

    [Header("총알 스피드")]
    public float speed;
}
