using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.UI
{
    public class PosTopping : MonoBehaviour
    {
        public PossibleToppings me;
        public void Click()
        {
            if (PizzeriaManagementUI.Instance != null)
            {
                PizzeriaManagementUI.Instance.AddToppingToSelected(this);
            }
        }
    }
}
