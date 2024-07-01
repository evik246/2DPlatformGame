using System;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
    public event Action OnCurrentHealthChanged;
    public event Action OnMaxHealthChanged;

    public event Action OnCurrentLevelChanged;

    [Header("Health")]
    [SerializeField]
    protected int _currentHealth = 100;
    [SerializeField]
    protected int _maxHealth = 100;
    [SerializeField]
    protected int _baseHealth = 100;

    [Header("Level")]
    [SerializeField]
    protected int _currentLevel = 1;

    public int CurrentHealth 
    {
        get => _currentHealth;
        set
        {
            _currentHealth = value > _maxHealth ? _maxHealth : value;
            OnCurrentHealthChanged?.Invoke();
        }
    }
    public int BaseHealth
    {
        get => _baseHealth;
        set => _baseHealth = value > _maxHealth ? _maxHealth : value;
    }
    public int MaxHealth 
    {
        get => _maxHealth;
        set
        {
            _maxHealth = value;
            OnMaxHealthChanged?.Invoke();
        } 
    }
    public int CurrentLevel 
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
            OnCurrentLevelChanged?.Invoke();
        }
    }

    protected virtual void Initialize()
    {
        MaxHealth = CalculateMaxHealth();
        CurrentHealth = _currentHealth;
        CurrentLevel = _currentLevel;
    }

    protected virtual int CalculateMaxHealth()
    {
        float healthIncrementPercent = 0.6f;
        return Mathf.CeilToInt(BaseHealth * (1 + healthIncrementPercent * (CurrentLevel - 1)));
    }
}
