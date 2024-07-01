using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private int _level;
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

        _playerStats.OnCurrentLevelChanged += SetLevel;
    }

    private void Start()
    {
        SetLevel();
    }

    private void SetLevel()
    {
        _level = _playerStats.CurrentLevel;
        _text.text = _level.ToString();
    }

    private void OnDestroy()
    {
        _playerStats.OnCurrentLevelChanged -= SetLevel;
    }
}
