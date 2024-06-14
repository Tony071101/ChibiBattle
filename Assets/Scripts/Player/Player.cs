using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting.Dependencies.Sqlite;
[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    protected Rigidbody _rigidbody;
    protected Animator _anim;
    protected Camera _mainCamera;
    protected WeaponManager _weaponManager;
    private HealthManagementSystem healthManagementSystem;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    #region InputSystem
    protected PlayerInput _playerInput;
    protected InputAction moveAction;
    protected InputAction attackAction;
    protected InputAction cameraLookAction; 
    protected InputAction reloadAction;
    #endregion

    [HideInInspector] [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    protected Vector3 mouseWorldPosition;
    private float angle;
    private float currentVelocity;
    private float smoothRotationTime = 0.05f;
    private float rotateSpeed = 5f;
    protected int maxAmmo = 90;
    protected int currentAmmo = 30;
    protected int totalAmmo;
    protected Transform spawnBulletPos;
    protected bool _isMoving = false;
    protected bool IsMoving {
        get { return _isMoving; }
        set {
            _isMoving = value;
            _anim.SetBool(AnimationStrings.isMoving, value);
        }
    }

    protected bool _isReloading = false;
    protected bool IsReloading {
        get { return _isReloading; }
        set
        {
            _isReloading = value;
            _anim.SetBool(AnimationStrings.isReloading, value);
        }
    }

    private void Awake() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        _anim = GetComponentInChildren<Animator>();
        _weaponManager = GetComponentInChildren<WeaponManager>();
        healthManagementSystem = GetComponentInChildren<HealthManagementSystem>();
        _mainCamera = Camera.main;
        moveAction = _playerInput.actions.FindAction("Move");
        attackAction = _playerInput.actions.FindAction("Attack");
        cameraLookAction = _playerInput.actions.FindAction("CameraLook");
        reloadAction = _playerInput.actions.FindAction("Reload");
        totalAmmo = maxAmmo;
        spawnBulletPos = GameObject.FindGameObjectWithTag("SpawnBulletPos").transform;
    }

    private void Update() {
        if(healthManagementSystem != null && healthManagementSystem.currentHealth <= 0) {
            DisablePlayerActions();
        }
    }

    private void FixedUpdate() {
        OnMove();
        OnAttack();
        OnReload();
    }

    protected virtual void OnReload(){}
    protected virtual void OnMove(){}
    protected virtual void OnAttack(){}
    protected void Rotate(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothRorate = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref currentVelocity,
            smoothRotationTime);
            if(attackAction.ReadValue<float>() != 0f) {
                //This is need to be empty to lock rotation when move while aimed.
            }else{
                transform.rotation = Quaternion.Euler(0, smoothRorate, 0);
            }
        }
    }

    protected void RotateCameraWhenAiming(Vector2 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            float mouseX = direction.x;
            Quaternion targetRotation = Quaternion.Euler(0f, _mainCamera.transform.eulerAngles.y + mouseX, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
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
        if (_playerInput != null) _playerInput.enabled = false;
    }
}
