using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace LyviN.UI
{
    public class OrderEntry : MonoBehaviour
    {
        public Order me;
        public TextMeshProUGUI myIDT;
        public TextMeshProUGUI nameT;
        int myID;
        private void Start()
        {
            myID = -999;
            if (me!=null && OrdersManager.Instance != null)
            {
                myID = OrdersManager.Instance.Orders.IndexOf(me);
            }
        }
        private void Update()
        {
            if (me!=null && myID != -999)
            {
                myIDT.text = $"{myID}";
                nameT.text = $"{me.PizzaName}";
            }
            else
            {
                myIDT.text = "??";
                nameT.text = "ERROR";
            }
        }
        public void Press()
        {
            if (OrdersUI.Instance != null)
            {
                OrdersUI.Instance.ChooseOrder(me);
            }
        }
    }
}