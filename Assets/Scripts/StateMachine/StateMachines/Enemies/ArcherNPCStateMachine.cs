using UnityEngine;

public class ArcherNPCStateMachine : StateMachineCore
{
    [Header("States")]
    [SerializeField]
    private State _patrollingState;

    [SerializeField]
    private State _idleState;

    [SerializeField]
    private State _rangedAttackState;

    [SerializeField]
    private State _bounceState;

    [SerializeField]
    private State _dieState;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInBounceChecks;

    [SerializeField]
    private CooldownChecks _attackCooldownChecks;

    [SerializeField]
    private CooldownChecks _bounceCooldownChecks;

    private void Start()
    {
        SetupInstances();
        StateMachine.SetState(_patrollingState);
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
        if (StateMachine.State == _patrollingState) 
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
            if (!_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_patrollingState);
            }
            else if (_attackCooldownChecks.HasEnded && _playerInSightChecks.IsTargetAlive)
            {
                StateMachine.SetState(_rangedAttackState);
            }
            else if (_bounceCooldownChecks.HasEnded && _playerInBounceChecks.HasTarget && !_attackCooldownChecks.HasEnded)
            {
                StateMachine.SetState(_bounceState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _rangedAttackState)
        {
            if (_rangedAttackState.IsComplete || !_playerInSightChecks.IsTargetAlive)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _bounceState)
        {
            if (_bounceState.IsComplete || !_playerInSightChecks.IsTargetAlive)
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
