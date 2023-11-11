using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private Transform field = null;

    [SerializeField]
    private GameObject enemy = null;

    private GameObject[] enemys;

    public int maxEnemy = 2;

    // Start is called before the first frame update
    void Start()
    {
        enemys = new GameObject[maxEnemy];
        // 배열 초기화 

        StartCoroutine(GeneratorEnemy());
    }

    IEnumerator GeneratorEnemy()
    {
        while (true)
        {
            Generate();
            yield return new WaitForSeconds(3.0f);
        }

    }

    void Generate()
    {
        for (int enemyCount = 0; enemyCount < enemys.Length; enemyCount++)
        {
            if (enemys[enemyCount] == null)
            {
                float xField = Random.Range(-90, field.position.x + 90);
                float zField = Random.Range(-90, field.position.z + 90);
                enemys[enemyCount] = Instantiate(enemy, new Vector3(xField, field.position.y + 10, zField), Quaternion.identity)
              as GameObject;

                return;
            }
        }
    }
}

