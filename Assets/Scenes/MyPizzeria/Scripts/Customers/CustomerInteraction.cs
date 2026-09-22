using LyviN.AI.Customers;
using LyviN.Input;
using LyviN.Map.Animatronics;
using LyviN.Map.Props;
using LyviN.Misc;
using LyviN.Player.Baking;
using LyviN.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Interactions
{
    public class CustomerInteraction : MonoBehaviour
    {
        public static CustomerInteraction Instance;
        [SerializeField] private LayerMask RaycastMask;
        public bool hits;
        public RaycastHit hit;
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
        void Update()
        {
            if (Binds.Instance != null)
            {
                if (Physics.Raycast(Crosshair3D.Instance.Camera.transform.position, Crosshair3D.Instance.Camera.transform.forward * 4f, out hit, 3f, RaycastMask))
                {
                    if (Binds.Press(Binds.Instance.Controls[8]))
                    {
                        PressInteraction();
                    }
                    if (UnityEngine.Input.GetMouseButtonDown(0))
                    {
                        MouseInteraction();
                    }
                    hits = true;
                    if (BaseBake.Instance != null)
                    {
                        BaseBake.Instance.hit = hit;
                    }
                }
                else
                {
                    hits = false;
                }
            }
            else
            {
                hits = false;
            }

            if (BaseBake.Instance != null)
            {
                BaseBake.Instance.hits = hits;
            }
        }
        GameObject FindDoughParent(GameObject d1)
        {
            if (d1.transform.parent != null && (d1.transform.parent.tag == "Pizza" || d1.transform.parent.tag.Contains("Dough")))
            {
                d1 = FindDoughParent(d1.transform.parent.gameObject);
            }

            return d1;
        }
        void PressInteraction()
        {
            if (Crosshair3D.Instance != null)
            {
                if (hits)
                {
                    if (hit.transform != null)
                    {
                        Interact(hit.transform.gameObject);
                    }

                }
            }
        }
        void MouseInteraction()
        {
            if (Crosshair3D.Instance != null)
            {
                if (hits)
                {
                    if (hit.transform != null)
                    {
                        MouseInteract(hit.transform.gameObject);
                    }

                }
            }
        }
        void MouseInteract(GameObject with)
        {
            bool tagsdone = false;
            MultipleTags ms = with.GetComponent<MultipleTags>();
            if (ms != null)
            {
                if (ms.has(CustomTags.Oven))
                {
                    if (BaseBake.Instance != null && BaseBake.Instance.bakeMode != "oven")
                    {
                        tagsdone = true;
                        ovenBase ov = ms.GetComponent<ovenBase>();
                        ov.isOpen = !ov.isOpen;
                    }
                }
            }
            if (!tagsdone)
            {

                if (with.tag == "Pizza")
                {
                    if (BaseBake.Instance != null && BaseBake.Instance.NotHoldingAnything())
                    {
                        with = CutTopping.ParentDough(with.transform);
                        Dough d = with.GetComponent<Dough>();
                        if (d.BakePercentage > 0 && d.myPlate != null)
                        {
                            d.Cut();
                            BaseBake.Instance.ChopSound.Play();
                        }
                    }
                }
            }
        }
        void Interact(GameObject with)
        {
            bool tagsdone = false;
            MultipleTags ms = with.GetComponent<MultipleTags>();
            if (ms != null)
            {
                if (ms.has(CustomTags.ToppingsShelfSlot))
                {
                    tagsdone = true;
                    ToppingsShelfSlot sl = ms.GetComponent<ToppingsShelfSlot>();
                    if (aname("NULL"))
                    {
                        if (BaseBake.Instance != null)
                        {
                            if (BaseBake.Instance.NotHoldingAnything())
                            {
                                BaseBake.Instance.GetTopping(sl);
                            }
                        }
                    }
                }
                else if (ms.has(CustomTags.Oven))
                {
                    tagsdone = true;
                    ovenBase ov = ms.GetComponent<ovenBase>();
                    if (aname("NULL"))
                    {
                        if (BaseBake.Instance != null)
                        {
                            if (BaseBake.Instance.NotHoldingAnything())
                            {
                                if (ov.myPizza != null && ov.isOpen)
                                {
                                    ov.myPizza.Pickup();
                                }
                                else
                                {
                                    BaseBake.Instance.InteractWithOven(ov);

                                }
                            }
                        }
                    }
                }
                else if (ms.has(CustomTags.droppedTopping))
                {
                    if (aname("NULL"))
                    {
                        if (BaseBake.Instance != null)
                        {
                            if (BaseBake.Instance.NotHoldingAnything())
                            {
                                BaseBake.Instance.PickupDroppedTopping(ms.GetComponent<PickedTopping>());
                            }
                        }
                    }

                }
                else if (ms.has(CustomTags.CornCan))
                {
                    if (aname("NULL"))
                    {
                        if (BaseBake.Instance != null)
                        {
                            if (BaseBake.Instance.NotHoldingAnything())
                            {
                                BaseBake.Instance.PickupCan(ms.GetComponent<CornCan>());
                            }
                        }
                    }

                }else if (ms.has(CustomTags.plateShelf))
                {
                    if (aname("NULL"))
                    {
                        if (BaseBake.Instance != null)
                        {
                            if (BaseBake.Instance.NotHoldingAnything())
                            {
                                BaseBake.Instance.GetPlate();
                            }
                        }
                    }
                }
                else if (ms.has(CustomTags.PizzaPlate))
                {
                    if (aname("NULL"))
                    {
                        ms.GetComponent<PizzaPlate>().Pickup();
                    }
                }
                else if (ms.has(CustomTags.curtain))
                {
                    ms.GetComponent<CurtainProp>().SwitchOpen();
                }
                else if (ms.has(CustomTags.Animatronic))
                {
                    ms.GetComponent<PlacedAnimatronic>().SwitchOn();
                }
            }
            if (!tagsdone)
            {
                if (with.tag == "Customer")
                {
                    with.GetComponent<CustomerMain>().Interaction();
                }
                else
                {
                    if (with.tag == "PlacedProp")
                    {
                        if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                        {
                            doughPotProp pot = with.GetComponent<doughPotProp>();
                            if (pot != null)
                            {
                                if (pot.FillPercentage >= 10 && pot.TimeRemaining <= 0)
                                {

                                    if (BaseBake.Instance != null)
                                    {
                                        pot.FillPercentage -= 10;
                                        BaseBake.Instance.SpawnDough();
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (with.tag == "Pizza" || with.tag.Contains("Dough"))
                    {
                        Debug.Log(1);
                        if (aname("NULL"))
                        {
                            Debug.Log(2);
                            GameObject d1 = with;
                            d1 = FindDoughParent(d1);
                            Dough d = d1.GetComponent<Dough>();
                            if (d != null)
                            {
                                Debug.Log(3);
                                d.Pickup();
                            }
                        }
                        else if (aname("HoldingRoller"))
                        {
                            GameObject d1 = with;
                            d1 = FindDoughParent(d1);
                            Dough d = d1.GetComponent<Dough>();
                            if (d != null)
                            {
                                BaseBake.Instance.enterBake("Roller", d);

                            }
                        }
                        else if (aname("HoldingLadle"))
                        {
                            GameObject d1 = with;
                            d1 = FindDoughParent(d1);
                            Dough d = d1.GetComponent<Dough>();
                            if (d != null)
                            {
                                BaseBake.Instance.enterBake("Ladle", d);

                            }
                        }
                        else if (aname("HoldingGrater"))
                        {
                            GameObject d1 = with;
                            d1 = FindDoughParent(d1);
                            Dough d = d1.GetComponent<Dough>();
                            if (d != null)
                            {
                                BaseBake.Instance.enterBake("Grater", d);

                            }
                        }
                    }
                    else if (with.tag == "Roller" && with.transform.parent.tag == "PlacedProp" && BaseBake.Instance != null && BaseBake.Instance.holdingRoller == null && BaseBake.Instance.holdingLadle == null && BaseBake.Instance.holdingGrater == null)
                    {
                        if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                        {
                            KitchenTableProp ktp = null;
                            ktp = with.transform.parent.GetComponent<KitchenTableProp>();
                            if (ktp != null)
                            {
                                ktp.SwitchRoller();
                            }
                        }
                    }
                    else if (with.tag == "Ladle" && with.transform.parent.tag == "PlacedProp" && BaseBake.Instance != null && (BaseBake.Instance.holdingLadle == null || BaseBake.Instance.holdingLadle == this) && BaseBake.Instance.holdingRoller == null && BaseBake.Instance.holdingGrater == null)
                    {
                        if (BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                        {
                            KitchenTableProp ktp = null;
                            ktp = with.transform.parent.GetComponent<KitchenTableProp>();
                            if (ktp != null)
                            {
                                ktp.SwitchLadle();
                            }
                        }
                    }
                    else if (with.tag == "Grater" && with.transform.parent.tag == "PlacedProp" && BaseBake.Instance != null && BaseBake.Instance.holdingLadle == null && BaseBake.Instance.holdingRoller == null && (BaseBake.Instance.holdingGrater == null || BaseBake.Instance.holdingGrater == this))
                    {
                        if (aname("NULL"))
                        {
                            KitchenTableProp ktp = null;
                            ktp = with.transform.parent.GetComponent<KitchenTableProp>();
                            if (ktp != null)
                            {
                                ktp.SwitchGrater();
                            }
                        }
                    }
                    else if (with.tag == "CutTopping")
                    {
                        if (aname("NULL"))
                        {
                            CutTopping ct = with.GetComponent<CutTopping>();
                            if (BaseBake.Instance != null && BaseBake.Instance.NotHoldingAnything())
                            {
                                BaseBake.Instance.GetChoppedTopping(ct);
                            }
                        }
                    }
                }
            }
        }
        bool aname(string name)
        {
            return BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName(name);
        }
    }

}