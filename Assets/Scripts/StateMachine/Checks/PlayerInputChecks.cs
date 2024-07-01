using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputChecks : MonoBehaviour
{
    [SerializeField]
    private InputAction _move;
    [SerializeField]
    private InputAction _jump;
    [SerializeField]
    private InputAction _basicAttack;
    [SerializeField]
    private InputAction _ladderClimb;
    [SerializeField]
    private InputAction _heal;
    [SerializeField]
    private InputAction _hungryBite;
    [SerializeField]
    private InputAction _sharpNeedles;
    [SerializeField]
    private InputAction _shadowStep;

    public InputAction Move { get => _move; set => _move = value; }
    public InputAction Jump { get => _jump; set => _jump = value; }
    public InputAction BasicAttack { get => _basicAttack; set => _basicAttack = value; }
    public InputAction LadderClimb { get => _ladderClimb; set => _ladderClimb = value; }
    public InputAction Heal { get => _heal; set => _heal = value; }
    public InputAction HungryBite { get => _hungryBite; set => _hungryBite = value; }
    public InputAction SharpNeedles { get => _sharpNeedles; set => _sharpNeedles = value; }
    public InputAction ShadowStep { get => _shadowStep; set => _shadowStep = value; }

    public Vector2 Movement {  get; private set; }

    [SerializeField]
    private bool _isMoving;
    public bool IsMoving { get => _isMoving; private set => _isMoving = value; }

    [SerializeField]
    private bool _isMovingHorizontally;
    public bool IsMovingHorizontally { get => _isMovingHorizontally; private set => _isMovingHorizontally = value; }

    [SerializeField]
    private bool _isMovingVertically;
    public bool IsMovingVertically { get => _isMovingVertically; private set => _isMovingVertically = value; }

    [SerializeField]
    private bool _isJumpPressed;
    public bool IsJumpPressed { get => _isJumpPressed; private set => _isJumpPressed = value; }

    [SerializeField]
    private bool _isBasicAttackPressed;
    public bool IsBasicAttackPressed { get => _isBasicAttackPressed; private set => _isBasicAttackPressed = value; }

    [SerializeField]
    private bool _isLadderClimbPressed;
    public bool IsLadderClimbPressed { get => _isLadderClimbPressed; private set => _isLadderClimbPressed = value; }

    [SerializeField]
    private bool _isHealPressed;
    public bool IsHealPressed { get => _isHealPressed; private set => _isHealPressed = value; }

    [SerializeField]
    private bool _isHungryBitePressed;
    public bool IsHungryBitePressed { get => _isHungryBitePressed; private set => _isHungryBitePressed = value; }

    [SerializeField]
    private bool _isSharpNeedlesPressed;
    public bool IsSharpNeedlesPressed { get => _isSharpNeedlesPressed; private set => _isSharpNeedlesPressed = value; }

    [SerializeField]
    private bool _isShadowStepPressed;
    public bool IsShadowStepPressed { get => _isShadowStepPressed; private set => _isShadowStepPressed = value; }

    private void OnEnable()
    {
        _move.Enable();
        _jump.Enable();
        _basicAttack.Enable();
        _ladderClimb.Enable();
        _heal.Enable();
        _hungryBite.Enable();
        _sharpNeedles.Enable();
        _shadowStep.Enable();

        _jump.started += ctx => IsJumpPressed = true;
        _jump.canceled += ctx => IsJumpPressed = false;

        _basicAttack.started += ctx => IsBasicAttackPressed = true;
        _basicAttack.canceled += ctx => IsBasicAttackPressed = false;

        _ladderClimb.started += ctx => IsLadderClimbPressed = true;
        _ladderClimb.canceled += ctx => IsLadderClimbPressed = false;

        _heal.started += ctx => IsHealPressed = true;
        _heal.canceled += ctx => IsHealPressed = false;

        _hungryBite.started += ctx => IsHungryBitePressed = true;
        _hungryBite.canceled += ctx => IsHungryBitePressed = false;

        _sharpNeedles.started += ctx => IsSharpNeedlesPressed = true;
        _sharpNeedles.canceled += ctx => IsSharpNeedlesPressed = false;

        _shadowStep.started += ctx => IsShadowStepPressed = true;
        _shadowStep.canceled += ctx => IsShadowStepPressed = false;
    }

    private void Update()
    {
        Movement = _move.ReadValue<Vector2>();
        IsMoving = Movement != Vector2.zero;
        IsMovingHorizontally = Mathf.Abs(Movement.x) > 0;
        IsMovingVertically = Mathf.Abs(Movement.y) > 0;
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
        _basicAttack.Disable();
        _ladderClimb.Disable();
        _heal.Disable();
        _hungryBite.Disable();
        _sharpNeedles.Disable();
        _shadowStep.Disable();
    }
}
