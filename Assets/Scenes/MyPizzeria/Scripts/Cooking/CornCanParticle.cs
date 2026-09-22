using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking {
    public class CornCanParticle : MonoBehaviour
    {
        public CornCan me;
        private void OnParticleCollision(GameObject other)
        {
            me.collision(other);
        }
    }
}