using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    private Transform target;
    private HealthManagementSystem targetHealthSystem;
    private EnemyReferences enemyReferences;
    private float attackDistance;
    private float pathUpdateDeadLine;

    private void Awake() {
        enemyReferences = GetComponent<EnemyReferences>();
    }

    private void Start() {
        attackDistance = enemyReferences.navMeshAgent.stoppingDistance;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null)
        {
            targetHealthSystem = target.GetComponentInChildren<HealthManagementSystem>();
        }
    }

    private void Update()
    {
        if (enemyReferences.healthManagementSystem.currentHealth > 0)
        {
            TargetInSight();
        }
        else
        {
            enemyReferences._anim.SetTrigger(AnimationStrings.death);
            enemyReferences.navMeshAgent.isStopped = true;
            enemyReferences._anim.SetFloat(AnimationStrings.enemyMoveSpeed, 0);
        }
    }

    private void TargetInSight()
    {
        if (target != null && targetHealthSystem.currentHealth > 0)
        {
            bool inRange = Vector3.Distance(transform.position, target.position) <= attackDistance;

            if (inRange)
            {
                LookAtTarget();
            }
            else
            {
                UpdatePath();
            }

            //anim attack implement here.
            enemyReferences._anim.SetBool(AnimationStrings.enemyPerformAttack, inRange);
        } else {
            enemyReferences._anim.SetBool(AnimationStrings.enemyPerformAttack, false);
            enemyReferences.navMeshAgent.isStopped = true;
            enemyReferences._anim.SetFloat(AnimationStrings.enemyMoveSpeed, 0);
        }
        enemyReferences._anim.SetFloat(AnimationStrings.enemyMoveSpeed, enemyReferences.navMeshAgent.desiredVelocity.sqrMagnitude);
    }

    private void UpdatePath()
    {
        if(Time.time >= pathUpdateDeadLine) {
            Debug.LogWarning("Updating path...");
            pathUpdateDeadLine = Time.time + enemyReferences.pathUpdateDelay;
            enemyReferences.navMeshAgent.SetDestination(target.position);
        }
    }

    private void LookAtTarget()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }
}
