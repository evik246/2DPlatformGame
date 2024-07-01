using TMPro;
using UnityEngine;

public class EXPBar : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private int _currentNumber = 0;
    [SerializeField]
    private int _maxNumber = 0;
    [SerializeField]
    private TMP_Text _text;

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

        _playerStats.OnCurrentEXPChanged += SetEXP;
        _playerStats.OnMaxEXPChanged += SetEXP;
    }

    private void Start()
    {
        SetEXP();
    }

    private void SetEXP()
    {
        _currentNumber = _playerStats.CurrentXP;
        _maxNumber = _playerStats.XPToNextLevel;

        _text.text = string.Format("{0}/{1}", _currentNumber, _maxNumber);
    }

    private void OnDestroy()
    {
        _playerStats.OnCurrentEXPChanged -= SetEXP;
        _playerStats.OnMaxEXPChanged -= SetEXP;
    }
}
