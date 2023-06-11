using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjController : MonoBehaviour
{
    [Header("속도 관련 변수")]
    [SerializeField]
    protected float speed;

    private Rigidbody myRigid;
    private Animator myAnimator;
    private Quaternion TargetRotation;

    private Transform target;
    private NavMeshAgent agent;

    public Vector3 basePosition; // 초기 위치를 저장해 둘 변수

    public Vector3 destination; // 목적지 
    private bool arrived = false;  // 도착했는가? 
    public float walkSpeed = 6.0f;
    public float waitBaseTime = 2.0f; // 대기 시간 
    public float waitTime; // 남은 대기 시간

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myRigid = GetComponent<Rigidbody>();

        //초기 위치 저장
        basePosition = transform.position;
        destination = basePosition;
        waitTime = waitBaseTime;
    }

    // Update is called once per frame
    void Update()
    {
      
        Walking();


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
                agent.SetDestination(destination);
                // 목적지를 향해 이동한다.
            }
        }
        else
        {
          //  Debug.Log("시간 없어서 호출된 부분");
            // 목적지에 도착한다.
            if (Arrived())
            {
                // 목적지에 도착하면 멈춘다. 
                StopMove();
                // 대기 상태로 전환한다.
                waitTime = Random.Range(waitTime, waitBaseTime * 2.0f);
            }
            // 타겟을 발견하면 추적한다. 
        }
    }

    void Chiasing()
    {
        agent.SetDestination(destination);
    }


    void SetDestination()
    {
        agent.speed = this.speed;
        arrived = false;
        Debug.Log("도착 설정");
        Vector2 randomValue = Random.insideUnitCircle * 50;
        Vector3 destinationPosition = basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
      // Instantiate(Flag, destinationPosition, Quaternion.identity);
        this.destination = destinationPosition;
    }
    void SetDestination(Vector3 destinationPosition)
    {
        arrived = false;
        // 범위 내의 어딘가
        Vector2 randomValue = Random.insideUnitCircle * 50;
        // 이동할 곳을 설정한다.
        destinationPosition = basePosition + new Vector3(randomValue.x, 0.0f, randomValue.y);
     //   agent.SetDestination(destinationPosition);
    }
   
    
    void StopMove()
    {
        agent.speed = 0;
        destination = transform.position;
    }

    // 도착 지점에 도착헀는가? 
    bool Arrived()
    {
        Vector3 direction = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);
       
        if (arrived || distance <= 6.0f)
        {
            arrived = true;
        }

        return this.arrived;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("적 감지");
            destination = other.transform.position;
        }
    }

}
