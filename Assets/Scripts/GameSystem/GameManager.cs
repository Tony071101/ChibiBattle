using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    private int currentLevel = 1;
    [SerializeField] private LevelManager levelManager;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeState(GameState.Playing);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        HandleGameStateChanged(newState);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                // Initialize MainMenu
                // uiManager.ShowMainMenu();
                break;
            case GameState.Playing:
                // Start the game
                StartGame();
                break;
            case GameState.Paused:
                // Pause the game
                PauseGame();
                break;
            case GameState.GameOver:
                // End the game
                EndGame();
                break;
        }
    }

    private void StartGame()
    {
        Debug.Log("GameManager Start Level " + currentLevel);
        StartCoroutine(levelManager.StartLevelWithDelay(currentLevel));
        // uiManager.ShowGameHUD();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        // uiManager.ShowPauseMenu();
    }

    private void EndGame()
    {
        Time.timeScale = 1;
        // uiManager.ShowGameOverScreen();
    }

    public void GameManagerOnAllEnemiesDefeated()
    {
        levelManager.OnAllEnemiesDefeated(currentLevel);
    }

    public void PlayerLevelUp() {
        // UImanager Handle what happens when the player levels up
        Debug.Log("Player leveled up!");
    }
}
