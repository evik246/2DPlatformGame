using UnityEngine;

public abstract class Button : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;
    protected UIManager UIManager => _uiManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;
    }

    public abstract void Do();
}
