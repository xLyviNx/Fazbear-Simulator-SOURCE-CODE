using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props {
    public class DoorTrigger : MonoBehaviour
    {
        public PlacedDoor door;
        private void OnTriggerStay(Collider other)
        {
            door.OnTriggerStay(other);
        }
        private void OnTriggerExit(Collider other)
        {
            door.OnTriggerExit(other);

        }
    }
}