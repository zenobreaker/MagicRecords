using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 한쪽 방향만 늘어나게 하는 캡슐 콜라이더 기능 시간과 속도에 따라 길이가 늘어나는 시간 차이가 있다.
/// </summary>
public class LaserCollider : MonoBehaviour
{
    public int skillDamage;
    public int hitCount;

    public bool isContiued; // 연속공격인가?

    public float recoveryTime;
    private bool isAction = false;

    public float maxLength;
    public float speed; 

    public CapsuleCollider capsuleCollider;
    public ParticleSystem ps_effctTarget;

    Coroutine coroutine;

    RaycastHit[] hits;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (capsuleCollider != null && ps_effctTarget != null)
        {
            var main = ps_effctTarget.main;
            // 속도값 계산
            speed = maxLength / ps_effctTarget.sizeOverLifetime.zMultiplier;
            // 캡슐 콜라이더 위치 및 길이 값 초기화 
            capsuleCollider.center = Vector3.zero;
            capsuleCollider.height = 0;
            
            coroutine = StartCoroutine(CoIncreaseColliderIncrease(maxLength, ps_effctTarget.sizeOverLifetime.zMultiplier));
        }
    }

    IEnumerator CoIncreaseColliderIncrease(float _length, float lifeTime = 0)
    {
        Debug.Log("이펙트 라이프타임 "+lifeTime);
        yield return new WaitForSeconds(lifeTime);
        //float length = 0;
        
        while (capsuleCollider.height < _length)
        {
          
            // 시간과 속도 만큼 길이를 늘린다.
            capsuleCollider.height += Time.deltaTime * speed;

            // 콜라이더 위치는 총 길이의 절반 값
            if (capsuleCollider.center.z <= maxLength / 2)
            {
                capsuleCollider.center = new Vector3(0, 0, capsuleCollider.height * 0.5f);
            }

            yield return null;
        }
    }
}
