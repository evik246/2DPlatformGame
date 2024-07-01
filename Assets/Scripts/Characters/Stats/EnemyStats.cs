using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyStats : CharacterStats
{
    [Header("Attack Abilities")]
    [SerializeField]
    protected List<AttackAbility> _attackAbilities = new();

    protected override void Initialize()
    {
        base.Initialize();

        foreach (var attackAbility in _attackAbilities)
        {
            attackAbility.CurrentDamage = CalculateAttackDamage(attackAbility.BaseDamage);
        }
    }

    private void Awake()
    {
        Initialize();
        SetLevelBasedOnScene();
    }

    public void LevelUp(int level)
    {
        CurrentLevel = level;
        MaxHealth = CalculateMaxHealth();

        foreach (var attackAbility in _attackAbilities)
        {
            attackAbility.CurrentDamage = CalculateAttackDamage(attackAbility.BaseDamage);
        }

        CurrentHealth = MaxHealth;
    }

    private int CalculateAttackDamage(int baseDamage)
    {
        float damageIncrementPercent = 0.05f;
        return Mathf.CeilToInt(baseDamage * (1 + damageIncrementPercent * (CurrentLevel - 1)));
    }

    private void SetLevelBasedOnScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        CurrentLevel = sceneIndex;
        LevelUp(sceneIndex);
    }
}
