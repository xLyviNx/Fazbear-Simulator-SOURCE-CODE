using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.Player.Build;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class DoorPlacing : MonoBehaviour
    {
        public static DoorPlacing Instance;
        public GameObject[] DoorPrefabs;
        public GameObject[] doorPreviews;
        public PropPreview currPreviewO;
        public List<Color> SelColors;

        public bool empl;
        public float WidthScale;
        public float HeightScale;

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
                foreach (GameObject pp in doorPreviews)
                {
                    if (pp.name.ToLower() != $"{BuildManager.Instance.currentlyBuilding}_preview")
                    {
                        pp.SetActive(false);
                    }
                    else
                    {
                        pp.SetActive(true);
                        currPreviewO = pp.GetComponent<PropPreview>();
                    }
                }
                if (currPreviewO != null)
                {
                    currPreviewO.gameObject.SetActive(true);
                }
                WidthScale = 1f;
                HeightScale = 1f;
            }
        }
        private void Update()
        {
            if (BuildManager.Instance != null && Binds.Instance != null && Crosshair3D.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Doors)
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
                                else if (UnityEngine.Input.GetMouseButton(0))
                                {
                                    HeightScale = MathF.Round(HeightScale + ((scroll * Binds.Instance.ScrollSensitivity * 0.2f) * CatalogueMain.Instance.HeightScrollSens), 2);
                                }
                                else if (UnityEngine.Input.GetMouseButton(1))
                                {
                                    WidthScale = MathF.Round(WidthScale + ((scroll * Binds.Instance.ScrollSensitivity * 0.2f) * CatalogueMain.Instance.HeightScrollSens), 2);
                                }
                                HeightScale = Mathf.Clamp(HeightScale, 0.1f, 2f);
                                WidthScale = Mathf.Clamp(WidthScale, 0.1f, 2f);
                                currPreviewO.transform.localScale = new Vector3(WidthScale, HeightScale, currPreviewO.transform.localScale.z);
                            }
                        }
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.PropRotationT.text = $"Rotation: {Mathf.Round(currPreviewO.transform.eulerAngles.y)}*, Width: {WidthScale}, Height: {HeightScale}";
                        }
                        GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                        if (a.tag == "Floor")
                        {

                            currPreviewO.isFloor = true;
                            if (!currPreviewO.isColliding)
                            {

                                BuildingUI.Instance.priceShowT.text = "";
                                if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                {
                                    PlaceDoor(Crosshair3D.Instance.pos, currPreviewO.transform.rotation, currPreviewO.transform.localScale);
                                }

                            }
                            else
                            {
                                BuildingUI.Instance.priceShowT.text = "A Door is too close to other object.";
                            }


                        }
                        else
                        {
                            currPreviewO.isFloor = false;
                            BuildingUI.Instance.priceShowT.text = "A Door has to be touching a floor.";
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
        void PlaceDoor(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            if (BuildManager.Instance != null && Crosshair3D.Instance != null && BuildingUI.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Doors)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance.hits)
                        {
                            GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                            if (a.tag == "Floor")
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
                                            string dname = BuildManager.Instance.currentlyBuilding;
                                            if (GameJoltAPI.Instance.HasSignedInUser)
                                            {
                                                TimeSpan span = DateTime.Now.Subtract(Shared.lastTransaction);
                                                if (span.TotalSeconds > 3)
                                                {
                                                    Shared.CheckDoorPrice(BuildManager.Instance.currentlyBuilding, price =>
                                                    {
                                                        if (price > 0)
                                                        {
                                                            Shared.TakeMoney(price, res =>
                                                            {
                                                                if (res == Shared.TakeMoneyResult.success)
                                                                {
                                                                    GameObject sp = DoorO(BuildManager.Instance.currentlyBuilding);
                                                                    if (sp != null)
                                                                    {
                                                                        GameObject door = Instantiate(sp, null);
                                                                        PlacedDoor d = door.GetComponent<PlacedDoor>();
                                                                        d.Pos = pos;
                                                                        d.Rot = rot;
                                                                        d.ID = nextID;
                                                                        d.doorname = dname;
                                                                        nextID++;
                                                                        d.myColors = SelColors.ToArray();
                                                                        d.Scale = scale;
                                                                        d.employees = empl;
                                                                        d.Set();
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
                                                GameObject sp = DoorO(BuildManager.Instance.currentlyBuilding);
                                                if (sp != null)
                                                {
                                                    GameObject door = Instantiate(sp, null);
                                                    PlacedDoor d = door.GetComponent<PlacedDoor>();
                                                    d.Pos = pos;
                                                    d.Rot = rot;
                                                    d.ID = nextID;
                                                    d.doorname = dname;
                                                    nextID++;
                                                    d.myColors = SelColors.ToArray();
                                                    d.Scale = scale;
                                                    d.employees = empl;
                                                    d.Set();
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
        public GameObject DoorO(string w)
        {
            switch (w)
            {
                case "door_door1":
                    {
                        return DoorPrefabs[0];
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}