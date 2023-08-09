using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespwanManager : MonoBehaviour
{
    //[SerializeField]
    //MonsterDatabase monsterDB = null;

    // 적 캐릭터가 출현할 위치를 담을 배열 
    public Transform[] points;
    // 적 캐릭터 프리팹을 저장할 변수 
    public GameObject[] enemies;
    public GameObject bossEnemy;
    Transform tr_PlayerTransform;  

    // 적 캐릭터를 생성할 주기 
    public float createTime = 2.0f;

    MonsterGrade monsterType; 

    public List<GameObject> curMonsters = new List<GameObject>();    // 생성된 몬스터를 담을 변수 
    public GameObject go_Player; // 플레이어 위치 생성자 

    [SerializeField] Animation bossWarning = null;
    //[SerializeField] MStatController mStat = null; 

    public void InitEnemies(GameObject _enemy,MonsterGrade p_monstertType = MonsterGrade.NORMAL)
    {
        enemies = new GameObject[1];
        enemies[0] = _enemy;
        monsterType = p_monstertType;
    }

    public void InitEnemies(GameObject[] _enemies)
    {
        enemies = new GameObject[_enemies.Length];
        enemies = _enemies;
    }

    // 몬스터 리스폰 / 생성 
    public void RespwanMonsterFormID(GameObject[] spawnObjects, int id, TeamTag tag)
    {
        if (MonsterDatabase.instance == null || spawnObjects == null) return;
        // 스폰 랜덤한 위치에서 스폰 시키기 
        int currentSpwanIndex = UnityEngine.Random.Range(0, spawnObjects.Length);

        // id값으로 적 오브젝트 생성한다 
        var enemyObject = MonsterDatabase.instance.CreateMonsterUnit(id);
        if(enemyObject.TryGetComponent<CharacterController>(out var characterController) == true)
        {
            characterController.teamTag = tag;
        }

        if (enemyObject == null || 
            (currentSpwanIndex > spawnObjects.Length && spawnObjects[currentSpwanIndex] == null))
            return;

        // 오브젝트 위치 조정 
        enemyObject.transform.position = spawnObjects[currentSpwanIndex].transform.position;
    }

    public void RespawnMonster(GameObject[] _spawns, MonsterGrade  _monsterType = MonsterGrade.NORMAL)
    {
        if (_monsterType != MonsterGrade.BOSS)
        {
            StartCoroutine(CreateEnemy(_spawns, _monsterType));
        }
        else if (_monsterType == MonsterGrade.BOSS)
        {
            StartCoroutine(CreateEnemy(_spawns, _monsterType));
        }
    }

    public void RespawnTestMon(Transform tr_position)
    {
        var Monster = Instantiate(enemies[0], tr_position.position, Quaternion.identity);
        Debug.Log("테스트몬 소환");
    }

    public void SavePlayerTransform(Transform p_Transform)
    {
        tr_PlayerTransform = p_Transform;
    }

    public IEnumerator CreateEnemy(GameObject[] _spawns, MonsterGrade _monsterType = MonsterGrade.NORMAL)
    {
        yield return new WaitForSeconds(createTime);
        //// TODO : 몬스터 관련 정보 수정 
        //var stageTable = StageInfoManager.instance.GetStageInfo();
        //if (stageTable != null)
        //{
        //    var selectSubNum = StageInfoManager.instance.selectMonsterStageNum;
        //    if (StageInfoManager.instance.GetStageInfo().monsterGroups.Count >= selectSubNum) yield return null;

        //    // selectSubNum  변수는 idx값으로 저장되지 않으므로 -1을 해준다. 
        //    var monsterGroups = StageInfoManager.instance.GetStageInfo().monsterGroups[selectSubNum-1];

        //    foreach(var ic in monsterGroups.monsterIdCounts)
        //    {
        //        // 튜플 리스트 (item1, item2)
        //        var id = ic.Item1;

        //        if (id == 0) continue;

        //        var monster = MonsterDatabase.instance.GetMonsterData(id);

        //        int randPos = Random.Range(0, _spawns.Length);
        //        Vector3 direction = go_Player.transform.position -
        //            monster.pf_Monster.transform.position;
        //        //Debug.Log("몬스터타입 " + monsterType + "스탯 " + enemies[0].GetComponent<Status>().MyAttack);

        //        var Monster = Instantiate(monster.pf_Monster, _spawns[randPos].transform.position,
        //            Quaternion.Euler(direction));

        //        // todo : 몬스터 타입별 능력치 값 새로세팅 하기 
        //        // 현재 그냥 1과 2로 구분 지어서 한다 1은 일반 2는 보스용 추후에 각 데이터별로 스탯을 구분지어서 보냄
        //        if(Monster.TryGetComponent<Status>(out var status))
        //        {
        //            if (MonsterGrade.NORMAL == _monsterType || MonsterGrade.ELITE == _monsterType)
        //            {
        //                //mStat.SetStatus(ref status, _monsterType);
        //                SetMonsterStatus(ref status, 0);
        //            }
        //            else if(MonsterGrade.BOSS == _monsterType)
        //            {
        //                //mStat.SetStatus(ref status, _monsterType);
        //                SetMonsterStatus(ref status, 1);
        //            }

        //            // 몬스터별로 패턴 개수가 다르므로 타입으로 해당 패턴을 할지말지 결정하게함
        //            //if (Monster.TryGetComponent<AttackMonster>(out var attackMonster))
        //            //{
        //            //    if (status != null)
        //            //    {
        //            //        attackMonster.MaxPattern = status.MyPattern;
        //            //    }
        //            //}

        //        }


        //        // 생성 될 때, 플레이어의 위치값을 받아냄
        //        Monster.GetComponent<FieldOfViewAngle>().target = tr_PlayerTransform;

        //        curMonsters.Add(Monster);
        //        Monster.transform.LookAt(go_Player.transform.position);

        //        yield return new WaitForSeconds(createTime);

        //    }

        //}

    }


    // 몬스터 스테이터스 설정 하기 
    public void SetMonsterStatus(ref Status status, int _num)
    {
        if (status == null) return;

        MonsterGrade type;
        if (_num == 1)
            type = MonsterGrade.BOSS;
        else
            type = MonsterGrade.NORMAL;

        PlayerData data = MonsterDatabase.instance.GetMonsterStatus(_num);

        if (data == null) return; 

        // todo 여기를 수정하자 
        status.myGrade = type;
        status.MyAttack = data.Attack;
        status.MyAttackDelay = data.AttackSpeed;
        status.MyDefence = data.Defence;
        status.MyHP = data.Hp;
        status.MyMaxHP = data.Hp;
        status.MyWalkSpeed = data.MoveSpeed;
        status.MyEXP = data.Exp;
        status.MyPattern = data.Pattten;
    }
   
    public IEnumerator CreateBoss(GameObject _spawn)
    {
        Vector3 direction = go_Player.transform.position - bossEnemy.transform.position;
        StartCoroutine(OutComeBoss());
        Instantiate(bossEnemy, _spawn.transform.position, Quaternion.Euler(direction));
        bossEnemy.transform.LookAt(go_Player.transform.position);
        yield return null;
    }


    public IEnumerator OutComeBoss()
    {
        yield return null;
        bossWarning.gameObject.SetActive(true);
        bossWarning.Play();
        yield return new WaitForSeconds(2.0f);
        bossWarning.gameObject.SetActive(false);
    }


    public void DeleteAllMonster()
    {
        foreach (var monster in curMonsters)
        {
            Destroy(monster);
        }
    }

    public void DeleteAllEffect()
    {
        
    }

}
