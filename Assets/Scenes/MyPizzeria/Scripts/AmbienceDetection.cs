using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LyviN.Player.Env
{
    public class AmbienceDetection : MonoBehaviour
    {
        public LayerMask DetectionMask;
        public bool Outside;


        public AudioInfo OutsideAmbient;
        public AudioInfo OutsideAmbientNight;
        public AudioReverbZone InsideReverb;
        // Start is called before the first frame update
        void Start()
        {
            EnvDetection();
        }

        // Update is called once per frame
        void Update()
        {
            OutsideAmbient.Mute = true;
            OutsideAmbientNight.Mute = true;
            if (Outside)
            {
                if (TimeSystem.Instance != null)
                {
                    if (TimeSystem.Instance.FinalHour > 6 && TimeSystem.Instance.FinalHour < 22)
                    {
                        OutsideAmbient.Mute = false;
                    }
                    else
                    {
                        OutsideAmbientNight.Mute = false;
                    }
                }
            }
            InsideReverb.gameObject.SetActive(!Outside);
        }
        public void EnvDetection()
        {
            CancelInvoke("EnvDetection");
            Outside = true;
            Ray rightray = new Ray(transform.position + Vector3.up * 0.3f, transform.right * 20f);
            Ray forray = new Ray(transform.position + Vector3.up * 0.3f, transform.forward * 20f);
            Ray backray = new Ray(transform.position + Vector3.up * 0.3f, transform.forward * -20f);
            Ray leftray = new Ray(transform.position + Vector3.up * 0.3f, transform.right * -20f);
            Ray upray = new Ray(transform.position + Vector3.up * 0.3f, Vector3.up *20f);

            if (Physics.Raycast(rightray, 100f, DetectionMask))
            {
                if (Physics.Raycast(forray, 100f, DetectionMask))
                {
                    if (Physics.Raycast(backray, 100f, DetectionMask))
                    {
                        if (Physics.Raycast(leftray, 100f, DetectionMask))
                        {
                            if (Physics.Raycast(upray, 100f, DetectionMask))
                            {
                                Outside = false;
                            }
                        }
                    }
                }
            }
            Invoke("EnvDetection", 1.5f);
        }
    }
}