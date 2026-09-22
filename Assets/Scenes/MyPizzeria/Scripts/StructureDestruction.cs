using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class StructureDestruction : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Ids
        private const int DESTROY_STRUCTURE_TROPHY_ID = 165218;
        private const int DESTROY_ANIMATRONIC_TROPHY_ID = 169759;

        public static StructureDestruction Instance;
        private BuiltWallMain hw;
        [SerializeField] private GameObject DestroyParticle;
        DateTime lastDestroy;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;

            }
            else
            {
                this.enabled = false;
            }
        }
        private void Update()
        {
            if (BuildManager.Instance != null && BuildingUI.Instance != null)
            {
                if (BuildManager.Instance.Building == BuildingType.Destroy)
                {
                    if (Binds.Instance != null)
                    {
                        if (Crosshair3D.Instance != null)
                        {
                            if (Crosshair3D.Instance.hits)
                            {
                                /*GameObject e = Crosshair3D.Instance.hit.transform.gameObject;
                                if (e.name == "Wall")
                                {
                                    if (hw != null)
                                    {
                                        hw.DestructionLayer.SetActive(true);
                                    }
                                    else
                                    {
                                        hw = e.transform.parent.GetComponent<BuiltWallMain>();
                                    }
                                }
                                */
                                if (Crosshair3D.Instance.hit.transform != null)
                                {
                                    GameObject ho = Crosshair3D.Instance.hit.transform.gameObject;
                                    if (ho.tag == "PlayerPlaced" || ho.tag=="PlacedProp" || ho.tag == "Floor" || ho.tag == "Wall" || ho.tag == "PlacedLight" || ho.tag == "PlacedDoor" || ho.tag == "PlacedAnimatronic")
                                    {
                                        if (UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].Key) || UnityEngine.Input.GetKeyDown(Binds.Instance.Building[4].AltKey))
                                        {
                                            DestroyObj(ho, Crosshair3D.Instance.hit.point);
                                        }

                                        string hasalt = "";
                                        if (Binds.Instance.Building[4].AltKey != KeyCode.None)
                                        {
                                            hasalt = $" or {Binds.Instance.Building[4].AltKey}";
                                        }
                                        BuildingUI.Instance.priceShowT.text = $"Pressing \"{Binds.Instance.Building[4].Key}\"{hasalt} will destroy selected structure.";
                                    }
                                    else
                                    {
                                        BuildingUI.Instance.priceShowT.text = $"Look at the Object you want to destroy.";
                                    }
                                }
                                else
                                {
                                    BuildingUI.Instance.priceShowT.text = $"Look at the Object you want to destroy.";
                                }
                            }
                            else
                            {
                                BuildingUI.Instance.priceShowT.text = $"Look at the Object you want to destroy.";
                            }

                        }
                        else
                        {
                            BuildingUI.Instance.priceShowT.text = $"3D Crosshair not initialized.";
                        }
                    }
                    else
                    {
                        BuildingUI.Instance.priceShowT.text = $"Keybinds not initialized.";
                    }
                }
            }
        }
        void DestroyObj(GameObject obj, Vector3 Pos)
        {
            TimeSpan span = DateTime.Now.Subtract(lastDestroy);
            if (span.TotalSeconds > 3)
            {
                if (obj.name == "Wall")
                {
                    GameObject e = obj.transform.parent.gameObject;
                    Destroy(e);
                    Trophies.TryUnlock(DESTROY_STRUCTURE_TROPHY_ID, cb => { });
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.WaitFrameAndSave();
                    }
                    Instantiate(DestroyParticle, Pos, DestroyParticle.transform.rotation);
                    lastDestroy = DateTime.Now;
                }
                else if (obj.tag == "PlacedProp")
                {
                    Destroy(obj);
                    Trophies.TryUnlock(DESTROY_STRUCTURE_TROPHY_ID, cb => { });
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.WaitFrameAndSave();
                    }
                    Instantiate(DestroyParticle, Pos, DestroyParticle.transform.rotation);
                    lastDestroy = DateTime.Now;
                }
                else if (obj.tag == "PlacedLight")
                {
                    Destroy(obj);
                    Trophies.TryUnlock(DESTROY_STRUCTURE_TROPHY_ID, cb => { });
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.WaitFrameAndSave();
                    }
                    Instantiate(DestroyParticle, Pos, DestroyParticle.transform.rotation);
                    lastDestroy = DateTime.Now;
                }
                else if (obj.tag == "PlacedDoor")
                {
                    GameObject w = lastParentWithTag("PlacedDoor", obj.transform);
                    Destroy(w);
                    Trophies.TryUnlock(DESTROY_STRUCTURE_TROPHY_ID, cb => { });
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.WaitFrameAndSave();
                    }
                    Instantiate(DestroyParticle, Pos, DestroyParticle.transform.rotation);
                    lastDestroy = DateTime.Now;
                }
                else if (obj.tag == "PlacedAnimatronic")
                {
                    Destroy(obj.gameObject);
                    Trophies.TryUnlock(DESTROY_ANIMATRONIC_TROPHY_ID, cb => { });
                    if (SaveSystem.Instance != null)
                    {
                        SaveSystem.Instance.WaitFrameAndSave();
                    }
                    Instantiate(DestroyParticle, Pos, DestroyParticle.transform.rotation);
                    lastDestroy = DateTime.Now;
                }
                //BuiltWallMain[] bwms = GameObject.FindObjectsOfType<BuiltWallMain>();

            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        GameObject lastParentWithTag(string tag, Transform which)
        {
            if (which.parent != null && which.parent.tag == tag)
            {
                return lastParentWithTag(tag, which.parent);
            }
            else
            {
                return which.gameObject;
            }
        }
    }
}