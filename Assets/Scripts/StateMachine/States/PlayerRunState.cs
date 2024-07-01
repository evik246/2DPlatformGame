using UnityEngine;

public class PlayerRunState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _runClip;
    [SerializeField]
    private AnimationClip _fallClip;
    [SerializeField]
    private float _runSpeed = 5f;

    [Header("Checks")]
    [SerializeField]
    private PlayerInputChecks _playerInputChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _movingSound = null;
    [SerializeField]
    private AudioClip _fallSound = null;

    private AudioSource _movingSoundSource;
    private bool _hasFall;

    private bool IsFalling
    {
        get
        {
            return !_core.DirectionChecks.IsGrounded && _core.Rigidbody2D.velocity.y < 0;
        }
    }

    public float CurrentMoveSpeed
    {
        get
        {
            if (_playerInputChecks.IsMoving && !_core.DirectionChecks.IsOnWallFront)
            {
                return _runSpeed;
            }
            return 0;
        }
    }

    public override void Enter()
    {
        if (IsFalling)
        {
            _core.Animator.Play(_fallClip.name);
        }
        else
        {
            _core.Animator.Play(_runClip.name);
            PlayMovingSound();
        }
    }

    public override void FixedDo()
    {
        if (_core.DirectionChecks.IsGrounded && _core.Rigidbody2D.velocity.y < 0 && !_hasFall && _fallSound != null)
        {
            SoundManager.Instance.PlaySound(_fallSound, SoundCategory.SFX, _core.GameObject.transform);
            _hasFall = true;
        }
        else if (!_core.DirectionChecks.IsGrounded)
        {
            _hasFall = false;
        }

        if (_playerInputChecks.Movement.x > 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Right;
        }
        else if (_playerInputChecks.Movement.x < 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Left;
        }

        _core.Rigidbody2D.velocity = new Vector2(_playerInputChecks.Movement.x * CurrentMoveSpeed, _core.Rigidbody2D.velocity.y);

        if (IsFalling)
        {
            if (!_core.Animator.GetCurrentAnimatorStateInfo(0).IsName(_fallClip.name))
            {
                _core.Animator.Play(_fallClip.name);
                StopMovingSound();
            }
        }
        else
        {
            if (!_core.Animator.GetCurrentAnimatorStateInfo(0).IsName(_runClip.name))
            {
                _core.Animator.Play(_runClip.name);
                PlayMovingSound();
            }
        }
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