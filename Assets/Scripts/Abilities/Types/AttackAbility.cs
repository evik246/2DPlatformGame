using UnityEngine;

public class AttackAbility : MonoBehaviour
{
    [SerializeField]
    private int _baseDamage = 10;
    [SerializeField]
    private int _currentDamage = 10;

    public int BaseDamage
    {
        get => _baseDamage;
        set => _baseDamage = value > _currentDamage ? _currentDamage : value;
    }

    public int CurrentDamage { get => _currentDamage; set => _currentDamage = value; }
}
