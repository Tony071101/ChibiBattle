using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Player
{
    private float moveSpeed;
    private Vector3 moveDirectionRelativeToCamera;
    private Vector3 direction;
    protected override void OnMove()
    {
        direction = moveAction.ReadValue<Vector3>();
        IsMoving = direction != Vector3.zero;

        if(attackAction.ReadValue<float>() != 0f && _weaponManager.CurrentWeaponType == WeaponType.GunnerType) {
            //this for aimcamera
            moveDirectionRelativeToCamera = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0) * direction;
            moveSpeed = 1.5f;
        } else {
            //this for normal camera.
            moveDirectionRelativeToCamera = _mainCamera.transform.TransformDirection(direction);
            moveSpeed = 5f;
        }
        moveDirectionRelativeToCamera.y = 0f;

        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
        _rigidbody.AddForce(moveDirectionRelativeToCamera * moveSpeed - currentPlayerHorizontalVelocity,
        ForceMode.VelocityChange);
        //Rotate Player.
        Rotate(moveDirectionRelativeToCamera);
    }

    private Vector3 GetPlayerHorizontalVelocity() {
        Vector3 playerHorizontalVelocity = this._rigidbody.velocity;
        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }
}
