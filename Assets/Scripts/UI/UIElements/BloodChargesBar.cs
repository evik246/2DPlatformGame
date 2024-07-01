using UnityEngine;

public class BloodChargesBar : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private int _currentNumber = 0;
    [SerializeField]
    private int _maxNumber = 0;
    [SerializeField]
    private Transform _barTransform;

    [Header("Resources")]
    [SerializeField]
    private string _chargePrefabPath = "UI/Bars/BloodCharge";
    [SerializeField]
    private string _emptyChargePrefabPath = "UI/Bars/EmptyBloodCharge";

    private GameObject _chargePrefab;
    private GameObject _emptyChargePrefab;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _chargePrefab = Resources.Load<GameObject>(_chargePrefabPath);
        _emptyChargePrefab = Resources.Load<GameObject>(_emptyChargePrefabPath);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player not found on the scene.");
        }

        _playerStats.OnCurrentBloodChargesChanged += DrawCharges;
        _playerStats.OnMaxBloodChargesChanged += DrawCharges;
    }

    private void Start()
    {
        DrawCharges();
    }

    private void DrawCharges()
    {
        _currentNumber = _playerStats.CurrentBloodCharges;
        _maxNumber = _playerStats.MaxBloodCharges;

        foreach (Transform child in _barTransform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _maxNumber; i++)
        {
            GameObject chargePrefab = (i + 1 <= _currentNumber) ? _chargePrefab : _emptyChargePrefab;
            GameObject charge = Instantiate(chargePrefab, _barTransform.position, Quaternion.identity);
            charge.transform.SetParent(_barTransform, false); // Setting the parent with worldPositionStays set to false
        }
    }

    private void OnDestroy()
    {
        _playerStats.OnCurrentBloodChargesChanged -= DrawCharges;
        _playerStats.OnMaxBloodChargesChanged -= DrawCharges;
    }
}
