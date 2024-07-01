using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAttackState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _attackClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private Transform _projecttileTransform;
    [SerializeField]
    private string _projecttilePrefabPath = "Characters/Enemies/ArcherNPC/ArcherArrow";
    [SerializeField]
    private AttackAbility _attackAbility;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _cooldownChecks;
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _attackSounds = new();

    private Vector2 _directionVector;
    private GameObject _projecttilePrefab;

    public override void Enter()
    {
        _projecttilePrefab = Resources.Load<GameObject>(_projecttilePrefabPath);
        _projecttilePrefab.GetComponent<Projecttile>().AttackAbility = _attackAbility;

        _eventHandler.OnFinished += StopAttack;
        _core.Animator.Play(_attackClip.name);
        if (_attackSounds.Any())
        {
            PlayRandomSound(_attackSounds);
        }
    }

    public override void FixedDo()
    {
        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);

        if (_playerInSightChecks.HasTarget)
        {
            _directionVector = (_playerInSightChecks.Target.transform.position - _core.transform.position).normalized;
            if (_directionVector.x > 0)
            {
                _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Right;
                _projecttilePrefab.transform.localScale = new Vector2(1, 1);
            }
            else if (_directionVector.x < 0)
            {
                _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Left;
                _projecttilePrefab.transform.localScale = new Vector2(-1, 1);
            }
        }
    }

    private void StopAttack()
    {
        _core.Animator.StopPlayback();
        Instantiate(_projecttilePrefab, _projecttileTransform.position, _projecttilePrefab.transform.rotation);
        _cooldownChecks.ResetCooldown();
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopAttack;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
