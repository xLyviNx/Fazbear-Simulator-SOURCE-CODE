using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.Map.Animatronics;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class AnimatronicPlacing : MonoBehaviour
    {
        public static AnimatronicPlacing Instance;
        public GameObject[] anmPrefabs;
        public GameObject[] anmPreviews;
        public AnimatronicPreview currPreviewO;
        public static uint nextID;
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
        public void SetPreview()
        {
            if (BuildManager.Instance != null)
            {
                foreach (GameObject pp in anmPreviews)
                {
                    if (pp.name.ToLower() != $"{BuildManager.Instance.currentlyBuilding}_preview")
                    {
                        pp.SetActive(false);
                    }
                    else
                    {
                        pp.SetActive(true);
                        currPreviewO = pp.GetComponent<AnimatronicPreview>();
                    }
                }
                if (currPreviewO != null)
                {
                    currPreviewO.gameObject.SetActive(true);
                }
            }
        }
        private void Update()
        {
            if (BuildManager.Instance != null && Binds.Instance != null && Crosshair3D.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Animatronics)
                {

                    if (Crosshair3D.Instance.hits && currPreviewO != null)
                    {
                        if (currPreviewO != null)
                        {
                            currPreviewO.gameObject.SetActive(true);
                        }
                        currPreviewO.transform.position = Crosshair3D.Instance.pos;
                        currPreviewO.gameObject.SetActive(true);
                        float scroll = UnityEngine.Input.mouseScrollDelta.y;
                        if (scroll != 0f)
                        {
                            if (CatalogueMain.Instance != null)
                            {
                                if (!UnityEngine.Input.GetMouseButton(0) && !UnityEngine.Input.GetMouseButton(1))
                                {
                                    currPreviewO.transform.Rotate(Vector3.up * CatalogueMain.Instance.HeightScrollSens * 5f * Mathf.Round(scroll));
                                    currPreviewO.transform.eulerAngles = new Vector3(currPreviewO.transform.eulerAngles.x, Mathf.Round(currPreviewO.transform.eulerAngles.y), currPreviewO.transform.eulerAngles.z);
                                }

                            }
                        }
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.PropRotationT.text = $"Rotation: {Mathf.Round(currPreviewO.transform.eulerAngles.y)}*";
                        }
                        GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                        if (a.tag == "Floor" || a.name.Contains("stage"))
                        {

                            currPreviewO.isFloor = true;
                            if (!currPreviewO.isColliding)
                            {
                                if ((currPreviewO.transform.position.y - a.transform.position.y) > 0)
                                {
                                    BuildingUI.Instance.priceShowT.text = "";
                                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                    {
                                        PlaceAnimatronic(Crosshair3D.Instance.pos, currPreviewO.transform.rotation);
                                    }
                                }
                                else
                                {
                                    currPreviewO.isFloor = false;
                                    BuildingUI.Instance.priceShowT.text = "An Animatronic has to be placed on TOP of the surface.";
                                }

                            }
                            else
                            {
                                BuildingUI.Instance.priceShowT.text = "An Animatronic is too close to other object.";
                            }


                        }
                        else
                        {
                            currPreviewO.isFloor = false;
                            BuildingUI.Instance.priceShowT.text = "An Animatronic has to be touching a floor or a stage.";
                        }
                    }

                }
                else
                {
                    if (currPreviewO != null)
                    {
                        currPreviewO.gameObject.SetActive(false);
                    }

                }

            }
        }
        public void PlaceAnimatronic(Vector3 pos, Quaternion rot)
        {

            if (BuildManager.Instance != null && Crosshair3D.Instance != null && BuildingUI.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Animatronics)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance.hits)
                        {
                            GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                            if (a.tag == "Floor" || a.name.Contains("stage"))
                            {
                                if (!currPreviewO.isColliding)
                                {
                                    if (!BuildManager.Instance.isBuilding)
                                    {
                                        if (GameJoltAPI.myInstance != null)
                                        {

                                            BuildManager.Instance.isBuilding = true;
                                            string aname = BuildManager.Instance.currentlyBuilding;
                                            if (GameJoltAPI.Instance.HasSignedInUser)
                                            {
                                                TimeSpan span = DateTime.Now.Subtract(Shared.lastTransaction);
                                                if (span.TotalSeconds > 3)
                                                {
                                                    Shared.CheckAnimatronicPrice(BuildManager.Instance.currentlyBuilding, price =>
                                                    {
                                                        if (price > 0)
                                                        {
                                                            Shared.TakeMoney(price, res =>
                                                            {
                                                                if (res == Shared.TakeMoneyResult.success)
                                                                {
                                                                    GameObject sp = AnimatronicO(BuildManager.Instance.currentlyBuilding);
                                                                    if (sp != null)
                                                                    {
                                                                        GameObject anm = Instantiate(sp, null);
                                                                        PlacedAnimatronic an = anm.GetComponent<PlacedAnimatronic>();
                                                                        an.Pos = pos;
                                                                        an.Rot = rot;
                                                                        an.ID = nextID;
                                                                        an.charname = aname;
                                                                        nextID++;
                                                                        an.Set();
                                                                        if (BuildingUI.Instance != null)
                                                                        {
                                                                            BuildingUI.Instance.BuildS.Play();
                                                                        }
                                                                        if (SaveSystem.Instance != null)
                                                                        {
                                                                            SaveSystem.Instance.WaitFrameAndSave();
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
                                                            }, "Animatronic Placing");
                                                        }
                                                        else
                                                        {
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
                                                GameObject sp = AnimatronicO(BuildManager.Instance.currentlyBuilding);
                                                if (sp != null)
                                                {
                                                    GameObject anm = Instantiate(sp, null);
                                                    PlacedAnimatronic an = anm.GetComponent<PlacedAnimatronic>();
                                                    an.Pos = pos;
                                                    an.Rot = rot;
                                                    an.ID = nextID;
                                                    an.charname = aname;
                                                    nextID++;
                                                    an.Set();
                                                    if (BuildingUI.Instance != null)
                                                    {
                                                        BuildingUI.Instance.BuildS.Play();
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
            }
        }
        public GameObject AnimatronicO(string w)
        {
            switch (w)
            {
                case "anm_freddy1":
                    {
                        return anmPrefabs[0];
                    }
                case "anm_bonnie1":
                    {
                        return anmPrefabs[1];
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}