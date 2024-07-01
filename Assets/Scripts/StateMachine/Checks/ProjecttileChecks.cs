using System.Collections.Generic;
using UnityEngine;

public class ProjecttileChecks : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _detectedTargetColliders = new List<Collider2D>();
    [SerializeField]
    private LayerMask _targetLayer;
    [SerializeField]
    private LayerMask _groundLayer;

    public List<Collider2D> DetectedTargetColliders => _detectedTargetColliders;
    public bool IsGrounded {  get; private set; }

    public bool ResetDetectedColliders()
    {
        _detectedTargetColliders.Clear();
        return _detectedTargetColliders.Count == 0;
    }

    public void ResetIsGround()
    {
        IsGrounded = false;
    }

    private bool IsLayer(LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLayer(_targetLayer, collision.gameObject.layer))
        {
            _detectedTargetColliders.Add(collision);
        }
        if (IsLayer(_groundLayer, collision.gameObject.layer))
        {
            IsGrounded = true;
        }
    }
}
