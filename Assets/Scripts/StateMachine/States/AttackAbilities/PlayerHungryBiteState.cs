using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHungryBiteState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _hungryBiteClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private AttackAbility _attackAbility;
    [SerializeField]
    private Vector2 _knockback = Vector2.zero;

    [Header("Checks")]
    [SerializeField]
    private NPCDetectionChecks _NPCDetectionChecks;
    [SerializeField]
    private PlayerStats _playerStats;
    [SerializeField]
    private CooldownChecks _cooldownChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _attackSounds = new();

    private bool _hasAttacked;
    private Vector2 _deliveredKnockback = Vector2.zero;

    public override void Enter()
    {
        _eventHandler.OnFinished += StopAttack;
        _core.Animator.Play(_hungryBiteClip.name);
        if (_attackSounds.Any())
        {
            PlayRandomSound(_attackSounds);
        }
        _hasAttacked = false;
    }

    public override void FixedDo()
    {
        if (_core.Damageable.IsHurt)
        {
            IsComplete = true;
        }

        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);

        if (_hasAttacked == false)
        {
            foreach (var collider in _NPCDetectionChecks.DetectedColliders)
            {
                Damageable damageable = collider.GetComponent<Damageable>();
                if (damageable != null)
                {
                    _deliveredKnockback = new Vector2(_knockback.x * _core.DirectionChecks.RunDirectionVector.x, _knockback.y);
                    _hasAttacked = true;
                    bool gotHit = damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);
                    if (gotHit)
                    {
                        _playerStats.GainBloodCharges();
                    }
                }
            }
        }
    }

    private void StopAttack()
    {
        _core.Animator.StopPlayback();
        _cooldownChecks.ResetCooldown();
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopAttack;
        _NPCDetectionChecks.ResetDetectedColliders();
        IsComplete = true;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
