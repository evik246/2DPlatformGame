using UnityEngine;

public class PlayerJumpState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _fallClip;
    [SerializeField]
    private AnimationClip _jumpClip;
    [SerializeField]
    private AnimationClip _riseClip;
    [SerializeField]
    private float _jumpForce = 4f;
    [SerializeField]
    private float _airRunSpeed = 3f;

    [Header("Checks")]
    [SerializeField]
    private PlayerInputChecks _playerInputChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _fallSound = null;

    public float CurrentMoveSpeed
    {
        get
        {
            if (_playerInputChecks.IsMoving && !_core.DirectionChecks.IsOnWallFront)
            {
                return _airRunSpeed;
            }
            return 0;
        }
    }

    public override void Enter()
    {
        _core.Rigidbody2D.velocity = new Vector2(_core.Rigidbody2D.velocity.x, _jumpForce);
    }

    public override void Do()
    {
        if (!_core.Damageable.IsHurt)
        {
            if (_core.Rigidbody2D.velocity.y > 0)
            {
                _core.Animator.Play(_riseClip.name);
            }
            else if (_core.Rigidbody2D.velocity.y == 0)
            {
                _core.Animator.Play(_jumpClip.name);
            }
            else if (_core.Rigidbody2D.velocity.y < 0)
            {
                _core.Animator.Play(_fallClip.name);
            }
        }
    }

    public override void FixedDo()
    {
        if (_core.DirectionChecks.IsGrounded && _core.Rigidbody2D.velocity.y < 0)
        {
            if (_fallSound != null)
            {
                SoundManager.Instance.PlaySound(_fallSound, SoundCategory.SFX, _core.GameObject.transform);
            }
            IsComplete = true;
            return;
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
    }

    public override void Exit()
    {
        IsComplete = true;
    }
}
