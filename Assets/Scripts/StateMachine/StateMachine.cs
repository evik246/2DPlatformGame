using UnityEngine;

public class StateMachine
{
    public State State { get; private set; }

    public void SetState(State newState, bool forceReset = false)
    {
        if (State != newState || forceReset)
        {
            State?.Exit();
            State = newState;
            State.Initialise();
            State.Enter();
        }
    }
}
