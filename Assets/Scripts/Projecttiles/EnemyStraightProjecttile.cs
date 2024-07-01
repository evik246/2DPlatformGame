using System.Linq;
using UnityEngine;

public class EnemyStraightProjecttile : Projecttile
{
    [SerializeField]
    private Vector2 _targetKnockback = Vector2.zero;
    [SerializeField]
    private Vector2 _projecttileKnockback = Vector2.zero;
    [SerializeField]
    private Vector2 _projecttileSpeed = new Vector2(3f, 0f);

    [Header("Checks")]
    [SerializeField]
    private ProjecttileChecks _projecttileChecks;
    [SerializeField]
    private CooldownChecks _flightTimeChecks;

    private Vector2 _deliveredKnockback = Vector2.zero;
    private Rigidbody2D _rigidbody;
    private bool _hasAttacked;
    private bool _hasProjecttileKnockback;
    private float _defaultGravity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _hasAttacked = false;
        _hasProjecttileKnockback = false;
        _defaultGravity = _rigidbody.gravityScale;
        _flightTimeChecks.ResetCooldown();
    }

    private void FixedUpdate()
    {
        if (!_projecttileChecks.IsGrounded && !_hasProjecttileKnockback && !_flightTimeChecks.HasEnded)
        {
            _rigidbody.gravityScale = 0f;
            _rigidbody.velocity = new Vector2(_projecttileSpeed.x * transform.localScale.x, 0f);
        }
        else if (_flightTimeChecks.HasEnded && !_hasProjecttileKnockback)
        {
            _rigidbody.gravityScale = _defaultGravity;
            if (_projecttileChecks.IsGrounded)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject, 1f);
            }
        }
        else if (_projecttileChecks.IsGrounded && !_hasProjecttileKnockback)
        {
            _rigidbody.gravityScale = _defaultGravity;
            Vector2 knockback = new Vector2(-_projecttileKnockback.x * Mathf.Sign(transform.localScale.x), _projecttileKnockback.y);
            _rigidbody.velocity = knockback;
            _hasProjecttileKnockback = true;
            _projecttileChecks.ResetIsGround();
        }
        else if (_projecttileChecks.IsGrounded && _hasProjecttileKnockback)
        {
            Destroy(gameObject);
        }

        if (!_hasAttacked && !_hasProjecttileKnockback && !_flightTimeChecks.HasEnded)
        {
            Collider2D playerCollider = _projecttileChecks.DetectedTargetColliders.FirstOrDefault();
            if (playerCollider != null)
            {
                Destroy(gameObject);
                Damageable damageable = playerCollider.GetComponent<Damageable>();
                if (damageable != null)
                {
                    _hasAttacked = true;
                    _deliveredKnockback = new Vector2(_targetKnockback.x * transform.localScale.x, _targetKnockback.y);
                    bool gotHit = damageable.Hit(_attackAbility.CurrentDamage, _deliveredKnockback);
                }
            }
        }
    }
}
