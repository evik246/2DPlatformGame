using UnityEngine;

public class PlayerLadderClimbState : State
{
    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _ladderClimbClip;
    [SerializeField]
    private float _climbSpeed = 4f;
    [SerializeField]
    private float _smallJumpHeight = 2f;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionChecks _playerDetectionInLadderChecks;
    [SerializeField]
    private PlayerInputChecks _playerInputChecks;

    private float _defaultGravityScale;
    private bool _isClimbing;

    public override void Enter()
    {
        _defaultGravityScale = _core.Rigidbody2D.gravityScale;
        _core.Rigidbody2D.gravityScale = 0f;
        _core.Animator.speed = 0;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerInputChecks = player?.GetComponent<PlayerInputChecks>();

        GameObject ladder = GameObject.FindGameObjectWithTag("Ladder");
        _playerDetectionInLadderChecks = ladder.GetComponent<PlayerDetectionChecks>();

        _core.Animator.Play(_ladderClimbClip.name);

        _isClimbing = false;
    }

    public override void FixedDo()
    {
        if (_playerInputChecks.IsMovingVertically)
        {
            if (!_isClimbing)
            {
                _core.Animator.speed = 1;
                _isClimbing = true;
            }
            _core.Rigidbody2D.velocity = new Vector2(0, _playerInputChecks.Movement.y * _climbSpeed);
        }
        else if (!_playerInputChecks.IsMovingVertically)
        {
            if (_isClimbing)
            {
                _core.Animator.speed = 0;
                _isClimbing = false;
            }
            _core.Rigidbody2D.velocity = Vector2.zero;
        }
    }

    public override void Exit()
    {
        _core.Rigidbody2D.gravityScale = _defaultGravityScale;
        _core.Animator.speed = 1;
    }
}
