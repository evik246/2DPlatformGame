using UnityEngine;

public abstract class State : MonoBehaviour
{
    public bool IsComplete {  get; set; }

    protected StateMachineCore _core;

    public virtual void Enter() { }
    public virtual void Do() { }
    public virtual void FixedDo() { }
    public virtual void Exit() { }

    public void Setup(StateMachineCore core)
    {
        _core = core;
    }

    public void Initialise()
    {
        IsComplete = false;
    }
}
