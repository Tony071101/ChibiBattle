using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float flySpeed = 100f;
    private int damage;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = transform.forward * flySpeed;
    }

    public void SetDamage(int damageAmount) {
        damage = damageAmount;
    }

    private void OnCollisionEnter(Collision other) {
        HealthManagementSystem _healthManagement = other.gameObject.GetComponent<HealthManagementSystem>();
        if(_healthManagement != null) {
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            _healthManagement.DamageDealt(damage);
        } else {
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
