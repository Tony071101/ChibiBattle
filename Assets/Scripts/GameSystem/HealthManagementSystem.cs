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
    private void Awake() {
        currentHealth = maxHealth;
    }

    public void DamageDealt(int damage) {
        if (currentHealth > 0) {
            currentHealth -= damage;

            Player player = GetComponentInParent<Player>();
            if (player != null) {
                GameManager.Instance.AudioOnHurt();
            }

            if (currentHealth <= 0) {
                currentHealth = 0;
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void IncreaseMaxHealth(int amount) {
        maxHealth += amount;
        currentHealth += amount;
    }

    public void Heal(int amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public int GetMaxHealth() { return maxHealth; }
}
