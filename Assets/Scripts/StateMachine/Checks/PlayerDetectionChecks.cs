using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionChecks : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _detectedColliders = new List<Collider2D>();
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private Collider2D _playerCollider;

    public Collider2D PlayerCollider => _playerCollider;
    public Collider2D Target { get; private set; }
    public bool HasTarget => Target != null;
    public bool IsTargetAlive
    {
        get
        {
            if (!HasTarget)
            {
                return false;
            }
            return Target.GetComponent<Damageable>().IsAlive;
        }
    }

    private void Update()
    {
        if (_playerCollider != null)
        {
            Target = _playerCollider;
        }
        else
        {
            Target = null;
        }
    }

    public bool ResetDetectedCollider()
    {
        _playerCollider = null;
        return PlayerCollider == null;
    }

    private bool IsPlayerLayer(int layer)
    {
        return _playerLayer == (_playerLayer | (1 << layer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedColliders.Add(collision);
        if (IsPlayerLayer(collision.gameObject.layer))
        {
            _playerCollider = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision);
        if (_playerCollider == collision)
        {
            _playerCollider = null;
        }
    }
}
