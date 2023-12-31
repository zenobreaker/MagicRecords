using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 맵에 나타나는 전반적인 트랩을 관리하도록 유도하는 클래스 
public class TrapContoller : MonoBehaviour
{
    public static TrapContoller instance; 

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // 함정이 나타날 위치 오브젝트
    List<GameObject> trapSpawnList = new List<GameObject>();

    public void SetSpawnObject()
    {
        if (GameManager.MyInstance == null)
            return;

        trapSpawnList = GameManager.MyInstance.GetCurrentMapTrapList();
    }

    // 함정 생성 (대상을 향해 움직이는 형태)
    public void CreateTrapByMoveType(int count, Vector3 targetVec, int trapPower)
    {
        if (trapSpawnList.Count == 0)
            return;

        // 정해진 수치 만큼 함정 오브젝트들의 배치 
        // 개수보다 오브젝트들의 수가 적다면 배치된 오브젝트에서 다나온다

        // 오브젝트는 생성될 때 정해진 곳을 향해 날아가도록 세팅된다. 

        // 오브젝트 리스트에서 함정이 등장할 오브젝트 추려내기 
        List<int> trapNumList = new List<int>();

        int max = trapSpawnList.Count;

        for (int i = 0; i < max; i++)
        {
            int rand = Random.Range(0, max - 1);
            if (trapNumList.Find(x => x == rand) != -1)
                trapNumList.Add(rand);
        }

        // 간추려낸 리스트의 값을 꺼내면서 해당 값에 오브젝트에 트랩 생성
        for (int i = 0; i < trapNumList.Count; i++)
        {
            int idx = trapNumList[i];
            var rot = targetVec - trapSpawnList[idx].transform.localPosition;
            Quaternion quaternion = Quaternion.LookRotation(rot);
            var trap = ObjectPooler.SpawnFromPool<EnemyBullet>("TrapBulletObject", trapSpawnList[idx].transform.position,
                quaternion);

            // 무시할 레이어 추가 
            trap.SetIgnoreLayer("Wall");
            trap.SetIgnoreLayer("Land");
            trap.SetIgnoreLayer("SpecialArea");

            // 함정 위력 추가 
            trap.MyDamage = trapPower;

        }
    }
}
