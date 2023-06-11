using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    protected float speed = 0f;
   

    protected Vector3 direction;
    protected Vector3 pos;

    public float startLife; 
    public float dir;
    public Transform tr_CreatersTR;

    protected Rigidbody myRigid;

    protected float life = 0f;


    [Header("피격 이펙트")]
    [SerializeField] protected GameObject go_RicochetEffect = null;

    [Header("총알 데미지")]
     protected int damage = 0;

    [Header("피격 효과음")]
    [SerializeField] protected string sound_Ricochet = null;

    public int MyDamage
    {
        set { damage = value; }
        get { return damage; }
    }

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

    protected void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        myRigid = GetComponent<Rigidbody>();
       // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Item"));
        // child.transform.rotation = Quaternion.Euler(0, dir, 0);
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

    

    /*
    private void OnCollisionEnter(Collision collision)
    {
        int effectNumber = EffectChoose();
        ContactPoint contactPoint = collision.contacts[0]; // 충돌한 객체의 접촉면에 대한 정보가 담긴 클래스 
                                                           //충돌한 객체의 접촉면 정보가 collision.contacts[0]에 저장되어 있다.(가장 가까운 접촉면) 
        SoundManager.instance.PlaySE(sound_Ricochet); 
        var clone = Instantiate(go_RicochetEffects[effectNumber], contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        //point 대상의 좌표 LootRotation = 특정 방향을 바라보게 하는 메소드 normal 부딪힌 객체의 접촉면 방향 
        Debug.Log(collision.transform.name);
        if (collision.transform.CompareTag("Monster")) //닿은 대상에 태그가 "Monster"라면
        {
            collision.transform.GetComponent<ObjectController>().Damaged(damage); // 해당 transform에 들어있는 Object컴포넌트에 Damgaed메소드를 호출하여 damge값 전달
            Destroy(clone, 0.5f);
            //Destroy(gameObject);
            this.gameObject.SetActive(false);
        }
        Destroy(clone, 0.5f);
        //Destroy(gameObject);
        this.gameObject.SetActive(false);

    }*/

 
    // 피격이펙트 관리
    protected virtual int EffectChoose()
    {
        return 0;
    }
}
