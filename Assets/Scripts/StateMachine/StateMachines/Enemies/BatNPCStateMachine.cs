using UnityEngine;

public class BatNPCStateMachine : StateMachineCore
{
    [Header("States")]
    [SerializeField]
    private State _sleepingState;

    [SerializeField]
    private State _idleState;

    [SerializeField]
    private State _chasingState;

    [SerializeField]
    private State _meleeAttackState;

    [SerializeField]
    private State _dieState;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInAttackActivationRangeChecks;

    [SerializeField]
    private CooldownChecks _attackCooldownChecks;

    private void Start()
    {
        SetupInstances();
        StateMachine.SetState(_sleepingState);
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
        if (StateMachine.State == _sleepingState)
        {
            if (_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _idleState)
        {
            if (_playerInSightChecks.HasTarget && !_playerInAttackActivationRangeChecks.HasTarget)
            {
                StateMachine.SetState(_chasingState);
            }
            else if (_playerInAttackActivationRangeChecks.HasTarget && _attackCooldownChecks.HasEnded && _playerInAttackActivationRangeChecks.IsTargetAlive)
            {
                StateMachine.SetState(_meleeAttackState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _chasingState)
        {
            if (_playerInAttackActivationRangeChecks.HasTarget || !_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _meleeAttackState)
        {
            if (_meleeAttackState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
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
