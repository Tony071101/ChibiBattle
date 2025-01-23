using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource hoverSound;
    public AudioSource sliderSound;
    public AudioSource swooshSound;

    private void Start() {
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void UpdateVolume (){
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void PlayHover(){
        hoverSound.Play();
    }

    public void PlaySFXHover(){
        sliderSound.Play();
    }

    public void PlaySwoosh(){
        swooshSound.Play();
    }
}
