using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundCOntroller : MonoBehaviour
{
    public AudioSource audioSourceOnce;
    public AudioSource audioSourceLoop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Audio volume
        if (audioSourceOnce != null)
        {
            audioSourceOnce.volume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
        }
        if (audioSourceLoop != null)
        {
            audioSourceLoop.volume = PlayerPrefs.GetFloat("SoundEffectVolume", 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
