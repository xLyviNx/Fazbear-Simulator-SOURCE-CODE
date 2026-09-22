using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.Map.Props;
using LyviN.Misc;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LyviN.Player.Build
{
    public class PropPlacing : MonoBehaviour
    {
        public static PropPlacing Instance;
        public GameObject[] propsPrefabs;
        public GameObject[] propsPreviews;
        public static uint nextID;
        public PropPreview currPreviewO;
        public Quaternion prevRot;
        public List<Color> SelColors;
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
                foreach (GameObject pp in propsPreviews)
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
            }
        }
        private void Update()
        {
            if (BuildManager.Instance != null && Crosshair3D.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Props)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance.hits && currPreviewO != null)
                        {
                            if (currPreviewO != null)
                            {
                                currPreviewO.gameObject.SetActive(true);
                            }
                            currPreviewO.transform.position = Crosshair3D.Instance.pos;
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
                                BuildingUI.Instance.PropRotationT.text = $"Prop Rotation: {Mathf.Round(currPreviewO.transform.eulerAngles.y)}*";
                            }
                            GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                            CounterTypeProp c = null;
                            if (a.tag == "PlacedProp")
                            {
                                c = a.GetComponent<CounterTypeProp>();
                            }
                            bool isCashOnCounter = BuildManager.Instance.currentlyBuilding.ToLower().Contains("cashregister") && c != null;
                            bool isOnOven = BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") && a != null && a.name.Contains("prop_kitchenoven");
                            bool cash = !BuildManager.Instance.currentlyBuilding.ToLower().Contains("cashregister") || isCashOnCounter;
                            bool oven = !BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") || isOnOven;
                            bool isOnMetalKitchenTable = false;
                            if (!oven && BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") && a.tag == "PlacedProp")
                            {
                                MultipleTags mt = a.GetComponent<MultipleTags>();
                                if (mt!=null)
                                {
                                    isOnMetalKitchenTable = mt.has(CustomTags.kitchenMetalTable) || mt.has(CustomTags.Oven);
                                }
                            }
                            if (((a.tag == "Floor" &&  oven) || (oven || isOnMetalKitchenTable)) && cash)
                            {
                                currPreviewO.isFloor = true;
                                if (!currPreviewO.isColliding)
                                {
                                    if ((currPreviewO.transform.position.y - a.transform.position.y) > 0)
                                    {
                                        BuildingUI.Instance.priceShowT.text = "";
                                        if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                        {
                                            PlaceProp(Crosshair3D.Instance.pos, currPreviewO.transform.rotation);
                                        }
                                    }
                                    else
                                    {
                                        currPreviewO.isFloor = false;
                                        BuildingUI.Instance.priceShowT.text = "A prop has to be placed on TOP of the floor.";
                                    }
                                }
                                else
                                {
                                    currPreviewO.isFloor = false;
                                    BuildingUI.Instance.priceShowT.text = "A prop is too close to other object.";
                                }
                            }
                            else
                            {
                                currPreviewO.isFloor = false;

                                if (BuildManager.Instance.currentlyBuilding.ToLower().Contains("cashregister"))
                                {
                                    BuildingUI.Instance.priceShowT.text = "This prop has to be placed on the counter.";
                                }
                                else if (BuildManager.Instance.currentlyBuilding.ToLower().Contains("oven"))
                                {
                                    BuildingUI.Instance.priceShowT.text = "This prop has to be placed on the Metal Table or another Oven.";
                                }
                                else
                                {
                                    BuildingUI.Instance.priceShowT.text = "A prop has to be placed on the floor.";
                                    currPreviewO.isFloor = false;
                                }
                            }
                        }
                        else
                        {
                            BuildingUI.Instance.priceShowT.text = "A prop has to be placed on the floor.";
                            if (currPreviewO != null)
                            {
                                currPreviewO.isFloor = false;
                            }
                        }
                    }
                }
                else
                {
                    if (BuildingUI.Instance != null && BuildManager.Instance.Building != BuildingType.Doors && BuildManager.Instance.Building != BuildingType.Lights)
                    {
                        BuildingUI.Instance.PropRotationT.text = $"";
                    }
                    if (currPreviewO != null)
                    {
                        currPreviewO.gameObject.SetActive(false);
                    }
                }
            }
        }
        void PlaceProp(Vector3 pos, Quaternion rot)
        {
            if (BuildManager.Instance != null && Crosshair3D.Instance != null && BuildingUI.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Props)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance.hits)
                        {
                            GameObject a = Crosshair3D.Instance.hit.transform.gameObject;
                            CounterTypeProp ca = null;
                            if (a.tag == "PlacedProp")
                            {
                                ca = a.GetComponent<CounterTypeProp>();
                            }
                            bool isCashOnCounter = BuildManager.Instance.currentlyBuilding.ToLower().Contains("cashregister") && ca != null;
                            bool isOnOven = BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") && a != null && a.name.Contains("prop_kitchenoven");
                            bool cash = !BuildManager.Instance.currentlyBuilding.ToLower().Contains("cashregister") || isCashOnCounter;
                            bool oven = !BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") || isOnOven;
                            bool isOnMetalKitchenTable = false;
                            if (!oven && BuildManager.Instance.currentlyBuilding.ToLower().Contains("prop_kitchenoven") && a.tag == "PlacedProp")
                            {
                                MultipleTags mt = a.GetComponent<MultipleTags>();
                                if (mt != null)
                                {
                                    isOnMetalKitchenTable = mt.has(CustomTags.kitchenMetalTable) || mt.has(CustomTags.Oven);
                                }
                            }
                            if (((a.tag == "Floor" && oven) || (oven || isOnMetalKitchenTable)) && cash)
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
                                                    Shared.CheckPropPrice(BuildManager.Instance.currentlyBuilding, price =>
                                                    {
                                                        if (price > 0)
                                                        {
                                                            Shared.TakeMoney(price, res =>
                                                            {
                                                                if (res == Shared.TakeMoneyResult.success)
                                                                {
                                                                    GameObject sp = PropO(BuildManager.Instance.currentlyBuilding);
                                                                    if (sp != null)
                                                                    {
                                                                        GameObject prop = Instantiate(sp, null);
                                                                        PlacedProp p = prop.GetComponent<PlacedProp>();
                                                                        p.Pos = pos;
                                                                        p.Rot = rot;
                                                                        p.ID = nextID;
                                                                        p.propname = BuildManager.Instance.currentlyBuilding;
                                                                        nextID++;
                                                                        p.customColors = new List<Color>(SelColors);
                                                                        if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
                                                                        {
                                                                            FazNavMesh.Instance.surface.BuildNavMesh();
                                                                        }
                                                                        p.SetColors();
                                                                        if (BuildingUI.Instance != null)
                                                                        {
                                                                            BuildingUI.Instance.BuildS.Play();
                                                                        }
                                                                        if (SaveSystem.Instance != null)
                                                                        {
                                                                            SaveSystem.Instance.SaveAll();
                                                                        }
                                                                        ChairProp chair = null;
                                                                        chair = p.GetComponent<ChairProp>();
                                                                        if (chair != null)
                                                                        {
                                                                            chair.StartFindTable();
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
                                                            }, "Prop Placing");
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
                                                GameObject sp = PropO(BuildManager.Instance.currentlyBuilding);
                                                if (sp != null)
                                                {
                                                    GameObject prop = Instantiate(sp, null);
                                                    PlacedProp p = prop.GetComponent<PlacedProp>();
                                                    p.Pos = pos;
                                                    p.Rot = rot;
                                                    p.ID = nextID;
                                                    p.propname = BuildManager.Instance.currentlyBuilding;
                                                    p.customColors = new List<Color>(SelColors);
                                                    p.SetColors();
                                                    if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
                                                    {
                                                        FazNavMesh.Instance.surface.BuildNavMesh();
                                                    }
                                                    nextID++;
                                                    if (BuildingUI.Instance != null)
                                                    {
                                                        BuildingUI.Instance.BuildS.Play();
                                                    }
                                                    ChairProp chair = null;
                                                    chair = p.GetComponent<ChairProp>();
                                                    if (chair != null)
                                                    {
                                                        chair.StartFindTable();
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
        public GameObject PropO(string name)
        {
            GameObject sp = null;
            switch (name)
            {
                case "prop_table1":
                    {
                        sp = propsPrefabs[0];
                        break;
                    }
                case "prop_chair1":
                    {
                        sp = propsPrefabs[1];
                        break;
                    }
                case "prop_chair2_s1":
                    {
                        sp = propsPrefabs[2];
                        break;
                    }
                case "prop_chair2_s2":
                    {
                        sp = propsPrefabs[3];
                        break;
                    }
                case "prop_chair3_s1":
                    {
                        sp = propsPrefabs[4];
                        break;
                    }
                case "prop_chair3_s2":
                    {
                        sp = propsPrefabs[5];
                        break;
                    }
                case "prop_chair4_s1":
                    {
                        sp = propsPrefabs[6];
                        break;
                    }
                case "prop_chair4_s2":
                    {
                        sp = propsPrefabs[7];
                        break;
                    }
                case "prop_chair5_s1":
                    {
                        sp = propsPrefabs[8];
                        break;
                    }
                case "prop_chair5_s2":
                    {
                        sp = propsPrefabs[9];
                        break;
                    }
                case "prop_chair6":
                    {
                        sp = propsPrefabs[10];
                        break;
                    }
                case "prop_chair7":
                    {
                        sp = propsPrefabs[11];
                        break;
                    }
                case "prop_chair8":
                    {
                        sp = propsPrefabs[12];
                        break;
                    }
                case "prop_counter1":
                    {
                        sp = propsPrefabs[13];
                        break;
                    }
                case "prop_counter1b":
                    {
                        sp = propsPrefabs[14];
                        break;
                    }
                case "prop_cashregister1":
                    {
                        sp = propsPrefabs[15];
                        break;
                    }
                case "prop_kitchentable1":
                    {
                        sp = propsPrefabs[16];
                        break;
                    }
                case "prop_kitchendoughpot1":
                    {
                        sp = propsPrefabs[17];
                        break;
                    }
                case "prop_kitchentoppings":
                    {
                        sp = propsPrefabs[18];
                        break;
                    }
                case "prop_kitchenoven1":
                    {
                        sp = propsPrefabs[19];
                        break;
                    }
                case "prop_kitchenoven2":
                    {
                        sp = propsPrefabs[20];
                        break;
                    }
                case "prop_kitchenoven3":
                    {
                        sp = propsPrefabs[21];
                        break;
                    }
                case "prop_kitchencupboard1":
                    {
                        sp = propsPrefabs[22];
                        break;
                    }
                case "prop_kitchenmetaltable":
                    {
                        sp = propsPrefabs[23];
                        break;
                    }
                case "prop_kitchenplateshelf":
                    {
                        sp = propsPrefabs[24];
                        break;
                    }
                case "prop_arcademachine1":
                    {
                        sp = propsPrefabs[25];
                        break;
                    }
                case "prop_balloons1":
                    {
                        sp = propsPrefabs[26];
                        break;
                    }
                case "prop_roundstage1":
                    {
                        sp = propsPrefabs[27];
                        break;
                    }
                case "prop_bigstage1":
                    {
                        sp = propsPrefabs[28];
                        break;
                    }
                case "prop_table2":
                    {
                        sp = propsPrefabs[29];
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return sp;
        }
        void resetText()
        {
            BuildingUI.Instance.priceShowT.text = "";
        }
    }
}