using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "Scriptable Object/Player Data", order = int.MaxValue)]
public class PlayerData : ScriptableObject
{
    // 플레이어 이름 
    [SerializeField]
    private string playerName;
    public string PlayerName { get { return playerName; } }

    [SerializeField]
    private int type;
    public int Type { get { return type; } }

    // 플레이어 체력 
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    // 플레이어 마나 
    [SerializeField]
    private int mp;
    public int Mp { get { return mp; } }

    // 플레이어 CP 
    [SerializeField]
    private int cp;
    public int Cp { get { return cp; } }

    // 공격력 
    [SerializeField]
    private int attack;
    public int Attack { get { return attack; } }

    // 방어력 
    [SerializeField]
    private int defence;
    public int Defence { get { return defence; } }

    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } }

    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float hpRecovery;
    public float HpRecovery { get { return hpRecovery; } }
    
    [SerializeField]
    private float mpRecovery;
    public float MpRecovery { get { return mpRecovery; } }

    [SerializeField]
    private int exp;
    public int Exp { get { return exp; } }

    [SerializeField]
    private int pattern;
    public int Pattten { get { return pattern; } }
    
}
