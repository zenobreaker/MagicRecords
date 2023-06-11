using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL, WAYPOINT }
    private const string wayPotinFile = "Enemy";
    public Type type = Type.NORMAL;

    public Color color = Color.yellow;
    public float radius = 0.1f;

    private void OnDrawGizmos()
    {
        if(type == Type.NORMAL)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
        else
        {
            Gizmos.color = color;
            // 위치 파일명 스케일 적용 유무
           // Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, wayPotinFile, true);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
