using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LyviN.Map
{
    public class PlacedDoor : MonoBehaviour
    {
        public uint ID;
        public string doorname;
        public Vector3 Pos;
        public Quaternion Rot;
        public Vector3 Scale;
        [Header("References")]
        public GameObject DestructionLayer;
        public Renderer[] MyColorRenderers;
        public Animator myAnim;
        public GameObject myDoorHitbox;
        public NavMeshObstacle Obstacle;
        [Header("Config")]
        public Color[] myColors;
        public bool employees;
        [Header("Values")]
        public bool Opened;
        public void Set()
        {
            transform.position = Pos;
            transform.rotation = Rot;
            transform.localScale = Scale;
            if (MyColorRenderers.Length == myColors.Length)
            {
                int i = 0;
                foreach (Renderer ren in MyColorRenderers)
                {
                    ren.material.SetColor("_BaseColor", myColors[i]);
                    i++;
                }
            }
        }
        private void Update()
        {
            myDoorHitbox.SetActive(!Opened);
            myAnim.SetBool("Opened", Opened);
            Obstacle.carving = employees;

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
        public void OnTriggerStay(Collider other)
        {
            if (!employees && (other.tag == "Customer") || other.tag == "Player")
            {
                Opened = true;
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (!employees && (other.tag == "Customer") || other.tag == "Player")
            {
                Opened = false;
            }
        }
    }
}