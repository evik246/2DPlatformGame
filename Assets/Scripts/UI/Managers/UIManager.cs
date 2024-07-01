using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Music For All Scenes")]
    [SerializeField]
    private List<AudioClip> _musicSounds = new();

    private GameObject _gameOverScreen;
    private GameObject _pauseScreen;
    private GameObject _settingsScreen;
    private GameObject _controlsScreen;
    private bool _isPauseAllowed = true;
    public bool IsPaused { get; private set; } = false;

    private PlayerStats _playerStats;

    private void Awake()
    {
        //Debug.Log("UIManager");
        if (Instance == null)
        {
            Instance = this;
        }

        _gameOverScreen = GameObject.FindWithTag("GameOver");
        _pauseScreen = GameObject.FindWithTag("Pause");
        _settingsScreen = GameObject.FindWithTag("Settings");
        _controlsScreen = GameObject.FindWithTag("Controls");

        _gameOverScreen?.SetActive(false);
        _pauseScreen?.SetActive(false);
        _settingsScreen?.SetActive(false);
        _controlsScreen?.SetActive(false);

        StartCoroutine(PlayMusicWithDelay());
    }

    private IEnumerator PlayMusicWithDelay()
    {
        yield return null;
        if (SoundManager.Instance != null && SceneManager.GetActiveScene().buildIndex < _musicSounds.Count)
        {
            Debug.Log("Playing music for the scene");
            SoundManager.Instance.PlayLoopingSound(_musicSounds[SceneManager.GetActiveScene().buildIndex], SoundCategory.Music);
        }
        else
        {
            Debug.Log("SoundManager.Instance is null or no music for this scene");
        }
    }

    private void Update()
    {
       if (_pauseScreen == null)
       {
           return;
       }

       if (Input.GetKeyDown(KeyCode.Escape) && _isPauseAllowed)
       {
           if (_pauseScreen.activeInHierarchy)
           {
               PauseGame(false);
           }
           else
           {
               PauseGame(true);
           }
       }
    }

   private void PauseGame(bool status)
   {
       _pauseScreen.SetActive(status);
       IsPaused = status;

       if (status)
       {
           Time.timeScale = 0;
       }
       else
       {
           Time.timeScale = 1;
       }
   }

   public void Quit()
   {
       Application.Quit();

#if UNITY_EDITOR
       UnityEditor.EditorApplication.isPlaying = false;
#endif
   }

   public void GameOver()
   {
       _gameOverScreen.SetActive(true);
       _isPauseAllowed = false;
   }

   public void RestartGame()
   {
       Time.timeScale = 1;
       SceneManager.LoadScene(1);
   }

   public void RestartLevel()
   {
       Time.timeScale = 1;
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
   }

   public void MainMenu()
   {
       SceneManager.LoadScene(0);
   }

   public void NextLevel(PlayerStats playerStats)
   {
       Time.timeScale = 1;
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

       PlayerState.Instance.SavePlayerStats(playerStats);
       PlayerState.Instance.RestorePlayerStats(playerStats);

       _playerStats = playerStats;
   }

   public void StartGame()
   {
       Time.timeScale = 1;
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

       if (PlayerStats.Instance != null)
       {
            PlayerStats.Instance.ResetToBaseStats();
            PlayerState.Instance.SavePlayerStats(PlayerStats.Instance);
       }
   }

    public void Settings()
    {
        _pauseScreen.SetActive(false);
        _controlsScreen.SetActive(false);
        _settingsScreen.SetActive(true);
        Time.timeScale = 0;
        _isPauseAllowed = false;
    }

    public void Controls()
    {
        _settingsScreen.SetActive(false);
        _controlsScreen.SetActive(true);
        Time.timeScale = 0;
        _isPauseAllowed = false;
    }

    public void Pause()
    {
        _settingsScreen.SetActive(false);
        _controlsScreen.SetActive(false);
        _pauseScreen.SetActive(true);
        _isPauseAllowed = true;
    }
}
