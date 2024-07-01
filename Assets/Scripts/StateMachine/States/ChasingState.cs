using UnityEngine;

public class ChasingState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _moveClip;
    [SerializeField]
    private AnimationClip _idleClip;
    [SerializeField]
    private float _moveSpeed = 3f;
    [SerializeField]
    private bool _isFlying = false;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _movingLoopingSound = null;
    [SerializeField]
    private AudioClip _noticeSound = null;

    private Transform _target;
    private Vector2 _directionVector;
    private float _defaultGravityScale;
    private bool _hasIdle = false;
    private AudioSource _movingSoundSource;

    public float CurrentMoveSpeed
    {
        get
        {
            if (!_core.DirectionChecks.IsOnWallFront)
            {
                return _moveSpeed;
            }
            return 0;
        }
    }

    public override void Enter()
    {
        if (_isFlying)
        {
            _defaultGravityScale = _core.Rigidbody2D.gravityScale;
            _core.Rigidbody2D.gravityScale = 0f;
        }
        _core.Animator.Play(_moveClip.name);
        _target = _playerInSightChecks.Target?.transform;

        if (_noticeSound != null)
        {
            SoundManager.Instance.PlaySound(_noticeSound, SoundCategory.SFX, _core.GameObject.transform);
        }
        PlayMovingSound();
    }

    public override void FixedDo()
    {
        _directionVector = (_target.position - _core.transform.position).normalized;

        if (_directionVector.x > 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Right;
        }
        else if (_directionVector.x < 0)
        {
            _core.DirectionChecks.RunDirection = DirectionChecks.MoveDirection.Left;
        }

        if (_isFlying)
        {
            _core.Rigidbody2D.velocity = new Vector2(_directionVector.x * CurrentMoveSpeed, _directionVector.y * CurrentMoveSpeed);
        }
        else
        {
            if (_core.DirectionChecks.IsOnCliffFront)
            {
                _core.Animator.Play(_idleClip.name);
                StopMovingSound();
                _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
                _hasIdle = true;
            }
            else
            {
                if (_hasIdle)
                {
                    _core.Animator.Play(_moveClip.name);
                    PlayMovingSound();
                    _hasIdle = false;
                }
                _core.Rigidbody2D.velocity = new Vector2(_directionVector.x * CurrentMoveSpeed, _core.Rigidbody2D.velocity.y);
            }
        }
    }

    public override void Exit()
    {
        if (_isFlying)
        {
            _core.Rigidbody2D.gravityScale = _defaultGravityScale;
            _core.Rigidbody2D.velocity = Vector2.zero;
        }
        StopMovingSound();
    }

    private void PlayMovingSound()
    {
        if (_movingLoopingSound != null && _movingSoundSource == null)
        {
            _movingSoundSource = SoundManager.Instance.PlayLoopingSound(_movingLoopingSound, SoundCategory.SFX, _core.GameObject.transform);
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
