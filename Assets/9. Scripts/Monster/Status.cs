using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Status : MonoBehaviour
{
    [Header("몬스터 타입")]
    public MonsterType myType;

    [SerializeField] private int maxHp = 0; //대상의 체력 
    private int currentHp;

    [SerializeField]
    private int attack = 0; // 공격 대미지
    [SerializeField]
    private float attackDelay = 0f; // 공격 딜레이

    [SerializeField] private float walkSpeed = 0f; // 걷기 스피드
                                                   //  [SerializeField] private float runSpeed = 0f; // 뛰기 스피드

    [SerializeField]
    private int deffence = 0;

    [SerializeField]
    private int exp = 0;

    [SerializeField]
    private int pattern; 

    public int MyAttack
    {
        get { return attack; }
        set { attack = value; }
    }
    public int MyDeffence
    {
        get { return deffence; }
        set { deffence = value; }
    }
    public float MyAttackDelay
    {
        get { return attackDelay; }
        set { attackDelay = value; }
    }

    public int MyHP
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    public int MyMaxHp
    {
        get { return maxHp; }
        set { maxHp = value; }
    }

    public float MyWalkSpeed
    {
        get { return walkSpeed; }
        set { walkSpeed = value; }
    }

    public int MyEXP
    {
        get { return exp; }
        set { exp = value; }
    }

    public int MyPattern
    {
        get { return pattern; }
        set { pattern = value;  }
    }

    private void Start()
    {
        currentHp = maxHp;
    }
}
