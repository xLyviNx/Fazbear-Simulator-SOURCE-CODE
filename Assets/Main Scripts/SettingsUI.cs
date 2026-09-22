using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LyviN;
using UnityEngine.SceneManagement;


namespace LyviN.UI
{
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance;
        public bool isSettings;
        public GameObject ContentO;
        public GameObject Content1;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            ContentO.SetActive(isSettings);
            if (Input.CursorManager.Instance != null)
            {
                Input.CursorManager.Instance.isSettings = isSettings;
            }
        }

        public void OpenSettings()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                isSettings = true;
                Content1.SetActive(!isSettings);
                Debug.Log("Opened Settings in MainMenu");
            }
            else
            {
                LyviN.UI.ESCMain.Instance.isEsc = false;
                isSettings = true;
                Debug.Log("Opened Settings");
            }
        }

        public void CloseSettings()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                isSettings = false;
                Content1.SetActive(!isSettings);
                Debug.Log("Closed Settings in MainMenu");
            }
            else
            {
                isSettings = false;
                Debug.Log("Closed Settings");
            }
        }
    }
}