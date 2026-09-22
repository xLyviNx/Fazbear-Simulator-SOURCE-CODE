using LyviN.AI.Customers;
using LyviN.Input;
using LyviN.Player.Baking;
using LyviN.Player.Build;
using LyviN.Player.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Control
{
    [RequireComponent(typeof(FirstPersonController))]
    public class FirstPersonCamera : MonoBehaviour
    {
        public static FirstPersonCamera Instance;
        [Header("References")]
        public GameObject CameraParent;
        public Camera MainCamera;
        public Camera DialogCamera;
        public Animator CameraAnim;
        public FirstPersonController FPCon;
        public Camera BuildOverlayCamera;
        public Camera FirstPersonOverlayCamera;
        public Animator ViewmodelAnimator;
        [Header("Values")]
        public float Sens = 5f;
        Vector2 pitchMinMax = new Vector2(-90, 90);
        float Yaw;
        float Pitch;
        public Vector3 Rot;
        CustomerMain lastCustomer;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                enabled = false;
            }
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isGameScene = true;
            }
        }
        
        private void Update()
        {
            if (Binds.Instance != null)
            {
                Sens = Binds.Instance.MouseSensitivity;
            }
            if (CursorManager.Instance != null && CursorManager.Instance.isLocked() && Binds.Instance != null && MainCamera.enabled)
            {
                if (BuildManager.Instance != null)
                {
                    if (BuildManager.Instance.Building == BuildingType.None && (BaseBake.Instance == null || (BaseBake.Instance != null && BaseBake.Instance.bakeMode != "tophold" && BaseBake.Instance.bakeMode != "cuttophold" && BaseBake.Instance.bakeMode != "pizza") ))
                    {
                        FirstPersonOverlayCamera.enabled = true;
                    }
                    else
                    {
                        FirstPersonOverlayCamera.enabled = false;
                    }
                }
                if (ViewmodelAnimator != null)
                {
                    Vector3 XZvel = new Vector3(FPCon.CC.velocity.x, 0f, FPCon.CC.velocity.z);
                    if (FPCon.CC.isGrounded)
                    {
                        ViewmodelAnimator.SetFloat("Velocity", FPCon.CC.velocity.magnitude);
                    }
                    else
                    {
                        ViewmodelAnimator.SetFloat("Velocity", 0f);

                    }
                }
                float MouseX = UnityEngine.Input.GetAxis("Mouse X");
                float MouseY = UnityEngine.Input.GetAxis("Mouse Y");
                Yaw += MouseX * Sens;
                Pitch -= MouseY * Sens;
                Pitch = Mathf.Clamp(Pitch, pitchMinMax.x, pitchMinMax.y);
                Rot = new Vector3(Pitch, Yaw);
                CameraParent.transform.eulerAngles = Rot;
                FPCon.PO.transform.eulerAngles = new Vector3(0f, Rot.y, 0f);
                if (CustomerInteraction.Instance != null)
                {
                    if (CustomerInteraction.Instance.hit.transform != null)
                    {
                        RaycastHit hit = CustomerInteraction.Instance.hit;
                        if (hit.transform.tag == "Customer")
                        {
                            DialogCamera.enabled = true;
                        }
                        else if (DialogCamera != null)
                        {
                            DialogCamera.enabled = false;
                        }
                    }
                    else if(DialogCamera != null)
                    {
                        DialogCamera.enabled = false;
                    }
                }
                else if (DialogCamera != null)
                {
                    DialogCamera.enabled = false;
                }
            }
            else
            {
                if (ViewmodelAnimator != null)
                {
                    ViewmodelAnimator.SetFloat("Velocity", 0f);
                }
            }
        }
    }
}