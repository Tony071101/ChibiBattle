using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.PlasticSCM.Editor.WebApi;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using SlimUI.ModernMenu;
[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Animator _anim;
    protected Camera _mainCamera;
    protected WeaponManager _weaponManager;
    public HealthManagementSystem healthManagementSystem { get; private set; }
    #region InputSystem
    protected PlayerInput _playerInput;
    protected InputAction moveAction;
    protected InputAction attackAction;
    protected InputAction cameraLookAction; 
    protected InputAction reloadAction;
    #endregion
    private int coinCurrency = 0;
    protected float xSensitivity;
    protected float ySensitivity;
    protected float mouseSmoothing;
    private bool isDead;
    #region Basic Functions
    protected virtual void Awake() {
    }
    protected virtual void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _anim = GetComponentInChildren<Animator>();
        _weaponManager = GetComponentInChildren<WeaponManager>();

        healthManagementSystem = GetComponentInChildren<HealthManagementSystem>();

        moveAction = _playerInput.actions.FindAction("Move");
        attackAction = _playerInput.actions.FindAction("Attack");
        cameraLookAction = _playerInput.actions.FindAction("CameraLook");
        reloadAction = _playerInput.actions.FindAction("Reload");

        UISettingsManager.SettingsUpdated += UpdatePlayerSettingsFromScene;

        DisablePlayerInput();
        UpdatePlayerSettings();
    }
    protected virtual void Update() { 
        PlayerDeath(); 
    }
    protected virtual void FixedUpdate() {}
    #endregion

    #region Children Functions
    protected virtual void OnReload(){}
    protected virtual void OnMove(){}
    protected virtual void OnAttack(){}
    #endregion

    #region Get/Set Functions
    public void DisablePlayerInput() { 
        if (_playerInput != null) {
            _playerInput.enabled = false;
        }
    }
    public void EnablePlayerInput() { 
        if (_playerInput != null) {
            _playerInput.enabled = true; 
        }
    }
    public void ResetIsDead() { isDead = false; }
    public int GetCoinCurrency() { return coinCurrency; }
    public void SetCoinCurrency(int amount) { coinCurrency = amount; }
    public void AddCoins(int amount) { coinCurrency += amount; }
    #endregion
    
    #region Logic Functions
    private void UpdatePlayerSettingsFromScene(object sender, EventArgs e) {
        UpdatePlayerSettings();
    }
    public void UpdatePlayerSettings()
    {
        xSensitivity = PlayerPrefs.GetFloat("XSensitivity", 1f);
        ySensitivity = PlayerPrefs.GetFloat("YSensitivity", 1f);
        mouseSmoothing = PlayerPrefs.GetFloat("MouseSmoothing", 0f);
    }

    private void DisablePlayerActions()
    {
        _anim.SetTrigger(AnimationStrings.death);
        DisablePlayerInput();
    }

    private void PlayerDeath()
    {
        if (healthManagementSystem != null && healthManagementSystem.currentHealth <= 0 && isDead == false)
        {
            DisablePlayerActions();
            StartCoroutine(WaitForDeathAnim());
            isDead = true;
        }
    }

    private IEnumerator WaitForDeathAnim() {
        float deathAnimLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimLength);
        GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
    }

    public void ApplyBonusHealth() {
        if(healthManagementSystem != null) {
            int additionalHealth = Mathf.RoundToInt(CharacterProgression.Instance.bonusHealth);
            healthManagementSystem.IncreaseMaxHealth(additionalHealth);
        }
    }
    #endregion
}