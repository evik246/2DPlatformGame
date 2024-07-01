using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldBlockState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _shieldBlockClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private AttackAbility _attackAbility;
    [SerializeField]
    private Vector2 _targetKnockback = Vector2.zero;
    [SerializeField]
    private Vector2 _enemyKnockback = Vector2.zero;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _cooldownChecks;
    [SerializeField]
    private TargetBehindChecks _targetBehindChecks;
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInShieldChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _sounds = new();

    private bool _hasDefenced;
    private Vector2 _deliveredKnockback = Vector2.zero;

    public override void Enter()
    {
        _eventHandler.OnFinished += StopDefence;
        _core.Animator.Play(_shieldBlockClip.name);
        if (_sounds.Any())
        {
            PlayRandomSound(_sounds);
        }
        _hasDefenced = false;
        _core.Damageable.IsDamageIgnored = false;
        _core.Rigidbody2D.velocity = new Vector2(_enemyKnockback.x * Mathf.Sign(_core.GameObject.transform.localScale.x), _enemyKnockback.y);
    }

    public override void FixedDo()
    {
        _core.Damageable.IsDamageIgnored = _targetBehindChecks.IsTargetBehind? false: true;

        if (_playerInShieldChecks.HasTarget && !_hasDefenced)
        {
            Damageable damageable = _playerInShieldChecks.Target.GetComponent<Damageable>();

            if (damageable != null)
            {
                _hasDefenced = true;
                _deliveredKnockback = new Vector2(_targetKnockback.x * _core.GameObject.transform.localScale.x, _targetKnockback.y);
                bool gotHit = damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);
            }
        }
    }

    private void StopDefence()
    {
        _core.Animator.StopPlayback();
        _cooldownChecks.ResetCooldown();
        _core.Damageable.IsDamageIgnored = false;
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopDefence;
        _core.Damageable.IsDamageIgnored = false;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
