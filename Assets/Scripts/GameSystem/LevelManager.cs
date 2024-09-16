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

    public void OnAllWavesCompleted(int level)
    {
        StartCoroutine(HandleLevelCompletion(level));
    }

    private IEnumerator HandleLevelCompletion(int level)
    {
        int maxLevel = 5;
        if(level < maxLevel) {
            yield return StartCoroutine(UIManager.Instance.ShowGameLevel(level + 1));
            enemySpawner.StartLevel(level + 1);
        } else {
            yield break;
        }
    }

    public void OnAllEnemiesDefeated(int currentLevel)
    {
        StartCoroutine(StartLevelWithDelay(currentLevel));
    }
}
