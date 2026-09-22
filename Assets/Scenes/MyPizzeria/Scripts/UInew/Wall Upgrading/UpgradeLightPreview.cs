using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.UI
{
    public class UpgradeLightPreview : MonoBehaviour
    {
        public MeshRenderer LightRend;
        private void Update()
        {
            if (StructureInteraction.Instance != null)
            {
                if (StructureInteraction.Instance.selectedLight != null)
                {
                    LightRend.material.SetColor("_BaseColor", StructureInteraction.Instance.LightColor.currColor);
                    LightRend.material.SetColor("_EmissionColor", new Color(StructureInteraction.Instance.LightColor.currColor.r * (StructureInteraction.Instance.LightIntensity.value / 50f), StructureInteraction.Instance.LightColor.currColor.g * (StructureInteraction.Instance.LightIntensity.value / 50f), StructureInteraction.Instance.LightColor.currColor.b * (StructureInteraction.Instance.LightIntensity.value / 50f)));

                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}