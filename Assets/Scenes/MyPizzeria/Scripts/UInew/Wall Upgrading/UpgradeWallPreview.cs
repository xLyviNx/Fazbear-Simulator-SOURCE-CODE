using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.UI
{
    public class UpgradeWallPreview : MonoBehaviour
    {
        public MeshRenderer myRend;
        public Material[] WallDefaultMats;
        public Material[] WallPatterns;

        public float StripeOffset;
        public GameObject stripeObject;
        public MeshRenderer stripeBorder1;
        public MeshRenderer stripeBorder2;
        public MeshRenderer stripeInside;

        public Material stripeInsideNormalMaterial;
        public Material stripeInsideTilesMaterial;
        public Material stripeTiles2Material;

        Vector3 initpos;
        // Update is called once per frame
        private void Start()
        {
            initpos = transform.localPosition;
        }
        void Update()
        {
            if (StructureInteraction.Instance != null && StructureInteraction.Instance.selectedWall != null)
            {
                //transform.localScale = new Vector3(transform.localScale.x, StructureInteraction.Instance.selectedWall.Height, transform.localScale.z);
                //transform.localPosition = new Vector3(transform.localPosition.x, initpos.y - StructureInteraction.Instance.selectedWall.Height / 2f, transform.localPosition.z);
                if (StructureInteraction.Instance.PaintDropdown.value != 0)
                {
                    if (myRend.material != WallPatterns[StructureInteraction.Instance.PaintDropdown.value])
                    {
                        myRend.material = WallPatterns[StructureInteraction.Instance.PaintDropdown.value];
                    }
                    if (StructureInteraction.Instance.WallMainColor.currColor != myRend.material.GetColor("_BaseColor"))
                    {
                        myRend.material.SetColor("_BaseColor", StructureInteraction.Instance.WallMainColor.currColor);
                    }
                }
                else
                {
                    myRend.material = WallPatterns[0]; // this will be changed when more types of wall comes
                }
                if (StructureInteraction.Instance.enabledStripe.isOn)
                {
                    stripeObject.SetActive(true);
                    stripeObject.transform.localPosition = Vector3.up * Mathf.Clamp((float)(System.Math.Round(StructureInteraction.Instance.StripeOffsetS.value, 2)), -0.4f, 0.4f);
                    stripeBorder1.material.SetColor("_BaseColor", StructureInteraction.Instance.StripeOutlineColor.currColor);
                    stripeBorder2.material = stripeBorder1.material;
                    if (!StructureInteraction.Instance.enabledTiles.isOn)
                    {
                        stripeInside.materials = new Material[] { stripeInsideNormalMaterial };
                        stripeInside.material.SetColor("_BaseColor", StructureInteraction.Instance.StripeInsideColor.currColor);
                    }
                    else
                    {
                        stripeInside.materials = new Material[] { stripeInsideTilesMaterial, stripeTiles2Material };
                        stripeInside.materials[0].SetColor("_BaseColor", StructureInteraction.Instance.StripeInsideColor.currColor);

                        stripeInside.materials[1].SetColor("_BaseColor", StructureInteraction.Instance.StripeTilesColor.currColor);
                    }

                }
                else
                {
                    stripeObject.SetActive(false);
                }
               
            }
        }
    }
}