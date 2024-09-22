using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, StartGame, Playing, Paused, GameOver } //Will be modify later.
    public GameState CurrentState { get; private set; }
    private EnemySpawner enemySpawner;
    private int currentLevel;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private PlayerData playerData;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Instance = this;
        }
    }

    private void Start()
    {
        ChangeState(GameState.StartGame);
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        HandleGameStateChanged(newState);
        Debug.Log(CurrentState);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                // Initialize MainMenu
                UIManager.Instance.ShowMainMenu();
                break;
            case GameState.StartGame:
                // Start the game
                StartGame();
                ResetPlayerState();
                break;
            case GameState.Playing:
                // Playing
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

    private void Update() {
        // Kiểm tra nếu nhấn phím Esc
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (CurrentState == GameState.Playing) {
                // Nếu đang chơi, nhấn Esc sẽ tạm dừng game
                ChangeState(GameState.Paused);
            } else if (CurrentState == GameState.Paused) {
                // Nếu đang tạm dừng, nhấn Esc sẽ tiếp tục game
                ResumeGame();
            }
        }
    }

    // Hàm tiếp tục game khi nhấn nút "Continue" hoặc phím Esc lần nữa
    private void ResumeGame() {
        UIManager.Instance.OnButtonClicked();
    }

    private void StartGame()
    {
        currentLevel = 1;
        StartCoroutine(levelManager.StartLevelWithDelay(currentLevel));
        StartCoroutine(UIManager.Instance.ShowGameLevel(currentLevel));
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowPauseMenu();
    }

    private void EndGame()
    {
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOverScreen();
        UpdateCoinCurrencyInPlayerData();
    }

    public void GameManagerOnAllEnemiesDefeated()
    {
        int maxLevel = 5;
        if(currentLevel == maxLevel) {
            ChangeState(GameState.GameOver);
        }
        else {
            currentLevel++;
            levelManager.OnAllEnemiesDefeated(currentLevel);
        }
    }

    public void PlayerLevelUp() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        UIManager.Instance.ShowCharacterProgressionScreen();
    }

    private void ResetPlayerState() {
        Player player = FindObjectOfType<Player>();
        if(player != null) {
            player.ResetIsDead();
        }
    }

    private void UpdateCoinCurrencyInPlayerData() {
        Player player = FindObjectOfType<Player>();
        if(player != null && playerData != null) {
            playerData.totalCoin += player.GetCoinCurrency();
        }
    }
}
