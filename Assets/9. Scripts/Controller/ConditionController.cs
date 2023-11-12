using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuffStock
{
    public Buff myBuff;         // 당할 버프 
    public Debuff myDebuff;     // 당할 디버프 
    public bool buffSwitch; // true : 버프 false : 디버프
    
    public float applyTime; // 버프/디버프 유지시간
    public int applyValue;  // 버프/디버프 값

    public bool isApply;    // 적용 중인 버프인지 체크

    public BuffStock(Buff myBuff, Debuff myDebuff, bool buffSwitch,
        float applyTime, int applyValue, bool isApply)
    {
        this.myBuff = myBuff;
        this.myDebuff = myDebuff;
        this.buffSwitch = buffSwitch;
        this.applyTime = applyTime;
        this.applyValue = applyValue;
        this.isApply = isApply;
    }
}

public enum Condition
{
    NONE = 0, // 일반 아무것도 아님
    MOVE_CONDITION,         // 움직이 가능한 컨디션 상태
    NONE_MOVE_CONDTION,     // 움직이 불가능한 상태 
    NONE_ALL_ACTION,        // 모든 액션이 불가능한 상태  
}

public class ConditionController : MonoBehaviour
{
    public List<BuffStock> buffStoks = new List<BuffStock>();

    Dictionary<Debuff, Condition> dic_debuffConnectConditionList = new Dictionary<Debuff, Condition>();

    // 컨디션컨트롤러는 캐릭터 클래스랑 서로 연결되는 구조로 구성되어 있다. 
    [SerializeField] WheelerController myBody;
    [SerializeField] Animator myAnim;

    public Condition myCondition;

    public static BuffStock CreateBuffStock()
    {
        BuffStock buff = new BuffStock(Buff.NONE, Debuff.NONE, false, 0, 0, false);
        return buff; 
    }

    public static BuffStock CreateBuffStock(Buff _buff = Buff.NONE, Debuff _debuff = Debuff.NONE,
        bool _isSwitch = false,  
        int value = 0, float _time = 0.0f, bool isApply = false)
    {
        BuffStock buff = new BuffStock(_buff, _debuff, _isSwitch, _time, value, isApply);

        return buff;
    }


    public void Awake()
    {
        // 디버프에 따른 상태 이상 설정 
        InitialDebuffByConditionList();

        InitialMyCondition();
    }


    private void Start()
    {
        // 타이밍 상 오류를 범하지 않기 위해 여기서 초기화한다. 

        // 스텟 변수에 스탯 정보가 없다면 이 컴포넌트가 부착된 대상에게서 찾는다.
        if (myBody == null)
        {
            myBody = gameObject.GetComponent<WheelerController>();

            // 부착된 오브젝트에서 찾을 수 없다면 부착된 오브젝트의 자식에서도 찾는다.
            if (myBody == null)
            {
                myBody = gameObject.GetComponentInChildren<WheelerController>();
            }
        }
    }

    private void Update()
    {
        UpdateBuffList();
    }

    // 제어형 디버프에 따른 상태 이상 설정 
    void InitialDebuffByConditionList()
    {
        // 홀드 상태
        dic_debuffConnectConditionList[Debuff.HOLD] = Condition.NONE_MOVE_CONDTION;
        // 기절 상태 
        dic_debuffConnectConditionList[Debuff.STURN] = Condition.NONE_ALL_ACTION;
    }

    // 자신의 상태 초기화 
    public void InitialMyCondition()
    {
        myCondition = Condition.MOVE_CONDITION;
    }

    // 자신의 상태를 변경하는 함수 
    public void ChanageMyCondition(Condition _newCondtion, float _condtionTime = 0.0f)
    {
        myCondition = _newCondtion;

        // 상태이상별 기능 
        AbnormalCondition(_condtionTime);
    }


    // 자신에게 걸린 버프/디버프 검사 
    public void CheckMyALLBuffList()
    {
        
    }


    // 버프리스트에 버프 추가하기 
    public void AddBuff(BuffStock _buff)
    {
        if (_buff == null) return;

        buffStoks.Add(_buff); 
    }

    // 버프리스트에 버프를 동작시킨다 버프의 지속시간을 줄이면서 버프의 기능을 동작시킨다. 
    void UpdateBuffList()
    {
        // 버프 리스트틀 돌면서 타이머를 체크 후 0이면 리스트에서 제거
        if (buffStoks == null || buffStoks.Count <= 0) return; 
        
        foreach(var buff in buffStoks)
        {
            if(buff != null)
            {
                buff.applyTime -= Time.deltaTime;
                // 버프의 적용 시간이 지나면 리스트에서 삭제한다.
                if(buff.applyTime <= 0 )
                {
                    buffStoks.Remove(buff);
                    break;
                }
                else
                {
                    // 버프별 기능 동작 
                    ApplyBuffCondition(buff);
                }
            }
        }

    }

    // 출혈과 중독같은 스탯에 영향을 주는 상태를 괸리하는 함수
    // 기각 - 인자를 버프스톡클래스로 받으면 버프 혹은 디버프를 판별하여 해당 버프의 내재된 enum 값을 진행시킨다. 
    public void ApplyBuffCondition(BuffStock _buff)
    {

        if (myBody == null) return; 


        // 이미 적용 중인 버프라면 연산을 처리하지 않도록 보낸다. 
        if(_buff.isApply == true)
        {
            return; 
        }

        _buff.isApply = true; 

        // 일반적 버프인가
        if(_buff.buffSwitch == true)
        {
            switch (_buff.myBuff)
            {
                case Buff.INCREASE_ATTACK:
                    break;
                case Buff.INCREASE_ATTACK_SPEED:
                    break;
                case Buff.INCREASE_DEFENSE:
                    break;
                case Buff.INCREASE_SPEED:
                    break;
            }
        }
        //  디버프라면
        else if(_buff.buffSwitch == false)
        {
            switch (_buff.myDebuff)
            {
                case Debuff.BREAK_AROMR:
                    break;
                case Debuff.BREAK_WEAPON:
                    break;
                case Debuff.DOWN_ATTACK_SPEED:
                    break;
                case Debuff.BLEED:
                    break;
                case Debuff.BURN:
                    break;
                case Debuff.CURSE:
                    break;
                case Debuff.SLOW:
                    break;
                case Debuff.STURN:
                case Debuff.HOLD:
                case Debuff.ICE:
                    {
                        // 움직임 제어형 디버프는 컨디션을 변경시키고 적용시킨다. 
                        myCondition = Condition.NONE_MOVE_CONDTION;
                        AbnormalCondition(_buff.applyTime);
                        Debug.Log("버프 동작 실행" + _buff.myDebuff);
                    }
                    break; 

            }

        }

    }
        

    // 기절과 빙결같은 행동에 제한을 두는 컨디션을 관리한다.
    // 인자로 시간값을 주면 상태변수의 값에 따라 상태별 기능을 동작하게 하는 함수
    public void AbnormalCondition(float _condtionTime = 0.0f)
    {
        Debug.Log("변경된 => " + myCondition  + " " + _condtionTime + "초 동안");
        switch (myCondition)
        {
            //case Condition.NONE:
            //    if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            //        myAnim.SetTrigger("Idle");
            //    break;
            case Condition.MOVE_CONDITION:
                break;
            case Condition.NONE_MOVE_CONDTION:
                StopCondition(_condtionTime);
                break;
            case Condition.NONE_ALL_ACTION:
                StopCondition(_condtionTime);
                break; 
        }
    }

    void StopCondition(float _coditionTime = 0.0f)
    {
        if(myAnim == null)
        {
            Debug.Log("Not has Animaotr");
            return;
        }


        myAnim.SetBool("Walking", false);
        //myAnim.SetBool("Attacking", false);
        myAnim.SetTrigger("Idle");
        myAnim.StopPlayback();

        StartCoroutine(RecoveryCondition(_coditionTime));
        
    }

    IEnumerator RecoveryCondition(float _coditionTime = 0.0f)
    {
        yield return new WaitForSeconds(_coditionTime);

        Debug.Log(myCondition + " 해결");
        myCondition = Condition.MOVE_CONDITION;
        myAnim.StopPlayback();
    }
}
