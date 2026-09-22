using LyviN.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlSettings : MonoBehaviour
{
    public static ControlSettings Instance;

    public GameObject mainPanel;
    public GameObject myPanel;

    public GameObject keybindPrefab;
    public Transform keybindParent;
    public List<KeybindEntry> spawnedBinds = new List<KeybindEntry>();
    public KeybindEntry ChangingKeybind;

    public Slider SensSlider;
    public TextMeshProUGUI SensT;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            enabled = false;
        }
        if (!PlayerPrefs.HasKey("Sens"))
        {
            PlayerPrefs.SetFloat("Sens", 1f);
        }

        bool a = true;
        Debug.Log(a ? "true" : "False");
        
    }
    public void Init(bool init)
    {
        mainPanel.SetActive(!init);
        myPanel.SetActive(init);
        if (init)
        {
            if (PlayerPrefs.HasKey("Sens"))
            {
                SensSlider.value = PlayerPrefs.GetFloat("Sens");
            }
            if (Binds.Instance != null)
            {
                foreach(KeybindEntry sb in spawnedBinds)
                {
                    if (sb != null && sb.gameObject != null)
                    {
                        Destroy(sb.gameObject);
                    }
                }
                spawnedBinds.Clear();
                foreach(FazBind b in Binds.Instance.General)
                {
                    if (!b.Hidden)
                    {
                        GameObject ob = Instantiate(keybindPrefab, keybindParent);
                        KeybindEntry ke = ob.GetComponent<KeybindEntry>();
                        spawnedBinds.Add(ke);
                        ke.Init(b.Name, b.Key, b.AltKey, 0);
                        ke.keyname = b.myKeyname();
                    }
                }
                foreach (FazBind b in Binds.Instance.Controls)
                {
                    if (!b.Hidden)
                    {
                        GameObject ob = Instantiate(keybindPrefab, keybindParent);
                        KeybindEntry ke = ob.GetComponent<KeybindEntry>();
                        spawnedBinds.Add(ke);
                        ke.Init(b.Name, b.Key, b.AltKey, 1);
                        ke.keyname = b.myKeyname();
                    }
                }
                foreach (FazBind b in Binds.Instance.Building)
                {
                    if (!b.Hidden)
                    {
                        GameObject ob = Instantiate(keybindPrefab, keybindParent);
                        KeybindEntry ke = ob.GetComponent<KeybindEntry>();
                        spawnedBinds.Add(ke);
                        ke.Init(b.Name, b.Key, b.AltKey, 2);
                        ke.keyname = b.myKeyname();
                        ke.keynameAlt = b.myAltKeyname();
                    }
                }
            }
        }
    }
    private void Update()
    {
        SensT.text = $"{MathF.Round(SensSlider.value, 2)}";
        if (ChangingKeybind != null)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(kcode) && ChangingKeybind != null)
                    {
                        ChangingKeybind.ChangeBind(kcode);
                    }
                }
            }
        }
    }
    public void ApplyControls()
    {
        PlayerPrefs.SetFloat("Sens", MathF.Round(SensSlider.value, 2));
        foreach (KeybindEntry b in spawnedBinds)
        {
            PlayerPrefs.SetString(b.keyname, b.awaitingKey.ToString());
            PlayerPrefs.SetString(b.keynameAlt, b.awaitingAltKey.ToString());
        }
        foreach (FazBind b in Binds.Instance.General)
        {
            b.Start();
        }
        foreach (FazBind b in Binds.Instance.Controls)
        {
            b.Start();
        }
        foreach (FazBind b in Binds.Instance.Building)
        {
            b.Start();
        }
        Binds.Instance.LoadSens();
    }
}
