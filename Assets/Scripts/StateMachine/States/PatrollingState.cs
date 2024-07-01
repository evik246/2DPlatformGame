using UnityEngine;

public class PatrollingState : State
{
    [SerializeField]
    private AnimationClip _runClip;
    [SerializeField]
    private float _runSpeed = 3f;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _movingSound = null;

    private AudioSource _movingSoundSource;

    public float CurrentMoveSpeed
    {
        get
        {
            if (!_core.DirectionChecks.IsOnWallFront)
            {
                return _runSpeed;
            }
            return 0;
        }
    }

    public override void Enter()
    {
        _core.Animator.Play(_runClip.name);
        PlayMovingSound();
    }

    public override void FixedDo()
    {
        if (_core.DirectionChecks.IsGrounded && (_core.DirectionChecks.IsOnWallFront || _core.DirectionChecks.IsOnCliffFront))
        {
            _core.DirectionChecks.FlipDirection();
        }
        _core.Rigidbody2D.velocity = new Vector2(CurrentMoveSpeed * _core.DirectionChecks.RunDirectionVector.x, _core.Rigidbody2D.velocity.y);
    }

    public override void Exit()
    {
        StopMovingSound();
    }

    private void PlayMovingSound()
    {
        if (_movingSound != null && _movingSoundSource == null)
        {
            _movingSoundSource = SoundManager.Instance.PlayLoopingSound(_movingSound, SoundCategory.SFX, _core.GameObject.transform);
        }
    }

    private void StopMovingSound()
    {
        if (_movingSoundSource != null)
        {
            SoundManager.Instance.StopSound(_movingSoundSource);
            _movingSoundSource = null;
        }
    }
}
