using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLine : MonoBehaviour
{

    [SerializeField] LineRenderer lr; 
    // Start is called before the first frame update
    void Start()
    {
       // lr = GetComponent<LineRenderer>();

        DrawCircle(5, 10f);
    }

    public void DrawCircle(float radius, float lineWidth)
    {
        var segments = 360;
        //var lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.startWidth = lineWidth;
        lr.positionCount = segments + 1;

        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
        }

        lr.SetPositions(points);
    }
}
