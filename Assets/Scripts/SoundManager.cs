using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioData;

    public void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioData.Play(0);
    }
}
