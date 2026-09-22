using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class CataloguePreviewEntity : MonoBehaviour
    {
        public Renderer[] ChangableCols;
        public List<Material> defaultMats;

        private void Start()
        {
            defaultMats = new List<Material>();
            foreach(Renderer r in ChangableCols)
            {
                defaultMats.Add(r.sharedMaterial);
            }
        }
        public void SetColors()
        {
            if (CatalogueMain.Instance != null)
            {
                if (CatalogueMain.Instance.customizeColors.Count > 0)
                {
                    for (int i = 0; i < CatalogueMain.Instance.customizeColors.Count; i++)
                    {
                        if (ChangableCols[i].material != null)
                        {
                            ChangableCols[i].material.SetColor("_BaseColor", CatalogueMain.Instance.customizeColors[i].currColor);
                            if (ChangableCols[i].material.name.Contains("Light") && ChangableCols[i].material.name.Contains("ON"))
                            {
                                ChangableCols[i].material.SetColor("_EmissionColor", CatalogueMain.Instance.customizeColors[i].currColor);

                            }
                        }
                    }
                }
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < ChangableCols.Length; i++)
            {
                if (ChangableCols[i].material != null)
                {
                    ChangableCols[i].material = defaultMats[i];
                }
            }
        }
    }
}
