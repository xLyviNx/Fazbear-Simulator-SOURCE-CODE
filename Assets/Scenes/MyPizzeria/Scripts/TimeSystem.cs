using GameJolt.API;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player
{
    public class TimeSystem : MonoBehaviour
    {
        public static TimeSystem Instance;
        public int Hour;
        public int Minute;
        public int RealHour;
        public int RealMinutes;
        public int RealSeconds;

        public int FinalHour;
        public int FinalMinutes;

        public bool skipnight;
        private void Start()
        {
            if (Instance == null)
            {
                StartCoroutine(LoadTime());

                Instance = this;
            }
            else
            {
                enabled = false;
            }
        }
        public bool isNight()
        {
            return FinalHour < 6 && FinalHour >= 0;
        }
        IEnumerator LoadTime()
        {
            while (GameJoltAPI.myInstance == null)
            {
                yield return new WaitForEndOfFrame();
            }
            //while (!GameJoltAPI.Instance.HasSignedInUser || GameJoltAPI.Instance.CurrentUser.ID == 0)
            //{
            //yield return new WaitForEndOfFrame();
            //}
            bool loading = false;
            while (true)
            {
                if (!loading)
                {
                    while (!gameObject.activeInHierarchy || !enabled)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    loading = true;
                    GameJolt.API.Misc.GetTime(time =>
                    {
                        loading = false;
                        if (time.Year >= 2022)
                        {
                            RealHour = time.Hour;
                            RealMinutes = time.Minute;
                            RealSeconds = time.Second;

                            Hour = (RealHour * 12) + 5;
                            int mHour = RealMinutes / 5;
                            Hour += mHour;
                            int hd = Hour / 24;
                            Hour = Hour - (hd * 24);

                            Minute = (int)(((RealMinutes - (mHour * 5)) / 5f) * 60f);
                            Minute = Minute + RealSeconds / 5;
                            FinalHour = Hour;
                            FinalMinutes = Minute;
                            if (skipnight)
                            {
                                if (FinalHour >= 0)
                                {
                                    if (FinalHour < 6)
                                    {
                                        FinalHour = FinalHour + 6;
                                    }
                                    else
                                    {
                                        skipnight = false;
                                    }
                                }
                                else
                                {
                                    skipnight = false;
                                }
                                
                            }


                        }
                    });
                }
                yield return new WaitForSecondsRealtime(4f);
            }
        }
    }
}
