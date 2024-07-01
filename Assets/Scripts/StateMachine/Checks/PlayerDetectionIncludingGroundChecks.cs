using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDetectionIncludingGroundChecks : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _detectedColliders = new List<Collider2D>();
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private Transform _enemyTransform;

    public Collider2D DetectCollider { get; private set; }
    public Collider2D Target { get; private set; }
    public bool HasTarget { get => Target != null; }
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

    private void Awake()
    {
        DetectCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        /*
        Collider2D playerCollider = detectedColliders.FirstOrDefault(collider => IsPlayerLayer(collider.gameObject.layer));
        Target = detectedColliders.IndexOf(playerCollider) == 0 ? playerCollider : null;
        */

        Collider2D playerCollider = _detectedColliders.FirstOrDefault(collider => IsPlayerLayer(collider.gameObject.layer));

        if (playerCollider != null)
        {
            if (CanSeePlayer(playerCollider))
            {
                Target = playerCollider;
            }
            else
            {
                Target = null;
            }
        }
        else
        {
            Target = null;
        }
    }

    private bool IsPlayerLayer(int layer)
    {
        return _playerLayer == (_playerLayer | (1 << layer));
    }

    private bool CanSeePlayer(Collider2D playerCollider)
    {
        float playerColliderHeight = playerCollider.bounds.size.y * 0.5f;
        float enemyColliderHeight = _enemyTransform.GetComponent<Collider2D>().bounds.size.y * 0.5f;

        Vector2 playerCenter = playerCollider.bounds.center;
        playerCenter.y -= playerColliderHeight / 2;

        Vector2 enemyCenter = _enemyTransform.position;
        enemyCenter.y -= enemyColliderHeight / 2;

        Debug.DrawLine(enemyCenter, playerCenter, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(enemyCenter, playerCenter - enemyCenter, Mathf.Infinity, _groundLayer);

        if (hit.collider == null)
        {
            return true;
        }

        if (hit.collider != null && IsGroundLayer(hit.collider.gameObject.layer))
        {
            float distanceToGround = Vector2.Distance(enemyCenter, hit.point);
            //Debug.Log("distanceToGround: " + distanceToGround);

            float distanceToPlayer = Vector2.Distance(enemyCenter, playerCenter);
            //Debug.Log("distanceToPlayer: " + distanceToPlayer);

            if (distanceToPlayer < distanceToGround)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsGroundLayer(int layer)
    {
        return _groundLayer == (_groundLayer | (1 << layer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _detectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision);
    }
}