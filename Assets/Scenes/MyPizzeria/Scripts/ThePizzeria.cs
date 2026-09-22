using GameJolt.API;
using LyviN.Player.Baking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LyviN.Player
{
    public class ThePizzeria : MonoBehaviour
    {
        public static ThePizzeria Instance;
        public int OpenHour;
        public int ClosingHour;
        [SerializeField] public Rating Rating = new Rating();
        bool refreshedRating;
        private protected bool gotarcadem;
        private protected bool wasOpened;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadRating();
            }
            else
            {
                enabled = false;
            }
        }
        void LoadRating()
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
            {
                float initpizzarating = 4;
                float initservicerating = 4;
                float servicerating = 4;
                float pizzarating = 4;
                DataStore.Get(Shared.myRating(), true, rat =>
                {
                    if (!GameJoltAPI.Instance.timedOut)
                    {
                        if (rat != null && rat.Trim() != "")
                        {
                            string[] ar = rat.Split('|');
                            if (ar.Length > 0)
                            {
                                if (ar[0].Trim() != "")
                                {
                                    initpizzarating = float.Parse(ar[0].Trim());
                                    pizzarating = float.Parse(ar[0].Trim());
                                }
                            }
                            if (ar.Length > 1)
                            {
                                if (ar[1].Trim() != "")
                                {
                                    initservicerating = float.Parse(ar[1].Trim());
                                    servicerating = float.Parse(ar[1].Trim());
                                }
                            }
                        }
                        DataStore.Get(Shared.myRemRatings(), true, a =>
                        {
                            if (!GameJoltAPI.Instance.timedOut)
                            {
                                float todiv = 0f;
                                float todivs = 0f;
                                int div = 0;
                                int divs = 0;
                                if (a != null && a.Trim() != "")
                                {

                                    string[] b = a.Trim().Split('\n');
                                    foreach (string c in b)
                                    {
                                        string[] arg = c.Split('|');
                                        if (arg.Length > 1 && arg[0].Trim() != "" && arg[1].Trim() != "")
                                        {
                                            float addp = float.Parse(arg[0].Trim());
                                            float adds = float.Parse(arg[1].Trim());
                                            if (addp > 0)
                                            {
                                                todiv = todiv + addp;
                                                div++;
                                            }
                                            if (adds > 0)
                                            {
                                                todivs = todivs + adds;
                                                divs++;
                                            }
                                        }
                                    }
                                    if ((todiv > 0 && div > 0) || (todivs > 0 && divs > 0))
                                    {
                                        todiv = todiv / div;
                                        todivs = todivs / divs;
                                        if (todiv > 0)
                                        {
                                            pizzarating = Mathf.Clamp(MathF.Round((pizzarating + todiv) / 2f, 2), 0.5f, 10f);
                                        }
                                        if (todivs > 0)
                                        {
                                            servicerating = Mathf.Clamp(MathF.Round((servicerating + todivs) / 2f, 2), 0.5f, 10f);
                                        }
                                        //Rating = new Rating()
                                        
                                        GameJolt.API.Objects.Score nS = new GameJolt.API.Objects.Score((int)(Rating.GeneralRating() * 100f), $"{Rating.GeneralRating()}", "", GameJoltAPI.Instance.CurrentUser.AvatarURL);
                                        Scores.Add(nS, 748613);
                                        if (pizzarating != initpizzarating || servicerating != initservicerating)
                                        {
                                            DataStore.Set(Shared.myRating(), $"{pizzarating}|{servicerating}", true, set =>
                                            {

                                                if (set)
                                                {
                                                    DataStore.Set(Shared.myRemRatings(), "", true, set2 => { });
                                                }
                                            });
                                        }
                                    }
                                    
                                }
                            }
                            Rating.PizzaRating = pizzarating;
                            Rating.ServiceRating = servicerating;
                        });

                    }
                    
                });
            }
        }
        public bool isOpened()
        {
            bool op = false;
            if (TimeSystem.Instance != null)
            {
                if (ClosingHour > OpenHour && TimeSystem.Instance.FinalHour >= OpenHour && TimeSystem.Instance.FinalHour < ClosingHour)
                {
                    op = true;
                }
            }
            if (op == true)
            {
                wasOpened = true;
            }
            return op;
        }
        private void Update()
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.timedOut)
            {
                MainMenu.timedout = true;
                SceneManager.LoadScene("MainMenu");
                LoadingScene s = GameObject.FindObjectOfType<LoadingScene>();
                if (s!=null)
                {
                    Destroy(s.gameObject);
                }
            }
            if (!isOpened() && !refreshedRating && OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <= 0 && TimeSystem.Instance != null && TimeSystem.Instance.isNight())
            {
                refreshedRating = true;
                LoadRating();
                ArcadeMoney();
            }
            else if (isOpened())
            {
                refreshedRating = false;
                if (!TimeSystem.Instance.isNight())
                {
                    gotarcadem = false;
                }
            }
            
        }
        void ArcadeMoney()
        {
            Debug.Log(1);
            if (!isOpened() && !gotarcadem && wasOpened)
            {
                Debug.Log(2);
                wasOpened = false;
                gotarcadem = true;
                ArcadeMachine[] ams = GameObject.FindObjectsOfType<ArcadeMachine>();
                int mon = 30 * ams.Length;
                if (mon > 0)
                {
                    Debug.Log(3);
                    Shared.AddMoney(mon, cb => { 
                    
                        
                    
                    }, "Arcades Income");
                }
            }
        }
    }
    [Serializable]
    public class Rating
    {
        [Range (0.5f,10)]
        public float PizzaRating = 4f;
        [Range(0.5f, 10)]
        public float ServiceRating = 4f;
        public float GeneralRating()
        {
            float lowest = 10f;
            if (PizzaRating < lowest)
            {
                lowest = PizzaRating;
            }
            if (ServiceRating < lowest)
            {
                lowest = ServiceRating;
            }

            return Mathf.Clamp (MathF.Round(Mathf.Clamp((PizzaRating + ServiceRating) / 2f, 0.5f, lowest * 1.25f),2), 0.5f, 10f);
        }


    }
    [Serializable]
    public struct VisitRating
    {
        public float ServiceRating;
        public float PizzaRating;
    }
}