using UnityEngine;

public abstract class Projecttile : MonoBehaviour
{
    [SerializeField]
    protected AttackAbility _attackAbility;
    public AttackAbility AttackAbility { get => _attackAbility; set => _attackAbility = value; }
}
