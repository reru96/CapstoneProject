using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAudioSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
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
        var audioManager = Container.Resolver.Resolve<AudioManager>();
        if (type == "Music")
        {
            audioManager.SetMusicVolume(value);
        }
        else if (type == "Sfx")
        {
            audioManager.SetSfxVolume(value);
        }

        PlayerPrefs.SetFloat($"{type}Volume", value);
    }
}
