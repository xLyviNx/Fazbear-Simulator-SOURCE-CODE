using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking
{
    public class PizzaPlate : MonoBehaviour
    {
        public Rigidbody myRigid;
        public bool dropped;
        public Dough myPizza;
        public bool BlockPickup;
        public void DropMe()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.currPlate == this)
            {
                BaseBake.Instance.currPlate = null;
                transform.SetParent(null);
                transform.eulerAngles = new Vector3(-90f,0f,0f);
                myRigid.isKinematic = false;
                myRigid.useGravity = true;
                BaseBake.Instance.bakeMode = "";
                dropped = true;
                
            }
        }
        public void Pickup()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.NotHoldingAnything() && dropped)
            {
                if (!BlockPickup)
                {
                    transform.SetParent(BaseBake.Instance.myCam.transform);
                    BaseBake.Instance.bakeMode = "tophold";
                    BaseBake.Instance.currPlate = this;
                    myRigid.isKinematic = true;
                    myRigid.useGravity = false;
                    transform.localPosition = new Vector3(0f, -0.09f, 1f);
                    dropped = false;
                    BaseBake.Instance.PickupDropSound.Play();
                }
            }
        }
        private void Update()
        {
            if (transform.parent != null)
            {
                transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            }
        }

    }
}
