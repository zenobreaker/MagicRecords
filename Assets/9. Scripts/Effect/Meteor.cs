using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float maximumlScaleSize; // 미티어가 최대로 커질 값 
    public float minimumlScaleSize; // 최소 크기값 
    public float scaleUpDelayTime; // 크기가 커진다면 걸리는 시간 값 
    public float dropSpeed;  // 낙하 속도 
    public GameObject bombEffect;   // 충돌 시, 나타나는 폭발 이펙트 오브젝트 
    public Rigidbody rigid;

    public Vector3 dropPoint;

    public float effectTimerValue = 0.5f;
    public float effectTimer = 0;

    float timer;
    Character playeOwn;
    Transform ownTrasnform;
    public LayerMask targetLayer;

    bool isExcute = false;

    private void Awake()
    {
        // 최소 크기로 변경 
        transform.localScale = Vector3.one * minimumlScaleSize;
        rigid = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        this.transform.localScale = Vector3.one * minimumlScaleSize;
    
    }

    void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // 한 객체에 한번만 
        CancelInvoke();    // Monobehaviour에 Invoke가 있다면 
    }

    public void SetOwn(Character own, Transform tr)
    {
        playeOwn = own;
        ownTrasnform = tr; 
    }

    public void StartMeteor()
    {
        isExcute = true; 
    }

    void Update()
    {
        if (isExcute == false)
            return; 


        // 1. 일정 시간 동안 최대 크기 까지 커진다. 
        if (transform.localScale.x < maximumlScaleSize)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / scaleUpDelayTime);
            float scaleFactor = Mathf.Lerp(0, maximumlScaleSize, progress);

            transform.localScale = Vector3.one * scaleFactor; 

            if (progress >= 1.0f)
            {
                transform.localScale = Vector3.one * maximumlScaleSize;
                timer = 0;
            }
        }
        // 2. 일정 시간이 지나면 낙하 지점으로 떨어진다. 
        else if(transform.position.y > dropPoint.y && rigid != null)
        {
            var dir = dropPoint - transform.position ;

            dir = dir.normalized * dropSpeed * Time.deltaTime;
            rigid.MovePosition(transform.position + dir);
        }
        // 3. 낙하 지점에 다달앗다면 이펙트를 발생
        else if(transform.position.y <= dropPoint.y && gameObject.activeInHierarchy == true)
        {
            var be = Instantiate(bombEffect);
            if( be != null )
            {
                var aa = be.GetComponent<AttackArea>();
                // 데미지 세팅 
                aa.SetAttackInfo(playeOwn, ownTrasnform, 2.0f);
                aa.SetLayer(targetLayer);
                aa.SetOnEnableCollider();
                aa.disableTime = 1.2f;

            }
            var beSize = maximumlScaleSize * 0.5f; 
            be.transform.localScale = new Vector3(beSize, beSize, beSize);
            be.transform.position = dropPoint;


            this.gameObject.SetActive(false);
        }

    }
}
