using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Volume Settings")]
    [SerializeField]
    [Range(0f, 1f)] 
    private float _musicVolume = 0.5f;
    [SerializeField]
    [Range(0f, 1f)] 
    private float _sfxVolume = 0.5f;

    public float MusicVolume => _musicVolume;
    public float SFXVolume => _sfxVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, SoundCategory category = SoundCategory.SFX, Transform sourceTransform = null, string gameObjectName = "Sound")
    {
        GameObject soundGameObject = new GameObject(gameObjectName);
        soundGameObject.tag = category.ToString();

        if (sourceTransform != null)
        {
            soundGameObject.transform.position = sourceTransform.position;
            soundGameObject.transform.parent = sourceTransform;
        }
        else
        {
            soundGameObject.transform.position = Camera.main.transform.position;
            soundGameObject.transform.parent = Camera.main.transform;
        }

        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = (category == SoundCategory.Music) ? _musicVolume : _sfxVolume;
        audioSource.Play();

        StartCoroutine(DestroyAudioSource(audioSource, clip.length));
    }

    public AudioSource PlayLoopingSound(AudioClip clip, SoundCategory category = SoundCategory.SFX, Transform sourceTransform = null, string gameObjectName = "LoopingSound")
    {
        GameObject soundGameObject = new GameObject(gameObjectName);
        soundGameObject.tag = category.ToString();
        
        if (sourceTransform != null)
        {
            soundGameObject.transform.position = sourceTransform.position;
            soundGameObject.transform.parent = sourceTransform;
        }
        else
        {
            soundGameObject.transform.position = Camera.main.transform.position;
            soundGameObject.transform.parent = Camera.main.transform;
        }

        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = (category == SoundCategory.Music) ? _musicVolume : _sfxVolume;
        audioSource.loop = true;
        audioSource.Play();
        return audioSource;
    }

    public void StopSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }

    public void StopAllSounds(Transform parentTransform)
    {
        AudioSource[] audioSources = parentTransform.GetComponentsInChildren<AudioSource>();

        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume, SoundCategory category)
    {
        if (category == SoundCategory.Music)
        {
            _musicVolume = volume;
        }
        else if (category == SoundCategory.SFX)
        {
            _sfxVolume = volume;
        }

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (audioSource.CompareTag(category.ToString()))
            {
                audioSource.volume = volume;
            }
        }
    }

    private IEnumerator DestroyAudioSource(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }
}

public enum SoundCategory
{
    Music,
    SFX
}
