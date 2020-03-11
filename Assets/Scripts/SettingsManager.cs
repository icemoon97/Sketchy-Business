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
    private Toggle soundEffectToggle;
    private Slider soundEffectVolumeSlider;


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
            soundEffectToggle = GameObject.Find("Sound Effects Toggle").GetComponent<Toggle>();
            soundEffectVolumeSlider = GameObject.Find("Sound Effects Slider").GetComponent<Slider>();
            
            UpdateUI();

            musicToggle.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });
            musicVolumeSlider.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });
            soundEffectToggle.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });
            soundEffectVolumeSlider.onValueChanged.AddListener(delegate {
                UpdateSettings();
            });
        }
    }

    //called when any settings UI (toggles, sliders, etc) are modified
    public void UpdateSettings()
    {
        GameManager.music = musicToggle.isOn;
        GameManager.musicVol = musicVolumeSlider.value;
        GameManager.soundEffects = soundEffectToggle.isOn;
        GameManager.soundEffectVol = soundEffectVolumeSlider.value;

        if (GameManager.music)
        {
            backgroundAudio.UnPause(); 
        }
        else
        {
            backgroundAudio.Pause();
        }
        backgroundAudio.volume = GameManager.musicVol;

        UpdateUI();
    }

    //updates settings page UI
    private void UpdateUI()
    {
        musicToggle.isOn = GameManager.music;

        musicVolumeSlider.gameObject.SetActive(GameManager.music);
        musicVolumeSlider.value = GameManager.musicVol;

        soundEffectToggle.isOn = GameManager.soundEffects;

        Debug.Log(GameManager.soundEffects);
        soundEffectVolumeSlider.gameObject.SetActive(GameManager.soundEffects);
        soundEffectVolumeSlider.value = GameManager.soundEffectVol;
    }
}
