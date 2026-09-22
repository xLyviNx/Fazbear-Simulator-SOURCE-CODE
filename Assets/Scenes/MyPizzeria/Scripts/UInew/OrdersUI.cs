using LyviN.Input;
using LyviN.Player.Baking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.UI
{
    public class OrdersUI : MonoBehaviour
    {
        public static OrdersUI Instance;
        public GameObject OrdersNotificationsParent;
        public GameObject OrderNotificationPrefab;
        public bool isOn;
        public GameObject Content;
        public Order SelectedOrder;

        public Transform OrdersParent;
        public GameObject OrderPrefab;
        public GameObject DetailsO;
        public TextMeshProUGUI DetailsText;
        int currID = -999;
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
            Content.SetActive(false);
        }
        private void Update()
        {
            Content.SetActive(isOn);
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isOrders = isOn;
            }
            if (Binds.Instance != null)
            {
                if (Binds.Press(Binds.Instance.General[2]))
                {
                    Switch();
                }
            }
            if (SelectedOrder != null)
            {
                if (!string.IsNullOrEmpty(SelectedOrder.PizzaName))
                {
                    if (SelectedOrder.customer != null)
                    {
                        TimeSpan when = DateTime.Now - SelectedOrder.whenOrdered;
                        DetailsText.text = $"<b>Order ID</b>:{currID}\n<b>Name:</b>{SelectedOrder.PizzaName}\n<b>Ordered</b>: {System.Math.Round(when.TotalMinutes, 1)} minutes ago.";
                        DetailsO.SetActive(true);
                    }
                    else
                    {
                        SelectedOrder = null;
                        DetailsO.SetActive(false);
                    }
                }
                else
                {
                    SelectedOrder = null;
                    DetailsO.SetActive(false);
                }
            }
            else
            {
                SelectedOrder = null;
                DetailsO.SetActive(false);
            }
        }
        public void Init()
        {
            foreach(Transform t in OrdersParent)
            {
                Destroy(t.gameObject);
            }
            if (OrdersManager.Instance != null)
            {
                foreach(Order o in OrdersManager.Instance.Orders)
                {
                    GameObject ce = Instantiate(OrderPrefab, OrdersParent);
                    ce.GetComponent<OrderEntry>().me = o;
                }
            }
        }
        public void ChooseOrder(Order o)
        {
            if (OrdersManager.Instance != null)
            {
                SelectedOrder = o;
                currID = OrdersManager.Instance.Orders.IndexOf(SelectedOrder);
            }
        }
        
        public void SpawnNotification()
        {
            Instantiate(OrderNotificationPrefab, OrdersNotificationsParent.transform);
        }
        void Switch()
        {
            if (CursorManager.Instance != null && (CursorManager.Instance.isLocked() || isOn))
            {
                isOn = !isOn;
                if (isOn)
                {
                    Init();
                }
                else
                {
                    SelectedOrder = null;
                }
            }
            
        }
    }
}