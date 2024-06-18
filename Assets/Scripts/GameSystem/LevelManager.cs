using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    private int currentLevel = 1;

    private void Start()
    {
        Debug.Log("Start Level " + currentLevel);
        StartCoroutine(StartLevelWithDelay(currentLevel));
    }

    private IEnumerator StartLevelWithDelay(int level)
    {
        yield return new WaitForSeconds(5f);
        StartLevel(level);
    }

    private void StartLevel(int level)
    {
        enemySpawner.StartLevel(level);
    }

    public void OnAllEnemiesDefeated()
    {
        if (currentLevel < 5)
        {
            currentLevel++;
            StartCoroutine(StartLevelWithDelay(currentLevel));
            Debug.Log("Level " + currentLevel);
        }
        else
        {
            // Game hoàn tất hoặc xử lý khác nếu level > 5
            Debug.Log("Game Completed!");
        }
    }
}
