using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EXPItem : MonoBehaviour
{
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerDetectionChecks;
    [SerializeField]
    private int _amount = 1;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _sounds = new();

    private PlayerStats _playerStats;
    private Renderer _renderer;
    private Collider2D _collider;
    private bool _hasCollected;

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
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider2D>();
        _hasCollected = false;
    }

    private void Update()
    {
        if (_playerDetectionChecks.HasTarget && _playerStats != null && !_hasCollected)
        {
            bool isGained = _playerStats.GainXP(_amount);
            if (isGained && _sounds.Any())
            {
                _hasCollected = true;
                AudioClip clip = PlayRandomSound(_sounds);
                MakeInvisible();
                Destroy(gameObject, clip.length);
            }
        }
    }

    private AudioClip PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, transform);
        return randomHitSound;
    }

    private void MakeInvisible()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
    }
}
