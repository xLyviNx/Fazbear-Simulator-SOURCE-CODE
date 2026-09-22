using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.Map.Props
{
    public class ToppingsShelf : MonoBehaviour
    {
        public List<ToppingsShelfSlot> slots;
        private void Start()
        {
            foreach (ToppingsShelfSlot s in slots)
            {
                s.checkContent();
            }
        }
    }
    
}