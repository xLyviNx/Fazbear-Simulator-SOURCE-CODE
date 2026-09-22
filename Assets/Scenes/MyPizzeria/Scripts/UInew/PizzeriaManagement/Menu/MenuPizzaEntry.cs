using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LyviN.UI
{
    public class MenuPizzaEntry : MonoBehaviour
    {

        public int myID;
        public TextMeshProUGUI myText;
        public void RemoveMe()
        {
            if (PizzeriaManagementUI.Instance != null)
            {
                PizzeriaManagementUI.Instance.currentPizzaObjects.Remove(this);
                PizzeriaManagementUI.Instance.currentPizzas.RemoveAt(myID);
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            if (PizzeriaManagementUI.Instance != null)
            {
                if (PizzeriaManagementUI.Instance.Menu_SelectedPizza == this)
                {
                    myText.color = new Color(1f, 0.8f, 0.5f);
                }
                else
                {
                    myText.color = Color.white;

                }
            }
        }
        public void ClickMe()
        {
            if (PizzeriaManagementUI.Instance != null)
            {
                if (PizzeriaManagementUI.Instance.Menu_SelectedPizza != this)
                {
                    PizzeriaManagementUI.Instance.Menu_SelectedPizza = this;
                    PizzeriaManagementUI.Instance.InitPizza();
                }
            }
        }
    }
}