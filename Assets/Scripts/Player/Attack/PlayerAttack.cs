using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Animations;
using Unity.Mathematics;

public class PlayerAttack : Player
{
    [HideInInspector] [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [HideInInspector] [SerializeField] private CinemachineVirtualCamera playerCamera;
    [HideInInspector] [SerializeField] private Transform bullet_Prefs;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    private Transform spawnBulletPos;
    private float lastSpawnTime = 0f;
    private int currentAmmo = 30;
    private int totalAmmo = 90;
    private float bulletSpawnDelay = 0.3f; //can be modified.
    private float reloadTime = 1.8f;
    private Vector2 previousInput = Vector2.zero;
    private bool isAiming = false;
    private Vector3 mouseWorldPosition;
    private float smoothingFactor = 0.1f;
    private float rotateSpeed = 5f;
    private bool playerRotatedToCamera = false;
    private bool _isReloading = false;
    private bool IsReloading {
        get { return _isReloading; }
        set
        {
            _isReloading = value;
            _anim.SetBool(AnimationStrings.isReloading, value);
        }
    }
    protected override void Awake() {}
    protected override void Start() {
        base.Start();

        _weaponManager = GetComponentInChildren<WeaponManager>();
        if(_weaponManager.CurrentWeaponType == WeaponType.GunnerType) {
            GameObject spawnBulletObj = GameObject.FindGameObjectWithTag("SpawnBulletPos");
            spawnBulletPos = spawnBulletObj.transform;
        }
    }
    protected override void Update() {
    }
    protected override void FixedUpdate() {
        base.FixedUpdate();
        OnAttack();
        OnReload();
        if (isAiming && !playerRotatedToCamera) {
            RotatePlayerToCamera();
            playerRotatedToCamera = true;
        }
    }
    private void HandleAim() {
        if (aimVirtualCamera == null)
        {
            Debug.LogError("aimVirtualCamera is not assigned!");
            return;
        }
        if(attackAction.ReadValue<float>() != 0f) {
            if(!isAiming) {
                aimVirtualCamera.gameObject.SetActive(true);
                isAiming = true;
                playerRotatedToCamera = false;
            }
            Vector2 adjustedLookInput = GetAdjustedLookInput();
            RotateCameraWhenAiming(adjustedLookInput);
            _anim.SetLayerWeight(1, Mathf.Lerp(_anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        } else {
            aimVirtualCamera.gameObject.SetActive(false);
            _anim.SetLayerWeight(1, Mathf.Lerp(_anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            isAiming = false;
        }
    }

    protected override void OnAttack()
    {
        if(_weaponManager.CurrentWeaponType == WeaponType.GunnerType) {
            HandleAim();
            if (attackAction.ReadValue<float>() != 0f && Time.time - lastSpawnTime >= bulletSpawnDelay) {
                if(currentAmmo > 0) {
                    CheckMouseOnWorldSpace();
                    Vector3 aimDir = (mouseWorldPosition - spawnBulletPos.position).normalized;
                    Instantiate(bullet_Prefs, spawnBulletPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
                    lastSpawnTime = Time.time;
                    currentAmmo--;
                    if(currentAmmo == 0) {
                        StartCoroutine(Reload());
                    }
                    GameManager.Instance.AudioAttackSFX();
                }
            }
        } else if(_weaponManager.CurrentWeaponType == WeaponType.MeleeType) {
            if(attackAction.ReadValue<float>() != 0f){
                _anim.SetTrigger(AnimationStrings.performAttack);
                GameManager.Instance.AudioAttackSFX();
            }
        }
    }

    protected override void OnReload()
    {
        if(reloadAction.triggered && currentAmmo != 30 && !IsReloading) {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        if (totalAmmo > 0)
        {
            IsReloading = true;
            _anim.SetLayerWeight(2, 1);
            yield return new WaitForSeconds(reloadTime);

            int bulletsToLoad = Mathf.Min(30 - currentAmmo, totalAmmo);
            currentAmmo += bulletsToLoad;
            totalAmmo -= bulletsToLoad;
            IsReloading = false;
            _anim.SetLayerWeight(2, 0);
        }
    }

    public void OnAmmoAdd(int amountToAdd) {
        if (totalAmmo + amountToAdd > 90)
        {
            amountToAdd = 90 - totalAmmo;
        }
        totalAmmo += amountToAdd;
    }

    private void RotatePlayerToCamera() {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float playerCameraYRotation = playerCamera.transform.rotation.eulerAngles.y;
        float adjustedYRotation = Mathf.Repeat(playerCameraYRotation, 360f);
        transform.rotation = Quaternion.Euler(currentRotation.x, adjustedYRotation, currentRotation.z);
    }

    private void RotateCameraWhenAiming(Vector2 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            float mouseX = direction.x;
            float mouseY = direction.y;
            float adjustedYaw = Mathf.Repeat(_mainCamera.transform.eulerAngles.y + mouseX, 360f);
            float currentPitch = _mainCamera.transform.eulerAngles.x;
            if (currentPitch > 180f) currentPitch -= 360f;
            float adjustedPitch = Mathf.Clamp(currentPitch - mouseY, -5f, 10f);
            Quaternion targetRotation = Quaternion.Euler(adjustedPitch, adjustedYaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * rotateSpeed);
        }
    }

    private Vector2 GetAdjustedLookInput()
    {
        Vector2 lookInput = cameraLookAction.ReadValue<Vector2>();
        
        lookInput.x *= xSensitivity;
        lookInput.y *= ySensitivity;

        if(mouseSmoothing != 0) {
            lookInput.x = Mathf.Lerp(previousInput.x, lookInput.x, mouseSmoothing * smoothingFactor);
            lookInput.y = Mathf.Lerp(previousInput.y, lookInput.y, mouseSmoothing * smoothingFactor);
            previousInput = lookInput;
        }

        return lookInput;
    }

    private void CheckMouseOnWorldSpace() {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = _mainCamera.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask)) {
            mouseWorldPosition = raycastHit.point;
        }
    }

    public int GetTotalAmmo() { return totalAmmo; }
    public int GetCurrentAmmno() { return currentAmmo; }
}
