using System.Collections.Generic;
using UnityEngine;

public class ComboMeleeAttackState : State
{
    [Header("Parameters")]
    [SerializeField]
    private List<AnimationClip> _attackClips = new();
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private List<AttackAbility> _attackAbilities = new();
    [SerializeField]
    private Vector2 _knockback = Vector2.zero;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _cooldownChecks;
    [SerializeField]
    private CooldownChecks _intermediate—ooldownChecks;
    [SerializeField]
    private List<PlayerDetectionIncludingGroundChecks> _playerInAttackRangeChecks = new();

    private bool _hasAttacked;
    private int _currentAttackIndex = 0;

    public override void Enter()
    {
        _eventHandler.OnFinished += StopAttack;
        if (_intermediate—ooldownChecks.HasEnded)
        {
            _currentAttackIndex = 0;
        }
        _core.Animator.Play(_attackClips[_currentAttackIndex].name);
        _hasAttacked = false;
    }

    public override void FixedDo()
    {
        if (_core.Damageable.IsHurt || !_playerInAttackRangeChecks[_currentAttackIndex].HasTarget)
        {
            _core.Animator.StopPlayback();
            IsComplete = true;
        }

        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);

        if (_playerInAttackRangeChecks[_currentAttackIndex].HasTarget && _hasAttacked == false)
        {
            Damageable damageable = _playerInAttackRangeChecks[_currentAttackIndex].Target.GetComponent<Damageable>();

            if (damageable != null)
            {
                _hasAttacked = true;
                bool gotHit = damageable.Hit(_attackAbilities[_currentAttackIndex].CurrentDamage, _knockback);
            }
        }
    }

    private void StopAttack()
    {
        _core.Animator.StopPlayback();

        _intermediate—ooldownChecks.ResetCooldown();

        _currentAttackIndex++;
        if (_currentAttackIndex >= _attackClips.Count)
        {
            IsComplete = true;
        }
        _core.Animator.Play(_attackClips[_currentAttackIndex].name);

        _hasAttacked = false;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopAttack;
        _cooldownChecks.ResetCooldown();
    }
}
