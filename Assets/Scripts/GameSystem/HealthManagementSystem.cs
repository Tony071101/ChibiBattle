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

            if (currentHealth <= 0) {
                currentHealth = 0;
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        } else {
        }
    }

    public void IncreaseMaxHealth(int amount) {
        maxHealth += amount;
        currentHealth += amount;
    }

    //This might be use in the future.
    public void Heal(int amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public int GetMaxHealth() { return maxHealth; }
}
