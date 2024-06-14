using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPref;
    private int initialEnemyCount = 5;
    private float spawnRangeX = 190f;
    private float spawnRangeZ = 190f;
    private int currentWave = 0;
    private List<GameObject> enemies = new List<GameObject>();

    private void Start() {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < 3)
        {
            yield return new WaitForSeconds(5f);
            currentWave++;
            int enemyCount = initialEnemyCount + (currentWave - 1) * 5;

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }

            // Đợi cho đến khi tất cả kẻ thù trong wave hiện tại bị tiêu diệt
            yield return new WaitUntil(() => enemies.Count == 0);
            // Tăng số lượng kẻ thù cho wave tiếp theo
        }
    }

    private void SpawnEnemy()
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-spawnRangeX, spawnRangeX),
            0,
            UnityEngine.Random.Range(-spawnRangeZ, spawnRangeZ)
        );
        GameObject enemy = Instantiate(enemyPref, randomPosition, Quaternion.identity);
        enemies.Add(enemy);
        HealthManagementSystem healthSystem = enemy.GetComponent<EnemyReferences>().healthManagementSystem;
        healthSystem.OnDeath += (sender, args) => OnEnemyDeath(enemy);
    }

    private void OnEnemyDeath(GameObject enemy) {
        enemies.Remove(enemy);
        Destroy(enemy, 5f);
    }
}
