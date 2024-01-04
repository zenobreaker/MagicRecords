using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float maximumlScaleSize; // ��Ƽ� �ִ�� Ŀ�� �� 
    public float minimumlScaleSize; // �ּ� ũ�Ⱚ 
    public float scaleUpDelayTime; // ũ�Ⱑ Ŀ���ٸ� �ɸ��� �ð� �� 
    public float dropSpeed;  // ���� �ӵ� 
    public GameObject bombEffect;   // �浹 ��, ��Ÿ���� ���� ����Ʈ ������Ʈ 
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
        // �ּ� ũ��� ���� 
        transform.localScale = Vector3.one * minimumlScaleSize;
        rigid = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        this.transform.localScale = Vector3.one * minimumlScaleSize;
    
    }

    void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);    // �� ��ü�� �ѹ��� 
        CancelInvoke();    // Monobehaviour�� Invoke�� �ִٸ� 
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


        // 1. ���� �ð� ���� �ִ� ũ�� ���� Ŀ����. 
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
        // 2. ���� �ð��� ������ ���� �������� ��������. 
        else if(transform.position.y > dropPoint.y && rigid != null)
        {
            var dir = dropPoint - transform.position ;

            dir = dir.normalized * dropSpeed * Time.deltaTime;
            rigid.MovePosition(transform.position + dir);
        }
        // 3. ���� ������ �ٴ޾Ѵٸ� ����Ʈ�� �߻�
        else if(transform.position.y <= dropPoint.y && gameObject.activeInHierarchy == true)
        {
            var be = Instantiate(bombEffect);
            if( be != null )
            {
                var aa = be.GetComponent<AttackArea>();
                // ������ ���� 
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
