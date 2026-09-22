using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeybindEntry : MonoBehaviour
{
    public TextMeshProUGUI ButtonText;
    public TextMeshProUGUI Button2Text;
    public TextMeshProUGUI NameText;
    public string keyname;
    public string keynameAlt;
    public int cat;
    public KeyCode awaitingKey;
    public KeyCode awaitingAltKey;
    public int KeyIndex;
    public void Init(string buttonName, KeyCode key1, KeyCode key2, int Category)
    {
        NameText.text = $"{buttonName}";
        awaitingKey = key1;
        awaitingAltKey = key2;
        cat = Category;
    }
    private void Update()
    {
        if (ControlSettings.Instance != null && ControlSettings.Instance.ChangingKeybind == this)
        {
            if (KeyIndex == 0)
            {
                ButtonText.text = "LEFT CLICK TO CANCEL";
            }
            else if (KeyIndex == 1)
            {
                Button2Text.text = "LEFT CLICK TO CANCEL";
            }
        }
        else
        {
            ButtonText.text = $"{awaitingKey.ToString()}";
            Button2Text.text = $"{awaitingAltKey.ToString()}";

        }
    }
    public void Click(bool isAlt)
    {
        if (ControlSettings.Instance != null && ControlSettings.Instance.ChangingKeybind == null)
        {
            ControlSettings.Instance.ChangingKeybind = this;
            KeyIndex = isAlt ? 1 : 0;
            Debug.Log($"{isAlt} : {KeyIndex}");
        }
    }
    public void ChangeBind(KeyCode k)
    {
        if (k != KeyCode.Mouse0)
        {
            if (k == KeyCode.Backspace)
            {
                k = KeyCode.None;
            }
            if (KeyIndex == 0)
            {
                awaitingKey = k;

            }
            else if (KeyIndex == 1)
            {
                awaitingAltKey = k;
            }
            if (ControlSettings.Instance != null && ControlSettings.Instance.ChangingKeybind == this)
            {
                ControlSettings.Instance.ChangingKeybind = null;
            }
        }
        else
        {
            if (ControlSettings.Instance != null && ControlSettings.Instance.ChangingKeybind == this)
            {
                ControlSettings.Instance.ChangingKeybind = null;
            }
        }
    }
}
