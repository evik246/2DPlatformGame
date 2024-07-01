using System.Collections.Generic;
using UnityEngine;

public class NPCDetectionChecks : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _detectedColliders = new List<Collider2D>();
    [SerializeField]
    private LayerMask _NPCLayer;

    public List<Collider2D> DetectedColliders => _detectedColliders;

    public bool ResetDetectedColliders()
    {
        _detectedColliders.Clear();
        return _detectedColliders.Count == 0;
    }

    private bool IsNPCLayer(int layer)
    {
        return _NPCLayer == (_NPCLayer | (1 << layer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsNPCLayer(collision.gameObject.layer))
        {
            _detectedColliders.Add(collision);
        }
    }
}
