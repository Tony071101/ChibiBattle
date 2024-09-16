using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject gamePauseCanvas;
    [SerializeField] private GameObject characterProgressionCanvas;
    [SerializeField] private GameObject btnCharacterProgression;
    [SerializeField] private Transform btnParent;
    private Player player;
    private CharacterProgression characterProgression;
    private EnemySpawner enemySpawner;
    private float lerpSpeed = 0.01f;
    // private HealthManagementSystem healthManagementSystem;
    public static UIManager Instance { get; private set; }
    // Start is called before the first frame update
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
        //Might need to disable other UIs.
        gameOverCanvas.SetActive(true);
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
        playerAmmoTxt.text = player.GetCurrentAmmno() + "/" + player.GetTotalAmmo();
    }

    public void GetPlayerCoin() {
        playerCoinTxt.text = "Coin: " + player.coinCurrency;
    }

    public void GetPlayerLevel() {
        playerEXPSlider.maxValue = CharacterProgression.Instance.GetPointToLvlUp();
        playerEXPSlider.value = CharacterProgression.Instance.experiencePoints;
        playerEXPTxt.text = "Lvl " + CharacterProgression.Instance.level;
    }

    private void CreateLevelUpBtn() {
        float btnSpacing = 700f;
        Vector2 startPos = new Vector2(0, 0);

        GameObject healthBtn = Instantiate(btnCharacterProgression, btnParent);
        healthBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Increase Health by " + characterProgression.healthPercentageIncrease + "%";
        healthBtn.GetComponent<Button>().onClick.AddListener(() => {
            characterProgression.OnLevelUpHealth();
            player.ApplyBonusHealth();
            OnButtonClicked();
        });

        RectTransform healthButtonRect = healthBtn.GetComponent<RectTransform>();
        healthButtonRect.anchoredPosition = startPos + new Vector2(-btnSpacing, 0);

        GameObject damageBtn = Instantiate(btnCharacterProgression, btnParent);
        damageBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Increase Damage by " + characterProgression.damagePercentageIncrease + "%";
        damageBtn.GetComponent<Button>().onClick.AddListener(() => {
            characterProgression.OnLevelUpDamage();
            OnButtonClicked();
        });

        RectTransform damageButtonRect = damageBtn.GetComponent<RectTransform>();
        damageButtonRect.anchoredPosition = startPos;

        GameObject healBtn = Instantiate(btnCharacterProgression, btnParent);
        healBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Heal";
        healBtn.GetComponent<Button>().onClick.AddListener(() => {
            player.healthManagementSystem.Heal(100); 
            OnButtonClicked();
        });

        RectTransform healButtonRect = healBtn.GetComponent<RectTransform>();
        healButtonRect.anchoredPosition = startPos + new Vector2(btnSpacing, 0);
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
}
