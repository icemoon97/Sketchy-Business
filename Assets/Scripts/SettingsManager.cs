using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{

    //sound 
    private AudioSource backgroundAudio;
    private Toggle musicToggle;
    private Slider musicVolumeSlider;

    //prevents duplicate settings manager from being created when going back to the main menu
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

        //makes it so onSceneLoaded is called properly
        SceneManager.sceneLoaded += OnSceneLoaded;

        backgroundAudio = GetComponent<AudioSource>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu")
        {
            musicToggle = GameObject.Find("Music Toggle").GetComponent<Toggle>();
            musicVolumeSlider = GameObject.Find("Music Volume Slider").GetComponent<Slider>();

            musicToggle.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });
            musicVolumeSlider.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });


            UpdateUI();
        }      
    }

    //called when any settings UI (toggles, sliders, etc) are modified
    public void UpdateSettings()
    {
        GameManager.music = musicToggle.isOn;
        GameManager.musicVol = musicVolumeSlider.value;

        if (GameManager.music)
        {
            backgroundAudio.UnPause();
            backgroundAudio.volume = GameManager.musicVol;
        }
        else
        {
            backgroundAudio.Pause();
        }

        UpdateUI();
    }

    //updates settings page UI
    private void UpdateUI()
    {
        musicToggle.isOn = GameManager.music;

        musicVolumeSlider.gameObject.SetActive(GameManager.music);
        musicVolumeSlider.value = GameManager.musicVol;
    }
}
