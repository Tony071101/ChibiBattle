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
    private WeaponManager playerWeaponManager;
    public int currentWave { get; private set; }
    private List<GameObject> enemies = new List<GameObject>();

    private void Start() {
        Player player = FindObjectOfType<Player>();
        playerWeaponManager = player.GetComponentInChildren<WeaponManager>();
    }

    public void StartLevel(int level)
    {
        StopAllCoroutines();
        currentWave = 1;
        initialEnemyCount = 5 + (level - 1) * 2;
        StartCoroutine(SpawnWaves(level));
    }

    private IEnumerator SpawnWaves(int level)
    {
        int baseWaves = 3;
        int maxWaves = baseWaves + (level % 2 == 0 ? 1 : 0);
        while (true)
        {
            yield return new WaitForSeconds(5f);
            int extraEnemies = (level == 3 || level == 5) ? 2 : 0;
            int enemyCount = initialEnemyCount + (currentWave - 1) * 2 + extraEnemies;
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
            }

            yield return new WaitUntil(() => enemies.Count == 0);
            currentWave++;
            if(currentWave > maxWaves) {
                currentWave = 1;
                break;
            } else {
                StartCoroutine(UIManager.Instance.ShowGameWave(currentWave));
            }
        }
        levelManager.OnAllWavesCompleted(level);
        GameManager.Instance.GameManagerOnAllEnemiesDefeated();
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
        DropCoinCurrency(new Vector3(enemy.transform.position.x, 
                                        enemy.transform.position.y + 0.1f, 
                                            enemy.transform.position.z + 0.1f));
        if (playerWeaponManager.CurrentWeaponType != WeaponType.MeleeType) {
            DropBulletPack(enemy.transform.position);
        }
        CharacterProgression.Instance.AddExperience();
    }

    private void DropBulletPack(Vector3 position)
    {
        Instantiate(bulletPickupPref, position, Quaternion.identity);
    }

    private void DropCoinCurrency(Vector3 position) {
        Instantiate(coinCurrencyPref, position, Quaternion.identity);
    }

    public void ResetWave() {
        currentWave = 1;
    }
}
