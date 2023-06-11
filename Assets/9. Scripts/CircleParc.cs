using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleParc : MonoBehaviour
{
    Transform tran;

    Vector3 retVector3;

    public int speed = 1;
    int degree = 0;

    // Start is called before the first frame update
    void Start()
    {
        tran = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            degree += speed;
            float radin = degree * Mathf.PI / 180;

            retVector3.x += 3.5f * Mathf.Cos(radin);
            retVector3.z += 3.5f * Mathf.Sin(radin);

            transform.position = retVector3;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            degree -= speed;
            float radin = degree * Mathf.PI / 180;

            retVector3.x -= 3.5f * Mathf.Cos(radin);
            retVector3.z -= 3.5f * Mathf.Sin(radin);

            transform.position = retVector3;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AngleTest();
        }
    }

    public void AngleTest()
    {
        degree += speed;
        float radin = speed * Mathf.PI / 180;


        retVector3.x += 3.5f * Mathf.Cos(radin);
        retVector3.z += 3.5f * Mathf.Sin(radin);

        transform.position = retVector3;
    }
}
