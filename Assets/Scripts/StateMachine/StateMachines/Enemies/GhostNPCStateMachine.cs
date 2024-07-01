using UnityEngine;

public class GhostNPCStateMachine : StateMachineCore
{
    [Header("States")]
    [SerializeField]
    private State _idleState;

    [SerializeField]
    private State _chasingState;

    [SerializeField]
    private State _explosionState;

    [SerializeField]
    private State _dieState;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInAttackActivationRangeChecks;

    private void Start()
    {
        SetupInstances();
        StateMachine.SetState(_idleState);
    }

    private void Update()
    {
        if (!Damageable.IsHurt)
        {
            SelectState();
            StateMachine.State.Do();
        }
        else
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
        }
    }

    private void FixedUpdate()
    {
        if (!Damageable.IsHurt)
        {
            StateMachine.State.FixedDo();
        }
    }

    protected override void SelectState()
    {
        if (StateMachine.State == _idleState)
        {
            if (_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_chasingState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _chasingState)
        {
            if (_playerInAttackActivationRangeChecks.HasTarget)
            {
                StateMachine.SetState(_explosionState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _explosionState)
        {
            if (_explosionState.IsComplete)
            {
                Destroy(GameObject);
            }
            if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
    }

    private void StopHurt()
    {
        Animator.StopPlayback();
        Damageable.IsHurt = false;
        Damageable.IsDamageIgnored = false;
        StateMachine.SetState(_idleState, true);
    }
}
