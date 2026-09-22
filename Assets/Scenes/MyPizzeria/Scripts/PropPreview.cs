using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LyviN.Player.Build
{
    public class PropPreview : MonoBehaviour
    {
        public Renderer[] myRenderers;
        public Renderer[] myColorRenderers;
        public List<Material> myDefaultMaterials;
        public Material BadMat;
        public Material[] NormalMats;
        bool badmatswitched;
        bool goodmatswitched;
        public bool isColliding;
        public bool isFloor;
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "PlacedProp" || other.tag == "Wall")
            {
                isColliding = true;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "PlacedProp" || other.tag == "Wall")
            {
                isColliding = false;
            }
        }
        private void OnEnable()
        {
            SetColor();
        }
        private void OnDisable()
        {
            for (int i = 0; i < myColorRenderers.Length; i++)
            {
                myColorRenderers[i].material = myDefaultMaterials[i];
            }
        }
        private void Start()
        {
            myDefaultMaterials = new List<Material>();
            foreach (Renderer r in myColorRenderers)
            {
                myDefaultMaterials.Add(r.sharedMaterial);
            }
        }
        private void Update()
        {
            if (isColliding || !isFloor)
            {
                if (!badmatswitched)
                {
                    foreach(Renderer r in myRenderers)
                    {
                        r.material = BadMat;
                    }
                    badmatswitched = true;
                }
                goodmatswitched = false;
            }
            else
            {
                if (!goodmatswitched)
                {
                    int i = 0;
                    foreach (Renderer r in myRenderers)
                    {
                        r.material = NormalMats[i];
                        i++;
                    }
                    SetColor();
                    goodmatswitched = true;
                }
                
                badmatswitched = false;
            }
        }
        void SetColor()
        {
            if (CatalogueMain.Instance != null)
            {
                if (!isColliding || isFloor)
                {
                    if (myColorRenderers.Length > 0 && CatalogueMain.Instance.customizeColors.Count > 0)
                    {
                        for (int i = 0; i < CatalogueMain.Instance.customizeColors.Count; i++)
                        {
                            Color col = new Color(CatalogueMain.Instance.customizeColors[i].currColor.r, CatalogueMain.Instance.customizeColors[i].currColor.g, CatalogueMain.Instance.customizeColors[i].currColor.b, 0.8f);
                            myColorRenderers[i].materials[0].SetColor("_BaseColor", col);
                        }
                    }
                }
            }
        }
    }
}