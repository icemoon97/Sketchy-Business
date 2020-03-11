using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audio;

    public void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audio.volume = GameManager.soundEffectVol;
        if (GameManager.soundEffects)
        {
            audio.Play(0);
        }
    }
}
