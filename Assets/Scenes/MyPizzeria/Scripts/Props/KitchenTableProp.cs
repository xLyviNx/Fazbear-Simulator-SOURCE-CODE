using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props
{
    public class KitchenTableProp : MonoBehaviour
    {
        public GameObject Player;
        public GameObject Roller;
        public GameObject Ladle;
        public GameObject Grater;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Player == null)
            {
                if (BaseBake.Instance != null)
                {
                    Player = BaseBake.Instance.gameObject;
                }
            }
            else if (!Roller.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, Player.transform.position);
                if (dist > 5f)
                {
                    SwitchRoller();
                }
            }
            else if (!Ladle.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, Player.transform.position);
                if (dist > 5f)
                {
                    SwitchLadle();
                }
            }
            else if (!Grater.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, Player.transform.position);
                if (dist > 5f)
                {
                    SwitchGrater();
                }
            }
        }
        public void SwitchRoller()
        {
            if (BaseBake.Instance != null && (BaseBake.Instance.holdingRoller == null || BaseBake.Instance.holdingRoller == this))
            {
                if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("RollerPickup") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("HoldingRoller"))
                {
                    Roller.SetActive(!Roller.activeInHierarchy);
                    if (!Roller.activeInHierarchy)
                    {
                        BaseBake.Instance.ViewportAnim.CrossFade("RollerPickup", 0.2f);
                        BaseBake.Instance.holdingRoller = this;
                    }
                    else
                    {
                        BaseBake.Instance.holdingRoller = null;
                        BaseBake.Instance.ViewportAnim.CrossFade("HideRoller", 0.1f);
                    }
                }
            }
        }
        public void SwitchLadle()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.holdingRoller == null && (BaseBake.Instance.holdingLadle == null || BaseBake.Instance.holdingLadle == this))
            {
                if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("HoldingLadle") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("PickupLadle"))
                {
                    Ladle.SetActive(!Ladle.activeInHierarchy);
                    if (!Ladle.activeInHierarchy)
                    {
                        BaseBake.Instance.ViewportAnim.CrossFade("PickupLadle", 0.15f);
                        BaseBake.Instance.holdingLadle = this;
                    }
                    else
                    {
                        BaseBake.Instance.holdingLadle = null;
                        BaseBake.Instance.ViewportAnim.CrossFade("HideLadle", 0.05f);
                    }
                }
            }
        }
        public void SwitchGrater()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.holdingRoller == null && (BaseBake.Instance.holdingGrater == null || BaseBake.Instance.holdingGrater == this))
            {
                if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("HoldingGrater") || BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("PickupGrater"))
                {
                    Grater.SetActive(!Grater.activeInHierarchy);
                    if (!Grater.activeInHierarchy)
                    {
                        BaseBake.Instance.ViewportAnim.CrossFade("PickupGrater", 0.15f);
                        BaseBake.Instance.holdingGrater = this;
                    }
                    else
                    {
                        BaseBake.Instance.holdingGrater = null;
                        BaseBake.Instance.ViewportAnim.CrossFade("HideGrater", 0.05f);
                    }
                }
            }
        }
    }
}