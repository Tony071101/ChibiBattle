using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float flySpeed = 100f; //can be modified.
    private int bulletDamage = 5; //This will be modified to add more elements like levels, upgrade,...
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        _rigidbody.velocity = transform.forward * flySpeed;
    }

    private void OnCollisionEnter(Collision other) {
        //Basiclly this is a testing identify which target hit, here's every gameobject have "targetdummy" script
        //attached to, will be changed to "health" script for both player and enemy.
        HealthManagementSystem _healthManagement = other.gameObject.GetComponent<HealthManagementSystem>();
        if(_healthManagement != null) {
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            _healthManagement.DamageDealt(bulletDamage);
        } else {
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
