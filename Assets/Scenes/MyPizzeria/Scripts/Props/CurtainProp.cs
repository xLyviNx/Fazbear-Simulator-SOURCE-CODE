using GameJolt.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props
{
    public class CurtainProp : MonoBehaviour
    {
        public bool isOpened;

        public GameObject openedO;
        public GameObject closedO;

        public AudioSource OpenS;
        public AudioSource CloseS;

        public Renderer ClosedRend;
        public Renderer OpenRend;
        private void Start()
        {
            Set();
            Invoke("Set", 1f);
        }
        public void Set()
        {
            openedO.SetActive(isOpened);
            closedO.SetActive(!isOpened);
            SetMat();
        }

        public void SetMat()
        {
            ClosedRend.material = OpenRend.material;
        }
        public void SwitchOpen()
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                OpenS.Play();
            }
            else
            {
                CloseS.Play();
            }
            
            openedO.SetActive(isOpened);
            closedO.SetActive(!isOpened);
            CancelInvoke("Save");
            Invoke("Save", 2f);
        }
        void Save()
        {
            CancelInvoke("Save");
            if (GameJoltAPI.myInstance!=null && GameJoltAPI.Instance.HasSignedInUser)
            {
                if (SaveSystem.Instance != null)
                {
                    SaveSystem.Instance.WaitFrameAndSave();
                }
            }
        }
    }
}
