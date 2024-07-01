using System;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinished;

    private void AnimationFinishedTrigger()
    {
        OnFinished?.Invoke();
    }
}
