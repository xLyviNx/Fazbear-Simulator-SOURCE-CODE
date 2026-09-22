using GameJolt.API;
using LyviN.Player;
using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LyviN.UI {
    public class MainUI : MonoBehaviour
    {
        public static MainUI Instance;
        public TextMeshProUGUI MoneyT;
        public TextMeshProUGUI TimeT;
        public GameObject skippingO;
        public string LoadedMoney = "...";

        public GameObject DefaultCrosshair;
        public GameObject KnifeCrosshair;

        public PickedTopping isToppingCut;
        public Dough isPizzaCut;
        public GameObject MCPrefab;
        public Transform MCParent;
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                StartCoroutine(MoneyLoading());
            }
            else
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isToppingCut != null || isPizzaCut != null)
            {
                DefaultCrosshair.SetActive(false);
                KnifeCrosshair.SetActive(true);
            }
            else
            {
                DefaultCrosshair.SetActive(true);
                KnifeCrosshair.SetActive(false);
            }
            if (TimeSystem.Instance != null)
            {
                string bh = "";
                string bm = "";
                if (TimeSystem.Instance.FinalHour < 10)
                {
                    bh = "0";
                }
                if (TimeSystem.Instance.FinalMinutes < 10)
                {
                    bm = "0";
                }
                TimeT.text = $"{bh}{TimeSystem.Instance.FinalHour}:{bm}{TimeSystem.Instance.FinalMinutes}";
            }
            MoneyT.text = LoadedMoney;
            skippingO.SetActive(TimeSystem.Instance != null && TimeSystem.Instance.skipnight);
        }
        public void ChangeMoneyUI(bool Added, int amount)
        {
            string color = Added ? "<color=\"green\">+" : "<color=\"red\">-";
            string text = $"{color}{amount}$";
            GameObject e = Instantiate(MCPrefab, MCParent);
            e.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
        IEnumerator MoneyLoading()
        {
            bool loaded = false;
            while (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {

                    while (GameJoltAPI.Instance.CurrentUser.ID == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    DataStore.Get("reset", false, rescb => { 
                    
                        if (rescb != null && !MainMenu.mereset)
                        {
                            SceneManager.LoadScene("MainMenu");
                        }
                        else
                        {
                            if (MainMenu.mereset)
                            {
                                DataStore.Delete("reset", false, delcd =>
                                {
                                    MainMenu.mereset = false;
                                });
                            }
                            DataStore.Get(Shared.myMoneyKey(), true, cb =>
                            {

                                if (cb != null && !string.IsNullOrEmpty(cb))
                                {
                                    loaded = true;
                                    LoadedMoney = $"{cb.Trim()}$";
                                }
                                else
                                {
                                    LoadedMoney = "Couldn't load your money.";
                                    loaded = true;
                                }

                            });
                        }
                    });
                    
                    while (!loaded)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                }
                yield return new WaitForSecondsRealtime(3f);
            }
        }
    }
}