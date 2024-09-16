using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponCollider : MonoBehaviour
{
    [SerializeField] private int meleeDamage; //This will be modified to add more elements like levels, upgrade,...
    private void OnTriggerEnter(Collider other) {
        HealthManagementSystem _healthManagement = other.gameObject.GetComponent<HealthManagementSystem>();
        if(_healthManagement != null) {
            int totalDamage = meleeDamage + Mathf.RoundToInt(CharacterProgression.Instance.bonusDamage);
            _healthManagement.DamageDealt(totalDamage);
        }
    }
}
