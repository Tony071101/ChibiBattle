using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private EnemySpawner enemySpawner;
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    public IEnumerator StartLevelWithDelay(int level)
    {
        yield return new WaitForSeconds(5f);
        StartLevel(level);
    }

    private void StartLevel(int level)
    {
        enemySpawner.StartLevel(level);
    }

    public void OnAllEnemiesDefeated(int currentLevel)
    {
        if (currentLevel < 5)
        {
            currentLevel++;
            StartCoroutine(StartLevelWithDelay(currentLevel));
            Debug.LogError("Level " + currentLevel);
        }
        else
        {
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
        }
    }
}
