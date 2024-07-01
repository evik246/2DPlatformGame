using UnityEngine;

public class BloodMagicAbility : MonoBehaviour
{
    [SerializeField]
    private int _bloodChargeNumber;

    public int BloodChargeNumber { get => _bloodChargeNumber; set => _bloodChargeNumber = value; }
}
