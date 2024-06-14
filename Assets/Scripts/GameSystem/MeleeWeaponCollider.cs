using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCollider : MonoBehaviour
{
    [SerializeField] private int meleeDamage; //This will be modified to add more elements like levels, upgrade,...
    private void OnTriggerEnter(Collider other) {
        HealthManagementSystem _healthManagement = other.gameObject.GetComponent<HealthManagementSystem>();
        if(_healthManagement != null) {
            Debug.Log("Melee weapon hit target: " + other.gameObject.name);
            _healthManagement.DamageDealt(meleeDamage);
        }
    }
}
