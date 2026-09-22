using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.UI
{
    public class UpgradeFloorPreview : MonoBehaviour
    {
        public MeshRenderer myRend;
        public Material[] FloorDefaultMats;
        public Material[] FloorPatterns;
        public Material[] FloorPatternsSecond;

        private void Update()
        {
            if (StructureInteraction.Instance != null && StructureInteraction.Instance.selectedFloor != null)
            {
                //transform.localScale = new Vector3(transform.localScale.x, StructureInteraction.Instance.selectedWall.Height, transform.localScale.z);
                //transform.localPosition = new Vector3(transform.localPosition.x, initpos.y - StructureInteraction.Instance.selectedWall.Height / 2f, transform.localPosition.z);
                if (StructureInteraction.Instance.FloorPatternDropdown.value != 0)
                {
                    if (myRend.materials[0] != FloorPatterns[StructureInteraction.Instance.FloorPatternDropdown.value])
                    {
                        if (StructureInteraction.Instance.FloorPatternDropdown.value != 2)
                        {
                            myRend.materials = new Material[] { FloorPatterns[StructureInteraction.Instance.FloorPatternDropdown.value] };
                        }
                        else
                        {
                            myRend.materials = new Material[] { FloorPatterns[StructureInteraction.Instance.FloorPatternDropdown.value], FloorPatternsSecond[StructureInteraction.Instance.FloorPatternDropdown.value] };

                        }
                    }
                    if (StructureInteraction.Instance.FloorColor.currColor != myRend.materials[0].GetColor("_BaseColor"))
                    {
                        if (StructureInteraction.Instance.FloorPatternDropdown.value != 2)
                        {
                            myRend.material.SetColor("_BaseColor", StructureInteraction.Instance.FloorColor.currColor);
                        }
                        else
                        {
                            myRend.materials[0].SetColor("_BaseColor", StructureInteraction.Instance.FloorColor.currColor);

                        }
                    }
                    if (StructureInteraction.Instance.FloorPatternDropdown.value == 2)
                    {
                        if (StructureInteraction.Instance.FloorTileColor.currColor != myRend.materials[1].GetColor("_BaseColor"))
                        {
                            myRend.materials[1].SetColor("_BaseColor", StructureInteraction.Instance.FloorTileColor.currColor);
                        }
                    }
                }
                else
                {
                    myRend.materials = new Material[] { FloorDefaultMats[0] };
                }
            }
        }
    }
}