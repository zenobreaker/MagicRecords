using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �ʿ� ��Ÿ���� �������� Ʈ���� �����ϵ��� �����ϴ� Ŭ���� 
public class TrapContoller : MonoBehaviour
{
    public static TrapContoller instance; 

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // ������ ��Ÿ�� ��ġ ������Ʈ
    List<GameObject> trapSpawnList = new List<GameObject>();

    public void SetSpawnObject()
    {
        if (GameManager.MyInstance == null)
            return;

        trapSpawnList = GameManager.MyInstance.GetCurrentMapTrapList();
    }

    // ���� ���� (����� ���� �����̴� ����)
    public void CreateTrapByMoveType(int count, Vector3 targetVec, int trapPower)
    {
        if (trapSpawnList.Count == 0)
            return;

        // ������ ��ġ ��ŭ ���� ������Ʈ���� ��ġ 
        // �������� ������Ʈ���� ���� ���ٸ� ��ġ�� ������Ʈ���� �ٳ��´�

        // ������Ʈ�� ������ �� ������ ���� ���� ���ư����� ���õȴ�. 

        // ������Ʈ ����Ʈ���� ������ ������ ������Ʈ �߷����� 
        List<int> trapNumList = new List<int>();

        int max = trapSpawnList.Count;

        for (int i = 0; i < max; i++)
        {
            int rand = Random.Range(0, max - 1);
            if (trapNumList.Find(x => x == rand) != -1)
                trapNumList.Add(rand);
        }

        // ���߷��� ����Ʈ�� ���� �����鼭 �ش� ���� ������Ʈ�� Ʈ�� ����
        for (int i = 0; i < trapNumList.Count; i++)
        {
            int idx = trapNumList[i];
            var rot = targetVec - trapSpawnList[idx].transform.localPosition;
            Quaternion quaternion = Quaternion.LookRotation(rot);
            var trap = ObjectPooler.SpawnFromPool<EnemyBullet>("TrapBulletObject", trapSpawnList[idx].transform.position,
                quaternion);

            // ������ ���̾� �߰� 
            trap.SetIgnoreLayer("Wall");
            trap.SetIgnoreLayer("Land");
            trap.SetIgnoreLayer("SpecialArea");

            // ���� ���� �߰� 
            trap.MyDamage = trapPower;

        }
    }
}
