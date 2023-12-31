using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType
{
    NONE = 0,
    MELEE = 1,  // 근거리
    RANGE = 2,  // 원거리(사격계)
}

public class WeaponController : MonoBehaviour
{
    public Character weaponOwn;     // 무기 소유주
    public Animator MyAnim { get; set; }

    public float dmageRate = 1.0f;
    public int damage = 0;
    public float critRate = 0.0f;
    public float critDamage = 0.0f;


    [Header("무기 타입 관련")]
    public WeaponType weaponType;

    [Header("착용 무기")]
    [SerializeField]
    protected Weapon currentWeapon = null;

    public LayerMask targetLayer = 0;

    protected Animation myAnim;
    public float currentFireRate;
    public float normalFireRate = 0.2f; 
    public float finalFireRate = 0.7f; 

    public float attackCoolTime = 0f;

    protected ComboState currentCombo;

    public Weapon MyWeapon
    {
        get { return currentWeapon; }
    }

    void Start()
    {
        if(MyAnim == null)
            MyAnim = GetComponent<Animator>();
    }

    void Update()
    {
        FireRateCalc();
    }


    public void SetWeaponOwn(Character own, LayerMask layerMask)
    {
        weaponOwn = own; 
        if(currentWeapon != null)
        {
            int extraLayer = 1 << layerMask;
            extraLayer = ~extraLayer;
            targetLayer = extraLayer;
            currentWeapon.SetTargetLayer(extraLayer);
        }
    }

    public void SetDamageAndCrit(float dmageRate, int damage, float chance, float critDmamge)
    {
        this.dmageRate = dmageRate;
        this.damage = damage;
        this.critRate = chance;
        this.critDamage = critDmamge;
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        if (MyAnim == null)
            return;

        MyAnim.SetFloat("AttackSpeed", 1.0f * attackSpeed);
    }
    void FireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }

    public void TryFire(ComboState _currentCombo)
    {
        currentCombo = _currentCombo; 

        switch (_currentCombo)
        {
            case ComboState.ATTACK_1:
                currentFireRate = normalFireRate;
                MyAnim.SetTrigger("Attack");
                break;
            case ComboState.ATTACK_2:
                currentFireRate = normalFireRate;
                MyAnim.SetTrigger("Attack");
                break;
            case ComboState.ATTACK_3:
                currentFireRate = normalFireRate;
                MyAnim.SetTrigger("Attack");
                break;
            case ComboState.ATTACK_4:
                currentFireRate = finalFireRate;
                MyAnim.SetTrigger("FinalAttack");
                break;
        }

    }


    public virtual void TryNormalMeeleAttack()
    {
        
    }

    public virtual void EndNormalMeeleAttack()
    {
       
    }

}
