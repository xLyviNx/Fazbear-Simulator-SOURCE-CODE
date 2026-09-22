using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public Slider Unassigned_S;
    public Slider SFX_S;
    public Slider MUS_S;
    public Slider ADMUS_S;

    public TextMeshProUGUI Unassigned_T;
    public TextMeshProUGUI SFX_T;
    public TextMeshProUGUI MUS_T;
    public TextMeshProUGUI ADMUS_T;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey($"volume_0"))
        {
            PlayerPrefs.SetFloat("volume_0", 1f);
        }
        if (!PlayerPrefs.HasKey($"volume_1"))
        {
            PlayerPrefs.SetFloat("volume_1", 1f);
        }
        if (!PlayerPrefs.HasKey($"volume_2"))
        {
            PlayerPrefs.SetFloat("volume_2", 1f);
        }
        if (!PlayerPrefs.HasKey($"volume_3"))
        {
            PlayerPrefs.SetFloat("volume_3", 1f);
        }
        if (!PlayerPrefs.HasKey($"volume_4"))
        {
            PlayerPrefs.SetFloat("volume_4", 1f);
        }
        LoadSliders();
    }
    void LoadSliders()
    {
        Unassigned_S.value = MathF.Round(PlayerPrefs.GetFloat("volume_0"),2);
        SFX_S.value = MathF.Round(PlayerPrefs.GetFloat("volume_1"),2);
        MUS_S.value = MathF.Round(PlayerPrefs.GetFloat("volume_2"),2);
        ADMUS_S.value = MathF.Round(PlayerPrefs.GetFloat("volume_4"),2);
    }
    public void ApplySound()
    {
        PlayerPrefs.SetFloat("volume_0", MathF.Round(Unassigned_S.value, 2));
        PlayerPrefs.SetFloat("volume_1", MathF.Round(SFX_S.value, 2));
        PlayerPrefs.SetFloat("volume_2", MathF.Round(MUS_S.value, 2));
        PlayerPrefs.SetFloat("volume_4", MathF.Round(ADMUS_S.value, 2));

        AudioInfo[] ai = GameObject.FindObjectsOfType<AudioInfo>();
        foreach(AudioInfo AI in ai)
        {
            AI.SetVolume();
        }
    }
    // Update is called once per frame
    void Update()
    {
        Unassigned_T.text = $"{MathF.Round(Unassigned_S.value,2)}";
        SFX_T.text = $"{MathF.Round(SFX_S.value,2)}";
        MUS_T.text = $"{MathF.Round(MUS_S.value,2)}";
        ADMUS_T.text = $"{MathF.Round(ADMUS_S.value,2)}";
    }
}
