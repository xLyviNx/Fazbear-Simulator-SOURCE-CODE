using GameJolt.API;
using LyviN.Input;
using LyviN.Map;
using LyviN.Map.Props;
using LyviN.Misc;
using LyviN.Player;
using LyviN.Player.Interactions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LyviN.AI.Customers.CustomerMain;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.UI
{
    public class StructureInteraction : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int STRUCTURE_UPGRADE_TROPHY_ID = 169418; // Structure upgrade trophy
        
        
        public static StructureInteraction Instance;
        [Header("References")]
        public GameObject Content;
        public GameObject notSelectedO;
        public GameObject SelectedO;
        public GameObject ToppingsO;
        public GameObject ToppingsFillO;
        public GameObject notEmptyToppingsO;
        public GameObject toppingsNotFoundO;
        public GameObject toppingPrefab;
        public GameObject toppingParent;
        public GameObject SelectedToppingO;
        public Slider FillAmountSlider;
        public TextMeshProUGUI willbelefttext;
        public TextMeshProUGUI ToppingNameText;
        public TextMeshProUGUI warningfill;
        public GameObject FillPizzeriaOpened;
        public GameObject previewsO;

        [Header("References but Walls")]
        public GameObject WallO;
        public GameObject enabledStripeO;
        public GameObject enabledTilesO;
        public GameObject walleditsO;
        public Toggle enabledStripe;
        public Toggle enabledTiles;
        public TextMeshProUGUI WallCostText;
        public CustomizeColorEntry StripeOutlineColor;
        public CustomizeColorEntry StripeInsideColor;
        public CustomizeColorEntry StripeTilesColor;
        public CustomizeColorEntry WallMainColor;
        public TMP_Dropdown PaintDropdown;
        public Slider StripeOffsetS;
        private string[] walleditprices;
        private System.DateTime walleditpricesfetched;
        public UpgradeWallPreview previewWall;
        public TMP_InputField PresetInput;
        [Header("Values")]
        public bool isOn;
        ToppingsShelfSlot selectedTopSlot = null;
        public PossibleToppings selectedTop;
        public int selectedTopAm;
        [SerializeField] bool isConfirming = false;
        [Header("Values but Walls")]
        public BuiltWallMain selectedWall;
        int currmoney;

        [Header("Floor")]
        public GameObject FloorO;
        public GameObject flooreditsO;
        public TMP_Dropdown FloorPatternDropdown;
        public CustomizeColorEntry FloorColor;
        public CustomizeColorEntry FloorTileColor;
        public UpgradeFloorPreview FloorPreview;
        private string[] flooreditprices;
        public BuiltWallMain selectedFloor;
        public TextMeshProUGUI FloorCostText;
        public TMP_InputField floorpreset;
        [Header("Light")]
        public GameObject LightsO;
        public GameObject LightEdits;
        public CustomizeColorEntry LightColor;
        public Slider LightIntensity;
        public Slider LightRange;
        public GameObject LightIntensityO;
        public GameObject LightRangeO;
        public GameObject LightPreviewO;
        public TMP_InputField lightpreset;
        [SerializeField] string[] lightprices;
        public PlacedLight selectedLight;
        public TextMeshProUGUI LightCostText;
        [Header("DoughPot")]
        public GameObject DP_O;
        public GameObject DP_PizO;
        public GameObject DP_PizC;
        public GameObject DP_FillTextO;
        public GameObject DP_ButtonO;
        public TextMeshProUGUI DP_InfoT;
        public doughPotProp selectedPot;
        [Header("More from Toppings")]
        public List<foundtop> Foundtops = new List<foundtop>();
        public List<int> topPrices = new List<int>();
        public PossibleToppings BuyingTopping;
        public GameObject buyTopO;
        public Slider BuyTopSlider;
        public TextMeshProUGUI BuyTopHeader;
        public TextMeshProUGUI BuyTopText;
        public TextMeshProUGUI BuyTopText2;
        public GameObject SelectedBuyTopO;
        void Update()
        {
            Content.SetActive(isOn);
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isStructureInteraction = isOn;
            }
            if (Binds.Instance != null)
            {
                if (Binds.Press(Binds.Instance.General[3]))
                {
                    Switch();
                }
            }
            if (willbelefttext.gameObject.activeInHierarchy && selectedTopAm > 0 && selectedTop != PossibleToppings.Unknown && selectedTopSlot != null)
            {
                willbelefttext.text = $"You will have {selectedTopAm - FillAmountSlider.value} left.";
                warningfill.text = $"<nobr>This will result in removing {FillAmountSlider.value} of {selectedTop.ToString().Replace('_', ' ')} from your storage. This can't be undone.</nobr>\n" +
                    $"You can't move selected toppings back to your storage after putting it in the container.";
            }
            SelectedToppingO.SetActive(selectedTop != PossibleToppings.Unknown && selectedTopSlot != null && selectedTopAm > 0 && isOn);
            if (selectedWall != null)
            {
                enabledStripeO.SetActive(enabledStripe.isOn);
                enabledTilesO.SetActive(enabledTiles.isOn);
                WallMainColor.gameObject.SetActive(PaintDropdown.value != 0);
            }
            if (selectedFloor != null)
            {
                FloorColor.gameObject.SetActive(FloorPatternDropdown.value != 0);
                FloorTileColor.gameObject.SetActive(FloorPatternDropdown.value == 2 && selectedFloor != null);
            }
            previewsO.SetActive((selectedFloor != null || selectedWall != null || selectedLight != null) && isOn);
            if (BuyingTopping != PossibleToppings.Unknown && buyTopO.activeInHierarchy && SelectedBuyTopO.activeInHierarchy)
            {
                if (topPrices.Count == Foundtops.Count && topPrices.Count > 0 && MainUI.Instance != null)
                {
                    BuyTopText2.text = $"You will pay {topPrices[(int)BuyingTopping] * BuyTopSlider.value}$\nYou currently have {MainUI.Instance.LoadedMoney}";
                }
            }
        }
        void Start()
        {
            if (Instance == null)
            {
                walleditpricesfetched = DateTime.Now;
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Switch()
        {
            if (!isConfirming)
            {
                if (CursorManager.Instance != null && (CursorManager.Instance.isLocked() || isOn))
                {
                    isOn = !isOn;
                    if (isOn)
                    {

                        Init();
                    }
                    else
                    {
                        selectedTopSlot = null;
                        previewsO.SetActive(false);
                        previewWall.gameObject.SetActive(false);
                        FloorPreview.gameObject.SetActive(false);
                        LightPreviewO.gameObject.SetActive(false);
                        selectedWall = null;
                        selectedFloor = null;
                        selectedLight = null;
                        selectedPot = null;
                        
                    }
                }
            }
            else
            {
                if (BuildingUI.Instance != null)
                {
                    BuildingUI.Instance.ErrorS.Play();
                }
            }
        }
        void Init()
        {
            DP_O.SetActive(false);
            DP_InfoT.text = "";
            DP_PizC.SetActive(false);
            DP_PizO.SetActive(false);
            DP_FillTextO.SetActive(false);
            DP_ButtonO.SetActive(false);
            FloorO.SetActive(false);
            flooreditsO.SetActive(false);
            toppingsNotFoundO.SetActive(false);
            FloorPreview.gameObject.SetActive(false);
            previewWall.gameObject.SetActive(false);
            SelectedO.SetActive(true);
            notSelectedO.SetActive(false);
            previewsO.SetActive(false);
            ToppingsO.SetActive(false);
            WallMainColor.gameObject.SetActive(false);
            WallO.SetActive(false);
            LightPreviewO.gameObject.SetActive(false);
            LightEdits.SetActive(false);
            LightsO.SetActive(false);
            buyTopO.SetActive(false);
            SelectedBuyTopO.SetActive(false);
            if (CustomerInteraction.Instance != null)
            {
                if (CustomerInteraction.Instance.hits)
                {
                    GameObject hitting = CustomerInteraction.Instance.hit.transform.gameObject;
                    if (hitting.tag == "Wall" || hitting.tag == "Floor")
                    {
                        BuiltWallMain bw = hitting.transform.parent.GetComponent<BuiltWallMain>();
                        if (bw != null)
                        {
                            if (bw.tag == "Wall")
                            {
                                previewsO.SetActive(true);
                                InitWall(bw);
                            }
                            else
                            {
                                if (bw.tag == "Floor")
                                {
                                    previewsO.SetActive(true);
                                    InitFloor(bw);
                                }
                            }
                        }
                        else
                        {
                            notSelected();
                        }
                    }
                    else if (hitting.tag == "PlacedLight")
                    {
                        previewsO.SetActive(true);
                        InitLight(hitting.GetComponent<PlacedLight>());
                    }
                    else
                    {
                        MultipleTags t = hitting.GetComponent<MultipleTags>();
                        if (t != null)
                        {
                            if (t.has(CustomTags.ToppingsShelfSlot))
                            {
                                FillPizzeriaOpened.SetActive(false);

                                ToppingsShelfSlot tss = t.GetComponent<ToppingsShelfSlot>();
                                if (tss != null)
                                {
                                    ToppingsO.SetActive(true);
                                    if (ThePizzeria.Instance != null && ThePizzeria.Instance.isOpened())
                                    {
                                        FillPizzeriaOpened.SetActive(true);
                                    }
                                    else if (tss.howMany == 0 && tss.what == PossibleToppings.Unknown)
                                    {
                                        notEmptyToppingsO.SetActive(false);
                                        ToppingsFillO.SetActive(true);
                                        selectedTopSlot = tss;
                                        InitToppingsSlot();
                                    }
                                    else
                                    {
                                        notEmptyToppingsO.SetActive(true);
                                        ToppingsFillO.SetActive(false);
                                    }
                                }
                                else
                                {
                                    notSelected();
                                }
                            }
                            else if (t.has(CustomTags.doughPot))
                            {
                                InitDoughPot(t.GetComponent<doughPotProp>());
                            }
                            else
                            {
                                notSelected();
                            }
                        }
                        else
                        {
                            notSelected();
                        }
                    }

                }
                else
                {
                    notSelected();
                }
            }
            else
            {
                notSelected();
            }
        }
        void InitToppingsSlot()
        {
            BuyTopText2.text = "";
            foreach (Transform t in toppingParent.transform)
            {
                Destroy(t.gameObject);
            }
            if (GameJoltAPI.myInstance != null)
            {
                bool any = false;

                DataStore.Get(Shared.myToppingsStorageKey(), true, cb =>
                {
                    Foundtops = new List<foundtop>();
                    for (int i = 0; i < 18; i++)
                    {
                        foundtop e1 = new foundtop();
                        e1.index = i;
                        Foundtops.Add(e1);
                        
                    }
                    if (cb != null)
                    {
                        if (!string.IsNullOrEmpty(cb))
                        {
                            string[] ts = cb.Trim().Split('\n');
                            foreach (string s in ts)
                            {
                                string[] a = s.Trim().Split('|');
                                int id = 0;
                                if (!string.IsNullOrEmpty(a[0].Trim()))
                                {
                                    id = int.Parse(a[0].Trim());
                                    int am = int.Parse(a[1].Trim());
                                    if (id != 0)
                                    {
                                        PossibleToppings t = (PossibleToppings)id;
                                        if (t != PossibleToppings.Unknown)
                                        {
                                            if (am > 0)
                                            {
                                                any = true;
                                                GameObject i = Instantiate(toppingPrefab, toppingParent.transform);
                                                i.GetComponent<ToppingEntry>().myText.text = $"{t.ToString().Replace('_', ' ')} ({am})";
                                                i.GetComponent<ToppingEntry>().howmany = am;
                                                i.GetComponent<ToppingEntry>().me = t;
                                            }

                                        }
                                    }
                                    foundtop e = new foundtop();
                                    e.index = id;
                                    e.amount = am;
                                    Foundtops[id] = (e);
                                }
                                
                            }
                        }
                    }
                    else
                    {
                        SaveSystem.checkTimeOut();
                    }
                    toppingsNotFoundO.SetActive(!any);
                });
            }
        }
        void InitDoughPot(doughPotProp dp)
        {
            if(ThePizzeria.Instance != null)
            {
                selectedPot = dp;
                DP_O.SetActive(true);
                if (ThePizzeria.Instance.isOpened())
                {
                    DP_PizO.SetActive(true);
                }
                else
                {
                    DP_PizC.SetActive(true);

                    if (dp.TimeRemaining > 0)
                    {
                        DP_InfoT.text = $"<size=120%>This Dough Pot is currently getting re-filled.\n<size=100%>Time Remaining: {dp.TimeRemaining}";
                        CancelInvoke("dpinfotfilltext");
                        Invoke("dpinfotfilltext", 1f);
                    }
                    else
                    {
                        DP_InfoT.text = $"Dough Remaining: {dp.FillPercentage}%\n";
                        if (dp.TimeRemaining == 0 && dp.FillPercentage < 100)
                        {
                            if (MainUI.Instance != null)
                            {
                                if (MainUI.Instance.LoadedMoney != "..." || MainUI.Instance.LoadedMoney != "Couldn't load your money.")
                                {
                                    DP_InfoT.text = DP_InfoT.text + $"You have: {MainUI.Instance.LoadedMoney}$";
                                }
                            }
                            DP_ButtonO.SetActive(true);
                            DP_FillTextO.SetActive(true);
                        }
                    }
                    
                }
            }
        }
        void dpinfotfilltext()
        {
            CancelInvoke("dpinfotfilltext");
            if (selectedPot != null)
            {
                if (selectedPot.TimeRemaining > 0 && selectedPot.FillPercentage < 100)
                {
                    DP_InfoT.text = $"<size=120%>This Dough Pot is currently getting re-filled.\n<size=100%>Time Remaining: {selectedPot.TimeRemaining}";
                    Invoke("dpinfotfilltext", 1f);
                }
            }
        }
        public void FillDoughPot()
        {
            if (selectedPot != null && selectedPot.FillPercentage < 100 && selectedPot.TimeRemaining <= 0 && !isConfirming)
            {
                isConfirming = true;
                Shared.TakeMoney(25, cb =>
                {
                    if (cb == Shared.TakeMoneyResult.success)
                    {
                        isConfirming = false;
                        StartCoroutine(selectedPot.FillMe());
                        if (isOn)
                        {
                            Switch();
                        }
                    }
                    isConfirming = false;
                }, "Dough Fill");
            }
        }
        public void SaveWallPreset()
        {
            string name = PresetInput.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                PresetInput.text = "Default";
            }
            string preset = $"{PaintDropdown.value}|{ColorUtility.ToHtmlStringRGB(WallMainColor.currColor)}|{enabledStripe.isOn}|{enabledTiles.isOn}|{Math.Round(StripeOffsetS.value, 2)}|{ColorUtility.ToHtmlStringRGB(StripeOutlineColor.currColor)}|{ColorUtility.ToHtmlStringRGB(StripeInsideColor.currColor)}|{ColorUtility.ToHtmlStringRGB(StripeTilesColor.currColor)}";
            PlayerPrefs.SetString($"wallpreset_{name}", preset);
        }
        public void SaveFloorPreset()
        {
            string name = floorpreset.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                floorpreset.text = "Default";
            }
            string preset = $"{FloorPatternDropdown.value}|{ColorUtility.ToHtmlStringRGB(FloorColor.currColor)}|{ColorUtility.ToHtmlStringRGB(FloorTileColor.currColor)}";
            PlayerPrefs.SetString($"floorpreset_{name}", preset);
        }
        public void LoadWallPreset()
        {
            string name = PresetInput.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                PresetInput.text = "Default";
            }
            string preset = PlayerPrefs.GetString($"wallpreset_{name}");
            if (!string.IsNullOrEmpty(preset))
            {
                string[] set = preset.Trim().Split('|');
                PaintDropdown.value = int.Parse(set[0].Trim());
                if (PaintDropdown.value != 0)
                {
                    Color sc = CustomizeColorEntry.hexToColor(set[1].Trim());
                    WallMainColor.RS.value = sc.r;
                    WallMainColor.GS.value = sc.g;
                    WallMainColor.BS.value = sc.b;
                }
                if (set[2].Trim().ToLower() == "true")
                {
                    enabledStripe.isOn = true;
                    float offs = float.Parse(set[4].Trim());
                    StripeOffsetS.value = offs;
                    Color sc = CustomizeColorEntry.hexToColor(set[5].Trim());
                    StripeOutlineColor.RS.value = sc.r;
                    StripeOutlineColor.GS.value = sc.g;
                    StripeOutlineColor.BS.value = sc.b;

                    Color si = CustomizeColorEntry.hexToColor(set[6].Trim());
                    StripeInsideColor.RS.value = si.r;
                    StripeInsideColor.GS.value = si.g;
                    StripeInsideColor.BS.value = si.b;

                    if (set[3].Trim().ToLower() == "true")
                    {
                        enabledTiles.isOn = true;
                        Color col = CustomizeColorEntry.hexToColor(set[7].Trim());
                        StripeTilesColor.RS.value = col.r;
                        StripeTilesColor.GS.value = col.g;
                        StripeTilesColor.BS.value = col.b;
                    }
                    else
                    {
                        enabledTiles.isOn = false;
                    }
                }
                else
                {
                    enabledStripe.isOn = false;
                    enabledTiles.isOn = false;
                }
            }
        }
        public void LoadFloorPreset()
        {
            string name = floorpreset.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                floorpreset.text = "Default";
            }
            Debug.Log(name);
            string preset = PlayerPrefs.GetString($"floorpreset_{name}");
            Debug.Log(preset);
            if (!string.IsNullOrEmpty(preset))
            {
                string[] set = preset.Trim().Split('|');
                FloorPatternDropdown.value = int.Parse(set[0].Trim());
                if (FloorPatternDropdown.value != 0)
                {
                    Color sc = CustomizeColorEntry.hexToColor(set[1].Trim());
                    FloorColor.RS.value = sc.r;
                    FloorColor.GS.value = sc.g;
                    FloorColor.BS.value = sc.b;
                    if (FloorPatternDropdown.value == 2)
                    {
                        Color fc = CustomizeColorEntry.hexToColor(set[2].Trim());
                        FloorTileColor.RS.value = fc.r;
                        FloorTileColor.GS.value = fc.g;
                        FloorTileColor.BS.value = fc.b;
                    }
                }
                
            }
        }
        public void LoadLightPreset()
        {
            string name = lightpreset.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                lightpreset.text = "Default";
            }
            string preset = PlayerPrefs.GetString($"lightpreset_{name}");
            if (!string.IsNullOrEmpty(preset))
            {
                string[] set = preset.Trim().Split('|');
                Color sc = CustomizeColorEntry.hexToColor(set[0].Trim());
                LightColor.RS.value = sc.r;
                LightColor.GS.value = sc.g;
                LightColor.BS.value = sc.b;
                float intensity = float.Parse(set[1].Trim());
                float range = float.Parse(set[2].Trim());
                LightIntensity.value = intensity;
                LightRange.value = range;
            }
        }
        public void SaveLightPreset()
        {
            string name = lightpreset.text;
            if (string.IsNullOrEmpty(name.Trim()))
            {
                name = "Default";
                lightpreset.text = "Default";
            }
            string preset = $"{ColorUtility.ToHtmlStringRGB(LightColor.currColor)}|{System.MathF.Round(LightIntensity.value,2)}|{System.MathF.Round(LightRange.value, 2)}";
            PlayerPrefs.SetString($"lightpreset_{name}", preset);
        }
        void InitWall(BuiltWallMain bw)
        {
            Debug.Log(1);
            if (bw!=null && bw.tag == "Wall")
            {
                Debug.Log(2);
                walleditsO.SetActive(false);
                WallO.SetActive(true);
                previewsO.SetActive(true);
                TimeSpan hm = DateTime.Now - walleditpricesfetched;
                if (hm.TotalSeconds >= 5 || walleditprices == null || walleditprices.Length <= 0)
                {
                    WallCostText.text = "Loading...";
                    walleditpricesfetched = System.DateTime.Now;
                    DataStore.Get($"system_wallUpgradePrices", true, cb =>
                    {
                        if (cb != null)
                        {
                            walleditprices = cb.Trim().Split('\n');
                            nextInitWall(bw);
                        }
                        DataStore.Get(Shared.myMoneyKey(), true, mon =>
                        {
                            if (cb != null)
                            {
                                currmoney = int.Parse(mon.Trim());
                                SetWallPrice();
                            }
                        });
                    });

                }
                else
                {
                    Debug.Log($"6:   {hm.TotalSeconds}");
                    nextInitWall(bw);
                }

            }
        }
        void InitFloor(BuiltWallMain bw)
        {
            if (bw != null && bw.tag == "Floor")
            {
                FloorTileColor.gameObject.SetActive(false);
                FloorColor.gameObject.SetActive(false);
                walleditsO.SetActive(false);
                flooreditsO.SetActive(false);
                WallO.SetActive(false);
                FloorO.SetActive(true);
                previewsO.SetActive(true);
                TimeSpan hm = DateTime.Now - walleditpricesfetched;
                if (hm.TotalSeconds >= 5 || flooreditprices == null || flooreditprices.Length <= 0)
                {
                    FloorCostText.text = "Loading...";
                    walleditpricesfetched = System.DateTime.Now;
                    DataStore.Get($"system_floorUpgradePrices", true, cb =>
                    {
                        if (cb != null)
                        {
                            flooreditprices = cb.Trim().Split('\n');
                            nextInitFloor(bw);
                        }
                        DataStore.Get(Shared.myMoneyKey(), true, mon =>
                        {
                            if (cb != null)
                            {
                                currmoney = int.Parse(mon.Trim());
                                SetFloorPrice();
                            }
                        });
                    });

                }
                else
                {
                    Debug.Log($"6:   {hm.TotalSeconds}");
                    nextInitFloor(bw);
                }

            }
        }
        void InitLight (PlacedLight li)
        {
            if (li != null && li.tag == "PlacedLight")
            {
                LightsO.SetActive(true);
                previewsO.SetActive(true);
                TimeSpan hm = DateTime.Now - walleditpricesfetched;
                if (hm.TotalSeconds >= 5 || lightprices == null || lightprices.Length <= 0)
                {
                    LightCostText.text = "Loading...";
                    walleditpricesfetched = System.DateTime.Now;
                    DataStore.Get($"system_lightUpgradePrices", true, cb =>
                    {
                        previewsO.SetActive(true);
                        if (cb != null)
                        {
                            lightprices = cb.Trim().Split('\n');
                            nextInitLight(li);
                        }
                        if (GameJoltAPI.Instance.HasSignedInUser)
                        {
                            DataStore.Get(Shared.myMoneyKey(), true, mon =>
                            {
                                if (cb != null)
                                {
                                    currmoney = int.Parse(mon.Trim());
                                    SetLightPrice();
                                }
                            });
                        }
                    });

                }
                else
                {
                    nextInitLight(li);
                }

            }
        }
        public void SetLightPrice()
        {
            if (selectedLight != null)
            {
                if (lightprices != null && lightprices.Length > 0)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser)
                    {
                        int finalprice = 0;
                        if (new Color(MathF.Round(selectedLight.LightColor.r, 2), MathF.Round(selectedLight.LightColor.g, 2), MathF.Round(selectedLight.LightColor.b, 2)) != new Color(MathF.Round(LightColor.currColor.r, 2), MathF.Round(LightColor.currColor.g, 2), MathF.Round(LightColor.currColor.b, 2)))
                        {
                            finalprice += int.Parse(lightprices[0].Trim().Split(':')[1].Trim());
                        }
                        if (System.MathF.Round(selectedLight.LightIntensity, 2) != System.MathF.Round(LightIntensity.value, 2))
                        {
                            finalprice += int.Parse(lightprices[1].Trim().Split(':')[1].Trim());
                        }
                        if (System.MathF.Round(selectedLight.Range, 2) != System.MathF.Round(LightRange.value, 2))
                        {
                            finalprice += int.Parse(lightprices[2].Trim().Split(':')[1].Trim());
                        }
                        LightCostText.text = $"<size=200%><nobr>You will pay</nobr></size>\n{finalprice}$\n<size=60%><nobr>(You have {currmoney}$)";
                    }
                    else
                    {
                        LightCostText.text = "DEMO";
                    }
                }
            }
        }
        public void SetFloorPrice()
        {
            if (selectedFloor != null)
            {
                if (flooreditprices != null && flooreditprices.Length > 0)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser)
                    {
                        int finalprice = 0;
                        if (selectedFloor.mainPattern != 1 && FloorPatternDropdown.value == 1)
                        {
                            
                            finalprice += int.Parse(flooreditprices[1].Trim().Split(':')[1].Trim());
                        }
                        else if (selectedFloor.mainPattern != 2 && FloorPatternDropdown.value == 2)
                        {
                            finalprice += int.Parse(flooreditprices[2].Trim().Split(':')[1].Trim());
                            if (selectedFloor.TilesColor != FloorTileColor.currColor)
                            {
                                finalprice += int.Parse(flooreditprices[3].Trim().Split(':')[1].Trim());
                            }
                        }
                        if (FloorPatternDropdown.value != 0)
                        {
                            if (new Color((float)Math.Round(selectedFloor.mainColor.r, 2), (float)Math.Round(selectedFloor.mainColor.g, 2), (float)Math.Round(selectedFloor.mainColor.b, 2), 1f) != FloorColor.currColor)
                            {
                                finalprice += int.Parse(flooreditprices[0].Trim().Split(':')[1].Trim());
                            }
                        }
                        else
                        {
                            FloorColor.RS.value = 1f;
                            FloorColor.GS.value = 1f;
                            FloorColor.BS.value = 1f;
                        }
                        finalprice = (int)(finalprice * (int)(selectedFloor.X_Scale * selectedFloor.Height * selectedFloor.Z_Scale));
                        FloorCostText.text = $"<size=200%><nobr>You will pay</nobr></size>\n{finalprice}$\n<size=60%><nobr>(You have {currmoney}$)";
                    }
                    else
                    {
                        FloorCostText.text = "DEMO";
                    }
                }
            }
        }
        void nextInitWall(BuiltWallMain bw)
        {
            if (bw != null && bw.tag == "Wall")
            {
                walleditsO.SetActive(true);
                selectedWall = bw;
                previewsO.SetActive(true);
                enabledStripe.isOn = bw.hasStripe;
                enabledTiles.isOn = bw.areTiles;
                previewWall.gameObject.SetActive(true);
                if (bw.hasStripe)
                {

                    StripeOutlineColor.RS.value = bw.stripeBorderColor.r;
                    StripeOutlineColor.GS.value = bw.stripeBorderColor.g;
                    StripeOutlineColor.BS.value = bw.stripeBorderColor.b;

                    StripeInsideColor.RS.value = bw.stripeColor.r;
                    StripeInsideColor.GS.value = bw.stripeColor.g;
                    StripeInsideColor.BS.value = bw.stripeColor.b;
                    if (bw.areTiles)
                    {
                        StripeTilesColor.RS.value = bw.TilesColor.r;
                        StripeTilesColor.GS.value = bw.TilesColor.g;
                        StripeTilesColor.BS.value = bw.TilesColor.b;
                    }
                }
                else
                {
                    StripeOutlineColor.RS.value = 0f;
                    StripeOutlineColor.GS.value = 0f;
                    StripeOutlineColor.BS.value = 0f;
                    StripeInsideColor.RS.value = 1f;
                    StripeInsideColor.GS.value = 1f;
                    StripeInsideColor.BS.value = 1f;
                    StripeTilesColor.RS.value = 0f;
                    StripeTilesColor.GS.value = 0f;
                    StripeTilesColor.BS.value = 0f;
                }
                StripeOffsetS.value = (float)Math.Round(bw.StripeOffset, 2);
                if (bw.mainPattern != 0)
                {
                    WallMainColor.gameObject.SetActive(true);
                    StripeTilesColor.gameObject.SetActive(true);
                    WallMainColor.RS.value = bw.mainColor.r;
                    WallMainColor.GS.value = bw.mainColor.g;
                    WallMainColor.BS.value = bw.mainColor.b;

                }
                else
                {
                    WallMainColor.gameObject.SetActive(true);
                    WallMainColor.RS.value = 1f;
                    WallMainColor.GS.value = 1f;
                    WallMainColor.BS.value = 1f;
                    WallMainColor.gameObject.SetActive(false);
                    PaintDropdown.value = 0;
                }
                PaintDropdown.value = bw.mainPattern;
                SetWallPrice();
            }
        }
        void nextInitFloor(BuiltWallMain bw)
        {
            if (bw != null && bw.tag == "Floor")
            {
                flooreditsO.SetActive(true);
                selectedFloor = bw;
                FloorPreview.gameObject.SetActive(true);
                if (bw.mainPattern != 0)
                {
                    FloorColor.gameObject.SetActive(true);
                    FloorColor.RS.value = bw.mainColor.r;
                    FloorColor.GS.value = bw.mainColor.g;
                    FloorColor.BS.value = bw.mainColor.b;
                    if (bw.mainPattern == 2)
                    {
                        FloorTileColor.gameObject.SetActive(true);
                        FloorTileColor.RS.value = bw.TilesColor.r;
                        FloorTileColor.GS.value = bw.TilesColor.g;
                        FloorTileColor.BS.value = bw.TilesColor.b;
                    }
                    else
                    {
                        FloorTileColor.gameObject.SetActive(false);
                        FloorTileColor.RS.value = 0f;
                        FloorTileColor.GS.value = 0f;
                        FloorTileColor.BS.value = 0f;
                    }

                }
                else
                {
                    FloorTileColor.gameObject.SetActive(false);

                    FloorTileColor.RS.value = 0f;
                    FloorTileColor.GS.value = 0f;
                    FloorTileColor.BS.value = 0f;
                }
                FloorPatternDropdown.value = bw.mainPattern;
                SetFloorPrice();
            }
        }
        void nextInitLight(PlacedLight li)
        {
            previewsO.SetActive(true);
            LightPreviewO.SetActive(true);
            if (li != null && li.tag == "PlacedLight")
            {
                LightEdits.SetActive(true);
                selectedLight = li;
                LightPreviewO.gameObject.SetActive(true);
                LightColor.gameObject.SetActive(true);
                LightColor.RS.value = li.LightColor.r;
                LightColor.GS.value = li.LightColor.g;
                LightColor.BS.value = li.LightColor.b;
                LightIntensity.value = MathF.Round(li.LightIntensity, 2);
                LightRange.value = MathF.Round(li.Range, 2);
                SetLightPrice();
            }
        }
        void notSelected()
        {
            SelectedO.SetActive(false);
            notSelectedO.SetActive(true);
        }
        public void ConfirmTopFill()
        {
            if (selectedTopSlot != null && isOn && selectedTop != PossibleToppings.Unknown && !isConfirming)
            {
                walleditsO.SetActive(false);
                isConfirming = true;
                SelectedToppingO.SetActive(false);
                GameJolt.API.Misc.GetTime(stime =>
                {
                    if (stime.Year >= 2022)
                    {
                        int hm = (int)FillAmountSlider.value;
                        Shared.StorageRemove((int)selectedTop, hm, succ =>
                        {
                            if (succ)
                            {
                                selectedTopSlot.SetWhat(selectedTop, hm, stime, cb => 
                                {
                                    if (cb)
                                    {
                                        SelectedToppingO.SetActive(false);
                                        isConfirming = false;
                                        if (isOn)
                                        {
                                            Switch();
                                        }
                                        Invoke("SaveNow", 0.05f);
                                    }
                                    else
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                    isConfirming = false;
                                    SelectedToppingO.SetActive(false);
                                    selectedTop = PossibleToppings.Unknown;
                                    selectedTopSlot = null;
                                });
                                
                            }
                            else
                            {
                                BuildingUI.Instance.ErrorS.Play();
                            }

                        });
                    }
                });
            }

        }
        public void SetWallPrice()
        {
            if (selectedWall != null)
            {
                if (walleditprices!=null && walleditprices.Length > 0)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser) { 
                    Debug.Log(1);
                    int finalprice = 0;
                    if (!selectedWall.hasStripe && enabledStripe.isOn)
                    {
                        finalprice += int.Parse(walleditprices[0].Trim().Split(':')[1].Trim());
                        if (selectedWall.stripeColor != StripeInsideColor.currColor)
                        {
                            Debug.Log(3);
                            finalprice += int.Parse(walleditprices[3].Trim().Split(':')[1].Trim());
                        }
                        if (selectedWall.stripeBorderColor != StripeOutlineColor.currColor)
                        {
                            Debug.Log(4);

                            finalprice += int.Parse(walleditprices[4].Trim().Split(':')[1].Trim());
                        }
                    }
                    if (!selectedWall.areTiles && enabledTiles.isOn)
                    {
                        
                        finalprice += int.Parse(walleditprices[1].Trim().Split(':')[1].Trim());
                        if (selectedWall.TilesColor != StripeTilesColor.currColor)
                        {
                            Debug.Log(5);
                            finalprice += int.Parse(walleditprices[5].Trim().Split(':')[1].Trim());
                        }
                    }
                    if (selectedWall.mainPattern != 1 && PaintDropdown.value == 1)
                    {
                        Debug.Log(6);
                        finalprice += int.Parse(walleditprices[2].Trim().Split(':')[1].Trim()); 
                    }
                    if (System.Math.Round(selectedWall.StripeOffset, 2) != System.Math.Round(StripeOffsetS.value, 2))
                    {
                        Debug.Log(7);
                        finalprice += int.Parse(walleditprices[7].Trim().Split(':')[1].Trim());
                    }
                    if (PaintDropdown.value != 0)
                    {
                        if (new Color((float)Math.Round(selectedWall.mainColor.r,2), (float)Math.Round(selectedWall.mainColor.g, 2), (float)Math.Round(selectedWall.mainColor.b, 2),1f) != WallMainColor.currColor)
                        {
                            Debug.Log(8);
                            finalprice += int.Parse(walleditprices[6].Trim().Split(':')[1].Trim());
                        }
                    }
                    else
                    {
                        WallMainColor.RS.value = 1f;
                        WallMainColor.GS.value = 1f;
                        WallMainColor.BS.value = 1f;
                    }
                    finalprice = (int)(finalprice * (int)(selectedWall.X_Scale * selectedWall.Height * selectedWall.Z_Scale));
                    WallCostText.text = $"<size=200%><nobr>You will pay</nobr></size>\n{finalprice}$\n<size=60%><nobr>(You have {currmoney}$)";
                    }
                    else
                    {
                        WallCostText.text = "DEMO";
                    }
                }
            }
        }
        public void ConfirmWallUpgrade()
        {
            if (selectedWall != null && isOn && !isConfirming)
            {
                isConfirming = true;
                BuiltWallMain editingwall = selectedWall;
                DataStore.Get($"system_wallUpgradePrices", true, cb =>
                {
                    if (cb != null)
                    {
                        string[] prices = cb.Trim().Split('\n');
                        int finalprice = 0;
                        if (!selectedWall.hasStripe && enabledStripe.isOn)
                        {
                            finalprice += int.Parse(walleditprices[0].Trim().Split(':')[1].Trim());
                            if (selectedWall.stripeColor != StripeInsideColor.currColor)
                            {
                                finalprice += int.Parse(walleditprices[3].Trim().Split(':')[1].Trim());
                            }
                            if (selectedWall.stripeBorderColor != StripeOutlineColor.currColor)
                            {
                                finalprice += int.Parse(walleditprices[4].Trim().Split(':')[1].Trim());
                            }
                        }
                        if (!selectedWall.areTiles && enabledTiles.isOn)
                        {
                            finalprice += int.Parse(walleditprices[1].Trim().Split(':')[1].Trim());
                            if (selectedWall.TilesColor != StripeTilesColor.currColor)
                            {
                                finalprice += int.Parse(walleditprices[5].Trim().Split(':')[1].Trim());
                            }
                        }
                        if (selectedWall.mainPattern != 1 && PaintDropdown.value == 1)
                        {
                            finalprice += int.Parse(walleditprices[2].Trim().Split(':')[1].Trim());
                        }
                        if (System.Math.Round(selectedWall.StripeOffset, 2) != System.Math.Round(StripeOffsetS.value, 2))
                        {
                            finalprice += int.Parse(walleditprices[7].Trim().Split(':')[1].Trim());
                        }
                        if (PaintDropdown.value != 0)
                        {
                            if (new Color((float)Math.Round(selectedWall.mainColor.r, 2), (float)Math.Round(selectedWall.mainColor.g, 2), (float)Math.Round(selectedWall.mainColor.b, 2), 1f) != WallMainColor.currColor)
                            {
                                finalprice += int.Parse(walleditprices[6].Trim().Split(':')[1].Trim());
                            }
                        }
                        else
                        {
                            WallMainColor.RS.value = 1f;
                            WallMainColor.GS.value = 1f;
                            WallMainColor.BS.value = 1f;
                        }
                        finalprice = (int)(finalprice * (int)(selectedWall.X_Scale * selectedWall.Height * selectedWall.Z_Scale));
                        if (finalprice > 0)
                        {
                            Shared.TakeMoney(finalprice, res =>
                            {
                                if (res == Shared.TakeMoneyResult.success)
                                {
                                    editingwall.mainPattern = PaintDropdown.value;
                                    if (PaintDropdown.value != 0)
                                    {
                                        editingwall.mainColor = WallMainColor.currColor;
                                    }
                                    else
                                    {
                                        editingwall.mainColor = Color.white;
                                    }
                                    editingwall.hasStripe = enabledStripe.isOn;
                                    editingwall.areTiles = enabledStripe.isOn && enabledTiles.isOn;
                                    if (enabledStripe.isOn)
                                    {
                                        editingwall.StripeOffset = (float)Math.Round(StripeOffsetS.value,2);
                                        editingwall.stripeBorderColor = StripeOutlineColor.currColor;
                                        editingwall.stripeColor = StripeInsideColor.currColor;
                                        if (enabledTiles.isOn)
                                        {
                                            editingwall.TilesColor = StripeTilesColor.currColor;
                                        }
                                        else
                                        {
                                            editingwall.TilesColor = Color.black;
                                        }
                                    }
                                    editingwall.Set();
                                    isConfirming = false;
                                    Trophies.TryUnlock(STRUCTURE_UPGRADE_TROPHY_ID);
                                    if (isOn)
                                    {
                                        Switch();
                                    }
                                    Invoke("SaveNow", 0.05f);
                                }
                                else
                                {
                                    if (BuildingUI.Instance!=null)
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                    walleditsO.SetActive(true);
                                }
                                isConfirming = false;
                            }, "Wall Upgrade");
                        }
                        else
                        {
                            editingwall.mainPattern = PaintDropdown.value;
                            if (PaintDropdown.value != 0)
                            {
                                editingwall.mainColor = WallMainColor.currColor;
                            }
                            else
                            {
                                editingwall.mainColor = Color.white;
                            }
                            editingwall.hasStripe = enabledStripe.isOn;
                            editingwall.areTiles = enabledStripe.isOn && enabledTiles.isOn;
                            if (enabledStripe.isOn)
                            {
                                editingwall.StripeOffset = (float)Math.Round(StripeOffsetS.value, 2);
                                editingwall.stripeBorderColor = StripeOutlineColor.currColor;
                                editingwall.stripeColor = StripeInsideColor.currColor;
                                if (enabledTiles.isOn)
                                {
                                    editingwall.TilesColor = StripeTilesColor.currColor;
                                }
                                else
                                {
                                    editingwall.TilesColor = Color.black;
                                }
                            }
                            editingwall.Set();
                            Invoke("SaveNow", 0.05f);
                            isConfirming = false;
                            if (isOn)
                            {
                                Switch();
                            }
                        }
                    }
                    else
                    {
                        isConfirming = false;
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.ErrorS.Play();
                        }
                        walleditsO.SetActive(true);
                    }
                });
            }
            else
            {
                if (BuildingUI.Instance != null)
                {
                    BuildingUI.Instance.ErrorS.Play();
                }
            }
        }
        public void ConfirmFloorUpgrade()
        {
            if (selectedFloor != null && isOn && !isConfirming)
            {
                isConfirming = true;
                BuiltWallMain editingFloor = selectedFloor;
                flooreditsO.SetActive(false);
                DataStore.Get($"system_floorUpgradePrices", true, cb =>
                {
                    if (cb != null)
                    {
                        string[] prices = cb.Trim().Split('\n');
                        int finalprice = 0;
                        if (selectedFloor.mainPattern != 1 && FloorPatternDropdown.value == 1)
                        {
                            finalprice += int.Parse(prices[1].Trim().Split(':')[1].Trim());
                        }
                        else if (selectedFloor.mainPattern != 2 && FloorPatternDropdown.value == 2)
                        {
                            finalprice += int.Parse(prices[2].Trim().Split(':')[1].Trim());
                            if (selectedFloor.TilesColor != FloorTileColor.currColor)
                            {
                                finalprice += int.Parse(prices[3].Trim().Split(':')[1].Trim());
                            }
                        }
                        if (FloorPatternDropdown.value != 0)
                        {
                            if (new Color((float)Math.Round(selectedFloor.mainColor.r, 2), (float)Math.Round(selectedFloor.mainColor.g, 2), (float)Math.Round(selectedFloor.mainColor.b, 2), 1f) != FloorColor.currColor)
                            {
                                finalprice += int.Parse(prices[0].Trim().Split(':')[1].Trim());
                            }
                        }
                        else
                        {
                            FloorColor.RS.value = 1f;
                            FloorColor.GS.value = 1f;
                            FloorColor.BS.value = 1f;
                        }
                        finalprice = (int)(finalprice * (int)(selectedFloor.X_Scale * selectedFloor.Height * selectedFloor.Z_Scale));
                        if (finalprice > 0)
                        {
                            Shared.TakeMoney(finalprice, res =>
                            {
                                if (res == Shared.TakeMoneyResult.success)
                                {
                                    editingFloor.mainPattern = FloorPatternDropdown.value;
                                    if (FloorPatternDropdown.value != 0)
                                    {
                                        editingFloor.mainColor = FloorColor.currColor;
                                        if (FloorPatternDropdown.value == 2)
                                        {
                                            editingFloor.TilesColor = FloorTileColor.currColor;
                                        }
                                    }
                                    else
                                    {
                                        editingFloor.mainColor = Color.white;
                                    }
                                    Trophies.TryUnlock(STRUCTURE_UPGRADE_TROPHY_ID);
                                    editingFloor.Set();
                                    isConfirming = false;
                                    if (isOn)
                                    {
                                        Switch();
                                    }
                                    Invoke("SaveNow", 0.05f);
                                }
                                else
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                    flooreditsO.SetActive(true);
                                }
                                isConfirming = false;
                            }, "Floor Upgrade");
                        }
                        else
                        {
                            editingFloor.mainPattern = FloorPatternDropdown.value;
                            if (FloorPatternDropdown.value != 0)
                            {
                                editingFloor.mainColor = FloorColor.currColor;
                                if (FloorPatternDropdown.value == 2)
                                {
                                    editingFloor.TilesColor = FloorTileColor.currColor;
                                }
                            }
                            else
                            {
                                editingFloor.mainColor = Color.white;
                            }

                            editingFloor.Set();
                            isConfirming = false;
                            if (isOn)
                            {
                                Switch();
                            }
                            Invoke("SaveNow", 0.05f);
                        }
                    }
                    else
                    {
                        isConfirming = false;
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.ErrorS.Play();
                        }
                        flooreditsO.SetActive(true);
                    }
                });
            }
            else
            {
                if (BuildingUI.Instance != null)
                {
                    BuildingUI.Instance.ErrorS.Play();
                }
            }
        }
        public void ConfirmLightUpgrade()
        {
            if (selectedLight != null && isOn && !isConfirming)
            {
                isConfirming = true;
                PlacedLight editingLight = selectedLight;
                LightEdits.SetActive(false);
                DataStore.Get($"system_lightUpgradePrices", true, cb =>
                {
                    if (cb != null)
                    {
                        string[] prices = cb.Trim().Split('\n');
                        int finalprice = 0;
                        if (new Color(LightColor.RS.value, LightColor.GS.value, LightColor.BS.value) != new Color(LightColor.currColor.r, LightColor.currColor.g, LightColor.currColor.b))
                        {
                            finalprice += int.Parse(lightprices[0].Trim().Split(':')[1].Trim());
                        }
                        if (System.MathF.Round(selectedLight.LightIntensity, 2) != System.MathF.Round(LightIntensity.value, 2))
                        {
                            finalprice += int.Parse(lightprices[1].Trim().Split(':')[1].Trim());
                        }
                        if (System.MathF.Round(selectedLight.Range, 2) != System.MathF.Round(LightRange.value, 2))
                        {
                            finalprice += int.Parse(lightprices[2].Trim().Split(':')[1].Trim());
                        }
                        if (finalprice > 0)
                        {
                            Shared.TakeMoney(finalprice, res =>
                            {
                                if (res == Shared.TakeMoneyResult.success)
                                {
                                    editingLight.LightColor = LightColor.currColor;
                                    editingLight.myColors[0] = LightColor.currColor;
                                    editingLight.LightIntensity = MathF.Round(LightIntensity.value,2);
                                    editingLight.Range = MathF.Round(LightRange.value,2);

                                    editingLight.Set();
                                    isConfirming = false;
                                    if (isOn)
                                    {
                                        Switch();
                                    }
                                    Invoke("SaveNow", 0.05f);
                                }
                                else
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.ErrorS.Play();
                                    }
                                    LightEdits.SetActive(true);
                                }
                                isConfirming = false;
                            }, "Light Upgrade");
                        }
                        else
                        {
                            editingLight.LightColor = LightColor.currColor;
                            editingLight.myColors[0] = LightColor.currColor;
                            editingLight.LightIntensity = MathF.Round(LightIntensity.value, 2);
                            editingLight.Range = MathF.Round(LightRange.value, 2);

                            editingLight.Set();
                            isConfirming = false;
                            if (isOn)
                            {
                                Switch();
                            }
                            Invoke("SaveNow", 0.05f);
                        }
                    }
                    else
                    {
                        isConfirming = false;
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.ErrorS.Play();
                        }
                        LightEdits.SetActive(true);
                    }
                });
            }
            else
            {
                if (BuildingUI.Instance != null)
                {
                    BuildingUI.Instance.ErrorS.Play();
                }
            }
        }
        void SaveNow()
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
            {
                if (SaveSystem.Instance != null)
                {
                    SaveSystem.Instance.SaveAll();
                }
            }
        }
        public void BuyMoreToppings()
        {
            if (!isConfirming)
            {
                isConfirming = true;
                DataStore.Get("system_toppingInfo", true, res => 
                {
                    if (res!=null)
                    {
                        topPrices.Clear();
                        string[] pr = res.Trim().Split('\n');
                        foreach(string p in pr)
                        {
                            string[] args = p.Split(':');
                            if (args.Length>1)
                            {
                                topPrices.Add(int.Parse(args[1].Trim()));
                            }
                        }
                        if (topPrices.Count > 0)
                        {
                            ToppingsO.SetActive(false);
                            buyTopO.SetActive(true);
                            SelectedBuyTopO.SetActive(false);
                        }
                        else
                        {
                            BuildingUI.Instance.ErrorS.Play();
                        }
                    }
                    else
                    {
                        BuildingUI.Instance.ErrorS.Play();
                    }
                    isConfirming = false;
                });
            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        public void ClickToppingBuyList(int i)
        {
            if (MainUI.Instance != null) {
                if (i != 0)
                {
                    if (topPrices.Count == Foundtops.Count && topPrices.Count > 0)
                    {
                        int max = 256 - Foundtops[i].amount;
                        if (max > 0)
                        {
                            SelectedBuyTopO.SetActive(true);
                            BuyingTopping = (PossibleToppings)i;
                            BuyTopSlider.minValue = 1;
                            BuyTopSlider.maxValue = max;
                            Debug.Log($"{(PossibleToppings)(Foundtops[i].index)}");
                            BuyTopText.text = $"You can only buy {BuyTopSlider.maxValue} of selected toppings";
                            BuyTopHeader.text = $"{((PossibleToppings)i).ToString().Replace('_', ' ')}";
                        }
                        else
                        {
                            SelectedBuyTopO.SetActive(false) ;
                            BuyingTopping = PossibleToppings.Unknown;
                        }

                    }
                }
            }
        }
        public void ConfirmBuyTop()
        {
            if (!isConfirming)
            {
                int am = (int)BuyTopSlider.value;
                if (am > 0)
                {
                    Debug.Log(1);
                    if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0)
                    {
                        Debug.Log(2);
                        if (BuyingTopping != PossibleToppings.Unknown)
                        {
                            Debug.Log(3);
                            int id = (int)BuyingTopping;
                            Shared.CheckToppingPrice(id, pr =>
                            {
                                Debug.Log(4);
                                if (pr > 0)
                                {
                                    Debug.Log(5);
                                    pr = pr * Mathf.Clamp(am, 1, 255);
                                    Shared.TakeMoney(pr, takeres =>
                                    {
                                        Debug.Log(6);
                                        if (takeres == Shared.TakeMoneyResult.success)
                                        {
                                            Debug.Log(7);
                                            Shared.StorageAdd(id, am, didAdd =>
                                            {
                                                Debug.Log(8);
                                                if (didAdd)
                                                {
                                                    Debug.Log(9);
                                                    isConfirming = false;
                                                    TopBuyBack();
                                                }
                                            });
                                        }
                                    }, "Buying Toppings");
                                }
                                else
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            });
                        }
                    }
                    else
                    {
                        BuyTopText2.text = "GameJolt Userdata ERROR.";
                        BuildingUI.Instance.ErrorS.Play();
                    }
                }
                else
                {
                    BuildingUI.Instance.ErrorS.Play();
                }
            }
        }
        public void TopBuyBack()
        {
            Init();
        }
    }
}