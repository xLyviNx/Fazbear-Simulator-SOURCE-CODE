using LyviN.AI.Customers;
using LyviN.Audio;
using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Animatronics
{

    public class PlacedAnimatronic : MonoBehaviour
    {
        public uint ID;
        public string charname;
        public Vector3 Pos;
        public Quaternion Rot;

        [Header("References")]
        public GameObject DestructionLayer;
        public Animator myAnim;
        public AudioInfo PerformanceMusic;
        [Header("Values")]
        public bool Activated;
        public bool ManualActivation;
        public bool CloseActivation;
        public bool CloseActivationPlayer;
        public static PlacedAnimatronic lastAnimatronicTurnedON;
        public void Set()
        {
            transform.position = Pos;
            transform.rotation = Rot;
            ManualActivation = false;
        }

        private void Update()
        {
            
            Activated = ManualActivation && (CloseActivation || CloseActivationPlayer);
            if (PerformanceMusic != null)
            {
                PerformanceMusic.Mute = (!Activated) || (lastAnimatronicTurnedON != null && lastAnimatronicTurnedON != this && (lastAnimatronicTurnedON.PerformanceMusic.myAudio.volume > 0 && lastAnimatronicTurnedON.PerformanceMusic.myAudio.isPlaying));

            }
            if (myAnim != null)
            {
                myAnim.SetBool("Activated", Activated);
            }
            if (MusicManager.Instance != null && Activated && CloseActivationPlayer && MusicManager.Instance.isCloseToAnimatronic == null)
            {
                MusicManager.Instance.isCloseToAnimatronic = this;
            }
            else if (MusicManager.Instance != null && MusicManager.Instance.isCloseToAnimatronic == this && (!CloseActivationPlayer || !Activated))
            {
                MusicManager.Instance.isCloseToAnimatronic = null;
            }
            if (Crosshair3D.Instance != null && BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Destroy)
            {
                if (Crosshair3D.Instance.hits)
                {
                    if (Crosshair3D.Instance.hit.transform.gameObject == gameObject)
                    {
                        DestructionLayer.SetActive(true);
                    }
                    else
                    {
                        DestructionLayer.SetActive(false);

                    }
                }
                else
                {
                    DestructionLayer.SetActive(false);

                }
            }
            else
            {
                DestructionLayer.SetActive(false);

            }
        }
        private void OnDestroy()
        {
            if (CloseActivation || CloseActivationPlayer || Activated)
            {
                CloseActivation = false;
                CloseActivationPlayer = false;
                Activated = false;
                if (MusicManager.Instance.isCloseToAnimatronic == this)
                {
                    MusicManager.Instance.isCloseToAnimatronic = null;
                }
            }
        }
        public void FindCustomers()
        {
            CancelInvoke("FindCustomers");
            if (CustomersManager.Instance != null)
            {
                bool found = false;
                bool foundP = false;
                if (Vector3.Distance(transform.position, CustomersManager.Instance.transform.position) < 15f)
                {
                    if (lastAnimatronicTurnedON == null)
                    {
                        lastAnimatronicTurnedON = this;
                    }
                    foundP = true;
                    CloseActivationPlayer = true;
                }
                if (!foundP)
                {
                    CustomerMain[] cs = CustomersManager.Instance.Customers.ToArray();
                    foreach (CustomerMain c in cs)
                    {
                        if (c != null)
                        {
                            if (Vector3.Distance(transform.position, c.transform.position) < 20f)
                            {
                                found = true;
                                CloseActivation = true;
                                break;
                            }
                        }
                    }
                }
                if (!found)
                {
                    CloseActivation = false;
                }
                if (!foundP)
                {
                    CloseActivationPlayer = false;
                    if (lastAnimatronicTurnedON == this)
                    {
                        lastAnimatronicTurnedON = null;
                    }
                }
            }
            Invoke("FindCustomers", 3f);
        }
        public void SwitchOn()
        {
            ManualActivation = !ManualActivation;
            CancelInvoke("FindCustomers");
            if (ManualActivation)
            {
                lastAnimatronicTurnedON = this;
                FindCustomers();

            }
            else if (lastAnimatronicTurnedON == this)
            {
                lastAnimatronicTurnedON = null;
            }
        }
    }
}