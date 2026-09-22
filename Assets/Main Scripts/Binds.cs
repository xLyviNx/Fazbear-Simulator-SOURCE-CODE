using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Input
{
    public class Binds : MonoBehaviour
    {
        public static Binds Instance;
        [Header("Config thingies")]
        public float ScrollSensitivity = 1f;
        public float MouseSensitivity = 1f;
        [Header("Keybinds")]
        [NonReorderable] public List<FazBind> General;
        public List<FazBind> Controls;
        [NonReorderable] public List<FazBind> Building;
        void Start()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            if (Instance == this)
            {
                foreach (FazBind B in Controls)
                {
                    B.Start();
                }
                foreach (FazBind B in Building)
                {
                    B.Start();
                }
                foreach (FazBind B in General)
                {
                    B.Start();
                }
            }
            LoadSens();
        }
        public void LoadSens()
        {
            if (!PlayerPrefs.HasKey("Sens"))
            {
                PlayerPrefs.SetFloat("Sens", 1f);
            }
            MouseSensitivity = PlayerPrefs.GetFloat("Sens");
        }
        // Update is called once per frame
        void Update()
        {
            
        }
        public static bool Press(FazBind b)
        {
            if (UnityEngine.Input.GetKeyDown(b.Key) || UnityEngine.Input.GetKeyDown(b.AltKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    [Serializable]
    public class FazBind
    {
        public string Name;
        public KeyCode Key;
        public KeyCode AltKey;
        public KeyCode DefaultKey;
        public KeyCode DefaultAltKey;
        public string overrideNameInSaves;

        private bool canPress;
        public bool Hidden;
        public void Start()
        {
            LoadBinds();
        }
        public bool triggered()
        {
            return UnityEngine.Input.GetKey(Key) || UnityEngine.Input.GetKey(AltKey);
        }
        public string myKeyname()
        {
            string keyname = "";
            if (string.IsNullOrEmpty(overrideNameInSaves))
            {
                keyname = $"binds_{Name}";
            }
            else
            {
                keyname = $"binds_{overrideNameInSaves}";
            }
            return keyname;
        }
        public string myAltKeyname()
        {
            string keyname = "";
            if (string.IsNullOrEmpty(overrideNameInSaves))
            {
                keyname = $"binds_{Name}_alt";
            }
            else
            {
                keyname = $"binds_{overrideNameInSaves}_alt";
            }
            return keyname;
        }
        public void LoadBinds()
        {
            if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(overrideNameInSaves))
            {

                string keyname = myKeyname();
                bool foundBinds = true;
                if (PlayerPrefs.HasKey(keyname))
                {
                    string loadedBind = PlayerPrefs.GetString(keyname);
                    Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), loadedBind);
                }
                else
                {
                    foundBinds = false;
                    Key = DefaultKey;
                }
                keyname = keyname + "_alt";
                if (PlayerPrefs.HasKey(keyname))
                {
                    string loadedBind = PlayerPrefs.GetString(keyname);
                    AltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), loadedBind);
                }
                else
                {
                    foundBinds = false;
                    AltKey = DefaultAltKey;
                }
                if (!foundBinds)
                {
                    SaveBinds();
                }
            }
            else
            {
                Debug.LogError($"ERROR! There is a wrong Keybind in the Keybind Manager!! It wasn't loaded.");
            }
        }
        public void SaveBinds(int which = 0)
        {
            string keyname = myKeyname();
            if (which == 1)
            {
                PlayerPrefs.SetString($"{keyname}", Key.ToString());
            }
            else if (which == 2)
            {
                PlayerPrefs.SetString($"{keyname}_alt", AltKey.ToString());
            }
            else
            {
                PlayerPrefs.SetString($"{keyname}", Key.ToString());
                PlayerPrefs.SetString($"{keyname}_alt", AltKey.ToString());
            }
        }
    }
}