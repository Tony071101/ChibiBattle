using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player UIs.")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider easeHealthSlider;
    [SerializeField] private Slider playerEXPSlider;
    [SerializeField] private TextMeshProUGUI playerHealthTxt;
    [SerializeField] private TextMeshProUGUI playerAmmoTxt;
    [SerializeField] private TextMeshProUGUI playerCoinTxt;
    [SerializeField] private TextMeshProUGUI playerEXPTxt;

    [Header("Level UIs.")]
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI gameLvlTxt;

    [Header("Game Pause UIs.")]
    [SerializeField] private GameObject gamePauseCanvas;
    [SerializeField] private TextMeshProUGUI hpLevelTxt;
    [SerializeField] private TextMeshProUGUI dmgLevelTxt;

    [Header("Game Over UIs.")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private TextMeshProUGUI gameOverTotalCocinTxt;

    [Header("Character Progression UIs.")]
    [SerializeField] private GameObject characterProgressionCanvas;
    [SerializeField] private GameObject btnCharacterProgression;
    [SerializeField] private Transform btnParent;

    [Header("Setting Uis.")]
    [SerializeField] private GameObject canvOptions;
    [SerializeField] private GameObject lineGame;
    [SerializeField] private GameObject lineVideo;
    [SerializeField] private GameObject lineControls;
    [SerializeField] private GameObject lineKeyBindings;
    [SerializeField] private GameObject lineMovement;
    [SerializeField] private GameObject lineCombat;
    [SerializeField] private GameObject lineGeneral;
    [SerializeField] private GameObject PanelControls;
    [SerializeField] private GameObject PanelVideo;
    [SerializeField] private GameObject PanelGame;
    [SerializeField] private GameObject PanelKeyBindings;
    [SerializeField] private GameObject PanelMovement;
    [SerializeField] private GameObject PanelCombat;
    [SerializeField] private GameObject PanelGeneral;
    private Player player;
    private CharacterProgression characterProgression;
    private EnemySpawner enemySpawner;
    private float lerpSpeed = 0.01f;
    private GameObject healthBtn;
    private GameObject damageBtn;
    private GameObject healBtn;
    private bool isAmmoVisible;
    private WeaponManager playerWeaponManager;
    public static UIManager Instance { get; private set; }
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
        characterProgression = CharacterProgression.Instance;
        player = FindObjectOfType<Player>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        playerWeaponManager = player.GetComponentInChildren<WeaponManager>();
        CheckWeaponType();
    }


    private void Update() {
        if(player != null) {
            GetPlayerHealth();
            GetPlayerCoin();
            GetPlayerLevel();
            HealthBarLerp();
            if(isAmmoVisible) {
                GetPlayerAmmo();
            }
        }
    }

    public void ShowMainMenu() {
        Debug.Log("Main Menu Showed.");
    }

    public IEnumerator ShowGameLevel(int level)
    {
        gameLvlTxt.enabled = true;
        gameLvlTxt.text = "Level " + level;
        yield return new WaitForSeconds(5f);
        gameLvlTxt.enabled = false;
        StartCoroutine(ShowGameWave(enemySpawner.currentWave));
    }

    public IEnumerator ShowGameWave(int wave)
    {
        waveTxt.enabled = true;
        waveTxt.text = "Wave " + wave;
        yield return new WaitForSeconds(5f);
        waveTxt.enabled = false;
    }

    public void ShowPauseMenu() {
        gamePauseCanvas.SetActive(true);
        player.DisablePlayerInput();
        GetCharacterProgressionHPLevel();
        GetCharacterProgressionDMGLevel();
    }

    private void DisablePauseMenu() {
        gamePauseCanvas.SetActive(false);
    }

    public void ShowGameOverScreen() {
        gameOverCanvas.SetActive(true);
        gameOverTotalCocinTxt.text = "Total coin: " + player.GetCoinCurrency();
        player.DisablePlayerInput();
    }

    private void DisableGameOverScreen() {
        gameOverCanvas.SetActive(false);
    }

    public void ShowCharacterProgressionScreen() {
        characterProgressionCanvas.SetActive(true);
        player.DisablePlayerInput();
        CreateLevelUpBtn();
    }

    private void DisableCharacterprogressionScreen() {
        characterProgressionCanvas.SetActive(false);
    }

    private void HealthBarLerp() {
        if(playerHealthSlider.value != easeHealthSlider.value) {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, player.healthManagementSystem.currentHealth, lerpSpeed);
        }
    }

    public void GetPlayerHealth() {
        playerHealthSlider.maxValue = player.healthManagementSystem.GetMaxHealth();
        playerHealthSlider.value = player.healthManagementSystem.currentHealth;
        easeHealthSlider.maxValue = player.healthManagementSystem.GetMaxHealth();
        playerHealthTxt.text = player.healthManagementSystem.currentHealth + "/" + player.healthManagementSystem.GetMaxHealth();
    }

    public void GetPlayerAmmo() {
        PlayerAttack playerAttack = player.GetComponent<PlayerAttack>();
        playerAmmoTxt.text = playerAttack.GetCurrentAmmno() + "/" + playerAttack.GetTotalAmmo();
    }

    private void CheckWeaponType()
    {
        if (playerWeaponManager.CurrentWeaponType == WeaponType.MeleeType)
        {
            playerAmmoTxt.enabled = false;
            isAmmoVisible = false;
        }
        else
        {
            playerAmmoTxt.enabled = true; 
            isAmmoVisible = true;
        }
    }

    public void GetPlayerCoin() {
        playerCoinTxt.text = "Coin: " + player.GetCoinCurrency();
    }

    public void GetPlayerLevel() {
        playerEXPSlider.maxValue = CharacterProgression.Instance.GetPointToLvlUp();
        playerEXPSlider.value = CharacterProgression.Instance.experiencePoints;
        playerEXPTxt.text = "Lvl " + CharacterProgression.Instance.level;
    }

    public void GetCharacterProgressionDMGLevel() {
        dmgLevelTxt.text = "DMG Upgrade Level: " + CharacterProgression.Instance.damageUpgradeCount;
    }

    public void GetCharacterProgressionHPLevel() {
        hpLevelTxt.text = "HP Upgrade Level: " + CharacterProgression.Instance.healthUpgradeCount;
    }

    private void CreateLevelUpBtn()
    {
        float btnSpacing = 500f;
        Vector2 startPos = new Vector2(0, 0);

        if(characterProgression.damageUpgradeCount >= characterProgression.maxDamageUpgrades && characterProgression.healthUpgradeCount >= characterProgression.maxHealthUpgrades) {
            CreateHealButton(startPos);
        }
        
        if(characterProgression.healthUpgradeCount < characterProgression.maxHealthUpgrades) {
            healthBtn = CreateButton("Increase Health by " + characterProgression.healthPercentageIncrease + "%",
            () =>
            {
                characterProgression.OnLevelUpHealth();
                player.ApplyBonusHealth();
                OnButtonClicked();
                Destroy(healthBtn);
                Destroy(damageBtn);
            },
            startPos + new Vector2(-btnSpacing, 0));
        }

        if(characterProgression.damageUpgradeCount < characterProgression.maxDamageUpgrades) {
            damageBtn = CreateButton("Increase Damage by " + characterProgression.damagePercentageIncrease + "%",
                () =>
                {
                    characterProgression.OnLevelUpDamage();
                    OnButtonClicked();
                    Destroy(damageBtn);
                    Destroy(healthBtn);
                },
                startPos + new Vector2(btnSpacing, 0));
        }
    }

    private void CreateHealButton(Vector2 startPos)
    {
        healBtn = CreateButton("Heal",
            () =>
            {
                player.healthManagementSystem.Heal(100);
                OnButtonClicked();
                Destroy(healBtn);
            },
            startPos);
    }

    public void OnButtonClicked() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        DisableCharacterprogressionScreen();
        DisablePauseMenu();
        DisableGameOverScreen();
        player.EnablePlayerInput();

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    private GameObject CreateButton(string buttonText, UnityAction onClickAction, Vector2 position) {
        GameObject btn = Instantiate(btnCharacterProgression, btnParent);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        btn.GetComponent<Button>().onClick.AddListener(onClickAction);

        RectTransform buttonRect = btn.GetComponent<RectTransform>();
        buttonRect.anchoredPosition = position;

        return btn;
    }

    public void LoadScene(string sceneName) {
        if(sceneName != "") {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
        }
    }

    public void SettingPanel() {
        GameManager.Instance.isSettingOpen = true;
        canvOptions.SetActive(true);
        gamePauseCanvas.SetActive(false);
    }

    public void ReturnToPauseMenu() {
        GameManager.Instance.isSettingOpen = false;
        canvOptions.SetActive(false);
        gamePauseCanvas.SetActive(true);
    }

    private void DisablePanels() {
        PanelControls.SetActive(false);
        PanelVideo.SetActive(false);
        PanelGame.SetActive(false);
        PanelKeyBindings.SetActive(false);
        
        lineGame.SetActive(false);
        lineControls.SetActive(false);
        lineVideo.SetActive(false);
        lineKeyBindings.SetActive(false);

        PanelMovement.SetActive(false);
        lineMovement.SetActive(false);
        PanelCombat.SetActive(false);
        lineCombat.SetActive(false);
        PanelGeneral.SetActive(false);
        lineGeneral.SetActive(false);
    }

    public void GamePanel(){
        DisablePanels();
        PanelGame.SetActive(true);
        lineGame.SetActive(true);
    }

    public void VideoPanel(){
        DisablePanels();
        PanelVideo.SetActive(true);
        lineVideo.SetActive(true);
    }

    public void ControlsPanel(){
        DisablePanels();
        PanelControls.SetActive(true);
        lineControls.SetActive(true);
    }

    public void KeyBindingsPanel(){
        DisablePanels();
        MovementPanel();
        PanelKeyBindings.SetActive(true);
        lineKeyBindings.SetActive(true);
    }

    public void MovementPanel(){
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelMovement.SetActive(true);
        lineMovement.SetActive(true);
    }

    public void CombatPanel(){
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelCombat.SetActive(true);
        lineCombat.SetActive(true);
    }

    public void GeneralPanel(){
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelGeneral.SetActive(true);
        lineGeneral.SetActive(true);
    }
}
