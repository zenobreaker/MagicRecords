using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    [SerializeField] int maxHp = 0; //대상의 체력 
    int currentHp;

    [SerializeField]
    private int attack = 0;
 
    public int MyAttack
    {
        get { return attack; }
    }

    public int MyHP
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    public int MyMaxHp
    {
        get { return maxHp; }
    }

    private void Start()
    {
        currentHp = maxHp;
    }

  
}
