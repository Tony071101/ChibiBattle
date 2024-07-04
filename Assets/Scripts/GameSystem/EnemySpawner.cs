using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPref;
    [SerializeField] private GameObject bulletPickupPref;
    [SerializeField] private GameObject coinCurrencyPref;
    [SerializeField] private LevelManager levelManager;
    private int initialEnemyCount = 5;
    private float spawnRangeX = 190f;
    private float spawnRangeZ = 190f;
    private int currentWave = 0;
    private List<GameObject> enemies = new List<GameObject>();

    public void StartLevel(int level)
    {
        StopAllCoroutines();
        currentWave = 0;
        initialEnemyCount = 5 + (level - 1) * 2;
        StartCoroutine(SpawnWaves(level));
    }

    private IEnumerator SpawnWaves(int level)
    {
        int maxWaves = 3 + (level / 2);
        while (currentWave < 3)
        {
            yield return new WaitForSeconds(5f);
            currentWave++;
            int enemyCount = initialEnemyCount + (currentWave - 1) * 2; //Plus + 2 enemies each waves.

            Debug.LogError("Wave: " + currentWave);
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }

            // Đợi cho đến khi tất cả kẻ thù trong wave hiện tại bị tiêu diệt
            yield return new WaitUntil(() => enemies.Count == 0);
            // Tăng số lượng kẻ thù cho wave tiếp theo
        }

        levelManager.OnAllEnemiesDefeated();
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
        //Drop Currency.
        DropCoinCurrency(new Vector3(enemy.transform.position.x, 
                                        enemy.transform.position.y + 0.1f, 
                                            enemy.transform.position.z + 0.1f));
        //Drop Bullets.
        DropBulletPack(enemy.transform.position);
    }

    private void DropBulletPack(Vector3 position)
    {
        Instantiate(bulletPickupPref, position, Quaternion.identity);
    }

    private void DropCoinCurrency(Vector3 position) {
        Instantiate(coinCurrencyPref, position, Quaternion.identity);
    }
}
