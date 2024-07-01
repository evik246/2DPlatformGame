using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSharpNeedlesState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _attackClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private Transform _projecttileTransform;
    [SerializeField]
    private string _projecttilePrefabPath = "Characters/Player/PlayerBloodNeedle";
    [SerializeField]
    private BloodMagicAbility _bloodMagicAbility;
    [SerializeField]
    private AttackAbility _attackAbility;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _cooldownChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _attackSounds = new();

    private GameObject _projecttilePrefab;
    private PlayerStats _playerStats;

    public override void Enter()
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

        bool isUsed = _playerStats.UseBloodCharges(_bloodMagicAbility.BloodChargeNumber);
        if (!isUsed)
        {
            IsComplete = true;
        }

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

        if (_core.DirectionChecks.RunDirection == DirectionChecks.MoveDirection.Right)
        {
            _projecttilePrefab.transform.localScale = new Vector2(1, 1);
        }
        else if (_core.DirectionChecks.RunDirection == DirectionChecks.MoveDirection.Left)
        {
            _projecttilePrefab.transform.localScale = new Vector2(-1, 1);
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
