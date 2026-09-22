using LyviN.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LyviN.UI
{
    public class ESCMain : MonoBehaviour
    {
        public static ESCMain Instance;
        public bool isEsc;
        public GameObject ContentO;
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
            if (Binds.Instance != null)
            {
                if (Binds.Press(Binds.Instance.General[0]))
                {
                    SwitchESC();
                }
            }
            ContentO.SetActive(isEsc);
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isESC = isEsc;
            }
        }
        void SwitchESC()
        {
            if (BuildingUI.Instance != null)
            {
                BuildingUI.Instance.SwitchPosS.Play();
            }
            isEsc = !isEsc;
        }
    }
}