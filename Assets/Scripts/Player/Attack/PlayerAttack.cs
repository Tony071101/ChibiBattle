using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine.Animations;
using Unity.Mathematics;

public class PlayerAttack : Player
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private Transform bullet_Prefs;
    private Transform spawnBulletPos;
    private float lastSpawnTime = 0f;
    private float bulletSpawnDelay = 0.3f; //can be modified.
    private float reloadTime = 1.8f;
    private bool isAiming = false;
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
    protected override void Update() {}
    protected override void FixedUpdate() {
        OnAttack();
        OnReload();
        if (isAiming && !playerRotatedToCamera) {
            RotatePlayerToCamera();
            playerRotatedToCamera = true;
        }
    }
    private void HandleAim() {
        if(attackAction.ReadValue<float>() != 0f) {
            //Lúc được lúc không.
            if(!isAiming) {
                aimVirtualCamera.gameObject.SetActive(true);
                isAiming = true;
                playerRotatedToCamera = false;
            }
            RotateCameraWhenAiming(cameraLookAction.ReadValue<Vector2>());
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
                }
            }
        } else if(_weaponManager.CurrentWeaponType == WeaponType.MeleeType) {
            if(attackAction.ReadValue<float>() != 0f){
                _anim.SetTrigger(AnimationStrings.performAttack);
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

    public int GetTotalAmmo() { return totalAmmo; }
    public int GetCurrentAmmno() { return currentAmmo; }
}
