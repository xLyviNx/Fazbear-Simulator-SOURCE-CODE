using LyviN.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Env
{
    [ExecuteInEditMode]
    public class DayNightManager : MonoBehaviour
    {
        public Light SkyLight;
        [Range(0, 23)]
        public int Hour;
        public float LerpSpeed;
        public Vector3 TargetRot;
        public Vector3 currRot;
        [SerializeField] [NonReorderable] private TimeValues[] Values = new TimeValues[24];
        private void Update()
        {
            if (Values.Length >= Hour + 1)
            {
                if (SkyLight != null)
                {
                    //SkyLight.intensity = Mathf.Lerp(SkyLight.intensity, Values[Hour].LightIntensity, LerpSpeed * Time.deltaTime);
                    SkyLight.colorTemperature = Mathf.Lerp(SkyLight.colorTemperature, Values[Hour].temp, LerpSpeed * Time.unscaledDeltaTime * 2f);
                    //if (SkyLight.transform.eulerAngles.)

                    if (TimeSystem.Instance != null)
                    {
                        Hour = TimeSystem.Instance.FinalHour;
                    }
                    float x = (Hour / 24f * 360f) - 90f;
                    if (TimeSystem.Instance != null)
                    {
                        x+= 0.11f * TimeSystem.Instance.FinalMinutes;
                    }
                    TargetRot = new Vector3(x, TargetRot.y, TargetRot.z);

                    if (currRot.x < 360f && TargetRot.x < currRot.x)
                    {
                        x = x + 360f;
                    }
                    else if (currRot.x > 360f && TargetRot.x < 360f)
                    {
                        currRot = new Vector3(currRot.x - 360f, currRot.y, currRot.z);
                    }
                    TargetRot = new Vector3(x, TargetRot.y, TargetRot.z);
                    currRot = Vector3.Lerp(currRot, TargetRot, Time.unscaledDeltaTime * LerpSpeed);
                    SkyLight.transform.eulerAngles = currRot ;
                }
            }
            else
            {
                Debug.LogError("NO HOUR DATA SPECIFIED");
            }
        }
    }
    [Serializable]
    struct TimeValues
    {
        [Header("Light")]
        public int temp;
    }
}
