using LyviN.AI.Customers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props
{
    public class CashRegisterProp : MonoBehaviour
    {
        public PlacedProp Prop;
        public List<CustomerMain> Queue;
        public float queueStartOffset;
        public float queueBetweenOffset;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position - gameObject.transform.forward * 3f);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Customer")
            {
                CustomerMain customer = other.GetComponent<CustomerMain>();
                if (Queue.Count < 5 && customer.PointlessWandering && !customer.chairSitting && !customer.GaveUp)
                {
                    StartCoroutine(CheckQueueSomeone(customer));
                }
            }
        }
        public IEnumerator CheckQueueSomeone(CustomerMain customer, bool skipFinding = false)
        {
            if (!customer.chairSitting && customer.TargetChair == null && customer.OrderID == -1 && !customer.GaveUp && customer.myArcade == null)
            {
                if (Queue.Count < 5)
                {
                    if (Queue.Count == 0)
                    {
                        QueueSomeone(customer);
                    }
                    else
                    {
                        if (!skipFinding)
                        {
                            customer.FindEmptyCash();
                            yield return new WaitForSeconds(1f);
                        }
                        if (customer.currCash == null)
                        {
                            if (Queue.Count < 5)
                            {
                                QueueSomeone(customer);
                            }
                            else
                            {

                                if (!skipFinding)
                                {
                                    customer.FindEmptyCash();
                                }
                                else
                                {
                                    customer.Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noCashRegister)}");
                                    yield return new WaitForSeconds(1f);
                                    if (customer.currCash == null)
                                    {
                                        customer.PointlessWandering = true;
                                        customer.currCash = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    if (customer.currCash == null)
                    {
                        customer.PointlessWandering = true;
                        customer.currCash = null;
                    }

                }
            }
        }
        public void RemoveFromQueue(CustomerMain who)
        {
            Queue.Remove(who);
            who.currCash = null;
            RefreshQueue();
            Invoke("NextQueues", 1f);
        }
        void NextQueues()
        {
            int i = 0;
            List<CustomerMain> nq = new List<CustomerMain>(Queue);
            foreach (CustomerMain c in nq)
            {
                if (c != null)
                {
                    QueueSomeone(c, i);
                    i++;
                }
            }
        }
        void QueueSomeone(CustomerMain customer, int w = -1)
        {
            Debug.Log("QUEUE");
            customer.PointlessWandering = false;
            if (!Queue.Contains(customer))
            {
                Queue.Add(customer);
            }
            if (w >= 0 && w < 5)
            {
                customer.GetInQueue(this, (byte)(w));
            }
            else
            {
                customer.GetInQueue(this, (byte)(Queue.Count - 1));
            }
        }
        public void RefreshQueue()
        {
            List<CustomerMain> nc = new List<CustomerMain>();
            foreach(CustomerMain c in Queue)
            {
                if (c!=null)
                {
                    nc.Add(c);
                }
            }
            Queue.Clear();
            Queue.AddRange(nc);
        }

    }

}