using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeAttackState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _attackClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private AttackAbility _attackAbility;
    [SerializeField]
    private Vector2 _knockback = Vector2.zero;
    [SerializeField]
    private bool _isFlying = false;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks? _cooldownChecks;
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInAttackRangeChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _attackSounds = new();

    private bool _hasAttacked;
    private Vector2 _deliveredKnockback = Vector2.zero;
    private float _defaultGravityScale;
    private bool _hasAttackCooldown = false;

    public override void Enter()
    {
        if (_isFlying)
        {
            _defaultGravityScale = _core.Rigidbody2D.gravityScale;
            _core.Rigidbody2D.gravityScale = 0f;
        }

        if (_cooldownChecks != null)
        {
            _hasAttackCooldown = true;
        }

        _eventHandler.OnFinished += StopAttack;
        _core.Animator.Play(_attackClip.name);
        if (_attackSounds.Any())
        {
            PlayRandomSound(_attackSounds);
        }
        _hasAttacked = false;
    }

    public override void FixedDo()
    {
        if (_isFlying)
        {
            _core.Rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
        }

        if (_playerInAttackRangeChecks.HasTarget && !_hasAttacked)
        {
            Damageable damageable = _playerInAttackRangeChecks.Target.GetComponent<Damageable>();

            if (damageable != null)
            {
                /*_hasAttacked = true;
                _deliveredKnockback = new Vector2(_knockback.x * _core.GameObject.transform.localScale.x, _knockback.y);
                damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);*/

                _hasAttacked = true;
                Vector2 knockbackDirection = new Vector2(_knockback.x * _core.GameObject.transform.localScale.x, _knockback.y);
                _deliveredKnockback = knockbackDirection;

                bool isAgainstWall = IsAgainstWall(damageable, knockbackDirection);

                if (!isAgainstWall)
                {
                    damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);
                }
                else
                {
                    damageable.Hit(_attackAbility.CurrentDamage, Vector2.zero);
                }
            }
        }
    }

    private bool IsAgainstWall(Damageable damageable, Vector2 knockbackDirection)
    {
        DirectionChecks directionChecks = damageable.GetComponent<DirectionChecks>();

        if (directionChecks != null)
        {
            if (knockbackDirection.x > 0 && directionChecks.IsOnWallFront)
            {
                return true;
            }
            if (knockbackDirection.x < 0 && directionChecks.IsOnWallBehind)
            {
                return true;
            }
        }
        return false;
    }

    private void StopAttack()
    {
        _core.Animator.StopPlayback();
        if (_cooldownChecks != null && _hasAttackCooldown)
        {
            _cooldownChecks.ResetCooldown();
        }
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopAttack;
        if (_isFlying)
        {
            _core.Rigidbody2D.gravityScale = _defaultGravityScale;
            _core.Rigidbody2D.velocity = Vector2.zero;
        }
        IsComplete = true;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
