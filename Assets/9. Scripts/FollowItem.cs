using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowItem : MonoBehaviour
{
   // [SerializeField] private float viewAngle = 0f; // 시야각
    public float viewDistance = 0f; // 시야 거리
    [SerializeField] private LayerMask targetMask = 0; // 타겟 마스크

    [SerializeField] private ItemPickUp itemPickUp = null;

    private Vector3 targetDirection;
    public float followSpeed = 4f;

    private void Update()
    {
        FollowTarget();
    }
   
    public void FollowTarget()
    {
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.gameObject.layer == LayerMask.NameToLayer("Player")) {
                targetDirection  = (_targetTf.position - transform.position).normalized;
                float _distance = Vector3.Distance(_targetTf.position, transform.position);

                if(_distance <= viewDistance)
                {
                    //transform.position = Vector3.Lerp(transform.position, _targetTf.position, Time.deltaTime * followSpeed);
                    transform.position = Vector3.MoveTowards(transform.position, _targetTf.position, followSpeed * Time.deltaTime); // 대상을 쫓도록 함
                    followSpeed += Time.deltaTime;  // 시간에 따른 속도 증가
                  
                    if (_distance <= 1)
                    {
                        itemPickUp.gameObject.GetComponentInChildren<Collider>().isTrigger = true;
                       // itemPickUp.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
                        itemPickUp.ItemPick();
                    }
                }
            }
        }
    }

}
