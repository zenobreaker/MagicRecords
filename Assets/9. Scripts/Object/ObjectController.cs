using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectController : MonoBehaviour
{

    [Header("경험치")]
    public int exp;
    
    [Header("속도 관련 변수")]
    [SerializeField]
    protected float speed;

    private Rigidbody myRigid;
    private Animator myAnimator;
    private Quaternion TargetRotation;

    private Object myObject;

    public float lookRadius = 10f;

    private Transform target;
    private NavMeshAgent agent;

    public float waitBaseTime = 2.0f; // 대기 시간 
    public float waitTime; // 남은 대기 시간
    public float walkRange = 5.0f; // 이동 범위 
    public Vector3 basePosition; // 초기 위치를 저장해 둘 변수

    public float searchTime = 5.0f;
    [SerializeField]
    private GameObject attackArea = null;

    //방향 회전을 지시하는가?
    public bool forceRotate = false;
    // 도착했는가?
    public bool arrived = false;
    // 목적지 
    public Vector3 destination;
    // 이동 속도 
    public float walkSpeed = 6.0f;
    // 회전 속도 
    public float rotationSpeed = 180;
    //향하게 하고 싶은 방향 
    Vector3 forceRotateDirection;
    public bool isAttacking = false;

    enum state { WALK, IDLE, CHAISE, ATTACK };

    state currentState, nextState;

    private Animator anim;

    public Transform attackTarget; // 공격 타겟 

    //[SerializeField]
   // private GameObject Flag;

    [Header("아이템 드랍")]
    public GameObject[] dropItem;

    // Start is called before the first frame update
    void Start()
    {
        myObject = GetComponent<Object>();

        //target = PlayerManager.instance.player.transform;
        //agent = GetComponent<NavMeshAgent>();
        myRigid = GetComponent<Rigidbody>();

        if (attackArea != null)
            attackArea.SetActive(false);
        // 초기 위치 저장 
        basePosition = new Vector3(transform.position.x,0,transform.position.z);

        waitTime = waitBaseTime;
        currentState = state.IDLE;
        nextState = state.IDLE;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        //   GetInput();
        //   Move();
        if (currentState != nextState)
            currentState = nextState;

        switch (currentState)
        {
            case state.IDLE:
                Waiting();
                break;
            case state.WALK:
                Walking();
                break;
            case state.CHAISE:
                Chasing();
                break;
            case state.ATTACK:
                Attacking();
                break;
           
        }
       // Debug.Log(currentState);
     //   Walking();
       
        //  float distanace = Vector3.Distance(target.position, transform.position);
        // agent.SetDestination(target.position);
    }


    private void changeState()
    {
        if (currentState != nextState)
            currentState = nextState;
    }

    void Waiting()
    {
        myRigid.velocity =  Vector3.zero;
        StopMove();

        if (waitTime > 0.0f)
        {
            waitTime -= Time.deltaTime;

            if(waitTime <= 0.0f)
            {
                waitTime = Random.Range(waitTime, waitBaseTime * 2.0f);
                nextState = state.WALK;
            }
        }
        else
            waitTime = Random.Range(waitTime, waitBaseTime * 2.0f);
    }

    // 이동
    void Walking()
    {

        // 대기 시간이 남았다면
        if (waitTime > 0.0f)
        {
            // 대기 시간을 감소한다.
            waitTime -= Time.deltaTime;

            //대기 시간이 없어지면
            if (waitTime <= 0.0f)
            {
                // 목적지를 지정한다
                SetDestination(); 
            }
        }
        else
        {
            // 목적지를 향해 이동한다.
            Move();
            // Debug.Log("시간 없어서 호출된 부분");
            // 목적지에 도착한다.
            if (Arrived())
            {
                // 목적지에 도착하면 멈춘다. 
                StopMove();
                // 대기 상태로 전환한다.
                waitTime = Random.Range(waitTime, waitBaseTime * 2.0f);
                nextState = state.IDLE;
            }
            if (attackTarget)
            {
                nextState = state.CHAISE;
            }
            // 타겟을 발견하면 추적한다. 
        }
    }

    // 목적지 설정 
    public void SetDestination( )
    {
        arrived = false;
        // 범위 내의 어딘가
        Vector2 randomValue = Random.insideUnitCircle * 50;
        // 이동할 곳을 설정한다.
        Vector3 destinationPosition = basePosition + new Vector3(randomValue.x, transform.position.y, randomValue.y);

        RaycastHit hit;
        Ray destRay = new Ray(destinationPosition +new Vector3(0,10,0), Vector3.down);
        
        Debug.DrawRay(destinationPosition, destinationPosition+ new Vector3(0, 10, 0),Color.red);
        if (Physics.Raycast(destRay, out hit))
        {
            if (hit.collider.tag == "land")
            {
               // Debug.Log("땅발견");
              //  Instantiate(Flag, destinationPosition, Quaternion.identity);
                this.destination = destinationPosition;
                if(destination.y <= 0)
                {
                    destination.y = 0;
                }
            }
            else
            {
                Debug.Log("땅이아니다");
                nextState = state.IDLE;
            }
        }
        else
            nextState = state.IDLE;

       
    }
    
    // 지정한 방향으로 향한다.
    public void SetDirection(Vector3 direction)
    {
        forceRotateDirection = direction;
        forceRotateDirection.y = 0;
        forceRotateDirection.Normalize();
        forceRotate = true;
    }

    bool Arrived()
    {
        Vector3 direction = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);

     //   Debug.Log("도착 검사 "+distance);
        // 목적지에 가깝다면 도착
        if (arrived || distance <=7.0f)
        {
        //    Debug.Log("도착");
            arrived = true;
            myRigid.velocity = Vector3.zero;
        }

        /*
        searchTime -= Time.deltaTime;
        if(searchTime <= 0)
        {
            arrived = true;
            searchTime = 5.0f;
        }
        */

        return arrived;
    }
    // 충돌 검사  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Debug.Log("벽에 부딪힘");
            // 벽에 부딪히면 새로운 목적지를 할당
            arrived = true;
           SetDestination();
           
        }
    }
    private void Move()
    {
        // 목적지까지 거리와 방향을 구한다.
        Vector3 direction = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);
        if (direction == Vector3.zero)
        {
            nextState = state.IDLE;
            return;
        }
        Quaternion characterTargetRotation = Quaternion.LookRotation(direction);
       // transform.rotation = characterTargetRotation;
       
        //  SetDirection(direction);
        ForceRotate(direction);

      //  Debug.Log("Move함수호출");
        anim.SetTrigger("Move");
        myRigid.velocity = new Vector3(direction.x,0,direction.z)*speed;
       // myRigid.velocity = Vector3.Lerp(currentVelocity, velocity, Mathf.Min(Time.deltaTime * 5.0f, 1.0f));
    }

    private void StopMove()
    {
        anim.SetTrigger("Idle");
        myRigid.velocity = Vector3.zero;
        destination = transform.position;
    }
    // 방향 지정 
    private void ForceRotate(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        /*
        if (!forceRotate)
        {
            // 바꾸고 싶은 방향으로 변경하기
            Quaternion characterTargetRotation = Quaternion.LookRotation(direction);
            Debug.Log("방향 1 "+ characterTargetRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion characterTargetRotation = Quaternion.LookRotation(direction);
            Debug.Log("방향 2 "+characterTargetRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, characterTargetRotation, rotationSpeed * Time.deltaTime);
            
        }
        */
    }

    public void SetAttackTarget(Transform target)
    {
        Debug.Log("대상 발견");
        attackTarget = target;
        if(currentState != state.ATTACK)
            nextState = state.CHAISE;
    }

    // 대상 추적 

    void Chasing()
    {
        Debug.Log("대상 추격 ");

        if (attackTarget)
        {
            destination = attackTarget.position;
            Move();

            float dis = Vector3.Distance(destination, attackTarget.position);

            if (dis <= 10.0f)
            {
                Debug.Log("대상 추격 종료 ");
                nextState = state.ATTACK;
                // nextState = state.IDLE;
                // GetComponentInChildren<SearchArea>().gameObject.SetActive(false);
            }
        }
        else if (attackTarget == null)
            nextState = state.IDLE;

    }

  
    void Attacking()
    {
        Debug.Log("공격 시작");

        if (attackTarget == null )
        {
       
             anim.ResetTrigger("Attack");
            //  GetComponentInChildren<SearchArea>().gameObject.SetActive(true);
       
             nextState = state.IDLE;
          
        }
        else
        { 
            SetDirection(attackTarget.position);
            anim.SetTrigger("Attack");
        }          
    }

    void OnAttackArea()
    {
        attackArea.SetActive(true);
        Debug.Log("공격범위 온");
    }

    void StartAttack()
    {
        isAttacking = true;
    }

    void FinishAttack()
    {
        attackArea.SetActive(false);
        isAttacking = false;
        Debug.Log("공격범위 제거");
    }

    public void DropItems()
    {
        if (dropItem.Length == 0) {
            return;
        }

        GameObject drop = dropItem[Random.Range(0, dropItem.Length)];
        Instantiate(drop, transform.position, Quaternion.identity);
    }

    public void Damaged(int pow)
    {
        myObject.MyHP -= pow;
        //UIManager.instance.ShowHealthBar(myObject);
        Debug.Log("대미지 입음 " + myObject.MyHP);
        if (myObject.MyHP <= 0)
        {
            Died();
        }
    }

    private void Died()
    {
        UIManager.instance.HideHealthBar();
        this.gameObject.tag = "Untagged";
       
        Debug.Log(exp + "경험치 획득!");
        DropItems();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager theGM = FindObjectOfType<GameManager>();
        if (theGM != null)
        {
            //theGM.enemyCounter -= 1;
            //theGM.MyGameScore += 1;
        }
    }
}
