using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
[RequireComponent (typeof(DirectionChecks), typeof(Damageable))]
[RequireComponent(typeof(CharacterStats))]
public abstract class StateMachineCore : MonoBehaviour
{
    public StateMachine StateMachine { get; protected set; }
    public State ActiveState => StateMachine.State;

    public Rigidbody2D Rigidbody2D {  get; protected set; }
    public Animator Animator { get; protected set; }
    public GameObject GameObject { get; protected set; }
    public DirectionChecks DirectionChecks { get; protected set; }
    public Damageable Damageable { get; protected set; }
    public CharacterStats CharacterStats { get; protected set; }

    public void SetupInstances()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        DirectionChecks = GetComponent<DirectionChecks>();
        Animator = GetComponent<Animator>();
        CharacterStats = GetComponent<CharacterStats>();
        Damageable = GetComponent<Damageable>();
        Damageable.Core = this;
        GameObject = gameObject;

        StateMachine = new StateMachine();

        State[] allStates = GetComponentsInChildren<State>();
        foreach (var state in allStates)
        {
            state.Setup(this);
        }
    }

    protected abstract void SelectState();
}
