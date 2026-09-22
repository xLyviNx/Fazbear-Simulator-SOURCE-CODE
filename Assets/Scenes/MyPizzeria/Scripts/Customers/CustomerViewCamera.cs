using LyviN.AI.Customers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LyviN.UI
{
    public class CustomerViewCamera : MonoBehaviour
    {
        public Camera myCam;
        public CustomerMain myCustomer;

        private void Update()
        {
            if (OrdersUI.Instance != null && OrdersUI.Instance.isOn)
            {
                if (OrdersUI.Instance.SelectedOrder != null && OrdersUI.Instance.SelectedOrder.customer == myCustomer)
                {
                    myCam.enabled = true;
                }
                else
                {
                    myCam.enabled = false;
                }
            }
            else
            {
                myCam.enabled = false;
            }
        }
    }
}