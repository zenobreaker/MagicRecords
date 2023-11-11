using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle = 0f; // 시야각
    public float viewDistance = 0f; // 시야거리
    public float meeleAttackDistance = 0f;// 근접 공격 유효 거리 
    public Transform target;
    [SerializeField] private LayerMask targetMask = 0; // 타겟 마스크 (플레이어)

    //private Pig thePig;
    [SerializeField]
    private PlayerControl thePlayer;

    private void Start()
    {
        //thePig = GetComponent<Pig>();
        if(thePlayer == null)
            thePlayer = FindObjectOfType<PlayerControl>();
    }

    public Transform GetTargetTransform()
    {
        return target;
    }

    public Vector3 GetTargetPos()
    {
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        // 타겟값이 있다면 대상을 따르도록 
        if(target != null)
        {
            return target.transform.position;
        }

        // 없으면 일정 범위 내에 적이 있는지 검사 
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.CompareTag( "Player" ))
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward); // 캐릭터 전방 레이와 대상과 캐릭터의 선분의 각

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.CompareTag("Player"))
                        {
                           
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            return _hit.transform.position;
                        }
                    }
                }
            }
        }

        return Vector3.zero;
    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += this.transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    public bool View()
    {
        if (target != null) return true; 

         Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
         Vector3 _rightBoundary  = BoundaryAngle(viewAngle * 0.5f);

         Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
         Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);
       
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.tag == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward); // 캐릭터 전방 레이와 대상과 캐릭터의 선분의 각

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.tag == "Player")
                        {
                            target = _hit.transform;
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool MeeleAttackRangeView()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Collider[] _target = Physics.OverlapSphere(transform.position, meeleAttackDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.tag == "Player")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward); // 캐릭터 전방 레이와 대상과 캐릭터의 선분의 각

                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;
                    if (Physics.Raycast(transform.position, _direction, out _hit, meeleAttackDistance))
                    {
                        if (_hit.transform.tag == "Player")
                        {
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
 
}
