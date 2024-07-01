using UnityEngine;

public class PlayerHealState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _healClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;

    [Header("Checks")]
    [SerializeField]
    private PlayerStats _playerStats;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _healSound = null;

    public override void Enter()
    {
        bool isUsed = _playerStats.UseBloodFlask();
        if (!isUsed)
        {
            IsComplete = true;
        }

        _eventHandler.OnFinished += StopHeal;
        _core.Animator.Play(_healClip.name);
        if (_healSound != null)
        {
            SoundManager.Instance.PlaySound(_healSound, SoundCategory.SFX, _core.GameObject.transform);
        }
    }

    public override void FixedDo()
    {
        if (_core.Damageable.IsHurt)
        {
            IsComplete = true;
        }

        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
    }

    private void StopHeal()
    {
        _core.Animator.StopPlayback();
        IsComplete = true;
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= StopHeal;
    }
}
