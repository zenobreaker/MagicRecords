using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AttackObject
{
    [SerializeField]
    protected float speed = 0f;

    protected Vector3 direction;
    protected Vector3 pos;

    public float startLife; 
    public float dir;

    protected Rigidbody myRigid;

    protected float life = 0f;

    [Header("피격 이펙트")]
    [SerializeField] protected GameObject go_RicochetEffect = null;

    [Header("피격 효과음")]
    [SerializeField] protected string sound_Ricochet = null;


    public float MyDir{
        get { return dir; }
        set { dir = value; }
     }

    protected virtual void OnEnable()
    {
        direction = transform.forward;
        pos = transform.position;
        life = startLife;
    }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        myRigid = GetComponent<Rigidbody>();
    }

  

    // Update is called once per frame
    protected void Update()
    {
        myRigid.velocity = direction.normalized * speed;

        //   float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //   transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        life -= Time.deltaTime;

        if (life <= 0) 
        {
            // Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

 
    // 피격이펙트 관리
    protected virtual int EffectChoose()
    {
        return 0;
    }
}
