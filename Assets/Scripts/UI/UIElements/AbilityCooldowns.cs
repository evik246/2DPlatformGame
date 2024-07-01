using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldowns : MonoBehaviour
{
    [SerializeField]
    private List<Image> abilityImages = new();

    private PlayerStats _playerStats;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player not found on the scene.");
        }
    }

    private void Update()
    {
        for (int i = 0; i < abilityImages.Count; i++)
        {
            UpdateCooldownImage(abilityImages[i], _playerStats.ChosenAbilityCooldownChecks[i]);
        }
    }

    private void UpdateCooldownImage(Image abilityImage, CooldownChecks cooldownChecks)
    {
        if (cooldownChecks.CoolDown > 0)
        {
            abilityImage.fillAmount = cooldownChecks.CurrentCooldown / cooldownChecks.CoolDown;
        }
        else
        {
            abilityImage.fillAmount = 0;
        }
    }
}
