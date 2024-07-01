using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; private set; }

    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentLevel { get; set; }
    public int CurrentBloodCharges { get; set; }
    public int MaxBloodCharges { get; set; }
    public int CurrentBloodFlasks { get; set; }
    public int MaxBloodFlasks { get; set; }
    public int CurrentXP { get; set; }
    public int XPToNextLevel { get; set; }
    public List<AttackAbility> AttackAbilities { get; set; } = new();

    private void Start()
    {
        //Debug.Log("PlayerState");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsInitialized => Instance.CurrentLevel != 0;

    public void SavePlayerStats(PlayerStats playerStats)
    {
        CurrentHealth = playerStats.CurrentHealth;
        MaxHealth = playerStats.MaxHealth;
        CurrentLevel = playerStats.CurrentLevel;
        CurrentBloodCharges = playerStats.CurrentBloodCharges;
        MaxBloodCharges = playerStats.MaxBloodCharges;
        CurrentBloodFlasks = playerStats.CurrentBloodFlasks;
        MaxBloodFlasks = playerStats.MaxBloodFlasks;
        CurrentXP = playerStats.CurrentXP;
        XPToNextLevel = playerStats.XPToNextLevel;

        if (playerStats.AttackAbilities != null)
        {
            AttackAbilities = new List<AttackAbility>(playerStats.AttackAbilities);
        }
        else
        {
            AttackAbilities = new List<AttackAbility>();
        }
    }

    public void LoadPlayerStats(PlayerStats playerStats)
    {
        playerStats.CurrentHealth = CurrentHealth;
        playerStats.MaxHealth = MaxHealth;
        playerStats.CurrentLevel = CurrentLevel;
        playerStats.CurrentBloodCharges = CurrentBloodCharges;
        playerStats.MaxBloodCharges = MaxBloodCharges;
        playerStats.CurrentBloodFlasks = CurrentBloodFlasks;
        playerStats.MaxBloodFlasks = MaxBloodFlasks;
        playerStats.CurrentXP = CurrentXP;
        playerStats.XPToNextLevel = XPToNextLevel;

        if (AttackAbilities != null)
        {
            playerStats.AttackAbilities = new List<AttackAbility>(AttackAbilities);
        }
        else
        {
            playerStats.AttackAbilities = new List<AttackAbility>();
        }
    }

    public void RestorePlayerStats(PlayerStats playerStats)
    {
        playerStats.CurrentHealth = playerStats.MaxHealth;
        CurrentHealth = playerStats.MaxHealth;

        playerStats.CurrentBloodFlasks = playerStats.MaxBloodFlasks;
        CurrentBloodFlasks = playerStats.MaxBloodFlasks;

        playerStats.CurrentBloodCharges = playerStats.MaxBloodCharges;
        CurrentBloodCharges = playerStats.MaxBloodCharges;
    }
}

