using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Character weaponOwn;     // 무기 소유주
    public Animator MyAnim { get; set; }

    public float dmageRate = 1.0f;
    public int damage = 0;
    public float critRate = 0.0f;
    public float critDamage = 0.0f;


    [Header("총")]
    [SerializeField]
    Weapon currentGun = null;


    private Animation myAnim;
    public float currentFireRate;
    public float normalFireRate = 0.2f; 
    public float finalFireRate = 0.7f; 

    public float attackCoolTime = 0f;

    private ComboState currentCombo;

    public Weapon MyWeapon
    {
        get { return currentGun; }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyAnim = GetComponent<Animator>();
        //MyAnim.SetFloat("AttackSpeed", 1);
        // myAnimator.SetFloat("test_float", 8.0f);
    }

    // Update is called once per frame
    void Update()
    {
        FireRateCalc();
        //TryFire();
    }

    public void SetWeaponOwn(Character own)
    {
        weaponOwn = own; 
    }

    public void SetDamageAndCrit(float dmageRate, int damage, float chance, float critDmamge)
    {
        this.dmageRate = dmageRate;
        this.damage = damage;
        this.critRate = chance;
        this.critDamage = critDmamge;
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
        //if (Input.GetKeyDown(KeyCode.X) && currentFireRate <= 0)

        //if (_currentCombo == ComboState.ATTACK_4)
        //    return;

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
                currentFireRate = finalFireRate;
                MyAnim.SetTrigger("Attack");
                break;
            case ComboState.ATTACK_4:
                currentFireRate = normalFireRate;
                MyAnim.SetTrigger("FinalAttack");
                break;
        }

    }


    private void CreateBullet()
    {
        //float pc_Rotation = 10.0f;// PlayerControl.MyInstance.TargetRotation;
        var clone = ObjectPooler.SpawnFromPool<MyBullet>("Bullet", currentGun.go_Muzzle.transform);
        //clone.SetDamageAndCrit(dmageRate, damage, critRate, critDamage);
        clone.AttackOwn = weaponOwn;
        //clone.transform.position = currentGun.go_Muzzle.transform.position;
        //clone.transform.rotation = currentGun.go_Muzzle.transform.rotation;
        //clone.GetComponent<MyBullet>().MyDamage = _dmg;
        //clone.GetComponent<MyBullet>().tr_CreatersTR = this.GetComponentInParent<PlayerControl>().transform;
        //clone.SetActive(true);
    }

    void ShotSoundPlay()
    {
        if (currentCombo != ComboState.ATTACK_4)
        {
            SoundManager.instance.PlaySE(currentGun.sound_Fire);
        }
        //else if (currentCombo == ComboState.ATTACK_4)
        //{
        //    SoundManager.instance.PlaySE(currentGun.sound_Fire2);
        //}

    }

    void LaserSound()
    {
        SoundManager.instance.PlaySE("LaserEffect");
        currentGun.ps_MuzzleFlash.Play();
    }

    void ShootBullet()
    {
        ShotSoundPlay();
        currentGun.ps_MuzzleFlash.Play();
        CreateBullet();
    }


    void ShootFinalBullet()
    {
        SoundManager.instance.PlaySE(currentGun.sound_Fire2);
        currentGun.ps_MuzzleFlash.Play();
        CreateBullet();
    }
}
