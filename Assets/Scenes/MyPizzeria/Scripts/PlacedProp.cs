using LyviN.Map.Visiting;
using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map
{
    public class PlacedProp : MonoBehaviour
    {
        public uint ID;
        public string propname;
        public Vector3 Pos;
        public Quaternion Rot;
        public GameObject DestructionLayer;
        public Renderer[] ChangableColorsR;
        public List<Color> customColors;
        void Update()
        {
            transform.position = Pos;
            transform.rotation = Rot;


            if (Crosshair3D.Instance != null && BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Destroy)
            {
                if (Crosshair3D.Instance.hits)
                {
                    if (Crosshair3D.Instance.hit.transform.gameObject == gameObject)
                    {
                        DestructionLayer.SetActive(true);
                    }
                    else
                    {
                        DestructionLayer.SetActive(false);

                    }
                }
                else
                {
                    DestructionLayer.SetActive(false);

                }
            }
            else
            {
                DestructionLayer.SetActive(false);

            }
        }
        public void SetColors()
        {
            if (CatalogueMain.Instance != null || LoadOtherPizzeria.Instance != null)
            {
                if (customColors.Count > 0 && ChangableColorsR.Length == customColors.Count)
                {
                    for (int i = 0; i < customColors.Count; i++)
                    {
                        float a = 1f;
                        if (ChangableColorsR[i].gameObject.name.ToLower().Contains("balloon")){
                            a = 0.8f;
                        }
                        ChangableColorsR[i].materials[0].SetColor("_BaseColor", new Color(customColors[i].r, customColors[i].g, customColors[i].b, a));
                    }
                }
            }
        }
    }
}