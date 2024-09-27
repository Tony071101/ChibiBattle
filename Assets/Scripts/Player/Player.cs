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
[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [HideInInspector] [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    protected Rigidbody _rigidbody;
    protected Animator _anim;
    protected Camera _mainCamera;
    protected WeaponManager _weaponManager;
    public HealthManagementSystem healthManagementSystem { get; private set; }
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    #region InputSystem
    protected PlayerInput _playerInput;
    protected InputAction moveAction;
    protected InputAction attackAction;
    protected InputAction cameraLookAction; 
    protected InputAction reloadAction;
    #endregion
    protected Vector3 mouseWorldPosition;
    private float angle;
    private float currentVelocity;
    private float smoothRotationTime = 0.05f;
    private bool isDead = false;
    private float rotateSpeed = 5f;
    protected int currentAmmo = 30;
    protected int totalAmmo = 90;
    private int coinCurrency = 0;
    
    protected virtual void Awake() {}
    protected virtual void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main;
        _weaponManager = GetComponentInChildren<WeaponManager>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        healthManagementSystem = GetComponentInChildren<HealthManagementSystem>();
        moveAction = _playerInput.actions.FindAction("Move");
        attackAction = _playerInput.actions.FindAction("Attack");
        cameraLookAction = _playerInput.actions.FindAction("CameraLook");
        reloadAction = _playerInput.actions.FindAction("Reload");
    }

    protected virtual void Update() { PlayerDeath(); }
    protected virtual void FixedUpdate() {}
    private void PlayerDeath()
    {
        if (healthManagementSystem != null && healthManagementSystem.currentHealth <= 0 && !isDead)
        {
            DisablePlayerActions();
            StartCoroutine(WaitForDeathAnim());
            isDead = true;
        }
    }


    protected virtual void OnReload(){}
    protected virtual void OnMove(){}
    protected virtual void OnAttack(){}
    protected void Rotate(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothRotate = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref currentVelocity,
            smoothRotationTime);
            smoothRotate = Mathf.Repeat(smoothRotate, 360f);
            if (attackAction.ReadValue<float>() == 0f) 
            {
                transform.rotation = Quaternion.Euler(0, smoothRotate, 0);
            }
        }
    }

    protected void RotateCameraWhenAiming(Vector2 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            float mouseX = direction.x;
            // Quaternion targetRotation = Quaternion.Euler(0f, _mainCamera.transform.eulerAngles.y + mouseX, 0f);
            float adjustedAngle = Mathf.Repeat(_mainCamera.transform.eulerAngles.y + mouseX, 360f);
            Quaternion targetRotation = Quaternion.Euler(0f, adjustedAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * rotateSpeed);
        }
    }

    protected void CheckMouseOnWorldSpace() {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = _mainCamera.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            mouseWorldPosition = raycastHit.point;
        }
    }

    private void DisablePlayerActions()
    {
        _anim.SetTrigger(AnimationStrings.death);
        DisablePlayerInput();
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
    public void DisablePlayerInput() { if (_playerInput != null) _playerInput.enabled = false; }
    public void EnablePlayerInput() { if (_playerInput != null) _playerInput.enabled = true; }
    public void ResetIsDead() { isDead = false; }
    public int GetCoinCurrency() { return coinCurrency; }
    public void SetCoinCurrency(int amount) { coinCurrency = amount; }
    public void AddCoins(int amount) { coinCurrency += amount; }
}