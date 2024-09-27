using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider easeHealthSlider;
    [SerializeField] private Slider playerEXPSlider;
    [SerializeField] private TextMeshProUGUI playerHealthTxt;
    [SerializeField] private TextMeshProUGUI playerAmmoTxt;
    [SerializeField] private TextMeshProUGUI playerCoinTxt;
    [SerializeField] private TextMeshProUGUI playerEXPTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI gameLvlTxt;
    [SerializeField] private TextMeshProUGUI gameOverTotalCocinTxt;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gamePauseCanvas;
    [SerializeField] private GameObject characterProgressionCanvas;
    [SerializeField] private GameObject btnCharacterProgression;
    [SerializeField] private Transform btnParent;
    private Player player;
    private CharacterProgression characterProgression;
    private EnemySpawner enemySpawner;
    private float lerpSpeed = 0.01f;
    private GameObject healthBtn;
    private GameObject damageBtn;
    public static UIManager Instance { get; private set; }
    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    private void Start() {
        characterProgression = CharacterProgression.Instance;
        player = FindObjectOfType<Player>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    //Change this not to use uppdate.
    private void Update() {
        if(player != null) {
            GetPlayerHealth();
            GetPlayerAmmo();
            GetPlayerCoin();
            GetPlayerLevel();
            HealthBarLerp();
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

    public void GetPlayerCoin() {
        playerCoinTxt.text = "Coin: " + player.GetCoinCurrency();
    }

    public void GetPlayerLevel() {
        playerEXPSlider.maxValue = CharacterProgression.Instance.GetPointToLvlUp();
        playerEXPSlider.value = CharacterProgression.Instance.experiencePoints;
        playerEXPTxt.text = "Lvl " + CharacterProgression.Instance.level;
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
            },
            startPos + new Vector2(-btnSpacing, 0));
        } else {
            Destroy(healthBtn);
        }

        if(characterProgression.damageUpgradeCount < characterProgression.maxDamageUpgrades) {
            damageBtn = CreateButton("Increase Damage by " + characterProgression.damagePercentageIncrease + "%",
                () =>
                {
                    characterProgression.OnLevelUpDamage();
                    OnButtonClicked();
                },
                startPos + new Vector2(btnSpacing, 0));
        } else {
            Destroy(damageBtn);
        }
        
    }

    private void CreateHealButton(Vector2 startPos)
    {
        CreateButton("Heal",
            () =>
            {
                player.healthManagementSystem.Heal(100);
                OnButtonClicked();
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
}
