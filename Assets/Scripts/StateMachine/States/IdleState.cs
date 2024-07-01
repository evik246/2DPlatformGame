using UnityEngine;

public class IdleState : State
{
    [SerializeField]
    private AnimationClip _idleClip;
    [SerializeField]
    private bool _isFlying = false;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _loopingSound = null;

    private float _defaultGravityScale;
    private AudioSource _loopingSoundSource;

    public override void Enter()
    {
        if (_isFlying)
        {
            _defaultGravityScale = _core.Rigidbody2D.gravityScale;
            _core.Rigidbody2D.gravityScale = 0f;
        }

        _core.Animator.StopPlayback();
        _core.Animator.Play(_idleClip.name);
        PlayLoopingSound();
    }

    public override void FixedDo()
    {
        if (_isFlying)
        {
            _core.Rigidbody2D.velocity = Vector2.zero;
        }
        else
        {
            _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
        }
    }

    public override void Exit()
    {
        if (_isFlying)
        {
            _core.Rigidbody2D.gravityScale = _defaultGravityScale;
        }
        StopLoopingSound();
    }

    private void PlayLoopingSound()
    {
        if (_loopingSound != null && _loopingSoundSource == null)
        {
            _loopingSoundSource = SoundManager.Instance.PlayLoopingSound(_loopingSound, SoundCategory.SFX, _core.GameObject.transform);
        }
    }

    private void StopLoopingSound()
    {
        if (_loopingSoundSource != null)
        {
            SoundManager.Instance.StopSound(_loopingSoundSource);
            _loopingSoundSource = null;
        }
    }
}
