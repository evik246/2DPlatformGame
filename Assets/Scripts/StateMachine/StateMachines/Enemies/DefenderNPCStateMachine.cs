using UnityEngine;

public class DefenderNPCStateMachine : StateMachineCore
{
    [Header("States")]
    [SerializeField]
    private State _patrollingState;

    [SerializeField]
    private State _idleState;

    [SerializeField]
    private State _chasingState;

    [SerializeField]
    private State _meleeAttackState;

    [SerializeField]
    private State _shieldBlockState;

    [SerializeField]
    private State _dieState;

    [Header("Checks")]
    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInSightChecks;

    [SerializeField]
    private PlayerDetectionIncludingGroundChecks _playerInAttackActivationRangeChecks;

    [SerializeField]
    private CooldownChecks _attackCooldownChecks;

    [SerializeField]
    private CooldownChecks _shieldBlockCooldownChecks;

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
                StateMachine.SetState(_chasingState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _chasingState)
        {
            if (!_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_patrollingState);
            }
            else if (_playerInAttackActivationRangeChecks.HasTarget)
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
            if (!_playerInAttackActivationRangeChecks.HasTarget)
            {
                StateMachine.SetState(_chasingState);
            }
            else if (!_playerInSightChecks.HasTarget)
            {
                StateMachine.SetState(_patrollingState);
            }
            else if (_attackCooldownChecks.HasEnded && !_shieldBlockCooldownChecks.HasEnded && _playerInSightChecks.IsTargetAlive)
            {
                StateMachine.SetState(_meleeAttackState);
            }
            else if (_shieldBlockCooldownChecks.HasEnded && _playerInSightChecks.IsTargetAlive)
            {
                StateMachine.SetState(_shieldBlockState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _meleeAttackState)
        {
            if (_meleeAttackState.IsComplete || !_playerInSightChecks.IsTargetAlive)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _shieldBlockState)
        {
            if (_shieldBlockState.IsComplete || !_playerInSightChecks.IsTargetAlive)
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
