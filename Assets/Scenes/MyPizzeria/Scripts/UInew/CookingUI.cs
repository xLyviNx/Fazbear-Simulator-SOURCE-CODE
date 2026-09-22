using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.UI
{
    public class CookingUI : MonoBehaviour
    {
        public static CookingUI Instance;
        public TextMeshProUGUI crosshairText;
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
        }
    }
}