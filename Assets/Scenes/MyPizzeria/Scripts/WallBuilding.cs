using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class WallBuilding : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int BUILD_STRUCTURE_TROPHY_ID = 165217;
        
        public static WallBuilding Instance;
        public GameObject PreviewWall;
        public Renderer PreviewWallRenderer;
        public Material GoodPlaceM;
        public Material BadPlaceM;
        public GameObject WallPrefab;
        public float Height;
        public TextMeshPro HeightT;
        public float currMeters;
        public bool willEmp;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

            }
            else
            {
                this.enabled = false;
            }
        }
        private void Update()
        {
            HeightT.gameObject.SetActive(PreviewWall.activeInHierarchy);
            if (BuildManager.Instance != null && Crosshair3D.Instance != null && Binds.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Walls || BuildManager.Instance.Building == BuildingType.Floor)
                {
                    Vector3 A = Crosshair3D.Instance.pos;
                    Vector3 B = Crosshair3D.Instance.pos;
                    if (BuildManager.Instance.A_Set)
                    {
                        A = BuildManager.Instance.PointA_O.transform.position;
                    }
                    if (BuildManager.Instance.B_Set)
                    {
                        B = BuildManager.Instance.PointB_O.transform.position;
                    }
                    float scroll = UnityEngine.Input.mouseScrollDelta.y;
                    if (scroll != 0f && Crosshair3D.Instance.hitsWithBuildLayer && Crosshair3D.Instance.hitWithBuildLayer.transform.gameObject == PreviewWall)
                    {
                        if (CatalogueMain.Instance != null)
                        {
                            Height = MathF.Round(Height +((scroll * Binds.Instance.ScrollSensitivity * 0.2f) * CatalogueMain.Instance.HeightScrollSens),2);
                        }
                    }
                    if (BuildManager.Instance.Building == BuildingType.Walls)
                    {
                        Height = Mathf.Clamp(Height, 0.1f, 4f);

                    }
                    else if (BuildManager.Instance.Building == BuildingType.Floor)
                    {
                        Height = Mathf.Clamp(Height, 0.1f, 1f);

                    }
                    if (BuildManager.Instance.A_Set || BuildManager.Instance.B_Set)
                    {
                        PreviewWall.SetActive(true);
                        Vector3 pos = Vector3.Lerp(A, B, 0.5f) + (Vector3.up * (Height / 2f));
                        PreviewWall.transform.position = pos;

                        float XLength = Vector3.Distance(new Vector3(A.x, 0f, 0f), new Vector3(B.x, 0f, 0f));
                        float ZLength = Vector3.Distance(new Vector3(0f, 0f, A.z), new Vector3(0f, 0f, B.z));
                        Vector3 scale = new Vector3(XLength, Height, ZLength);
                        PreviewWall.transform.localScale = scale;
                        float ht = (float)System.Math.Round(Height, 2);
                        if (CatalogueMain.Instance != null && CatalogueMain.Instance.showHeight)
                        {
                            HeightT.text = $"~{ht}m";
                            HeightT.transform.position = PreviewWall.transform.position + Vector3.up * (Height / 2f);
                            if (Camera.main != null)
                            {
                                HeightT.transform.rotation = Quaternion.LookRotation(HeightT.transform.position - Camera.main.transform.position);
                            }
                        }
                        else
                        {
                            HeightT.text = "";
                        }
                        bool isAnyFloor = BuildManager.Instance.isAFloor || BuildManager.Instance.isBFloor;
                        bool isAnyWall = BuildManager.Instance.isAWall || BuildManager.Instance.isBWall;
                        if (CatalogueMain.Instance != null)
                        {
                            int dprice = (int)((scale.x * scale.y * scale.z) * CatalogueMain.Instance.CurrPrice);
                            if (dprice < 1)
                            {
                                dprice = 1;
                            }
                            BuildingUI.Instance.priceShowT.text = $"Price: {dprice}$$";
                        }
                        bool war1 = (XLength < 0.25f || ZLength < 0.25f);
                        bool war2 = A.y != B.y;
                        bool war3 = (!isAnyFloor && !isAnyWall && BuildManager.Instance.Building != BuildingType.Floor);
                        bool war4 = (XLength * ZLength) > 10000;
                        bool war5 = (Mathf.Abs(A.x) > 90f || Mathf.Abs(A.y) > 90f || Mathf.Abs(A.z) > 90f);
                        bool war6 = (Mathf.Abs(B.x) > 90f || Mathf.Abs(B.y) > 90f || Mathf.Abs(B.z) > 90f);
                        //Debug.Log($"A {Mathf.Abs(A.x)}, {Mathf.Abs(A.y)}, {Mathf.Abs(A.z)}");
                        //Debug.Log($"B {Mathf.Abs(B.x)}, {Mathf.Abs(B.y)}, {Mathf.Abs(B.z)}");
                        //Debug.Log($"AB {war5} || {war6}");
                        if (war1 || war2 || war3 || war4 || war5 || war6)
                        {
                            PreviewWallRenderer.material = BadPlaceM;
                            if (Binds.Instance != null)
                            {
                                if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                }
                            }
                        }
                        else
                        {
                            PreviewWallRenderer.material = GoodPlaceM;
                            if (Binds.Instance != null)
                            {
                                if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                {
                                    PutWall(pos, scale);
                                }
                            }
                        }

                        
                    }
                    else
                    {
                        BuildingUI.Instance.priceShowT.text = $"";
                        PreviewWall.SetActive(false);
                    }
                }
                else
                {
                    PreviewWall.SetActive(false);
                }
            }
        }
        void PutWall(Vector3 pos, Vector3 scale)
        {
            bool exists = false;
            BuiltWallMain[] walls = GameObject.FindObjectsOfType<BuiltWallMain>();
            foreach(BuiltWallMain w in walls)
            {
                if (w.StructureID == BuildManager.NextID)
                {
                    exists = true;
                    Debug.LogError("Placing a structure with an ID that already exists! Aborting!");
                    return;
                }
            }
            if (BuildManager.NextID <= 100)
            {
                if (!exists && BuildManager.Instance != null && BuildingUI.Instance != null)
                {
                    if (GameJoltAPI.myInstance != null)
                    {
                        if (GameJoltAPI.Instance.HasSignedInUser)
                        {
                            TimeSpan span = DateTime.Now.Subtract(Shared.lastTransaction);
                            if (span.TotalSeconds > 5)
                            {
                                if (!Shared.hasTransaction && !BuildManager.Instance.isBuilding)
                                {
                                    BuildManager.Instance.isBuilding = true;
                                    Shared.CheckStructurePrice(BuildManager.Instance.currentlyBuilding, price =>
                                    {
                                        if (price >= 0)
                                        {
                                            price = (int)(price * (scale.x * scale.y * scale.z));
                                            if (price < 1)
                                            {
                                                price = 1;
                                            }
                                            Shared.TakeMoney(price, cb =>
                                            {
                                                if (cb == Shared.TakeMoneyResult.success)
                                                {
                                                    GameObject wall = Instantiate(WallPrefab, null);
                                                    BuiltWallMain bw = wall.GetComponent<BuiltWallMain>();
                                                    bw.Position = pos;
                                                    bw.X_Scale = scale.x;
                                                    bw.Height = scale.y;
                                                    bw.Z_Scale = scale.z;
                                                    bw.StructureID = BuildManager.NextID;
                                                    bw.type = BuildManager.Instance.currentlyBuilding;
                                                    bw.EmployeesOnly = willEmp;
                                                    bw.Set();
                                                    BuildManager.Instance.A_Set = false;
                                                    BuildManager.Instance.B_Set = false;
                                                    BuildingUI.Instance.BuildS.Play();
                                                    if (BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Floor)
                                                    {
                                                        Height = 0.5f;
                                                    }
                                                    else
                                                    {
                                                        Height = 4f;
                                                    }
                                                    BuildManager.NextID++;
                                                    if (SaveSystem.Instance != null)
                                                    {
                                                        SaveSystem.Instance.SaveAll();
                                                    }
                                                    BuildManager.Instance.isBuilding = false;
                                                    Trophies.TryUnlock(BUILD_STRUCTURE_TROPHY_ID, cb => { });
                                                }
                                                else
                                                {
                                                    BuildManager.Instance.isBuilding = false;
                                                    Debug.LogWarning(cb);
                                                }
                                            }, "Built a wall");
                                        }
                                        else
                                        {
                                            Debug.LogError($"Price Check Error: {price}");
                                        }
                                    });
                                }
                                else
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                }
                            }
                            else
                            {
                                if (BuildingUI.Instance != null)
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            }
                        }
                        else
                        {
                            GameObject wall = Instantiate(WallPrefab, null);
                            BuiltWallMain bw = wall.GetComponent<BuiltWallMain>();
                            bw.Position = pos;
                            bw.X_Scale = scale.x;
                            bw.Height = scale.y;
                            bw.Z_Scale = scale.z;
                            bw.StructureID = BuildManager.NextID;
                            bw.type = BuildManager.Instance.currentlyBuilding;
                            bw.EmployeesOnly = willEmp;
                            bw.Set();
                            BuildManager.Instance.A_Set = false;
                            BuildManager.Instance.B_Set = false;
                            BuildingUI.Instance.BuildS.Play();
                            if (BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Floor)
                            {
                                Height = 0.5f;
                            }
                            else
                            {
                                Height = 4f;
                            }
                            BuildManager.NextID++;
                        }
                        //int cost = (int)((scale.x * scale.y * scale.z) * )


                    }
                }
            }
        }
    }
}
