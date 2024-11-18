using System.Collections;
using System.Collections.Generic;
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
    private List<CharacterData> characterDatas = new List<CharacterData>();
    [SerializeField] private AudioSource audioSource;
    private GameObject selectedCharacterModel;
    private const string TotalCoinKey = "TotalCoin";
    public bool isSettingOpen { get; set; } = false;
    private CharacterData selectedCharacter;
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
        int gameModeValue = PlayerPrefs.GetInt("GameMode", (int)GameMode.NormalGameMode);
        currentGameMode = (GameMode)gameModeValue;
        LoadCharacterDatas();
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
        audioSource.volume = PlayerPrefs.GetFloat("CharacterVolume");
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

    public void UpdateVolume (){
        audioSource.volume = PlayerPrefs.GetFloat("CharacterVolume");
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

    private void LoadCharacterDatas()
    {
        CharacterData[] loadedCharacterDatas = Resources.LoadAll<CharacterData>("CharacterDatas");
        characterDatas.AddRange(loadedCharacterDatas);
    }

    private void LoadSelectedCharacter() {
        string selectedCharacterName = PlayerPrefs.GetString("SelectedCharacter");
        selectedCharacter = characterDatas.Find(character => character.characterName == selectedCharacterName);

        if(selectedCharacter != null) {
            selectedCharacterModel = Instantiate(selectedCharacter.characterModel, player.transform.position, Quaternion.identity, player.transform);
            audioSource.clip = selectedCharacter.onGameStart;
            audioSource.Play();
        } else {
            Debug.LogError("Can't find selected character!");
        }
    }

    public int GetCharacterBaseDamage() {
        if (selectedCharacter != null) {
            return selectedCharacter.baseDamage;
        } else {
            return 0;
        }
    }

    public int GetCharacterBaseHealth() {
        if (selectedCharacter != null) {
            return selectedCharacter.baseHealth;
        } else {
            return 0;
        }
    }


    public void AudioOnMove() {
        if(selectedCharacter != null && selectedCharacter.onMove != null) {
            audioSource.clip = selectedCharacter.onMove;
            audioSource.Play();
        }
    }

    public void AudioOnHurt() {
        if(selectedCharacter != null && selectedCharacter.onHurt != null) {
            audioSource.clip = selectedCharacter.onHurt;
            audioSource.Play();
        }
    }

    public void AudioAttackSFX() {
        if(selectedCharacter != null && selectedCharacter.attackSFX != null) {
            audioSource.clip = selectedCharacter.attackSFX;
            audioSource.Play();
        }
    }
}
