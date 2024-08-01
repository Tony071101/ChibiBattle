using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProgression : MonoBehaviour
{
    public static CharacterProgression Instance;
    public float bonusHealth { get; private set; } = 10;
    public float bonusDamage { get; private set; } = 10;
    private int experiencePoints;
    private int level;
    private const int pointsToLevelUp = 50;
    public event EventHandler OnLevelUp;

    private void Awake() {
        if(Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
        }
    }

    public void AddExperience()
    {
        experiencePoints += 10;
        Debug.LogWarning("Experience point: " + experiencePoints);
        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if (experiencePoints >= pointsToLevelUp)
        {
            experiencePoints -= pointsToLevelUp; // Reset experience points after leveling up
            level++;
            OnLevelUpHealth();
            OnLevelUpDamage();
            Debug.Log("Level Up! New Level: " + level);

            OnLevelUp?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnLevelUpHealth()
    {
        float percentageIncrease = UnityEngine.Random.Range(2f, 10f); // Random percentage between 2% and 10%
        bonusHealth += bonusHealth * percentageIncrease;
    }

    private void OnLevelUpDamage() {
        float percentageIncrease = UnityEngine.Random.Range(0.02f, 0.10f); // Random percentage between 2% and 10%
        bonusDamage += bonusDamage * percentageIncrease;
    }
}
