using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.AI.Customers
{
    public class RandomLook : MonoBehaviour
    {
        public SkinnedMeshRenderer[] RandomColors;
        public Material[] Eyes;
        public Material[] HairSkins;
        public Material[] Skins;
        public Material[] FaceSkins;
        public SkinnedMeshRenderer Face;
        public SkinnedMeshRenderer Arms;
        public SkinnedMeshRenderer Eye;
        public SkinnedMeshRenderer Boots;
        public SkinnedMeshRenderer Hair;
        private void OnEnable()
        {
            foreach (SkinnedMeshRenderer r in RandomColors)
            {
                foreach (Material m in r.materials)
                {
                    m.color = new Color(Random.Range(0.2f, 1f), Random.Range(0.2f, 1f), Random.Range(0.2f, 1f));
                }
            }
            Material[] facemats = Face.materials;
            Material[] bootmats = Boots.materials;

            int we = Random.Range(0, Eyes.Length);
            Eye.material = Eyes[we];
            int skin = Random.Range(0, FaceSkins.Length);
            facemats[0] = FaceSkins[skin];
            Arms.material = Skins[skin];
            Color bootsC = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            Color bootsC2 = bootsC * 0.65f;
            int bv = Random.Range(0, 2);
            if (bv == 0)
            {
                bootmats[0].color = bootsC;
                bootmats[1].color = bootsC2;
            }
            else
            {
                bootmats[1].color = bootsC;
                bootmats[0].color = bootsC2;
            }
            int hair = randomhair();
            facemats[2] = HairSkins[hair];
            Hair.material = HairSkins[hair];

            Face.materials = facemats;
            Boots.materials = bootmats;
        }
        int randomhair()
        {
            int hair = Random.Range(0, HairSkins.Length);
            if (hair == 4)
            {
                int chance = Random.Range(0, 5);
                if (chance != 3)
                {
                    hair = randomhair();
                }
            }
            return hair;
        }
    }
}