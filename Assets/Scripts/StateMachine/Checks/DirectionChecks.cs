using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DirectionChecks : MonoBehaviour
{
    [SerializeField]
    private ContactFilter2D contactFilter;
    [SerializeField]
    private float groundDistance = 0.02f;
    [SerializeField]
    private float wallDistanceFront = 0.2f;
    [SerializeField]
    private float wallDistanceBehind = 0.3f;
    [SerializeField]
    private float ceilingDistance = 0.05f;
    [SerializeField]
    private float cliffDistanceFront = 1.2f;
    [SerializeField]
    private float cliffDistanceBehind = 1.3f;

    private Collider2D bodyCollider;
    public Collider2D BodyCollider {  get { return bodyCollider; } }
    
    public float GroundDistance => groundDistance;
    public float WallDistanceFront => wallDistanceFront;
    public float WallDistanceBehing => wallDistanceBehind;
    public float CeilingDistance => ceilingDistance;
    public float CliffDistanceFront => cliffDistanceFront;
    public float CliffDistanceBehind => cliffDistanceBehind;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];
    RaycastHit2D[] cliffFrontHits = new RaycastHit2D[5];
    RaycastHit2D[] cliffBehindHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded = true;
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value;} }

    [SerializeField]
    private bool _isOnWallFront = true;
    public bool IsOnWallFront { get { return _isOnWallFront; } set { _isOnWallFront = value; } }

    [SerializeField]
    private bool _isOnWallBehind = true;
    public bool IsOnWallBehind { get { return _isOnWallBehind; } set { _isOnWallBehind = value; } }

    [SerializeField]
    private bool _isOnCeiling = true;
    public bool IsOnCeiling { get { return _isOnCeiling; } set { _isOnCeiling = value; } }

    [SerializeField]
    private bool _isOnCliffFront = true;
    public bool IsOnCliffFront { get { return _isOnCliffFront; } set { _isOnCliffFront = value; } }

    [SerializeField]
    private bool _isOnCliffBehind = true;
    public bool IsOnCliffBehind { get { return _isOnCliffBehind; } set { _isOnCliffBehind = value; } }

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    private Vector2 wallCheckDirectionBack => gameObject.transform.localScale.x > 0 ? Vector2.left : Vector2.right;

    private Vector2 _runDirectionVector = Vector2.right;
    public Vector2 RunDirectionVector { get => _runDirectionVector; }

    public enum MoveDirection { Right, Left }

    [SerializeField]
    private MoveDirection _runDirection;
    public MoveDirection RunDirection
    {
        get => _runDirection;
        set
        {
            if (_runDirection != value)
            {
                bodyCollider.gameObject.transform.localScale = new Vector2(bodyCollider.gameObject.transform.localScale.x * -1, bodyCollider.gameObject.transform.localScale.y);

                if (value == MoveDirection.Right)
                {
                    _runDirectionVector = Vector2.right;
                }
                else if (value == MoveDirection.Left)
                {
                    _runDirectionVector = Vector2.left;
                }
            }
            _runDirection = value;
        }
    }

    private void Awake()
    {
        bodyCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        //IsGrounded = bodyCollider.Cast(Vector2.down, contactFilter, groundHits, groundDistance) > 0;
        IsOnWallFront = bodyCollider.Cast(wallCheckDirection, contactFilter, wallHits, wallDistanceFront) > 0;
        IsOnWallBehind = bodyCollider.Cast(wallCheckDirectionBack, contactFilter, wallHits, wallDistanceBehind) > 0;
        IsOnCeiling = bodyCollider.Cast(Vector2.up, contactFilter, ceilingHits, ceilingDistance) > 0;

        Vector2 groundOrigin = GetOrigin(bodyCollider, Vector2.down, groundDistance) + Vector2.down * groundDistance;
        Vector2 cliffFrontOrigin = new Vector2(groundOrigin.x, groundOrigin.y);
        Vector2 cliffBehindOrigin = new Vector2(groundOrigin.x, groundOrigin.y);

        Vector2 leftGroundOrigin = GetGroundOrigin(bodyCollider, true);
        Vector2 rightGroundOrigin = GetGroundOrigin(bodyCollider, false);

        IsGrounded = Physics2D.Raycast(leftGroundOrigin, Vector2.down, groundDistance, contactFilter.layerMask) ||
                     Physics2D.Raycast(rightGroundOrigin, Vector2.down, groundDistance, contactFilter.layerMask) ||
                     bodyCollider.Cast(Vector2.down, contactFilter, groundHits, groundDistance) > 0;

        IsOnCliffFront = Physics2D.Raycast(cliffFrontOrigin, wallCheckDirection, contactFilter, cliffFrontHits, cliffDistanceFront) != 0;
        IsOnCliffBehind = Physics2D.Raycast(cliffBehindOrigin, wallCheckDirectionBack, contactFilter, cliffBehindHits, cliffDistanceBehind) != 0;

        DrawColliderCast(bodyCollider, Vector2.down, groundDistance, Color.red);
        DrawColliderCast(bodyCollider, wallCheckDirection, wallDistanceFront, Color.blue);
        DrawColliderCast(bodyCollider, wallCheckDirectionBack, wallDistanceBehind, Color.green);
        DrawColliderCast(bodyCollider, Vector2.up, ceilingDistance, Color.yellow);

        Debug.DrawRay(leftGroundOrigin, Vector2.down * groundDistance, Color.red);
        Debug.DrawRay(rightGroundOrigin, Vector2.down * groundDistance, Color.red);
        Debug.DrawRay(cliffFrontOrigin, wallCheckDirection * cliffDistanceFront, Color.cyan);
        Debug.DrawRay(cliffBehindOrigin, wallCheckDirectionBack * cliffDistanceBehind, Color.cyan);
    }

    public void DrawColliderCast(Collider2D collider, Vector2 direction, float distance, Color color)
    {
        Vector2 origin = GetOrigin(collider, direction, distance);
        Debug.DrawRay(origin, direction * distance, color);
    }

    public Vector2 GetGroundOrigin(Collider2D collider, bool isLeft)
    {
        Bounds bounds = collider.bounds;
        float offset = isLeft ? -bounds.extents.x : bounds.extents.x;
        return new Vector2(bounds.center.x + offset, bounds.min.y);
    }

    public Vector2 GetOrigin(Collider2D collider, Vector2 direction, float distance)
    {
        Bounds bounds = collider.bounds;

        if (direction == Vector2.down)
        {
            return new Vector2(bounds.center.x, bounds.min.y);
        }
        else if (direction == Vector2.up)
        {
            return new Vector2(bounds.center.x, bounds.max.y);
        }
        else
        {
            float xOrigin = direction.x > 0 ? bounds.max.x : bounds.min.x;
            return new Vector2(xOrigin, bounds.center.y);
        }
    }

    public void FlipDirection()
    {
        if (RunDirection == MoveDirection.Right)
        {
            RunDirection = MoveDirection.Left;
        }
        else if (RunDirection == MoveDirection.Left)
        {
            RunDirection = MoveDirection.Right;
        }
        else
        {
            Debug.LogError("Current movedirection is not set to legal values of right or left");
        }
    }
}
