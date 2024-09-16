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
    [SerializeField] private Transform bullet_Prefs;
    private float lastSpawnTime = 0f;
    private float bulletSpawnDelay = 0.3f; //can be modified.
    private float reloadTime = 1.8f;

    protected override void Awake() {}
    private void HandleAim() {
        if(attackAction.ReadValue<float>() != 0f) {
            aimVirtualCamera.gameObject.SetActive(true);
            RotateCameraWhenAiming(cameraLookAction.ReadValue<Vector2>());
            //Lerp used for smooth transition.
            _anim.SetLayerWeight(1, Mathf.Lerp(_anim.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        } else {
            aimVirtualCamera.gameObject.SetActive(false);
            _anim.SetLayerWeight(1, Mathf.Lerp(_anim.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
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
                } else {
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
            // UI will take care of this.
            // Debug.LogWarning("Reloading...");
            yield return new WaitForSeconds(reloadTime);

            int bulletsToLoad = Mathf.Min(30 - currentAmmo, totalAmmo);
            currentAmmo += bulletsToLoad;
            totalAmmo -= bulletsToLoad;
            IsReloading = false;
            _anim.SetLayerWeight(2, 0);
            // Play reload complete sound if needed
            // Debug.LogWarning("Reload complete.");
            // UI will take care of this.
        }
        else
        {
            // No bullets left in reserve
            // UI will take care of this.
            // Debug.LogError("No bullets left in reserve!");
        }
    }

    public void OnAmmoAdd(int amountToAdd) {
        if (totalAmmo + amountToAdd > 90)
        {
            amountToAdd = 90 - totalAmmo;
        }
        totalAmmo += amountToAdd;
    }
}
