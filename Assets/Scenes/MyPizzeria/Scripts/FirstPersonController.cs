using GameJolt.API;
using LyviN.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Control
{
    public class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController Instance;
        [Header("References")]
        public CharacterController CC;
        public FirstPersonCamera FPC;
        public GameObject PO;
        [Header("Values")]
        public float Mass = 1f;
        public float BaseMovementSpeed = 6f;
        public float RunMovementSpeed = 12f;
        public float CrouchMovementSpeed = 3f;
        public float jumpForce = 8f;
        public float CurrentMovementSpeed = 0f;
        public float TargetMovementSpeed = 3f;
        public Vector3 Direction;
        public Vector3 jumpVelocity;
        public float airSpeed = 10f;
        public float airFriction = 0.25f;
        public Vector2 XZ;
        public Vector2 TargetXZ;
        public float InputInterpolationSpeed = 13f;
        public float SpeedInterpolation = 7f;
        [Header("CC")]
        [SerializeField] private float CC_Radius = 0.54f;
        [SerializeField] private float CC_Height = 1.75f;
        [SerializeField] private float CC_StepOffset = 1f;
        [SerializeField] private Vector3 PlayerModelScale = new Vector3(0.8242f, 0.81886f, 0.8242f);
        [SerializeField] private float CamHeight = 0.45f;

        [Space(5)]
        [SerializeField] private float CC_Radius_crouch = 0.54f;
        [SerializeField] private float CC_Height_crouch = 1f;
        [SerializeField] private float CC_StepOffset_crouch = 0.5f;
        [SerializeField] private float CamHeight_crouch = 0.35f;
        [SerializeField] private Vector3 PlayerModelScale_crouch = new Vector3(0.8242f, 0.5f, 0.8242f);
        [Header("AntiClipCrouching")]
        public LayerMask CeilingCheckLayer;
        public bool isCrouch;
        [Header("Footsteps")]
        public GameObject StepSoundPrefab;
        public AudioClip[] InsideStepSounds;
        public AudioClip[] OutsideStepSounds;
        public LayerMask GroundTypeCheckLayer;
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
        private void Update()
        {
            
            
            if (InputInterpolationSpeed == 0)
            {
                XZ = TargetXZ;
            }
            else {
                XZ = Vector2.Lerp(XZ, TargetXZ, UnityEngine.Time.deltaTime * InputInterpolationSpeed);
                    }
            if (CursorManager.Instance != null && Binds.Instance != null)
            {
                float a = 0;
                float b = 0;
                if (CursorManager.Instance.isLocked())
                {
                    if (Binds.Instance.Controls[1].triggered() && !Binds.Instance.Controls[2].triggered())
                    {
                        b = 1;
                    }
                    else if (!Binds.Instance.Controls[1].triggered() && Binds.Instance.Controls[2].triggered())
                    {
                        b = -1;
                    }
                    if (Binds.Instance.Controls[3].triggered() && !Binds.Instance.Controls[4].triggered())
                    {
                        a = -1;
                    }
                    else if (!Binds.Instance.Controls[3].triggered() && Binds.Instance.Controls[4].triggered())
                    {
                        a = 1;
                    }
                }
                TargetMovementSpeed = BaseMovementSpeed;
                TargetXZ = new Vector2(a, b);

                bool isSprint = false;
                if (Binds.Instance != null &&
                    Binds.Instance.Controls[5].triggered() &&
                    CursorManager.Instance != null &&
                    CursorManager.Instance.isLocked())
                { isSprint = true; }
                if (isSprint) { TargetMovementSpeed = RunMovementSpeed; }
                bool lastCrouch = isCrouch;
                if (lastCrouch)
                {
                    if (Physics.Raycast(FPC.CameraAnim.transform.position, FPC.CameraAnim.transform.position + Vector3.up * 2, 0.5f, CeilingCheckLayer) || (lastCrouch && (Physics.Raycast(FPC.CameraAnim.transform.position + Vector3.left * 0.3f, FPC.CameraAnim.transform.position + Vector3.left * 0.3f + Vector3.up * 2, 0.5f, CeilingCheckLayer) || Physics.Raycast(FPC.CameraAnim.transform.position + Vector3.right * 0.45f, FPC.CameraAnim.transform.position + Vector3.right * 0.45f + Vector3.up * 2, 0.5f, CeilingCheckLayer) || Physics.Raycast(FPC.CameraAnim.transform.position + Vector3.forward * 0.45f, FPC.CameraAnim.transform.position + Vector3.forward * 0.45f + Vector3.up * 2, 0.5f, CeilingCheckLayer) || Physics.Raycast(FPC.CameraAnim.transform.position + Vector3.forward * -0.45f, FPC.CameraAnim.transform.position + Vector3.forward * -0.45f + Vector3.up * 2, 0.5f, CeilingCheckLayer))))
                    {

                        isCrouch = true;
                    }
                    else
                    {
                        isCrouch = Binds.Instance.Controls[7].triggered();
                    }
                }
                else
                {
                    isCrouch = Binds.Instance.Controls[7].triggered();

                }
                if (isCrouch)
                {
                    isSprint = false;
                    TargetMovementSpeed = CrouchMovementSpeed;

                    FPC.CameraParent.transform.localPosition = Vector3.Lerp(FPC.CameraParent.transform.localPosition, new Vector3(FPC.CameraParent.transform.localPosition.x, CamHeight_crouch, FPC.CameraParent.transform.localPosition.z), Time.deltaTime * SpeedInterpolation);
                    PO.transform.localScale = Vector3.Lerp(PO.transform.localScale, PlayerModelScale_crouch, Time.deltaTime * SpeedInterpolation);
                    CC.radius = Mathf.Lerp(CC.radius, CC_Radius_crouch, Time.deltaTime * SpeedInterpolation);
                    CC.height = Mathf.Lerp(CC.height, CC_Height_crouch, Time.deltaTime * SpeedInterpolation);
                    CC.stepOffset = Mathf.Lerp(CC.stepOffset,CC_StepOffset_crouch, Time.deltaTime * SpeedInterpolation);
                }
                else
                {
                    FPC.CameraParent.transform.localPosition = Vector3.Lerp(FPC.CameraParent.transform.localPosition, new Vector3(FPC.CameraParent.transform.localPosition.x, CamHeight, FPC.CameraParent.transform.localPosition.z), Time.deltaTime * SpeedInterpolation);
                    PO.transform.localScale = Vector3.Lerp(PO.transform.localScale, PlayerModelScale, Time.deltaTime * SpeedInterpolation);
                    CC.radius = Mathf.Lerp(CC.radius, CC_Radius, Time.deltaTime * SpeedInterpolation);
                    CC.height = Mathf.Lerp(CC.height, CC_Height, Time.deltaTime * SpeedInterpolation);
                    CC.stepOffset = Mathf.Lerp(CC.stepOffset, CC_StepOffset, Time.deltaTime * SpeedInterpolation);
                }
                FPC.CameraAnim.SetBool("isRunning", isSprint);

            }

            if (XZ.magnitude != 0)
            {

                Direction = new Vector3(XZ.x, 0, XZ.y);

                Direction = PO.transform.TransformDirection(Vector3.ClampMagnitude(Direction, 1f));
            }
            else
            {
                Direction = Vector3.zero;
            }
            FPC.CameraAnim.SetBool("isWalking", Direction != Vector3.zero && (CC.isGrounded || FPC.CameraAnim.GetFloat("FallTime") < 0.3f));
            if(SpeedInterpolation == 0f){
                CurrentMovementSpeed = TargetMovementSpeed;
            }
            else
            {
                CurrentMovementSpeed = Mathf.Lerp(CurrentMovementSpeed, TargetMovementSpeed, UnityEngine.Time.deltaTime * SpeedInterpolation);
            }
            if (CC.isGrounded)
            {
                FPC.CameraAnim.SetFloat("FallTime", 0f);

                CC.slopeLimit = 45.0f;
                Direction *= CurrentMovementSpeed;
                if (Binds.Instance.Controls[6].triggered() && CursorManager.Instance.isLocked())
                {
                    //FPC.CameraAnim.CrossFade("Jump", 0.01f);
                    jumpVelocity = Direction * airFriction;
                    jumpVelocity.y = jumpForce;
                    CC.slopeLimit = 90.0f;
                }
                else
                {
                    jumpVelocity = Vector3.zero;
                }
            }
            else
            {
                FPC.CameraAnim.SetFloat("FallTime", FPC.CameraAnim.GetFloat("FallTime") + UnityEngine.Time.deltaTime * 3f);
                Direction *= airSpeed;
            }
            FPC.CameraAnim.SetBool("isGrounded", CC.isGrounded);

            
                jumpVelocity.y -= 60f * (0.4f * Mass) * Time.deltaTime;
            

            CC.Move((Direction + jumpVelocity) * Time.deltaTime);
            if (transform.position.y < -10f)
            {
                CC.enabled = false;
                transform.position = new Vector3(0, 4f, 0);
                CC.enabled = true;
                //chuj ze i bez tego false true by teraz zadzialalo HOPEFULLY, ale i tak wyjebe, teraz jak dalem na koniec
                //to moze zadziala
            }
        }
        public void StepSound()
        {
            if (CC.isGrounded && CursorManager.Instance != null && CursorManager.Instance.isLocked())
            {
                GameObject sound = Instantiate(StepSoundPrefab, transform);
                sound.transform.position = transform.position;
                AudioSource aS = sound.GetComponent<AudioSource>();
                bool isGrass = false;
                AudioClip clip = null;
                Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, 2f, GroundTypeCheckLayer))
                {
                    if (hit.transform.tag == "Grass")
                    {
                        isGrass = true;
                    }
                    if (isGrass)
                    {
                        int i = Random.Range(0, OutsideStepSounds.Length);
                        clip = OutsideStepSounds[i];
                    }
                    else
                    {
                        int i = Random.Range(0, InsideStepSounds.Length);
                        clip = InsideStepSounds[i];
                    }
                    aS.clip = clip;
                    if (clip != null)
                    {
                        aS.Play();
                    }
                }
            }
        }
    }
}