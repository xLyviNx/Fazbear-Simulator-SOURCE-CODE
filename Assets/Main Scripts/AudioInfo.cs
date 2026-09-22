using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInfo : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioType Type;
    public float InitVolume;
    public float SettingsVolume;
    public float TargetVolume;
    public float LerpSpeed = 2f;
    public bool isPaused;
    public bool Mute;
    public bool PassStartMute;
    public bool isMusic;
    private void Start()
    {
        InitVolume = myAudio.volume;
        SettingsVolume = InitVolume;
        if (!PassStartMute)
        {
            Mute = true;
            myAudio.volume = 0f;
        }
        SetVolume();
    }
    public void SetVolume()
    {
        if (PlayerPrefs.HasKey($"volume_{(int)Type}"))
        {
            SettingsVolume = InitVolume * PlayerPrefs.GetFloat($"volume_{(int)Type}");
        }
    }
    private void Update()
    {
        if (Mute)
        {
            TargetVolume = 0f;
        }
        else
        {
            TargetVolume = SettingsVolume;
        }
        if (myAudio.volume != TargetVolume)
        {
            if (Mathf.Abs(TargetVolume - myAudio.volume) < 0.002f)
            {
                myAudio.volume = TargetVolume;
            }
            else
            {
                if (LerpSpeed > 0)
                {
                    myAudio.volume = Mathf.Lerp(myAudio.volume, TargetVolume, Time.unscaledDeltaTime * LerpSpeed);
                }
                else
                {
                    myAudio.volume = TargetVolume;
                }
            }
        }
        if (myAudio.volume <= 0.0005f && !isPaused && isMusic)
        {
            isPaused = true;
            myAudio.Pause();
        }
        else if (myAudio.volume > 0.001f && isPaused && isMusic)
        {
            isPaused = false;
            myAudio.UnPause();
            if (!myAudio.isPlaying)
            {
                myAudio.Play();
            }
        }
    }
}
public enum AudioType
{
    Unknown,
    SFX,
    Music,
    Dialogs,
    BackgroundMusic,
}