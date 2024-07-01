using UnityEngine;

[RequireComponent (typeof(PlayerInputChecks), typeof(AnimationEventHandler))]
public class PlayerStateMachine : StateMachineCore
{
    [Header("States")]
    [SerializeField]
    private State _idleState;

    [SerializeField]
    private State _runState;

    [SerializeField]
    private State _jumpState;

    [SerializeField]
    private State _basicAttackState;

    [SerializeField]
    private State _dieState;

    [SerializeField]
    private State _ladderClimbState;

    [SerializeField]
    private State _healState;

    [SerializeField]
    private State _hungryBiteState;

    [SerializeField]
    private State _sharpNeedlesState;

    [SerializeField]
    private State _shadowStepState;

    [Header("Checks")]
    [SerializeField]
    private CooldownChecks _hungryBiteCooldownChecks;
    [SerializeField]
    private CooldownChecks _shadowStepCooldownChecks;
    [SerializeField]
    private BloodMagicAbility _sharpNeedlesBloodMagicAbility;

    private PlayerInputChecks _playerInputChecks;
    private PlayerDetectionChecks _playerDetectionInLadderChecks;
    private PlayerStats _playerStats;

    private void Awake()
    {
        SetupInstances();

        _playerInputChecks = GetComponent<PlayerInputChecks>();
        _playerStats = GetComponent<PlayerStats>();

        GameObject ladder = GameObject.FindGameObjectWithTag("Ladder");
        _playerDetectionInLadderChecks = ladder.GetComponent<PlayerDetectionChecks>();

        StateMachine.SetState(_idleState);
    }

    private void Update()
    {
        if (!Damageable.IsHurt && !UIManager.Instance.IsPaused)
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
        if (!Damageable.IsHurt && !UIManager.Instance.IsPaused)
        {
            StateMachine.State.FixedDo();
        }
    }

    protected override void SelectState()
    {
        if (StateMachine.State == _idleState)
        {
            if (_playerInputChecks.IsMovingHorizontally && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_runState);
            }
            else if (_playerInputChecks.IsJumpPressed && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_jumpState);
            }
            else if (_playerInputChecks.IsBasicAttackPressed && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_basicAttackState);
            }
            else if (_playerDetectionInLadderChecks.HasTarget && _playerInputChecks.IsLadderClimbPressed)
            {
                StateMachine.SetState(_ladderClimbState);
            }
            else if (_playerInputChecks.IsHealPressed && _playerStats.CurrentBloodFlasks > 0 && _playerStats.CurrentHealth < _playerStats.MaxHealth)
            {
                StateMachine.SetState(_healState);
            }
            else if (_playerInputChecks.IsHungryBitePressed && _hungryBiteCooldownChecks.HasEnded && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_hungryBiteState);
            }
            else if (_playerInputChecks.IsSharpNeedlesPressed && _playerStats.CurrentBloodCharges - _sharpNeedlesBloodMagicAbility.BloodChargeNumber >= 0 && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_sharpNeedlesState);
            }
            else if (_playerInputChecks.IsShadowStepPressed && _shadowStepCooldownChecks.HasEnded && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_shadowStepState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _runState)
        {
            if (!_playerInputChecks.IsMovingHorizontally && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_idleState);
            }
            else if (_playerInputChecks.IsJumpPressed && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_jumpState);
            }
            else if (_playerInputChecks.IsBasicAttackPressed && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_basicAttackState);
            }
            else if (_playerDetectionInLadderChecks.HasTarget && _playerInputChecks.IsLadderClimbPressed)
            {
                StateMachine.SetState(_ladderClimbState);
            }
            else if (_playerInputChecks.IsHealPressed && _playerStats.CurrentBloodFlasks > 0 && _playerStats.CurrentHealth < _playerStats.MaxHealth)
            {
                StateMachine.SetState(_healState);
            }
            else if (_playerInputChecks.IsHungryBitePressed && _hungryBiteCooldownChecks.HasEnded && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_hungryBiteState);
            }
            else if (_playerInputChecks.IsSharpNeedlesPressed && _playerStats.CurrentBloodCharges - _sharpNeedlesBloodMagicAbility.BloodChargeNumber >= 0 && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_sharpNeedlesState);
            }
            else if (_playerInputChecks.IsShadowStepPressed && _shadowStepCooldownChecks.HasEnded && DirectionChecks.IsGrounded)
            {
                StateMachine.SetState(_shadowStepState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _jumpState)
        {
            if (_jumpState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (_playerDetectionInLadderChecks.HasTarget && _playerInputChecks.IsLadderClimbPressed)
            {
                StateMachine.SetState(_ladderClimbState);
            }
            else if (_playerInputChecks.IsBasicAttackPressed)
            {
                StateMachine.SetState(_basicAttackState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _basicAttackState)
        {
            if (_basicAttackState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _ladderClimbState)
        {
            if (!_playerDetectionInLadderChecks.HasTarget || _playerInputChecks.IsJumpPressed)
            {
                StateMachine.SetState(_jumpState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _healState)
        {
            if (_healState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _hungryBiteState)
        {
            if (_hungryBiteState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _sharpNeedlesState)
        {
            if (_sharpNeedlesState.IsComplete)
            {
                StateMachine.SetState(_idleState);
            }
            else if (!Damageable.IsAlive)
            {
                StateMachine.SetState(_dieState);
            }
        }
        else if (StateMachine.State == _shadowStepState)
        {
            if (_shadowStepState.IsComplete)
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
        if (StateMachine.State == _ladderClimbState)
        {
            StateMachine.SetState(_ladderClimbState, true);
        }
        else
        {
            StateMachine.SetState(_idleState, true);
        }
    }
}
