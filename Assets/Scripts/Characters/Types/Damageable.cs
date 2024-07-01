using UnityEngine;

public class Damageable : MonoBehaviour
{
    public StateMachineCore Core { get; set; }

    [Header("Parameters")]
    [SerializeField]
    private AnimationClip _hurtClip;
    [SerializeField]
    private bool _isDamageIgnored = false;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _hurtSound = null;

    private AudioSource _movingSoundSource;
    private float timeSinceHit = 0;
    private float invincibilityTime = 0.25f;
    private bool _isInvincible = false;

    public bool IsAlive => Core.CharacterStats.CurrentHealth > 0;
    public bool IsHurt { get; set; } = false;
    public bool IsInvincible => _isInvincible;
    public bool IsDamageIgnored {  get => _isDamageIgnored; set => _isDamageIgnored = value; }

    private void Update()
    {
        if (_isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                _isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !IsDamageIgnored)
        {
            SoundManager.Instance.StopAllSounds(Core.GameObject.transform);
            if (_hurtSound != null)
            {
                SoundManager.Instance.PlaySound(_hurtSound, SoundCategory.SFX, Core.GameObject.transform);
            }

            Core.CharacterStats.CurrentHealth -= damage;
            if (Core.CharacterStats.CurrentHealth < 0)
            {
                Core.CharacterStats.CurrentHealth = 0;
            }
            _isInvincible = true;

            if (Core.CharacterStats.CurrentHealth > 0)
            {
                IsHurt = true;
                if (!Core.DirectionChecks.IsOnWallFront && !Core.DirectionChecks.IsOnWallBehind)
                {
                    Core.Rigidbody2D.velocity = new Vector2(knockback.x, Core.Rigidbody2D.velocity.y + knockback.y);
                }
                else
                {
                    Core.Rigidbody2D.velocity = new Vector2(0, Core.Rigidbody2D.velocity.y);
                }
                Core.Animator.speed = 1;
                Core.Animator.Play(_hurtClip.name);
            }
            return true;
        }
        return false;
    }
}
