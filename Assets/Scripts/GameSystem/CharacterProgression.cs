using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProgression : MonoBehaviour
{
    public static CharacterProgression Instance { get; private set; }
    public float bonusHealth { get; private set; } = 20f;
    public float bonusDamage { get; private set; } = 5f;
    public int experiencePoints { get; private set; }
    public int healthPercentageIncrease { get; private set; }
    public int damagePercentageIncrease { get; private set; }
    public int level { get; private set; }
    public int healthUpgradeCount { get; private set; } = 0;
    public int damageUpgradeCount { get; private set; } = 0;
    public int maxHealthUpgrades { get; private set; } = 5;
    public int maxDamageUpgrades { get; private set; } = 5;
    private const int pointsToLevelUp = 50;

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
        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if (experiencePoints >= pointsToLevelUp)
        {
            experiencePoints -= pointsToLevelUp; // Reset experience points after leveling up
            level++;
            healthPercentageIncrease = UnityEngine.Random.Range(2, 11);
            damagePercentageIncrease = UnityEngine.Random.Range(2, 11);
            GameManager.Instance.PlayerLevelUp();
        }
    }

    public void OnLevelUpHealth() {
        bonusHealth += bonusHealth * (healthPercentageIncrease / 100f);
        healthUpgradeCount++;
    }

    public void OnLevelUpDamage() {
        bonusDamage += bonusDamage * (damagePercentageIncrease / 10f);
        damageUpgradeCount++;
    }
    public int GetPointToLvlUp() { return pointsToLevelUp; }
}
