using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BounceState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _fallClip;
    [SerializeField]
    private AnimationClip _riseClip;
    [SerializeField]
    private Vector2 _bounceForceVector = new Vector2(3f, 5f);

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _bounceCooldownChecks;
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInBounceChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _sounds = new();

    private Transform _target;
    private Vector2 _directionVector;

    public override void Enter()
    {
        if (!_playerInBounceChecks.HasTarget)
        {
            IsComplete = true;
        }

        _target = _playerInBounceChecks.Target?.transform;
        _directionVector = (_target.position - _core.transform.position).normalized;

        if (_directionVector.x > 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Right;
        }
        else if (_directionVector.x < 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Left;
        }

        if (_core.DirectionChecks.IsOnWallFront || _core.DirectionChecks.IsOnCliffFront)
        {
            _bounceForceVector.x = Mathf.Min(_bounceForceVector.x, Mathf.Abs(_target.position.x - _core.transform.position.x));
        }

        _core.Rigidbody2D.velocity = new Vector2(_bounceForceVector.x * -_core.DirectionChecks.RunDirectionVector.x, _bounceForceVector.y);
    }

    public override void FixedDo()
    {
        if (!_core.Damageable.IsHurt)
        {
            if (_core.Rigidbody2D.velocity.y > 0)
            {
                _core.Animator.Play(_riseClip.name);
            }
            else if (_core.Rigidbody2D.velocity.y < 0)
            {
                _core.Animator.Play(_fallClip.name);
            }
        }

        if ((_core.DirectionChecks.IsGrounded && _core.Rigidbody2D.velocity.y < 0) || _core.DirectionChecks.IsOnWallBehind || _core.DirectionChecks.IsOnCliffBehind)
        {
            if (_sounds.Any())
            {
                PlayRandomSound(_sounds);
            }
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        _bounceCooldownChecks.ResetCooldown();
        _core.Animator.StopPlayback();
        _core.Damageable.IsDamageIgnored = false;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
