using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Misc
{
    public class MultipleTags : MonoBehaviour
    {
        public List<CustomTags> Tags = new List<CustomTags>();

        public bool has (CustomTags ctag)
        {
            if (Tags.Count > 0)
            {
                return Tags.Contains(ctag);
            }
            else
            {
                return false;
            }
        }
    }
    public enum CustomTags
    {
        Unknown,
        ToppingsShelf,
        ToppingsShelfSlot,
        doughPot,
        Oven,
        droppedTopping,
        CornCan,
        kitchenMetalTable,
        plateShelf,
        PizzaPlate,
        curtain,
        Animatronic,
    }
}
