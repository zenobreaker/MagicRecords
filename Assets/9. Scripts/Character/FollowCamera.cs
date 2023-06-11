using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform player;

    public float rotateDamping; // 회전 속도
    public float moveDamping;   // 이동속도 
    public float distance; // 카메라 거리
    public float height;    // 카메라 높이 
    public Vector3 offset;
    public float targetOffset;
    RaycastHit hitInfo;
    RaycastHit[] hits;

    public List<GameObject> nowAddedWall = new List<GameObject>();

    public void setOffset(Transform _player)
    {
        player = _player;
        //offset = transform.position - player.transform.position;
        
    }
 
       // update 이후 처리 플레이어가 프레임을 이동후 확인 가능
    private void Update()
    {

        if (player != null)
        {
           // var camPos = player.transform.position - (player.transform.forward * distance) + (player.transform.up * height);

            // transform.position = Vector3.Slerp(transform.position, camPos, Time.deltaTime * moveDamping);
            //  tr.rotation = Quaternion.Slerp(tr.rotation, player.transform.rotation, Time.deltaTime + rotateDamping);
            //대상이 회전함에 따라 카메라도 이동 
            //   
            transform.position =  player.position + offset;
            transform.LookAt(player.position + (player.up * targetOffset));

            /*    if (Physics.Linecast(player.transform.position + offset, transform.position, out hitInfo,
               1 << LayerMask.NameToLayer("Ground")))
                    transform.position = hitInfo.point;
                    */

            Debug.DrawRay(this.gameObject.transform.position, player.position - this.gameObject.transform.position, Color.red);

          //  Transparency();
        }
    }


    // 카메라와 캐릭터 사이의 물체가 있다면 그 물체를 투명화
    private void Transparency()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Attack") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyAttack"); 
        layerMask = ~layerMask;

        if (Physics.Raycast(this.gameObject.transform.position, player.position - this.gameObject.transform.position, out hitInfo,
            Vector3.Distance(transform.position, player.position), layerMask))
        {
            if (!hitInfo.transform.CompareTag("Player"))
            {
                if (hitInfo.transform.GetComponent<MeshRenderer>() == null)
                    return;
                Color _color = hitInfo.transform.GetComponent<MeshRenderer>().material.color;
                _color.a = 0.5f;
                hitInfo.transform.GetComponent<MeshRenderer>().material.color = _color;
                nowAddedWall.Add(hitInfo.collider.gameObject);
            }
            else
            {
                for (int i = 0; i < nowAddedWall.Count; i++)
                {
                    Color _color = nowAddedWall[i].GetComponent<MeshRenderer>().material.color;
                    _color.a = 1f;
                    nowAddedWall[i].transform.GetComponent<MeshRenderer>().material.color = _color;
                }
                nowAddedWall.Clear();
            }

        }
    }

    // 카메라가 바라보는 곳이 어딘지를 알아보기 쉽게 표현하기 위해 Gizmo를 생성
    private void OnDrawGizmos()
    {
        if (player == null)
            return;
        else
        {
            Gizmos.color = Color.green;
        //    Gizmos.DrawWireSphere(player.position + (player.up * targetOffset), 0.1f);
         //   Gizmos.DrawLine(player.position + (player.up * targetOffset), transform.position);
        }
    }
}
