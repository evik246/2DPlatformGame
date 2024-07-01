using System.Collections.Generic;
using UnityEngine;

public class TargetBehindChecks : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _detectedColliders = new List<Collider2D>();
    [SerializeField]
    private LayerMask _targetLayer;
    [SerializeField]
    private Transform _enemyTransform;

    public bool IsTargetBehind { get; private set; }

    private void Update()
    {
        
    }
}
