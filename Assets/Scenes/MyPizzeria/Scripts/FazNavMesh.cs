using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
namespace LyviN.Map
{
    public class FazNavMesh : MonoBehaviour
    {
        public static FazNavMesh Instance;
        public NavMeshSurface surface;
        public NavMeshQueryFilter queryfilter;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                if (surface != null)
                {
                    queryfilter.agentTypeID = surface.agentTypeID;
                    queryfilter.areaMask = surface.defaultArea;
                }
            }
            else
            {
                enabled = false;
            }
        }
    }
}