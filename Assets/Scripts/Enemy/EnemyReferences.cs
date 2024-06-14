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

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        healthManagementSystem = GetComponent<HealthManagementSystem>();
    }
}
