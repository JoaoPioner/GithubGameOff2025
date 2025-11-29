using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class MenuSettingsInitializer : MonoBehaviour
{
    [Header("Controles de Música")]
    public Slider musicSlider;
    public Toggle musicToggle;

    [Header("Controles de SFX")]
    public Slider sfxSlider;
    public Toggle sfxToggle;

    void Start()
    {
        // Certifica-se de que o AudioManager existe
        if (AudioManager.instance == null)
        {
            Debug.LogError("AudioManager não encontrado na cena. Verifique a instância.");
            return;
        }

        // Carregar e sincronizar os controles de música e SFX com os valores salvos
        float savedMusicVolume = PlayerPrefs.GetFloat("musicVolume", 0.3f);

        bool savedMusicToggleState = PlayerPrefs.GetInt("musicToggleState", 1) == 1;

        AudioManager am = AudioManager.instance;

        // Aplica os valores carregados à UI
        if (musicSlider != null)
        {
            musicSlider.value = savedMusicVolume;
        }

        if (musicToggle != null)
        {     
            musicToggle.SetIsOnWithoutNotify(savedMusicToggleState);
        }

        // Carregar e sincronizar os controles de SFX
        float savedSfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0.3f);
        bool savedSfxToggleState = PlayerPrefs.GetInt("sfxMute", 1) == 1; 

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSfxVolume;
        }

        if (sfxToggle != null)
        {
            sfxToggle.SetIsOnWithoutNotify(savedSfxToggleState);
        }

        bool sfxToggleShouldBeOn = !(PlayerPrefs.GetInt("sfxMute", 0) == 1);

        if (sfxToggle != null)
        {
            sfxToggle.SetIsOnWithoutNotify(sfxToggleShouldBeOn);
        }
        am.SetMusicVolume(am.musicSource.volume);
        am.SetSFXVolume(am.sfxSource.volume);
    }
}