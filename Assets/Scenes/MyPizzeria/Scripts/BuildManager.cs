using LyviN.Input;
using LyviN.Map;
using LyviN.Player.Control;
using LyviN.UI;
using TMPro;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;
        public FirstPersonCamera FPC;
        public BuildingType Building;
        public BuildingType LastBuilding;
        [Header("Building itself")]
        public GameObject PointA_O;
        public GameObject PointB_O;
        public bool A_Set;
        public bool B_Set;
        public LineRenderer Line;
        public TextMeshPro LengthText;
        public static uint NextID = 0;
        public string currentlyBuilding;
        public string lastBuildingType;
        public bool isAFloor;
        public bool isBFloor;
        public bool isAWall;
        public bool isBWall;
        public bool showGrid;
        public bool isBuilding;
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
            LengthText.gameObject.SetActive(Line.enabled);
            if (BuildingUI.Instance != null)
            {
                BuildingUI.Instance.GridO.SetActive(showGrid && Building != BuildingType.Destroy);
                BuildingUI.Instance.TypeText.text = $"Currently Editing: {Building.ToString()}";
                if (Crosshair3D.Instance != null)
                {
                    BuildingUI.Instance.GridO.transform.position = new Vector3(BuildingUI.Instance.GridO.transform.position.x, Crosshair3D.Instance.Pointer.transform.position.y, BuildingUI.Instance.GridO.transform.position.z);
                    if (Building != BuildingType.Destroy)
                    {
                        if (A_Set || B_Set)
                        {
                            Vector3 aP = Crosshair3D.Instance.Pointer.transform.position;
                            Vector3 bP = Crosshair3D.Instance.Pointer.transform.position;
                            if (A_Set)
                            {
                                aP = PointA_O.transform.position;
                            }
                            if (B_Set)
                            {
                                bP = PointB_O.transform.position;
                            }
                            if (CatalogueMain.Instance != null && CatalogueMain.Instance.showMeasure.isOn)
                            {
                                Line.enabled = true;
                            }
                            else
                            {
                                Line.enabled = false;
                            }

                            Line.SetPosition(0, aP);
                            Line.SetPosition(1, bP);
                            float dist = (float)System.Math.Round(Vector3.Distance(aP, bP), 2);
                            float xdist = (float)System.Math.Round(Mathf.Abs(bP.x - aP.x), 2);
                            float zdist = (float)System.Math.Round(Mathf.Abs(bP.z - aP.z), 2);
                            LengthText.transform.position = Vector3.Lerp(aP, bP, 0.5f) + Vector3.up * 0.4f;
                            LengthText.text = $"{dist}m\n<size=40%>X: {xdist}m, Z: {zdist}m</size>";
                            if (WallBuilding.Instance != null)
                            {
                                WallBuilding.Instance.currMeters = xdist * zdist * WallBuilding.Instance.Height;
                            }
                            else
                            {
                                Building = BuildingType.None;
                                BuildingUI.Instance.ErrorS.Play();
                                Debug.LogError("Wall Building Instance not found. Aborting building");
                            }
                            if (Camera.main != null)
                            {
                                LengthText.transform.rotation = Quaternion.LookRotation(LengthText.transform.position - Camera.main.transform.position);
                            }
                        }
                        else
                        {
                            Line.enabled = false;
                        }
                    }
                    else
                    {
                        Line.enabled = false;
                    }
                }
                if (Building == BuildingType.None)
                {
                    FPC.BuildOverlayCamera.gameObject.SetActive(false);
                    BuildingUI.Instance.Content.SetActive(false);
                    Line.enabled = false;
                }
                else
                {
                    FPC.BuildOverlayCamera.gameObject.SetActive(true);
                    BuildingUI.Instance.Content.SetActive(true);
                }
            }
            else
            {
                Line.enabled = false;
            }
            if (Binds.Instance != null)
            {
                if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[3].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[3].AltKey))
                {
                    SwitchGridSnap();
                }
                if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[0].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[0].AltKey))
                {
                    SwitchBuild();
                }
                if (Building != BuildingType.None && Building != BuildingType.Destroy && Building != BuildingType.Props && Building != BuildingType.Lights)
                {
                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[1].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[1].AltKey))
                    {
                        SelectPoint(0);
                    }
                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[2].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[2].AltKey))
                    {
                        SelectPoint(1);
                    }
                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[5].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[5].AltKey))
                    {
                        ResetBuild();
                    }
                    if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[6].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[6].AltKey))
                    {
                        SwitchGrid();
                    }
                }

            }
            PointA_O.SetActive(A_Set);
            PointB_O.SetActive(B_Set);
        }
        public void ResetBuild()
        {
            A_Set = false;
            B_Set = false;
            if (WallBuilding.Instance != null)
            {
                WallBuilding.Instance.Height = 4f;
            }

            if (BuildingUI.Instance != null)
            {
                BuildingUI.Instance.ResetS.Play();
            }
        }
        public void SelectPoint(int i)
        {
            if (Crosshair3D.Instance != null && BuildingUI.Instance != null)
            {
                if (Building == BuildingType.Walls || Building == BuildingType.Floor)
                {
                    if ((Crosshair3D.Instance.hits || (A_Set || B_Set)) && !Shared.hasTransaction)
                    {
                        if (i == 0)
                        {
                            PointA_O.transform.position = Crosshair3D.Instance.pos;
                            A_Set = true;
                            if (Crosshair3D.Instance.hits)
                            {

                                GameObject hitw = Crosshair3D.Instance.hit.transform.gameObject;
                                if (hitw.name == "Wall")
                                {
                                    BuiltWallMain wm = hitw.transform.parent.gameObject.GetComponent<BuiltWallMain>();
                                    if (wm != null)
                                    {
                                        isAFloor = wm.type.ToLower().Contains("floor");
                                        isAWall = wm.type.ToLower().Contains("wall");
                                    }
                                    else
                                    {
                                        isAFloor = false;
                                    }
                                }
                                else
                                {
                                    isAFloor = false;
                                }
                            }
                            else
                            {
                                isAFloor = false;
                            }
                            BuildingUI.Instance.PointSetS.Play();
                        }
                        else
                        {
                            PointB_O.transform.position = Crosshair3D.Instance.pos;
                            B_Set = true;
                            if (Crosshair3D.Instance.hits)
                            {

                                GameObject hitw = Crosshair3D.Instance.hit.transform.gameObject;
                                if (hitw.name == "Wall")
                                {
                                    BuiltWallMain wm = hitw.transform.parent.gameObject.GetComponent<BuiltWallMain>();
                                    if (wm != null)
                                    {
                                        isBFloor = wm.type.ToLower().Contains("floor");
                                        isBWall = wm.type.ToLower().Contains("wall");
                                    }
                                    else
                                    {
                                        isBFloor = false;
                                    }
                                }
                                else
                                {
                                    isBFloor = false;
                                }
                            }
                            else
                            {
                                isBFloor = false;
                            }
                            BuildingUI.Instance.PointSetS.Play();
                        }
                    }
                    else
                    {
                        BuildingUI.Instance.ErrorS.Play();
                    }
                }
            }
        }
        public void SwitchBuild()
        {
            if (CursorManager.Instance != null && CursorManager.Instance.isLocked())
            {
                if (Crosshair3D.Instance != null)
                {
                    if (Building != BuildingType.None && Building != BuildingType.Destroy)
                    {
                        LastBuilding = Building;
                        lastBuildingType = currentlyBuilding;
                        Crosshair3D.Instance.SnapToGrid = false;
                        Building = BuildingType.Destroy;
                        currentlyBuilding = "";
                    }
                    else if (Building == BuildingType.None)
                    {
                        Show(true);
                    }
                    else if (Building == BuildingType.Destroy)
                    {
                        Show(false);
                    }

                    if (BuildingUI.Instance != null)
                    {
                        if (Building == BuildingType.None)
                        {
                            BuildingUI.Instance.OFFS.Play();
                        }
                        else
                        {
                            BuildingUI.Instance.ONS.Play();
                        }
                    }
                }
            }
        }
        public void SwitchGrid()
        {
            if (Building != BuildingType.None || (CatalogueMain.Instance != null && CatalogueMain.Instance.isON))
            {
                if (BuildingUI.Instance != null)
                {
                    showGrid = !showGrid;
                    BuildingUI.Instance.SWS.Play();
                }
            }
        }
        public void SwitchGridSnap()
        {
            if (Building != BuildingType.None)
            {
                if (Crosshair3D.Instance != null)
                {
                    Crosshair3D.Instance.SnapToGrid = !Crosshair3D.Instance.SnapToGrid;
                    if (Crosshair3D.Instance.SnapToGrid && (!Crosshair3D.Instance.xSnap && !Crosshair3D.Instance.ySnap && !Crosshair3D.Instance.zSnap))
                    {
                        if (CatalogueMain.Instance != null)
                        {
                            CatalogueMain.Instance.xSnap.isOn = true;
                            CatalogueMain.Instance.ySnap.isOn = true;
                            CatalogueMain.Instance.zSnap.isOn = true;
                        }
                    }
                    if (BuildingUI.Instance != null)
                    {
                        BuildingUI.Instance.SWS.Play();
                    }
                }
            }
        }
        public void Show(bool t)
        {
            A_Set = false;
            B_Set = false;

            if (!t)
            {
                if (Building != BuildingType.Destroy)
                {
                    LastBuilding = Building;
                }
                Building = BuildingType.None;
            }
            else
            {
                if (LastBuilding != BuildingType.None)
                {
                    Building = LastBuilding;
                    if (currentlyBuilding == "")
                    {
                        currentlyBuilding = lastBuildingType;
                    }
                    else if (Building == BuildingType.Props)
                    {
                        if (PropPlacing.Instance != null)
                        {
                            PropPlacing.Instance.SetPreview();
                            if (PropPlacing.Instance.currPreviewO != null)
                            {
                                PropPlacing.Instance.currPreviewO.gameObject.SetActive(true);
                            }
                        }
                    }
                    else if (Building == BuildingType.Lights)
                    {
                        if (LightPlacing.Instance != null)
                        {
                            LightPlacing.Instance.SetPreview();
                            if (LightPlacing.Instance.currPreviewO != null)
                            {
                                LightPlacing.Instance.currPreviewO.gameObject.SetActive(true);
                            }
                        }
                    }
                    else if (Building == BuildingType.Doors)
                    {
                        if (DoorPlacing.Instance != null)
                        {
                            DoorPlacing.Instance.SetPreview();
                            if (DoorPlacing.Instance.currPreviewO != null)
                            {
                                DoorPlacing.Instance.currPreviewO.gameObject.SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    CatalogueMain.Instance.isON = true;
                }
            }
        }
    }
    public enum BuildingType
    {
        None,
        Destroy,
        Floor,
        Walls,
        Props,
        Lights,
        Doors,
        Animatronics,
    }
}