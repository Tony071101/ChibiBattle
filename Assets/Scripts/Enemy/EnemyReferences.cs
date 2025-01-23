using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class EnemyReferences : MonoBehaviour
{
    public NavMeshAgent navMeshAgent { get; private set; }
    public Animator _anim { get; private set; }
    public HealthManagementSystem healthManagementSystem { get; private set; }
    public float pathUpdateDelay { get; private set; } = 0.2f;
    public WorldspaceHealthBar worldspaceHealthBar { get; private set; }
    public MeleeWeaponCollider meleeWeaponCollider { get; private set; }
    private int attackDamage = 5;
    public int enemyMaxHealth { get; private set; } = 100;
    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        healthManagementSystem = GetComponent<HealthManagementSystem>();
        worldspaceHealthBar = GetComponentInChildren<WorldspaceHealthBar>();
        meleeWeaponCollider = GetComponentInChildren<MeleeWeaponCollider>();

        if (healthManagementSystem != null) {
            healthManagementSystem.InitializeHealth(enemyMaxHealth);
        }
    }

    public int GetAttackDamage() {
        return attackDamage;
    }
}
