using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Build
{
    public class AnimatronicPreview : MonoBehaviour
    {
        public Renderer[] myRenderers;
        public Material BadMat;
        public Material TransparentMat;
        public List<Material[]> NormalMats = new List<Material[]>();
        bool badmatswitched;
        bool goodmatswitched;
        public bool isColliding;
        public bool isFloor;
        private void Start()
        {
            for (int i = 0; i < myRenderers.Length; i++)
            {
                NormalMats.Add(myRenderers[i].materials);
            }
            List<Material[]> nl = new List<Material[]>();
            for (int i = 0; i < NormalMats.Count; i++)
            {
                List<Material> mats = new List<Material>();
                foreach(Material m in NormalMats[i])
                {
                    Material nm = new Material(TransparentMat);
                    Color color = m.GetColor("_BaseColor");
                    color.a = 0.7f;
                    nm.SetTexture("_BaseMap", m.GetTexture("_BaseMap"));
                    nm.SetTexture("_SpecGlossMap", m.GetTexture("_SpecGlossMap"));
                    //nm.SetFloat("_Smoothness", m.GetTexture("_BaseMap"));
                    nm.SetFloat("_Smoothness", m.GetFloat("_Smoothness"));
                    nm.SetFloat("_Metallic", m.GetFloat("_Metallic"));
                    nm.SetColor("_BaseColor", color);
                    mats.Add(nm);
                    
                }
                nl.Add(mats.ToArray());
            }
            NormalMats = nl;
        }
        private void OnDisable()
        {
            for (int i = 0; i < myRenderers.Length; i++)
            {
                myRenderers[i].materials = NormalMats[i];
            }
        }
        private void Update()
        {
            if (isColliding || !isFloor)
            {
                if (!badmatswitched)
                {
                    foreach (Renderer r in myRenderers)
                    {
                        List<Material> mats = new List<Material>();
                        foreach(Material m in r.materials)
                        {
                            mats.Add(BadMat);
                        }
                        r.materials = mats.ToArray();
                    }
                    badmatswitched = true;
                }
                goodmatswitched = false;
            }
            else
            {
                if (!goodmatswitched)
                {
                    int i = 0;
                    foreach (Renderer r in myRenderers)
                    {
                        r.materials = NormalMats[i];
                        i++;
                    }

                    goodmatswitched = true;
                }

                badmatswitched = false;
            }
        }
    }
}