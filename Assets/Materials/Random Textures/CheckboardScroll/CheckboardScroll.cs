using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LyviN.UI
{
    public class CheckboardScroll : MonoBehaviour
    {
        public Material ScrollMat;
        public float scrollSpeed = 0.15f;
        public float scroll;
        private void OnEnable()
        {
            scroll = 0f;
        }
        private void Update()
        {
            if (ScrollMat != null)
            {
                scroll = scroll + (Time.deltaTime * scrollSpeed);
                if (scroll > 1f)
                {
                    scroll = 0f;
                }
                ScrollMat.SetTextureOffset("_MainTex", new Vector2(scroll, -scroll - 0.038f));
            }

        }
        private void OnDisable()
        {
            ScrollMat.SetTextureOffset("_MainTex", new Vector2(0f, -0.038f));
        }
    }
}