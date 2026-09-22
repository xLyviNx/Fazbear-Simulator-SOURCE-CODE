using LyviN.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.UI
{
    public class MenuToppingEntry : MonoBehaviour
    {
        public PossibleToppings Me;
        public MenuPizzaEntry myPizza;
        public TextMeshProUGUI myText;
        private void Start()
        {
            myText.text = $"{Me.ToString().Replace('_', ' ')}";
        }
        public void RemoveMe()
        {
            if (ThePizzeria.Instance != null && !ThePizzeria.Instance.isOpened())
            {
                if (PizzeriaManagementUI.Instance != null)
                {
                    if (PizzeriaManagementUI.Instance.Menu_SelectedPizza == myPizza)
                    {
                        PizzeriaManagementUI.Instance.currentPizzas[myPizza.myID].Toppings.Remove(Me);
                        Destroy(gameObject);
                        PizzeriaManagementUI.Instance.EnableTopping(Me);
                    }
                }
            }
        }
    }
}
