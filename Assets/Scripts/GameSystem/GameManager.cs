using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, StartGame, Playing, Paused, GameOver } //Will be modify later.
    public GameState CurrentState { get; private set; }
    public GameMode currentGameMode { get; private set; }
    private EnemySpawner enemySpawner;
    private Player player;
    private int currentLevel;
    private int maxLevel = 5;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InputActionAsset resetInput;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private List<PlayerData> playerDatas = new List<PlayerData>();
    [SerializeField] private AudioSource audioSource;
    private GameObject selectedCharacterModel;
    private const string TotalCoinKey = "TotalCoin";
    public bool isSettingOpen { get; set; } = false;
    private PlayerData selectedPlayer;
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
        int gameModeValue = PlayerPrefs.GetInt("GameMode", (int)GameMode.NormalGameMode);
        currentGameMode = (GameMode)gameModeValue;
    }

    private void Start()
    {
        if (currentGameMode == GameMode.EndlessGameMode)
        {
            maxLevel = int.MaxValue;
        }
        ChangeState(GameState.StartGame);
        enemySpawner = FindObjectOfType<EnemySpawner>();
        player = FindObjectOfType<Player>();
        LoadSelectedCharacter();
    }

    private void OnEnable() {
        pauseAction.action.performed += OnPause;
        pauseAction.action.Enable();
    }

    private void OnDisable() {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();
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
                break;
            case GameState.StartGame:
                // Start the game
                StartGame();
                ResetPlayerState();
                break;
            case GameState.Playing:
                // Playing
                if (player != null)
                {
                    player.EnablePlayerInput();
                }
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

    private void OnPause(InputAction.CallbackContext context) {
        if (CurrentState == GameState.Playing) {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame() {
        if(CurrentState == GameState.Paused && isSettingOpen == false) {
            UIManager.Instance.OnButtonClicked();
        }
    }

    private void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentLevel = 1;
        StartCoroutine(levelManager.StartLevelWithDelay(currentLevel));
        StartCoroutine(UIManager.Instance.ShowGameLevel(currentLevel));
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        UIManager.Instance.ShowPauseMenu();
    }

    private void EndGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        UIManager.Instance.ShowGameOverScreen();
        UpdateCoinCurrencyInPlayerData();
    }

    public void GameManagerOnAllEnemiesDefeated()
    {
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
        if(player != null) {
            player.ResetIsDead();
        }
    }

    private void UpdateCoinCurrencyInPlayerData() {
        if(player != null) {
            int currentTotalCoin = PlayerPrefs.GetInt(TotalCoinKey, 0);
            int newTotalCoin = currentTotalCoin + player.GetCoinCurrency();
            PlayerPrefs.SetInt(TotalCoinKey, newTotalCoin);
            PlayerPrefs.Save();
        }
    }
    
    public void ResetBinding() {
        foreach(InputActionMap map in resetInput.actionMaps) {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    private void LoadSelectedCharacter() {
        string selectedCharacterName = PlayerPrefs.GetString("SelectedCharacter");

        selectedPlayer = playerDatas.Find(player => player.characterName == selectedCharacterName);

        if(selectedPlayer != null) {
            selectedCharacterModel = Instantiate(selectedPlayer.characterModel, player.transform.position, Quaternion.identity, player.transform);
            audioSource.clip = selectedPlayer.onGameStart;
            audioSource.Play();
        } else {
            Debug.LogError("Can't dinf selected character!");
        }
    }

    public void AudioOnMove() {
        if(selectedPlayer != null && selectedPlayer.onMove != null) {
            audioSource.clip = selectedPlayer.onMove;
            ResetVolume();
            audioSource.Play();
        }
    }

    public void AudioOnHurt() {
        if(selectedPlayer != null && selectedPlayer.onHurt != null) {
            audioSource.clip = selectedPlayer.onHurt;
            ResetVolume();
            audioSource.Play();
        }
    }

    public void AudioAttackSFX() {
        if(selectedPlayer != null && selectedPlayer.attackSFX != null) {
            audioSource.clip = selectedPlayer.attackSFX;
            audioSource.volume = 0.2f;
            audioSource.Play();
        }
    }

    private void ResetVolume() {
        audioSource.volume = 1f;
    }
}
