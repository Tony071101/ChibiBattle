using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Player
{
    private float moveSpeed;
    private Vector3 moveDirectionRelativeToCamera;
    private Vector3 direction;
    private float angle;
    private float currentVelocity;
    private float smoothRotationTime = 0.05f;
    private bool _isMoving = false;
    private bool IsMoving {
        get { return _isMoving; }
        set {
            _isMoving = value;
            _anim.SetBool(AnimationStrings.isMoving, value);
        }
    }
    protected override void Awake() {}
    protected override void Start() { base.Start(); }
    protected override void Update() {
        
    }
    protected override void FixedUpdate()
    {
        OnMove();
    }
    protected override void OnMove()
    {
        direction = moveAction.ReadValue<Vector3>();
        IsMoving = direction != Vector3.zero;
        if(attackAction.ReadValue<float>() != 0f && _weaponManager.CurrentWeaponType == WeaponType.GunnerType) {
            //this for aimcamera
            float adjustedCameraAngle = Mathf.Repeat(_mainCamera.transform.eulerAngles.y, 360f);
            moveDirectionRelativeToCamera = Quaternion.Euler(0, adjustedCameraAngle, 0) * direction;
            moveSpeed = 2.5f;
        } else {
            //this for normal camera.
            Vector3 cameraForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_mainCamera.transform.right.x, 0, _mainCamera.transform.right.z).normalized;
            moveDirectionRelativeToCamera = (cameraForward * direction.z + cameraRight * direction.x).normalized;
            moveSpeed = 5f;
            //Rotate Player.
            Rotate(moveDirectionRelativeToCamera);
        }
        moveDirectionRelativeToCamera.y = 0f;
        Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
        _rigidbody.AddForce(moveDirectionRelativeToCamera * moveSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
    }

    private void Rotate(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f)
        {
            angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float smoothRotate = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref currentVelocity,
            smoothRotationTime);
            smoothRotate = Mathf.Repeat(smoothRotate, 360f);
            if (attackAction.ReadValue<float>() == 0f && _weaponManager.CurrentWeaponType == WeaponType.GunnerType) 
            {
                transform.rotation = Quaternion.Euler(0, smoothRotate, 0);
            }
        }
    }

    private Vector3 GetPlayerHorizontalVelocity() {
        Vector3 playerHorizontalVelocity = this._rigidbody.velocity;
        playerHorizontalVelocity.y = 0f;
        return playerHorizontalVelocity;
    }
}
