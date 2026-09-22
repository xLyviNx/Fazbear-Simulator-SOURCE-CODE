using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Input
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance;
        public bool isGameScene;
        public bool isESC;
        public bool isCat;
        public bool isCookNotGrabbingTool;
        public bool isOrders;
        public bool isStructureInteraction;
        public bool isPizzeriaManagement;
        public bool isSettings;
        public bool isOven;
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public CursorLockMode Mode()
        {
            if (!isGameScene)
            {
                return CursorLockMode.None;
            }
            if (isESC)
            {
                return CursorLockMode.None;
            }
            if (isCat)
            {
                return CursorLockMode.None;
            }
            if (isCookNotGrabbingTool)
            {
                return CursorLockMode.None;
            }
            if (isOrders)
            {
                return CursorLockMode.None;
            }          
            if (isStructureInteraction)
            {
                return CursorLockMode.None;
            }
            if (isPizzeriaManagement)
            {
                return CursorLockMode.None;
            }
            if (isSettings)
            {
                return CursorLockMode.None;
            }
            if (isOven)
            {
                return CursorLockMode.None;
            }
            return CursorLockMode.Locked;
        }
        private void Update()
        {
            if (Instance == this)
            {
                Cursor.lockState = Mode();
            }
        }
        public bool isLocked()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}