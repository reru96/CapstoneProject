using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private AudioManager audioManager;

    private IEnumerator Start()
    {

        audioManager = CoreSystem.Instance.Container.Resolve<AudioManager>();

        if (audioManager == null)
        {
            Debug.LogError("[UIAudioSettings] AudioManager non trovato!");
            yield break;
        }

        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVol = PlayerPrefs.GetFloat("SfxVolume", 0.8f);

        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        ApplyVolume("Music", musicVol);
        ApplyVolume("Sfx", sfxVol);

        musicSlider.onValueChanged.AddListener(v => ApplyVolume("Music", v));
        sfxSlider.onValueChanged.AddListener(v => ApplyVolume("Sfx", v));
    }

    private void ApplyVolume(string type, float value)
    {
        if (audioManager == null) return; 

        if (type == "Music")
            audioManager.SetMusicVolume(value);
        else if (type == "Sfx")
            audioManager.SetSfxVolume(value);

        PlayerPrefs.SetFloat($"{type}Volume", value);
    }
}
