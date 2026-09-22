using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJolt.API;
using TMPro;
using System;
using LyviN.UI;

namespace LyviN.Player.Build
{
    public class CatalogueGameJolt : MonoBehaviour
    {
        public static CatalogueGameJolt Instance;
        public TextMeshProUGUI myMoneyT;
        public int myMoney;
        public DateTime lastFetch;
        public DateTime lastFetchStructure;
        public string[] structureprices;
        public string[] propprices;
        public string[] lampprices;
        public string[] doorprices;
        public string[] animatronicprices;
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
            private void OnEnable()
        {
            FetchMoney();
            FetchPropPrices();
            FetchStructurePrices();
            FetchLampPrices();
        }
        public void FetchStructurePrices()
        {
            TimeSpan span = DateTime.Now.Subtract(lastFetchStructure);
            if (span.TotalSeconds > 7)
            {
                if (GameJoltAPI.myInstance != null)
                {

                    DataStore.Get("system_structurePrices", true, cb =>
                    {
                        if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                        {
                            structureprices = cb.Split('\n');
                            lastFetchStructure = DateTime.Now;
                        }
                        else
                        {
                            if (CatalogueMain.Instance != null && CatalogueMain.Instance.isON)
                            {
                                CatalogueMain.Instance.isON = false;
                                if (BuildingUI.Instance != null)
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            }
                        }
                    });

                }
            }
        } 
        public void FetchPropPrices()
        {
            TimeSpan span = DateTime.Now.Subtract(lastFetchStructure);
            if (span.TotalSeconds > 5)
            {
                if (GameJoltAPI.myInstance != null)
                {

                    DataStore.Get("system_propPrices", true, cb =>
                    {
                        if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                        {
                            propprices = cb.Split('\n');
                            DataStore.Get("system_doorPrices", true, cb =>
                            {
                                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                                {
                                    doorprices = cb.Split('\n');
                                }
                                else
                                {
                                    if (CatalogueMain.Instance != null && CatalogueMain.Instance.isON)
                                    {
                                        CatalogueMain.Instance.isON = false;
                                        if (BuildingUI.Instance != null)
                                        {
                                            BuildingUI.Instance.ErrorS.Play();
                                        }
                                    }
                                }
                                DataStore.Get("system_anmPrices", true, cb =>
                                {
                                    if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                                    {
                                        animatronicprices = cb.Split('\n');
                                    }
                                    else
                                    {
                                        if (CatalogueMain.Instance != null && CatalogueMain.Instance.isON)
                                        {
                                            CatalogueMain.Instance.isON = false;
                                            if (BuildingUI.Instance != null)
                                            {
                                                BuildingUI.Instance.ErrorS.Play();
                                            }
                                        }
                                    }
                                });
                            });
                            
                        }
                        else
                        {
                            if (CatalogueMain.Instance != null && CatalogueMain.Instance.isON)
                            {
                                CatalogueMain.Instance.isON = false;
                                if (BuildingUI.Instance != null)
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            }
                        }
                    });

                }
            }
        }      
        public void FetchLampPrices()
        {
            TimeSpan span = DateTime.Now.Subtract(lastFetchStructure);
            if (span.TotalSeconds > 6)
            {
                if (GameJoltAPI.myInstance != null)
                {

                    DataStore.Get("system_lightPrices", true, cb =>
                    {
                        if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                        {
                            lampprices = cb.Split('\n');
                        }
                        else
                        {
                            if (CatalogueMain.Instance != null && CatalogueMain.Instance.isON)
                            {
                                CatalogueMain.Instance.isON = false;
                                if (BuildingUI.Instance != null)
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            }
                        }
                    });

                }
            }
        }
        public void FetchMoney()
        {
            TimeSpan span = DateTime.Now.Subtract(lastFetch);
            if (span.TotalSeconds > 10)
            {
                if (!myMoneyT.text.ToLower().Contains("$"))
                {
                    myMoneyT.text = "Loading...";
                }
                if (GameJoltAPI.myInstance != null)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser)
                    {

                        DataStore.Get(Shared.myMoneyKey(), true, cb =>
                        {
                            if (cb != null)
                            {
                                if (!string.IsNullOrEmpty(cb.Trim()))
                                {
                                    myMoneyT.text = $"{cb.Trim()}$$";
                                }
                                else
                                {
                                    myMoneyT.text = $"Err <size=10%>empty";
                                }
                            }
                            else
                            {
                                myMoneyT.text = $"Err <size=10%>no data";
                            }
                            lastFetch = DateTime.Now;
                        });
                    }
                    else
                    {
                        myMoneyT.text = "DEMO";
                    }
                }
                else
                {
                    myMoneyT.text = "Err <size=10%>GJ";
                }
            }
        }
    }
}