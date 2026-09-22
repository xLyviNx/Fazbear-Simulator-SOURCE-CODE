using GameJolt.API;
using LyviN.Player.Interactions;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props
{
    public class doughPotProp : MonoBehaviour
    {
        [Header("Values")]
        [Range(0, 100)]
        public int FillPercentage;
        [Header("Config")]
        public float R_LerpSpeed = 1f;
        [Header("References")]
        public SkinnedMeshRenderer doughRenderer;
        public AudioInfo fillingS;
        [Header("Other Random Values")]
        public float R1;
        public float R2;
        public float TR1;
        public float TR2;
        public float TS;
        public float S;
        private int OldFillPercentage;
        bool wasmytext;
        public int TimeRemaining;

        private void Update()
        {
            if (R1 != TR1)
            {
                R1 = Mathf.Lerp(R1, TR1, Time.deltaTime * R_LerpSpeed);
            }
            if (R2 != TR2)
            {
                R2 = Mathf.Lerp(R2, TR2, Time.deltaTime * R_LerpSpeed);
            }
            doughRenderer.SetBlendShapeWeight(0, R1);
            doughRenderer.SetBlendShapeWeight(1, R2);
            doughRenderer.SetBlendShapeWeight(2, 100 - FillPercentage);
            if (OldFillPercentage != FillPercentage)
            {
                OldFillPercentage = FillPercentage;
                RandomizeDoughSeed();
            }
            fillingS.Mute = (TimeRemaining <= 0);
            if (CookingUI.Instance != null)
            {
                if (CustomerInteraction.Instance != null)
                {
                    if (CustomerInteraction.Instance.hits && CustomerInteraction.Instance.hit.transform == transform)
                    {
                        TS = 1.1f;
                        if (FillPercentage >= 10 && TimeRemaining <= 0)
                        {
                            CookingUI.Instance.crosshairText.text = $"Dough left: {FillPercentage}%";
                        }
                        else if (TimeRemaining <= 0)
                        {
                            CookingUI.Instance.crosshairText.text = $"NO DOUGH LEFT!";
                        }
                        else if (TimeRemaining > 0)
                        {
                            CookingUI.Instance.crosshairText.text = $"FILLING ({TimeRemaining}s left)";
                        }
                        wasmytext = true;
                    }
                    else
                    {
                        TS = 1f;
                        if (wasmytext)
                        {
                            CookingUI.Instance.crosshairText.text = $"";
                            wasmytext = false;
                        }
                    }
                }
            }
            if (S != TS)
            {
                S = Mathf.Lerp(S, TS, Time.deltaTime * R_LerpSpeed);
            }
            transform.localScale = Vector3.one * S;
        }

        public void RandomizeDoughSeed()
        {
            if (FillPercentage > 0)
            {
                TR1 = Random.Range(0, 100);
                TR2 = Random.Range(0, 100);
            }
            else
            {
                TR1 = 0;
                TR2 = 0;
            }
        }
        public IEnumerator FillMe()
        {
            if (FillPercentage < 100)
            {
                int timer = 300;
                if (GameJoltAPI.myInstance != null && !GameJoltAPI.Instance.HasSignedInUser)
                {
                    timer = 30;
                }

                while (timer > 0)
                {
                    yield return new WaitForSecondsRealtime(1f);
                    timer--;
                    TimeRemaining = timer;
                }
                TimeRemaining = 0;
                FillPercentage = 100;
                Invoke("Save", 0.1f);
            }
            yield return null;
        }
        void Save()
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0 && SaveSystem.Instance != null)
                {
                    SaveSystem.Instance.WaitFrameAndSave();
                }
            }
        }
    }
}