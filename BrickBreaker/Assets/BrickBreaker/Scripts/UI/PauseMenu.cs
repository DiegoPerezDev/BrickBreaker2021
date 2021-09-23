using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public AudioMixer musicMixer, SFX_Mixer;
    public Slider musicSlider, sfxSlider;
    private static float musicVol = 0f, sfxVol = 0f;


    void Start()
    {
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
        SetMusicVolume(musicVol);
        SetSFX_Volume(sfxVol);
    }


    public void SetMusicVolume(float volume) =>  musicMixer.SetFloat("volume", musicVol = volume);

    public void SetSFX_Volume(float volume) => SFX_Mixer.SetFloat("volume", sfxVol = volume);

}
