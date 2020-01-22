using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    //sound
    private AudioSource backgroundAudio;
    public Toggle musicToggle;
    public Slider musicVolumeSlider;

    public static SettingsManager instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        backgroundAudio = GetComponent<AudioSource>();
        UpdateSettings();
    }

    //called when any settings UI (toggles, sliders, etc) are modified
    public void UpdateSettings()
    {
        musicVolumeSlider.gameObject.SetActive(musicToggle.isOn);

        if (musicToggle.isOn)
        {
            backgroundAudio.UnPause();

            backgroundAudio.volume = musicVolumeSlider.value;
        }
        else
        {
            backgroundAudio.Pause();
        }
    }
}
