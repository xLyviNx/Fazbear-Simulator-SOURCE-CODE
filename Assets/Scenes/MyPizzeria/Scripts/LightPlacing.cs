using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class LightPlacing : MonoBehaviour
    {
        public static LightPlacing Instance;
        public GameObject[] LightPrefabs;
        public GameObject[] lightPreviews;
        public LightPreview currPreviewO;
        public List<Color> SelColors;
        bool isWallLight;
        public static uint nextID;
        public float currIntensity;
        public float currRange;
        public bool Shadows;
        public bool GroundLight;
        public void SetPreview()
        {
            if (BuildManager.Instance != null)
            {
                foreach (GameObject pp in lightPreviews)
                {
                    if (pp.name.ToLower() != $"{BuildManager.Instance.currentlyBuilding}_preview")
                    {
                        pp.SetActive(false);
                    }
                    else
                    {
                        pp.SetActive(true);
                        currPreviewO = pp.GetComponent<LightPreview>();
                    }
                }
                if (currPreviewO != null)
                {
                    currPreviewO.gameObject.SetActive(true);
                }
            }
        }
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
            if (BuildManager.Instance != null && Binds.Instance != null && Crosshair3D.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Lights)
                {
                    if (Crosshair3D.Instance.hits && currPreviewO != null)
                    {

                        currPreviewO.transform.position = Crosshair3D.Instance.pos;
                        currPreviewO.gameObject.SetActive(true);
                        float scroll = UnityEngine.Input.mouseScrollDelta.y;
                        if (scroll != 0f)
                        {
                            if (CatalogueMain.Instance != null)
                            {
                                currPreviewO.transform.Rotate(Vector3.up * CatalogueMain.Instance.HeightScrollSens * 5f * Mathf.Round(scroll));
                                currPreviewO.transform.eulerAngles = new Vector3(currPreviewO.transform.eulerAngles.x, Mathf.Round(currPreviewO.transform.eulerAngles.y), currPreviewO.transform.eulerAngles.z);
                            }
                        }
                        Debug.DrawLine(currPreviewO.transform.position + Vector3.up * 0.5f, currPreviewO.transform.position + Vector3.up * 0.5f + currPreviewO.transform.forward * 1f);
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.PropRotationT.text = $"Lamp Rotation: {Mathf.Round(currPreviewO.transform.eulerAngles.y)}*";
                        }
                        GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                        if (a.tag == "Floor" || a.tag == "Wall")
                        {
                            if (!currPreviewO.isColliding)
                            {
                                currPreviewO.isFloor = true;
                                if (!currPreviewO.isColliding)
                                {

                                    BuildingUI.Instance.priceShowT.text = "";
                                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                    {
                                        PlaceLamp(Crosshair3D.Instance.pos, currPreviewO.transform.rotation);
                                    }

                                }
                                else
                                {
                                    BuildingUI.Instance.priceShowT.text = "A lamp is too close to other object.";
                                }
                            }
                            else
                            {
                                BuildingUI.Instance.priceShowT.text = "A lamp has to be touching a wall, or a floor.";
                            }
                        }
                    }
                }

            }
        }
        void PlaceLamp(Vector3 pos, Quaternion rot)
        {
            if (BuildManager.Instance != null && Crosshair3D.Instance != null && BuildingUI.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Lights)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance.hits)
                        {
                            GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                            if (a.tag == "Floor" || a.tag == "Wall")
                            {
                                if (!currPreviewO.isColliding)
                                {
                                    if (!BuildManager.Instance.isBuilding)
                                    {
                                        if (GameJoltAPI.myInstance != null)
                                        {

                                            BuildManager.Instance.isBuilding = true;
                                            SelColors = new List<Color>();
                                            if (CatalogueMain.Instance.customizeColors.Count > 0)
                                            {
                                                foreach (CustomizeColorEntry c in CatalogueMain.Instance.customizeColors)
                                                {
                                                    Color nc = new Color(c.R, c.G, c.B);
                                                    SelColors.Add(nc);
                                                }
                                            }
                                            if (GameJoltAPI.Instance.HasSignedInUser)
                                            {
                                                TimeSpan span = DateTime.Now.Subtract(Shared.lastTransaction);
                                                if (span.TotalSeconds > 3)
                                                {
                                                    Shared.CheckLampPrice(BuildManager.Instance.currentlyBuilding, price =>
                                                    {
                                                        if (price > 0)
                                                        {
                                                            Shared.TakeMoney(price, res =>
                                                            {
                                                                if (res == Shared.TakeMoneyResult.success)
                                                                {
                                                                    GameObject sp = LampO(BuildManager.Instance.currentlyBuilding);
                                                                    if (sp != null)
                                                                    {
                                                                        GameObject lamp = Instantiate(sp, null);
                                                                        PlacedLight l = lamp.GetComponent<PlacedLight>();
                                                                        l.Pos = pos;
                                                                        l.Rot = rot;
                                                                        l.ID = nextID;
                                                                        l.lightname = BuildManager.Instance.currentlyBuilding;
                                                                        nextID++;
                                                                        l.myColors = SelColors.ToArray();
                                                                        l.LightIntensity = currIntensity;
                                                                        l.Range = currRange;
                                                                        l.FloorCulling = GroundLight;
                                                                        l.enableShadows = Shadows;
                                                                        l.Set();
                                                                        if (BuildingUI.Instance != null)
                                                                        {
                                                                            BuildingUI.Instance.BuildS.Play();
                                                                        }
                                                                        if (SaveSystem.Instance != null)
                                                                        {
                                                                            SaveSystem.Instance.SaveAll();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        BuildingUI.Instance.priceShowT.text = "ERROR.";
                                                                        BuildingUI.Instance.ErrorS.Play();
                                                                        Invoke("resetText", 1f);
                                                                    }
                                                                    BuildManager.Instance.isBuilding = false;
                                                                }
                                                                else
                                                                {
                                                                    BuildingUI.Instance.priceShowT.text = $"ERROR. Transaction: {res}";
                                                                    BuildingUI.Instance.ErrorS.Play();
                                                                    Invoke("resetText", 1f);
                                                                    BuildManager.Instance.isBuilding = false;

                                                                }
                                                            }, "Light Placing");
                                                        }
                                                        else
                                                        {
                                                            Debug.LogError(price);
                                                            BuildManager.Instance.isBuilding = false;
                                                            BuildingUI.Instance.ErrorS.Play();

                                                        }
                                                    });
                                                }
                                                else
                                                {
                                                    BuildManager.Instance.isBuilding = false;
                                                    BuildingUI.Instance.ErrorS.Play();
                                                }
                                            }
                                            else
                                            {
                                                GameObject sp = LampO(BuildManager.Instance.currentlyBuilding);
                                                if (sp != null)
                                                {
                                                    GameObject lamp = Instantiate(sp, null);
                                                    PlacedLight l = lamp.GetComponent<PlacedLight>();
                                                    l.Pos = pos;
                                                    l.Rot = rot;
                                                    l.ID = nextID;
                                                    l.lightname = BuildManager.Instance.currentlyBuilding;
                                                    nextID++;
                                                    l.myColors = SelColors.ToArray();
                                                    l.LightIntensity = currIntensity;
                                                    l.Range = currRange;
                                                    l.FloorCulling = GroundLight;
                                                    l.enableShadows = Shadows;
                                                    
                                                    l.Set();
                                                    if (BuildingUI.Instance != null)
                                                    {
                                                        BuildingUI.Instance.BuildS.Play();
                                                    }
                                                    if (SaveSystem.Instance != null)
                                                    {
                                                        SaveSystem.Instance.SaveAll();
                                                    }
                                                }
                                                else
                                                {
                                                    BuildingUI.Instance.priceShowT.text = "ERROR.";
                                                    BuildingUI.Instance.ErrorS.Play();
                                                    Invoke("resetText", 1f);
                                                }
                                                BuildManager.Instance.isBuilding = false;
                                            }
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
        public GameObject LampO(string w)
        {
            switch (w)
            {
                case "lamp_classicbulb":
                    {
                        return LightPrefabs[0];
                    }      
                case "lamp_ceilingflat":
                    {
                        return LightPrefabs[1];
                    } 
                case "lamp_wfnaf":
                    {
                        return LightPrefabs[2];
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}