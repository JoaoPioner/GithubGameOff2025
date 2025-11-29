using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicTrack
{
    public string name;
    public AudioClip clip;
}

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music List")]
    public List<MusicTrack> musicTracks = new List<MusicTrack>();

    [Header("SFX List")]
    public List<SoundEffect> sfxClips = new List<SoundEffect>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumes();
    }

    void Start()
    {
        PlayMusicByScene(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicByScene(scene.name);
    }

    // Toque música
    public void PlayMusic(string name)
    {
        MusicTrack track = musicTracks.Find(t => t.name == name);
        if (track == null)
        {
            Debug.LogWarning("Music not found: " + name);
            return;
        }

        if (musicSource.clip == track.clip && musicSource.isPlaying)
            return;

        musicSource.clip = track.clip;
        musicSource.Play();
    }

    public void PlayMusicByScene(string sceneName)
    {
        PlayMusic(sceneName);
    }

    // Toque efeitos sonoros
    public void PlaySFX(string name)
    {
        SoundEffect sfx = sfxClips.Find(s => s.name == name);
        if (sfx == null)
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }

        sfxSource.PlayOneShot(sfx.clip);
    }

    // Ajustar volumes
    public void SetMusicVolume(float v)
    {
        musicSource.volume = v;
        PlayerPrefs.SetFloat("musicVolume", v);
    }

    public void SetSFXVolume(float v)
    {
        sfxSource.volume = v;
        PlayerPrefs.SetFloat("sfxVolume", v);
    }

    // Mutar/Desmutar
    public void ToggleMusicMute(bool muted)
    {
        musicSource.mute = !muted;
        PlayerPrefs.SetInt("musicMute", !muted ? 1 : 0);
    }

    public void ToggleSFXMute(bool muted)
    {
        sfxSource.mute = !muted;
        PlayerPrefs.SetInt("sfxMute", !muted ? 1 : 0);
    }

    // Carregar configurações de áudio salvas
    void LoadVolumes()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
            musicSource.volume = PlayerPrefs.GetFloat("musicVolume", 0.3f);

        if (PlayerPrefs.HasKey("sfxVolume"))
            sfxSource.volume = PlayerPrefs.GetFloat("sfxVolume", 0.3f);

        if (PlayerPrefs.HasKey("musicMute"))
            musicSource.mute = PlayerPrefs.GetInt("musicMute") == 1;

        if (PlayerPrefs.HasKey("sfxMute"))
            sfxSource.mute = PlayerPrefs.GetInt("sfxMute") == 1;
    }
}
