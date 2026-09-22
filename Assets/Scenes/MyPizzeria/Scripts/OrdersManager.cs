using LyviN.AI.Customers;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking
{
    public class OrdersManager : MonoBehaviour
    {
        public static OrdersManager Instance;
        public List<Order> Orders = new List<Order>();

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
        public void AddOrder(Order or, Action<int> OrderID)
        {
            int id = Orders.Count;
            Orders.Add(or);
            ShowNewOrderNotification();
            OrderID(id);
        }

        public void ShowNewOrderNotification()
        {
            if (OrdersUI.Instance != null)
            {
                OrdersUI.Instance.SpawnNotification();
            }
        }
        public void RemoveOrder(CustomerMain customer)
        {
            if (customer != null && customer.OrderID >= 0)
            {
                if (Orders.Count >= customer.OrderID + 1)
                {
                    Orders.RemoveAt(customer.OrderID);
                    customer.OrderID = -1;
                    int i = 0;
                    foreach (Order o in Orders)
                    {
                        if (o.customer != null)
                        {
                            o.customer.OrderID = i;
                        }
                        i++;
                    }
                }
            }

        }
        public void RemoveOrderAt(int ID)
        {
            if (ID >= 0)
            {
                if (Orders.Count >= ID + 1)
                {
                    Orders.RemoveAt(ID);
                    int i = 0;
                    foreach (Order o in Orders)
                    {
                        if (o.customer != null)
                        {
                            o.customer.OrderID = i;
                        }
                        i++;
                    }
                }
            }
        }
    }
    [Serializable]
    public class Order
    {
        public CustomerMain customer;
        public string PizzaName;
        public DateTime whenOrdered;
        public Order(CustomerMain Customer, string pizzaName, DateTime when)
        {
            customer = Customer;
            PizzaName = pizzaName;
            whenOrdered = when;

        }
    }
}
