using UnityEngine;

public class NPCDieState : State
{
    [SerializeField]
    private AnimationClip _dieClip;

    [Header("Collectable items")]
    [SerializeField]
    private string _bloodFlaskPrefabPath = "Items/BloodFlask";
    [SerializeField]
    private string _xpPointPrefabPath = "Items/EXP";

    [Header("Sound Effects")]
    [SerializeField]
    private AudioClip _loopingSound = null;
    [SerializeField]
    private AudioClip _sound = null;

    private GameObject _bloodFlaskPrefab;
    private GameObject _xpPointPrefab;
    private AudioSource _loopingSoundSource;

    private PlayerStats _playerStats;

    public override void Enter()
    {
        _core.Animator.Play(_dieClip.name);

        _bloodFlaskPrefab = Resources.Load<GameObject>(_bloodFlaskPrefabPath);
        _xpPointPrefab = Resources.Load<GameObject>(_xpPointPrefabPath);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerStats = player.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player not found on the scene.");
        }

        CreateCollectableItems();

        if (_sound != null)
        {
            SoundManager.Instance.PlaySound(_sound, SoundCategory.SFX, _core.GameObject.transform);
        }
        PlayLoopingSound();
    }

    public override void FixedDo()
    {
        _core.Rigidbody2D.velocity = new Vector2(0, _core.Rigidbody2D.velocity.y);
        Destroy(_core.gameObject, 5f);
    }

    private void CreateCollectableItems()
    {
        int bloodFlasksToDrop = _playerStats.CalculateGainedBloodFlasks(_core.CharacterStats.CurrentLevel);
        int xpToDrop = _playerStats.CalculateGainedXP(_core.CharacterStats.CurrentLevel, 15);

        for (int i = 0; i < bloodFlasksToDrop; i++)
        {
            CreateItemWithRandomForce(_bloodFlaskPrefab);
        }

        for (int i = 0; i < xpToDrop; i++)
        {
            CreateItemWithRandomForce(_xpPointPrefab);
        }
    }

    private void CreateItemWithRandomForce(GameObject itemPrefab)
    {
        GameObject item = Instantiate(itemPrefab, _core.GameObject.transform.position, Quaternion.identity);
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Добавление случайной силы для имитации разбега предметов
            float randomForceX = Random.Range(-5f, 5f);
            float randomForceY = Random.Range(2f, 5f);
            rb.AddForce(new Vector2(randomForceX, randomForceY), ForceMode2D.Impulse);
        }
    }

    public override void Exit()
    {
        StopLoopingSound();
    }

    private void PlayLoopingSound()
    {
        if (_loopingSound != null && _loopingSoundSource == null)
        {
            _loopingSoundSource = SoundManager.Instance.PlayLoopingSound(_loopingSound, SoundCategory.SFX, _core.GameObject.transform);
        }
    }

    private void StopLoopingSound()
    {
        if (_loopingSoundSource != null)
        {
            SoundManager.Instance.StopSound(_loopingSoundSource);
            _loopingSoundSource = null;
        }
    }
}
