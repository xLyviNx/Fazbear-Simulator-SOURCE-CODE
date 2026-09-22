using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.UI
{
    public class BuildingUI : MonoBehaviour
    {
        public static BuildingUI Instance;
        public GameObject Content;
        public TextMeshProUGUI SnapText;
        public TextMeshProUGUI TypeText;
        public TextMeshProUGUI GridShowText;
        public TextMeshProUGUI MeasureShow;
        public TextMeshProUGUI HeightShow;
        public TextMeshProUGUI priceShowT;
        public TextMeshProUGUI PropRotationT;
        public GameObject GridO;


        public AudioSource ONS;
        public AudioSource OFFS;
        public AudioSource SWS;
        public AudioSource SwitchPosS;
        public AudioSource PointSetS;
        public AudioSource ErrorS;
        public AudioSource BuildS;
        public AudioSource ResetS;
        public GameObject HelpyBuildingUI;
        void Start()
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
            if (Crosshair3D.Instance != null && Crosshair3D.Instance.Pointer != null)
            {
                SnapText.text = $"Snap to grid - {Crosshair3D.Instance.SnapToGrid} (X: {Crosshair3D.Instance.xSnap}, Y: {Crosshair3D.Instance.ySnap}, Z: {Crosshair3D.Instance.zSnap})";


            }
            else
            {
                SnapText.text = "<color=\"red\">3D Crosshair/Cursor not detected.</color>";
            }
            GridShowText.text = $"Show Grid: {GridO.activeInHierarchy}";
            MeasureShow.text = $"Show Measurements: {CatalogueMain.Instance != null && CatalogueMain.Instance.showMeasure.isOn}";
            HeightShow.text = $"Show Height: {CatalogueMain.Instance != null && CatalogueMain.Instance.showHeight.isOn}";
            HelpyBuildingUI.SetActive(BuildManager.Instance != null && BuildManager.Instance.isBuilding);
        }
    }
}