using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCollider : MonoBehaviour
{
    private int damage;

    public void SetDamage(int damageAmount) {
        damage = damageAmount;
    }

    private void OnTriggerEnter(Collider other) {
        HealthManagementSystem _healthManagement = other.gameObject.GetComponent<HealthManagementSystem>();
        if(_healthManagement != null) {
            _healthManagement.DamageDealt(damage);
        }
    }
}
