using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : CharacterStats
{
    public static PlayerStats Instance { get; private set; }

    public bool IsInitialized { get; private set; } = false;

    public event Action OnCurrentBloodChargesChanged;
    public event Action OnMaxBloodChargesChanged;

    public event Action OnCurrentBloodFlasksChanged;
    public event Action OnMaxBloodFlasksChanged;

    public event Action OnCurrentEXPChanged;
    public event Action OnMaxEXPChanged;

    [Header("Blood Charge")]
    [SerializeField]
    protected int _currentBloodCharges = 4;
    [SerializeField]
    protected int _maxBloodCharges = 4;
    [SerializeField]
    protected int _baseBloodCharges = 4;

    [Header("Blood Flask")]
    [SerializeField]
    protected int _currentBloodFlasks = 2;
    [SerializeField]
    protected int _maxBloodFlasks = 2;
    [SerializeField]
    protected int _baseBloodFlasks = 2;

    [Header("Experience Points")]
    [SerializeField]
    protected int _currentXP = 0;
    [SerializeField]
    protected int _xpToNextLevel = 100;

    [Header("Attack Abilities")]
    [SerializeField]
    protected List<AttackAbility> _attackAbilities = new();

    [Header("Chosen Ability Cooldowns")]
    [SerializeField]
    protected List<CooldownChecks> _chosenAbilityCooldownChecks = new();

    public int CurrentBloodCharges 
    { 
        get => _currentBloodCharges;
        set
        {
            _currentBloodCharges = value > _maxBloodCharges ? _maxBloodCharges : value;
            OnCurrentBloodChargesChanged?.Invoke();
        }
    }
    public int CurrentBloodFlasks
    {
        get => _currentBloodFlasks;
        set
        { 
            _currentBloodFlasks = value > _maxBloodFlasks ? _maxBloodFlasks : value;
            OnCurrentBloodFlasksChanged?.Invoke();
        }
    }
    public int CurrentXP
    {
        get => _currentXP;
        set
        {
            _currentXP = value > _xpToNextLevel ? _xpToNextLevel : value;
            OnCurrentEXPChanged?.Invoke();
        }
    }
    public int BaseBloodCharges
    {
        get => _baseBloodCharges;
        set => _baseBloodCharges = value > _maxBloodCharges ? _maxBloodCharges : value;
    }
    public int BaseBloodFlasks
    {
        get => _baseBloodFlasks;
        set => _baseBloodFlasks = value > _maxBloodFlasks ? _maxBloodFlasks : value;
    }
    public int MaxBloodCharges 
    { 
        get => _maxBloodCharges; 
        set 
        {
            _maxBloodCharges = value;
            OnMaxBloodChargesChanged?.Invoke();
        } 
    }
    public int MaxBloodFlasks 
    {
        get => _maxBloodFlasks;
        set
        {
            _maxBloodFlasks = value;
            OnMaxBloodFlasksChanged?.Invoke();
        }
    }
    public int XPToNextLevel 
    { 
        get => _xpToNextLevel;
        set
        {
            _xpToNextLevel = value;
            OnMaxEXPChanged?.Invoke();
        }
    }
    public List<AttackAbility> AttackAbilities { get => _attackAbilities; set => _attackAbilities = value; }
    public List<CooldownChecks> ChosenAbilityCooldownChecks { get => _chosenAbilityCooldownChecks; }

    protected override void Initialize()
    {
        base.Initialize();

        MaxBloodCharges = CalculateMaxBloodCharges();
        MaxBloodFlasks = CalculateMaxBloodFlasks();
        XPToNextLevel = CalculateXPToNextLevel(CurrentLevel);

        CurrentBloodCharges = _currentBloodCharges;
        CurrentBloodFlasks = _currentBloodFlasks;
        CurrentXP = _currentXP;

        foreach (var attackAbility in _attackAbilities)
        {
            attackAbility.CurrentDamage = CalculateAttackDamage(attackAbility.BaseDamage);
        }

        IsInitialized = true;
    }

    private void Awake()
    {
        //Debug.Log("PlayerStats");
        Initialize();

        if (Instance == null)
        {
            Instance = this;
        }

        if (PlayerState.Instance != null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            ResetToBaseStats();
            PlayerState.Instance.SavePlayerStats(this);
        }
        else if (PlayerState.Instance != null && !PlayerState.Instance.IsInitialized)
        {
            PlayerState.Instance.SavePlayerStats(this);
        }
        else if (PlayerState.Instance != null)
        {
            PlayerState.Instance.LoadPlayerStats(this);
            PlayerState.Instance.RestorePlayerStats(this);
        }
    }

    public bool GainBloodFlasks(int gainedBloodFlasks)
    {
        if (gainedBloodFlasks > 0 && CurrentBloodFlasks < MaxBloodFlasks)
        {
            CurrentBloodFlasks += gainedBloodFlasks;
            return true;
        }
        return false;
    }

    public bool GainXP(int gainedXP)
    {
        if (gainedXP > 0)
        {
            CurrentXP += gainedXP;
            // Проверка на повышение уровня
            if (CurrentXP >= XPToNextLevel)
            {
                LevelUp();
            }
            return true;
        }
        return false;
    }

    public int CalculateGainedBloodFlasks(int enemyLevel)
    {
        if (CurrentBloodFlasks < MaxBloodFlasks)
        {
            float baseChance = 0.2f; // базовый шанс 20%
            float levelDifference = enemyLevel - CurrentLevel;
            float adjustment = levelDifference * 0.05f; // изменение шанса на 5% за уровень разницы
            float currentChance = baseChance + adjustment;

            // Ограничиваем вероятность выпадения в пределах 0% и 100%
            currentChance = Mathf.Clamp(currentChance, 0f, 1f);

            // С учетом вероятности выпадения банки крови
            if (UnityEngine.Random.value < currentChance)
            {
                return 1;
            }
        }
        return 0;
    }

    public int CalculateGainedXP(int enemyLevel, int enemiesOnLocation)
    {
        if (CurrentXP < XPToNextLevel)
        {
            float levelUpMultiplier = 1.5f; // множитель для повышения уровня при убийстве всех врагов
            float baseXP = XPToNextLevel * levelUpMultiplier / enemiesOnLocation; // расчет базового XP
            float levelDifferenceMultiplier = 5; // множитель для разницы уровней
            int xpGained = Mathf.CeilToInt(baseXP + (enemyLevel - CurrentLevel) * levelDifferenceMultiplier);

            xpGained = Mathf.Max(xpGained, 1); // минимальное количество опыта - 1
            return xpGained;
        }
        return 0;
    }

    private void LevelUp()
    {
        CurrentLevel++;
        CurrentXP -= XPToNextLevel; // остаток опыта переносится на следующий уровень
        MaxHealth = CalculateMaxHealth();

        foreach (var attackAbility in _attackAbilities)
        {
            attackAbility.CurrentDamage = CalculateAttackDamage(attackAbility.BaseDamage);
        }

        MaxBloodCharges = CalculateMaxBloodCharges();
        MaxBloodFlasks = CalculateMaxBloodFlasks();
        XPToNextLevel = CalculateXPToNextLevel(CurrentLevel); // Установка нового количества опыта для следующего уровня
    }

    private int CalculateXPToNextLevel(int level)
    {
        // Логика для расчета количества опыта, необходимого для следующего уровня
        // пример: каждый следующий уровень требует на 150 больше опыта
        return level * 150;
    }

    private int CalculateMaxBloodCharges()
    {
        int bloodChargesIncrement = 1;
        return _baseBloodCharges + bloodChargesIncrement * Mathf.FloorToInt(CurrentLevel / 3);
    }

    private int CalculateMaxBloodFlasks()
    {
        return _baseBloodFlasks + Mathf.FloorToInt((CurrentLevel - 1) / 5);
    }

    private int CalculateAttackDamage(int baseDamage)
    {
        float damageIncrementPercent = 0.06f;
        return Mathf.CeilToInt(baseDamage * (1 + damageIncrementPercent * (CurrentLevel - 1)));
    }

    public bool GainBloodCharges()
    {
        if (CurrentBloodCharges < MaxBloodCharges)
        {
            int chargesGained = 1 + Mathf.FloorToInt(CurrentLevel / 3); // расчет количества зарядов крови
            CurrentBloodCharges = Mathf.Min(CurrentBloodCharges + chargesGained, MaxBloodCharges);
            return true;
        }
        return false;
    }

    public bool UseBloodFlask()
    {
        if (CurrentBloodFlasks > 0 && CurrentHealth < MaxHealth)
        {
            float efficiencyReductionPercent = 0.05f; // процент снижения эффективности
            int healthRestored = Mathf.Max(Mathf.CeilToInt(MaxHealth * (1 - (CurrentLevel * efficiencyReductionPercent))), 1); // расчет восстанавливаемого здоровья

            CurrentHealth = Mathf.Min(CurrentHealth + healthRestored, MaxHealth); // не превышаем максимальное здоровье
            CurrentBloodFlasks--;
            return true;
        }
        return false;
    }

    public bool UseBloodCharges(int bloodChargeNumber)
    {
        if (CurrentBloodCharges - bloodChargeNumber >= 0)
        {
            CurrentBloodCharges -= bloodChargeNumber;
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return string.Format("Health {0}/{1}\nFlasks {2}/{3}\nCharges {4}/{5}\nXP {6}/{7}\nLevel {8}",
            CurrentHealth, MaxHealth,
            CurrentBloodFlasks, MaxBloodFlasks,
            CurrentBloodCharges, MaxBloodCharges,
            CurrentXP, XPToNextLevel,
            CurrentLevel);
    }

    public void ResetToBaseStats()
    {
        CurrentLevel = 1;
        CurrentXP = 0;
        XPToNextLevel = CalculateXPToNextLevel(1);

        CurrentHealth = MaxHealth = CalculateMaxHealth();
        CurrentBloodCharges = BaseBloodCharges;
        MaxBloodCharges = CalculateMaxBloodCharges();
        CurrentBloodFlasks = BaseBloodFlasks;
        MaxBloodFlasks = CalculateMaxBloodFlasks();

        foreach (var attackAbility in _attackAbilities)
        {
            attackAbility.CurrentDamage = CalculateAttackDamage(attackAbility.BaseDamage);
        }

        // Trigger the relevant events to update the UI or other systems
        OnCurrentBloodChargesChanged?.Invoke();
        OnMaxBloodChargesChanged?.Invoke();
        OnCurrentBloodFlasksChanged?.Invoke();
        OnMaxBloodFlasksChanged?.Invoke();
        OnCurrentEXPChanged?.Invoke();
        OnMaxEXPChanged?.Invoke();
    }

}
