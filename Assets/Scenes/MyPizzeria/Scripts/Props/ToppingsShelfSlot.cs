using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LyviN.Player.Baking;
using TMPro;
using System;
using LyviN.Player.Interactions;
using GameJolt.API;

namespace LyviN.Map.Props
{
    public class ToppingsShelfSlot : MonoBehaviour
    {
        public ToppingsShelf myShelf;
        public Pizza.PossibleToppings what = Pizza.PossibleToppings.Unknown;
        [Range(0, 30)]
        public uint howMany;
        public MeshRenderer box;
        public GameObject content;
        public TextMeshPro slotstext;
        public GameObject[] ContentPrefabs;
        public DateTime when;
        public Renderer myRend;
        public Material[] NormalMat;
        public Material[] HoverMat;
        public Material WoodMat;
        public Material BlueMat;
        public static bool TakeOutCD;
        bool initset;
        private void Update()
        {

            /*if (CustomerInteraction.Instance != null)
            {
                if (CustomerInteraction.Instance.hits)
                {
                    if (CustomerInteraction.Instance.hit.transform == transform)
                    {
                        myRend.materials = HoverMat;
                    }
                    else
                    {
                        myRend.materials = NormalMat;
                    }
                }
                else
                {
                    myRend.materials = NormalMat;
                }
            }
            else
            {
                myRend.materials = NormalMat;
            }*/
            

        }
        void endcd()
        {
            CancelInvoke("endcd");
            TakeOutCD = false;
        }
        public void SetWhat(Pizza.PossibleToppings w, int amount, DateTime When, Action<bool> succ)
        {
            if (!TakeOutCD || !initset)
            {
                if (w != Pizza.PossibleToppings.Unknown)
                {
                    if (When.Year >= 2022)
                    {
                        what = w;
                        when = When;
                        SetAmount(amount, did =>
                        {

                            if (did)
                            {
                                TakeOutCD = true;
                                CancelInvoke("endcd");
                                Invoke("endcd", 0.5f);
                                succ(true);
                                initset = true;
                            }
                            else
                            {
                                succ(false);
                                what = Pizza.PossibleToppings.Unknown;
                            }

                        });
                    }
                    else
                    {
                        succ(false);
                    }
                }
                else
                {
                    succ(false);
                }
            }
            else
            {
                succ(false);
            }
        }
        public void Add(int howmany, Action<bool> succ)
        {
            SetAmount(howmany, cb => { succ(cb); });
        }
        public void SetAmount(int howmany, Action<bool> succ)
        {
            if (!TakeOutCD || !initset)
            {
                if (howmany > 0 && howmany <= 30)
                {
                    TakeOutCD = true;
                    CancelInvoke("endcd");
                    Invoke("endcd", 0.5f);
                    howMany = (uint)howmany;
                    succ(true);
                    checkContent();
                    slotstext.text = $"{howMany}";
                    initset = true;

                }
                else if (howmany == 0)
                {
                    howMany = 0;
                    what = Pizza.PossibleToppings.Unknown;
                    slotstext.text = "";
                    Destroy(content);
                    content = null;
                    TakeOutCD = true;
                    CancelInvoke("endcd");
                    Invoke("endcd", 0.5f);
                    succ(true);
                    initset = true;
                }
                else
                {
                    succ(false);
                }
            }
            else
            {
                succ(false);
            }
        }
        public void checkContent()
        {
            slotstext.text = $"";

            if (content != null)
            {
                if (howMany > 0 && box.enabled)
                {
                    content.SetActive(true);
                    slotstext.text = $"{howMany}";
                }
                else 
                {
                    if (howMany == 0 || what == Pizza.PossibleToppings.Unknown)
                    {
                        howMany = 0;
                        Destroy(content);
                        what = Pizza.PossibleToppings.Unknown;
                        content = null;
                        slotstext.text = $"";
                    }
                    if (content != null)
                    {
                        content.SetActive(false);
                    }
                    slotstext.text = $"";
                }
            }
            else
            {
                slotstext.text = $"";
                if (what != Pizza.PossibleToppings.Unknown && howMany > 0)
                {
                    SetContent();
                }
            }
        }
        void SetContent()
        {
            if (what != Pizza.PossibleToppings.Unknown && howMany > 0)
            {
                if (ContentPrefabs.Length >= (int)what+1 && ContentPrefabs[(int)what] != null)
                {
                    content = Instantiate(ContentPrefabs[(int)what], transform);
                    if (content != null)
                    {
                        checkContent();
                    }
                    if (what == Pizza.PossibleToppings.Shrimps || what == Pizza.PossibleToppings.Anchovies)
                    {
                        myRend.material = BlueMat;
                    }
                    else
                    {
                        myRend.material = WoodMat;
                    }
                }
                
            }
        }
    }
}