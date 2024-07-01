using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private Slider _healthSlider;
    [SerializeField]
    private TMP_Text _text;

    private PlayerStats _playerStats;

    private void Awake()
    {
        //Debug.Log("HealthBar");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player not found on the scene.");
        }

        if (_playerStats != null)
        {
            _playerStats.OnCurrentHealthChanged += SetHealth;
            _playerStats.OnMaxHealthChanged += SetHealth;
        }
    }

    private void Start()
    {
        if (_playerStats != null && _playerStats.IsInitialized)
        {
            SetHealth();
        }
        else
        {
            StartCoroutine(WaitForInitialization());
        }
    }

    private IEnumerator WaitForInitialization()
    {
        while (_playerStats != null && !_playerStats.IsInitialized)
        {
            yield return null;
        }
        SetHealth();
    }

    private void SetHealth()
    {
        _healthSlider.value = _playerStats.CurrentHealth;
        _healthSlider.maxValue = _playerStats.MaxHealth;

        //Debug.Log("_playerStats.CurrentHealth: " + _playerStats.CurrentHealth);
        //Debug.Log("_healthSlider.value: " + _healthSlider.value);

        _text.text = string.Format("{0}/{1}", _playerStats.CurrentHealth, _playerStats.MaxHealth);
    }

    private void OnDestroy()
    {
        if (_playerStats != null)
        {
            _playerStats.OnCurrentHealthChanged -= SetHealth;
            _playerStats.OnMaxHealthChanged -= SetHealth;
        }
    }
}
