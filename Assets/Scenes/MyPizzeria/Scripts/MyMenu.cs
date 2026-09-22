using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.Player.Baking
{

    public class MyMenu : MonoBehaviour
    {
        public static MyMenu Instance;
        [NonReorderable] public List<Pizza> Menu;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                enabled = false;
            }
        }
        public Pizza GetPizzaByName(string pname)
        {
            Pizza pz = null;
            foreach(Pizza p in Menu)
            {
                if (p.Name == pname)
                {
                    pz = p;
                }
            }
            return pz;
        }
    }
    [Serializable]
    public class Pizza
    {
        public string Name;
        public enum PossibleToppings
        {
            Unknown = 0,
            Tomatoes = 1,
            Olives = 2,
            Mushrooms = 3,
            Salami = 4,
            Sweet_Pepper = 5,
            Chili_Pepper = 6,
            Corn = 7,
            Ham = 8,
            Onions = 9,
            Pepperoni = 10,
            Pickles = 11,
            Spinach = 12,
            Basil = 13,
            Garlic = 14,
            Shrimps = 15,
            Anchovies = 16,
            Pineapple = 17
        }
        public List<PossibleToppings> Toppings;
        [Range(1, 100)]
        public int Price;
    }
}