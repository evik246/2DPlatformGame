using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBasicAttackState : State
{
    [Header("Parameters")]
    [SerializeField]
    private List<AnimationClip> _attackClips = new();
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private AttackAbility _attackAbility;
    [SerializeField]
    private Vector2 _targetKnockback = Vector2.zero;
    [SerializeField]
    private float _dashForce = 5f;

    [Header("Checks")]
    [SerializeField]
    private PlayerInputChecks _playerInputChecks;
    [SerializeField]
    private List<NPCDetectionChecks> _NPCDetectionChecks = new();

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _attackSounds = new();
    [SerializeField]
    private List<AudioClip> _missAttackSounds = new();

    private bool _hasAttacked;
    private bool _hasTargetHit;
    private bool _hasMissed;
    private int _currentAttackIndex = 0;
    private Vector2 _deliveredKnockback = Vector2.zero;

    public override void Enter()
    {
        _eventHandler.OnFinished += StopAttack;
        _core.Animator.Play(_attackClips[_currentAttackIndex].name);
        _hasAttacked = false;
        _hasMissed = false;
        _hasTargetHit = false;
        _core.Rigidbody2D.velocity = new Vector2(_dashForce * Mathf.Sign(_core.GameObject.transform.localScale.x), _core.Rigidbody2D.velocity.y);
    }

    public override void FixedDo()
    {
        if (_core.Damageable.IsHurt)
        {
            IsComplete = true;
        }

        if (!_hasAttacked)
        {
            foreach (var collider in _NPCDetectionChecks[_currentAttackIndex].DetectedColliders)
            {
                Damageable damageable = collider.GetComponent<Damageable>();
                if (damageable != null)
                {
                    _deliveredKnockback = new Vector2(_targetKnockback.x * _core.DirectionChecks.RunDirectionVector.x, _targetKnockback.y);
                    _hasAttacked = true;
                    _hasTargetHit = damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);
                    if (_hasTargetHit && _attackSounds.Any())
                    {
                        PlayRandomSound(_attackSounds);
                    }
                }
            }

            if (!_hasTargetHit && !_hasMissed && _missAttackSounds.Any())
            {
                PlayRandomSound(_missAttackSounds);
                _hasMissed = true;
            }
        }
    }

    private void StopAttack()
    {
        _core.Animator.StopPlayback();
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopAttack;
        _NPCDetectionChecks[_currentAttackIndex].ResetDetectedColliders();
        _currentAttackIndex++;
        if (_currentAttackIndex >= _attackClips.Count)
        {
            _currentAttackIndex = 0;
        }
        IsComplete = true;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
