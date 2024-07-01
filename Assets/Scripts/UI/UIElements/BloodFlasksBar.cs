using TMPro;
using UnityEngine;

public class BloodFlasksBar : MonoBehaviour
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

        _playerStats.OnCurrentBloodFlasksChanged += SetBloodFlasks;
        _playerStats.OnMaxBloodFlasksChanged += SetBloodFlasks;
    }

    private void Start()
    {
        SetBloodFlasks();
    }

    private void SetBloodFlasks()
    {
        _currentNumber = _playerStats.CurrentBloodFlasks;
        _maxNumber = _playerStats.MaxBloodFlasks;

        _text.text = string.Format("{0}/{1}", _currentNumber, _maxNumber);
    }

    private void OnDestroy()
    {
        _playerStats.OnCurrentBloodFlasksChanged -= SetBloodFlasks;
        _playerStats.OnMaxBloodFlasksChanged -= SetBloodFlasks;
    }
}
