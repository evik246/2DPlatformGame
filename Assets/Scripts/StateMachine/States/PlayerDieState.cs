using UnityEngine;

public class PlayerDieState : State
{
    [SerializeField]
    private AnimationClip _dieClip;
    [SerializeField]
    private AnimationEventHandler _eventHandler;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _fallSound = null;

    public override void Enter()
    {
        _eventHandler.OnFinished += PlayDieSound;
        _core.Animator.Play(_dieClip.name);
    }

    public override void FixedDo()
    {
        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
    }

    private void PlayDieSound()
    {
        SoundManager.Instance.PlaySound(_fallSound, SoundCategory.SFX, _core.GameObject.transform);
        _eventHandler.OnFinished += ShowDieScreen;
        _eventHandler.OnFinished -= PlayDieSound;
    }

    private void ShowDieScreen()
    {
        UIManager.Instance.GameOver();
    }

    public override void Exit()
    {
        _eventHandler.OnFinished -= ShowDieScreen;
    }
}
