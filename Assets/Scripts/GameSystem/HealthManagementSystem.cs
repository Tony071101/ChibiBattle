using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HealthManagementSystem : MonoBehaviour
{
    public int currentHealth { get; private set; }
    [SerializeField] private int maxHealth;
    public event EventHandler OnDeath;

    private void Start() {
        currentHealth = maxHealth;
    }

    public void DamageDealt(int damage) {
        if (currentHealth > 0) {
            currentHealth -= damage;
            Debug.LogError(gameObject.name + " took damage, current health: " + currentHealth);

            if (currentHealth <= 0) {
                currentHealth = 0; // Ensure health doesn't drop below 0
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        } else {
            Debug.Log(gameObject.name + " is already dead.");
        }
    }
}
