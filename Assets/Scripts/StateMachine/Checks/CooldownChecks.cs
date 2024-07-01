using UnityEngine;

public class CooldownChecks : MonoBehaviour
{
    [Header("Cooldown")]
    [SerializeField]
    private float _cooldown = 5f;
    public float CoolDown => _cooldown;

    [SerializeField]
    private float _currentCooldown = 0f;
    public float CurrentCooldown => _currentCooldown;

    public bool HasEnded => _currentCooldown <= 0f;

    private void Update()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    public void ResetCooldown()
    {
        _currentCooldown = _cooldown;
    }
}
