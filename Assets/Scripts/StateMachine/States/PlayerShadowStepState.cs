using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerShadowStepState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _shadowStepClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;
    [SerializeField]
    private float _dashForce = 4f;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _cooldownChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private List<AudioClip> _sounds = new();

    public override void Enter()
    {
        _eventHandler.OnFinished += StopDash;
        _core.Animator.Play(_shadowStepClip.name);
        if (_sounds.Any())
        {
            PlayRandomSound(_sounds);
        }
        _core.Damageable.IsDamageIgnored = true;
        _core.Rigidbody2D.velocity = new Vector2(_dashForce * Mathf.Sign(_core.GameObject.transform.localScale.x), _core.Rigidbody2D.velocity.y);
    }

    private void StopDash()
    {
        _core.Animator.StopPlayback();
        _cooldownChecks.ResetCooldown();
        _core.Damageable.IsDamageIgnored = false;
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopDash;
        IsComplete = true;
    }

    private void PlayRandomSound(List<AudioClip> sounds)
    {
        AudioClip randomHitSound = sounds[Random.Range(0, sounds.Count)];
        SoundManager.Instance.PlaySound(randomHitSound, SoundCategory.SFX, _core.GameObject.transform);
    }
}
