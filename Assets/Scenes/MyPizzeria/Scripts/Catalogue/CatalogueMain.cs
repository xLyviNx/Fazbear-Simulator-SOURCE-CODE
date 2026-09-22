using LyviN.Input;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameJolt.API;
    namespace LyviN.Player.Build
{
    public class CatalogueMain : MonoBehaviour
    {
        public static CatalogueMain Instance;
        public bool isON;
        public GameObject ContentO;
        public Button[] CategoriesButtons;
        
        [Header("Item")]
        public string SelectedItem;
        public TextMeshProUGUI ItemNameT;
        public TextMeshProUGUI ItemStatsT;
        public TextMeshProUGUI ItemPriceT;
        public GameObject ListsO;
        public GameObject PreviewImgO;
        public GameObject ChooseButtonO;
        public GameObject PropertiesO;
        public float CurrPrice;
        [Header("ItemPreviews")]
        public GameObject mainPreviewO;
        public GameObject[] FloorPreviewO;
        public GameObject[] WallsPreviewO;
        public GameObject[] PropsPreviewO;
        public GameObject[] LightsPreviewO;
        public GameObject[] DoorsPreviewO;
        public GameObject[] anmPreviewO;
        [Header("Properties")]
        public Toggle xSnap;
        public Toggle ySnap;
        public Toggle zSnap;
        public Toggle showGrid;
        public Toggle showMeasure;
        public Toggle showHeight;
        public Slider HeightScrollSensS;
        public TextMeshProUGUI HeightScrollSensT;
        public float HeightScrollSens;
        public enum Categories
        {
            None,
            Floor,
            Walls,
            Props,
            Kitchen,
            Lights,
            Doors,
            Animatronics,
            Customize,
        }
        [Header("CAT")]
        public Categories LastCategory;
        public Categories Category;
        public GameObject[] CategoriesLists;
        public GameObject CategoriesO;
        public GameObject inPreviewO;
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int CATALOGUE_OPEN_TROPHY_ID = 165219;
        [Header("Customize")]
        public GameObject CustomizeO;
        public GameObject customizeColorPrefab;
        public GameObject customizeParent;
        public List<CustomizeColorEntry> customizeColors = new List<CustomizeColorEntry>();
        public TextMeshProUGUI customizeButtonText;
        public bool isEmployeesOnly;
        public Toggle EmployeesOnlyToggle;
        public Toggle Light_Shadows;
        public Toggle Light_Terrain;
        public Slider Light_Intensity;
        public Slider Light_Range;       
        public GameObject Light_IntensityO;
        public GameObject Light_RangeO;
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            ShowPrev();
            Category = Categories.None;
        }
        public void SwitchCategory(string w)
        {
            if (w == "walls")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = false;

                Category = Categories.Walls;
            }
            else if (w == "floor")
            {
                Category = Categories.Floor;
            }
            else if (w == "props")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = false;
                Category = Categories.Props;
            }
            else if (w == "kitchen")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = false;
                Category = Categories.Kitchen;
            }
            else if (w == "lamps")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = false;
                Category = Categories.Lights;
            }
            else if (w == "doors")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = true;
                Category = Categories.Doors;
            }
            else if (w == "animatronics")
            {
                WallBuilding.Instance.willEmp = false;
                EmployeesOnlyToggle.isOn = false;
                Category = Categories.Animatronics;
            }
            else if (w == "customize")
            {
                if (Category != Categories.Customize)
                {
                    LastCategory = Category;
                    Category = Categories.Customize;
                }
                else
                {
                    Category = LastCategory;
                    ApplyCategory();    
                    ShowPrev();
                }
            }
            else
            {
                Category = Categories.None;
            }
            ApplyCategory();
        }
        void ApplyCategory()
        {
            foreach (GameObject x in CategoriesLists)
            {
                x.SetActive(false);
            }
            if (Category == Categories.Floor)
            {
                CategoriesButtons[0].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[0].SetActive(true);
            }
            else
            {
                CategoriesLists[0].SetActive(false);
                CategoriesButtons[0].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Walls)
            {
                CategoriesButtons[1].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[1].SetActive(true);
            }
            else
            {
                CategoriesLists[1].SetActive(false);
                CategoriesButtons[1].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Props)
            {
                CategoriesButtons[2].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[2].SetActive(true);
            }
            else
            {
                CategoriesLists[2].SetActive(false);
                CategoriesButtons[2].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Kitchen)
            {
                CategoriesButtons[3].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[3].SetActive(true);
            }
            else
            {
                CategoriesLists[3].SetActive(false);
                CategoriesButtons[3].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Lights)
            {
                CategoriesButtons[4].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[4].SetActive(true);
            }
            else
            {
                CategoriesLists[4].SetActive(false);
                CategoriesButtons[4].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Doors)
            {
                CategoriesButtons[5].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[5].SetActive(true);
            }
            else
            {
                CategoriesLists[5].SetActive(false);
                CategoriesButtons[5].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Animatronics)
            {
                CategoriesButtons[6].transform.localScale = Vector3.one * 1.1f;
                CategoriesLists[6].SetActive(true);
            }
            else
            {
                CategoriesLists[6].SetActive(false);
                CategoriesButtons[6].transform.localScale = Vector3.one * 1f;
            }
            if (Category == Categories.Customize)
            {
                ShowPrev();
                customizeButtonText.text = "Details";
            }
            else
            {
                customizeButtonText.text = "Customize";
            }

        }
        void Update()
        {
            HeightScrollSens = (float)System.Math.Round(HeightScrollSensS.value, 2);
            HeightScrollSensT.text = $"Height Sensitivity: {HeightScrollSens}";
            HeightScrollSens = Mathf.Clamp(HeightScrollSens, 0.01f, 5f);
            
            if (Crosshair3D.Instance != null)
            {
                Crosshair3D.Instance.xSnap = xSnap.isOn;
                Crosshair3D.Instance.ySnap = ySnap.isOn;
                Crosshair3D.Instance.zSnap = zSnap.isOn;
            }
           
            if (Binds.Instance != null && ((CursorManager.Instance != null && CursorManager.Instance.isLocked()) || isON))
            {
                if (Binds.Press(Binds.Instance.Building[8]))
                {
                    SwitchCAT();
                }
            }
            ContentO.SetActive(isON);
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isCat = isON;
            }
        }
        void SwitchCAT()
        {
            if (BuildingUI.Instance != null)
            {
                BuildingUI.Instance.SwitchPosS.Play();
            }
            isON = !isON;
            if (BuildManager.Instance != null)
            {
                if (isON)
                {
                    WallBuilding.Instance.willEmp = false;
                    EmployeesOnlyToggle.isOn = false;
                    Trophies.TryUnlock(CATALOGUE_OPEN_TROPHY_ID, cb => { });
                    BuildManager.Instance.Show(false);
                }
                else if (BuildManager.Instance.LastBuilding != BuildingType.None)
                {
                    BuildManager.Instance.Show(true);
                }
            }
        }
        void ShowPrev()
        {
            if (isON && SelectedItem != "")
            {

                if (Category != Categories.Customize)
                {
                    Light_Shadows.gameObject.SetActive(false);
                    Light_Terrain.gameObject.SetActive(false);
                    EmployeesOnlyToggle.gameObject.SetActive(false);
                    Light_IntensityO.gameObject.SetActive(false);
                    Light_RangeO.gameObject.SetActive(false);
                    CustomizeO.SetActive(false);
                    PropertiesO.SetActive(false);
                    mainPreviewO.SetActive(true);
                    PreviewImgO.SetActive(true);
                    ListsO.SetActive(false);
                    ChooseButtonO.SetActive(true);
                    CategoriesO.SetActive(false);
                    inPreviewO.SetActive(true);
                    foreach (Button b in CategoriesButtons)
                    {
                        b.gameObject.SetActive(false);
                    }
                    foreach (GameObject w in WallsPreviewO)
                    {
                        w.SetActive(false);
                    }
                    foreach (GameObject f in FloorPreviewO)
                    {
                        f.SetActive(false);
                    }
                    foreach (GameObject w in PropsPreviewO)
                    {
                        w.SetActive(false);
                    }
                    foreach (GameObject w in LightsPreviewO)
                    {
                        w.SetActive(false);
                    }
                    foreach (GameObject w in DoorsPreviewO)
                    {
                        w.SetActive(false);
                    }
                    foreach (GameObject w in anmPreviewO)
                    {
                        w.SetActive(false);
                    }
                    if (!SelectedItem.ToLower().Contains("floor"))
                    {
                        WallBuilding.Instance.willEmp = false;
                        EmployeesOnlyToggle.isOn = false;
                    }
                    switch (SelectedItem)
                    {
                        case "concretefloor":
                            {
                                ItemNameT.text = "Concrete Floor";
                                ItemStatsT.text = $"<size=70%>Basic Floor/ceiling.</size>\n" +
                                    $"<size=50%>Can be upgraded</size>\n" +
                                    $"<size=50%>Is it employees only zone? Check out the Customize Section.</size>\n" +
                                    "<size=50%>No stats yet.";
                                foreach (GameObject f in FloorPreviewO)
                                {
                                    if (f != FloorPreviewO[0])
                                    {
                                        f.SetActive(false);
                                    }
                                    else
                                    {
                                        f.SetActive(true);
                                    }
                                }
                                break;
                            }
                        case "aacblockswall":
                            {
                                ItemNameT.text = "AAC Blocks Wall";
                                ItemStatsT.text = $"<size=60%>Basic Wall made of AAC Blocks.</size>\n" +
                                    $"<size=50%>Can be decorated.</size>\n" +
                                    "<size=50%>No stats yet.";
                                foreach (GameObject w in WallsPreviewO)
                                {
                                    if (w != WallsPreviewO[0])
                                    {
                                        w.SetActive(false);
                                    }
                                    else
                                    {
                                        w.SetActive(true);
                                    }
                                }
                                break;
                            }
                        case "prop_table1":
                            {
                                ItemNameT.text = "Dining Area Table";
                                ItemStatsT.text = $"<size=60%>A Table. What else do you need to know?</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[0].SetActive(true);
                                break;
                            }
                        case "prop_chair1":
                            {
                                ItemNameT.text = "Ugly Stool";
                                ItemStatsT.text = $"<size=60%>A Stool. It's ugly.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[1].SetActive(true);
                                break;
                            }
                        case "prop_chair2_s1":
                            {
                                ItemNameT.text = "Wood Chair (Variant 1)";
                                ItemStatsT.text = $"<size=60%>A chair made of wood. Only. Wood.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[2].SetActive(true);
                                break;
                            }
                        case "prop_chair2_s2":
                            {
                                ItemNameT.text = "Wood Chair (Variant 2)";
                                ItemStatsT.text = $"<size=60%>A chair made of wood. Only. Wood.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[3].SetActive(true);
                                break;
                            }
                        case "prop_chair3_s1":
                            {
                                ItemNameT.text = "Classic Chair (Variant 1)";
                                ItemStatsT.text = $"<size=60%>A classic chair, not the best, but also not the worst.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[4].SetActive(true);
                                break;
                            }
                        case "prop_chair3_s2":
                            {
                                ItemNameT.text = "Classic Chair (Variant 2)";
                                ItemStatsT.text = $"<size=60%>A classic chair, not the best, but also not the worst.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[5].SetActive(true);
                                break;
                            }
                        case "prop_chair4_s1":
                            {
                                ItemNameT.text = "Weird Metal & Wood Chair (Variant 1)";
                                ItemStatsT.text = $"<size=60%>It really is weird.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[6].SetActive(true);
                                break;
                            }
                        case "prop_chair4_s2":
                            {
                                ItemNameT.text = "Weird Metal & Wood Chair (Variant 2)";
                                ItemStatsT.text = $"<size=60%>It really is weird.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[7].SetActive(true);
                                break;
                            }
                        case "prop_chair5_s1":
                            {
                                ItemNameT.text = "Metal Chair (Variant 1)";
                                ItemStatsT.text = $"<size=60%>That's a decent-looking chair guys.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[8].SetActive(true);
                                break;
                            }
                        case "prop_chair5_s2":
                            {
                                ItemNameT.text = "Metal Chair (Variant 2)";
                                ItemStatsT.text = $"<size=60%>That's a decent-looking chair guys.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[9].SetActive(true);
                                break;
                            }
                        case "prop_chair6":
                            {
                                ItemNameT.text = "Cool Stool";
                                ItemStatsT.text = $"<size=60%>Wow, it's indeed cool!</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[10].SetActive(true);
                                break;
                            }
                        case "prop_chair7":
                            {
                                ItemNameT.text = "Fancy Chair";
                                ItemStatsT.text = $"<size=60%>Looks epic</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[11].SetActive(true);
                                break;
                            }
                        case "prop_chair8":
                            {
                                ItemNameT.text = "Leather Chair";
                                ItemStatsT.text = $"<size=60%>It's ok.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[12].SetActive(true);
                                break;
                            }
                        case "prop_counter1":
                            {
                                ItemNameT.text = "Wooden Counter";
                                ItemStatsT.text = $"<size=60%>Sounds like income.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[13].SetActive(true);
                                break;
                            }
                        case "prop_counter1b":
                            {
                                ItemNameT.text = "Wooden Counter (Corner)";
                                ItemStatsT.text = $"<size=60%>Sounds like income.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[14].SetActive(true);
                                break;
                            }
                        case "prop_cashregister1":
                            {
                                ItemNameT.text = "Cash Register";
                                ItemStatsT.text = $"<size=60%>Allows you to sell stuff. :)\nNeeds to be placed on a Counter</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[15].SetActive(true);
                                break;
                            }
                        case "prop_kitchentable1":
                            {
                                ItemNameT.text = "Basic Kitchen Table";
                                ItemStatsT.text = $"<size=60%>Useful if you want to make Pizza...</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[16].SetActive(true);
                                break;
                            }
                        case "prop_kitchendoughpot1":
                            {
                                ItemNameT.text = "Basic Kitchen Dough Pot";
                                ItemStatsT.text = $"<size=60%>Useful if you want to make Pizza...</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[17].SetActive(true);
                                break;
                            }
                        case "prop_kitchentoppings":
                            {
                                ItemNameT.text = "Pizza Toppings Shelf";
                                ItemStatsT.text = $"<size=60%>Useful if you want to make Pizza...</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[18].SetActive(true);
                                break;
                            }
                        case "lamp_classicbulb":
                            {
                                ItemNameT.text = "Classic Lamp";
                                ItemStatsT.text = $"<size=60%></size>" +
                                    "<size=50%>No stats yet.";
                                LightsPreviewO[0].SetActive(true);
                                break;
                            }
                        case "lamp_ceilingflat":
                            {
                                ItemNameT.text = "Ceiling Lamp";
                                ItemStatsT.text = $"<size=60%></size>" +
                                    "<size=50%>No stats yet.";
                                LightsPreviewO[1].SetActive(true);
                                break;
                            }
                        case "lamp_wfnaf":
                            {
                                ItemNameT.text = "Fancy Wall Lamp";
                                ItemStatsT.text = $"<size=60%></size>" +
                                    "<size=50%>No stats yet.";
                                LightsPreviewO[2].SetActive(true);
                                break;
                            }
                        case "door_door1":
                            {
                                ItemNameT.text = "Door";
                                ItemStatsT.text = $"<size=60%>Just a door.</size>\n" +
                                    "<size=50%>No stats yet.";
                                DoorsPreviewO[0].SetActive(true);
                                break;
                            }
                        case "prop_kitchenoven1":
                            {
                                ItemNameT.text = "An Electric Pizza Oven";
                                ItemStatsT.text = $"<size=60%>Pizza time.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[19].SetActive(true);
                                break;
                            }
                        case "prop_kitchenoven2":
                            {
                                ItemNameT.text = "A Basic Electric Pizza Oven";
                                ItemStatsT.text = $"<size=60%>Pizza time.\nAllows you to set the timer up to 8 minutes.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[20].SetActive(true);
                                break;
                            }
                        case "prop_kitchenoven3":
                            {
                                ItemNameT.text = "An Advanced Electric Pizza Oven";
                                ItemStatsT.text = $"<size=60%>Now with the LCD Display!.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[21].SetActive(true);
                                break;
                            }
                        case "prop_kitchencupboard1":
                            {
                                ItemNameT.text = "A cupboard";
                                ItemStatsT.text = $"<size=60%>Just a cupboard.</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[22].SetActive(true);
                                break;
                            }
                        case "prop_kitchenmetaltable":
                            {
                                ItemNameT.text = "A Metal Table";
                                ItemStatsT.text = $"<size=60%>Useful for placing Ovens</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[23].SetActive(true);
                                break;
                            }          
                        case "prop_kitchenplateshelf":
                            {
                                ItemNameT.text = "A Plate Shelf";
                                ItemStatsT.text = $"<size=60%>Who needs some plates?</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[24].SetActive(true);
                                break;
                            }
                        case "prop_arcademachine1":
                            {
                                ItemNameT.text = "An Arcade Machine";
                                ItemStatsT.text = $"<size=60%>Kid's best friend.</size>\n" +
                                    "<size=50%>Gives 30$ each day (when the night begins, if your Pizzeria was opened)" +
                                    "\nNo stats yet.";
                                PropsPreviewO[25].SetActive(true);
                                break;
                            }
                        case "prop_balloons1":
                            {
                                ItemNameT.text = "Balloons";
                                ItemStatsT.text = $"<size=60%>Wow!</size>\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[26].SetActive(true);
                                break;
                            }
                        case "prop_roundstage1":
                            {
                                ItemNameT.text = "A Round Stage";
                                ItemStatsT.text = $"" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[27].SetActive(true);
                                break;
                            }
                        case "prop_bigstage1":
                            {
                                ItemNameT.text = "A Big Stage";
                                ItemStatsT.text = $"" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[28].SetActive(true);
                                break;
                            }
                        case "anm_freddy1":
                            {
                                ItemNameT.text = "Freddy Fazbear";
                                ItemStatsT.text = $"<size=60%>The one... the only!\n" +
                                    "<size=50%>No stats yet.";
                                anmPreviewO[0].SetActive(true);
                                break;
                            }
                        case "prop_table2":
                            {
                                ItemNameT.text = "A Wooden Table";
                                ItemStatsT.text = $"Cheap. It's all that matters.\n" +
                                    "<size=50%>No stats yet.";
                                PropsPreviewO[29].SetActive(true);
                                break;
                            }
                        case "anm_bonnie1":
                            {
                                ItemNameT.text = "Bonnie the Bunny";
                                ItemStatsT.text = $"<size=60%>" +
                                    "<size=50%>No stats yet.";
                                anmPreviewO[1].SetActive(true);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    if (SelectedItem.Contains("wall") || SelectedItem.Contains("floor"))
                    {
                        if (CatalogueGameJolt.Instance != null)
                        {
                            if (CatalogueGameJolt.Instance.structureprices.Length > 0)
                            {
                                bool found = false;
                                foreach (string c in CatalogueGameJolt.Instance.structureprices)
                                {
                                    string[] arg = c.Trim().Split(':');
                                    if (arg[0].Trim() == SelectedItem.Trim())
                                    {
                                        string ad = "";

                                        ad = "<size=70%>/m<sup>3</sup></size>";

                                        CurrPrice = (float)float.Parse(arg[1].Trim());
                                        ItemPriceT.text = $"{CurrPrice} ${ad}";
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    ItemPriceT.text = $"Unknown Price";
                                }
                            }
                            else
                            {
                                ItemPriceT.text = $"Loading Price...";
                                Invoke("ShowPrev", 3f);
                            }
                        }
                        else
                        {
                            ItemPriceT.text = $"GameJolt Integration Error";
                        }
                    }
                    else if (SelectedItem.Contains("prop_"))
                    {
                        if (CatalogueGameJolt.Instance != null)
                        {
                            if (CatalogueGameJolt.Instance.propprices.Length > 0)
                            {
                                bool found = false;
                                foreach (string c in CatalogueGameJolt.Instance.propprices)
                                {
                                    string[] arg = c.Trim().Split(':');
                                    if (arg[0].Trim() == SelectedItem.Trim())
                                    {

                                        CurrPrice = (float)float.Parse(arg[1].Trim());
                                        ItemPriceT.text = $"{CurrPrice} $";
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    ItemPriceT.text = $"Unknown Price";
                                }
                            }
                            else
                            {
                                ItemPriceT.text = $"Loading Price...";
                                Invoke("ShowPrev", 3f);
                            }
                        }
                        else
                        {
                            ItemPriceT.text = $"GameJolt Integration Error";
                        }
                    }
                    else if (SelectedItem.Contains("lamp_"))
                    {
                        Light_Shadows.gameObject.SetActive(true);
                        Light_Terrain.gameObject.SetActive(true);
                        Light_IntensityO.gameObject.SetActive(true);
                        Light_RangeO.gameObject.SetActive(true);
                        Light_Intensity.value = 5f;
                        Light_Range.value = 15f;
                        if (CatalogueGameJolt.Instance != null)
                        {
                            if (CatalogueGameJolt.Instance.lampprices.Length > 0)
                            {
                                bool found = false;
                                foreach (string c in CatalogueGameJolt.Instance.lampprices)
                                {
                                    string[] arg = c.Trim().Split(':');
                                    if (arg[0].Trim() == SelectedItem.Trim())
                                    {
                                        CurrPrice = (float)float.Parse(arg[1].Trim());
                                        ItemPriceT.text = $"{CurrPrice} $";
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    ItemPriceT.text = $"Unknown Price";
                                }
                            }
                            else
                            {
                                ItemPriceT.text = $"Loading Price...";
                                Invoke("ShowPrev", 3f);
                            }
                        }
                        else
                        {
                            ItemPriceT.text = $"GameJolt Integration Error";
                        }

                    }
                    else if (SelectedItem.Contains("door_"))
                    {
                        EmployeesOnlyToggle.gameObject.SetActive(true);
                        if (CatalogueGameJolt.Instance != null)
                        {
                            if (CatalogueGameJolt.Instance.doorprices.Length > 0)
                            {
                                bool found = false;
                                foreach (string c in CatalogueGameJolt.Instance.doorprices)
                                {
                                    string[] arg = c.Trim().Split(':');
                                    if (arg[0].Trim() == SelectedItem.Trim())
                                    {
                                        CurrPrice = (float)float.Parse(arg[1].Trim());
                                        ItemPriceT.text = $"{CurrPrice} $";
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    ItemPriceT.text = $"Unknown Price";
                                }
                            }
                            else
                            {
                                ItemPriceT.text = $"Loading Price...";
                                Invoke("ShowPrev", 3f);
                            }
                        }
                        else
                        {
                            ItemPriceT.text = $"GameJolt Integration Error";
                        }
                    }
                    else if (SelectedItem.Contains("anm_"))
                    {
                        if (CatalogueGameJolt.Instance != null)
                        {
                            if (CatalogueGameJolt.Instance.animatronicprices.Length > 0)
                            {
                                bool found = false;
                                foreach (string c in CatalogueGameJolt.Instance.animatronicprices)
                                {
                                    string[] arg = c.Trim().Split(':');
                                    if (arg[0].Trim() == SelectedItem.Trim())
                                    {
                                        Debug.Log(5);
                                        CurrPrice = (float)float.Parse(arg[1].Trim());
                                        ItemPriceT.text = $"{CurrPrice} $";
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    ItemPriceT.text = $"Unknown Price";
                                }
                            }
                            else
                            {
                                ItemPriceT.text = $"Loading Price...";
                                Invoke("ShowPrev", 3f);
                            }
                        }
                        else
                        {
                            ItemPriceT.text = $"GameJolt Integration Error";
                        }
                    }
                }
                else
                {
                    CustomizeO.SetActive(true);
                    ItemNameT.text = "Customize";
                    ItemStatsT.text = "";
                    Color[] setcolors = new Color[0];
                    foreach (CustomizeColorEntry cce in customizeColors)
                    {
                        Destroy(cce.gameObject);
                    }
                    customizeColors.Clear();
                    switch (SelectedItem)
                    {
                        case "prop_chair1":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f) };
                                break;
                            }
                        case "prop_chair2_s1":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f) };
                                break;
                            }
                        case "prop_chair2_s2":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f) };

                                break;
                            }
                        case "prop_chair3_s1":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f), new Color(0.49f, 0.61f, 0.79f) };

                                break;
                            }
                        case "prop_chair3_s2":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f), new Color(0.49f, 0.61f, 0.79f) };
                                break;
                            }
                        case "prop_chair4_s1":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f), new Color(0.47f, 0.47f, 0.47f) };

                                break;
                            }
                        case "prop_chair4_s2":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.57f, 0.52f), new Color(0.47f, 0.47f, 0.47f) };
                                break;
                            }
                        case "prop_chair5_s1":
                            {
                                setcolors = new Color[] { new Color(0.47f, 0.47f, 0.47f) };
                                break;
                            }
                        case "prop_chair5_s2":
                            {
                                setcolors = new Color[] { new Color(0.47f, 0.47f, 0.47f) };
                                break;
                            }
                        case "prop_chair6":
                            {
                                setcolors = new Color[] { new Color(0.31f, 0.31f, 0.31f), Color.black };
                                break;
                            }
                        case "prop_chair7":
                            {
                                setcolors = new Color[] { new Color(0.31f, 0.31f, 0.31f), Color.black, new Color(1f, 0.78f, 0f) };
                                break;
                            }
                        case "prop_chair8":
                            {
                                setcolors = new Color[] { new Color(0.31f, 0.31f, 0.31f), Color.black };
                                break;
                            }
                        case "prop_counter1":
                            {
                                setcolors = new Color[] { new Color(0.18f, 0.18f, 0.18f), Color.white, new Color(0.18f, 0.18f, 0.18f) };
                                break;
                            }
                        case "prop_counter1b":
                            {
                                setcolors = new Color[] { new Color(0.18f, 0.18f, 0.18f), Color.white, new Color(0.18f, 0.18f, 0.18f) };
                                break;
                            }
                        case "prop_cashregister1":
                            {
                                setcolors = new Color[] { Color.white };
                                break;
                            }
                        case "prop_kitchentable1":
                            {
                                setcolors = new Color[] { Color.white * 0.11f };
                                break;
                            }
                        case "prop_kitchendoughpot1":
                            {
                                setcolors = new Color[] { Color.white };
                                break;
                            }
                        case "prop_kitchentoppings":
                            {
                                setcolors = new Color[] { new Color(0.52f, 0.49f, 0.4f) };
                                break;
                            }
                        case "concretefloor":
                            {
                                break;
                            }
                        case "lamp_classicbulb":
                            {
                                setcolors = new Color[] { Color.white, new Color(0.3f, 0.3f, 0.3f) };
                                break;
                            }
                        case "lamp_ceilingflat":
                            {
                                setcolors = new Color[] { Color.white, Color.white };
                                break;
                            }
                        case "lamp_wfnaf":
                            {
                                setcolors = new Color[] { Color.white, new Color(0.5f, 0f, 1f), new Color(0.5f, 0f, 1f), new Color(0.5f, 0f, 1f), new Color(1f, 0.78f, 0f) };
                                break;
                            }
                        case "door_door1":
                            {
                                setcolors = new Color[] { CustomizeColorEntry.hexToColor("48773E"), Color.black, CustomizeColorEntry.hexToColor("E7B761"), Color.black };
                                break;
                            }
                        case "prop_kitchenoven1":
                            {
                                setcolors = new Color[] { Color.white * 0.8f, Color.white, Color.white, Color.black, new Color(1f,1f,0.92f)};
                                break;
                            }
                        case "prop_kitchenoven2":
                            {
                                setcolors = new Color[] { Color.white * 0.8f, Color.white, Color.white, Color.black, new Color(1f, 1f, 0.92f), new Color(1f, 1f, 0.92f) };
                                break;
                            }
                        case "prop_kitchenoven3":
                            {
                                setcolors = new Color[] { Color.white * 0.8f, Color.white * 0.72f, Color.white, Color.white, Color.black, new Color(1f, 1f, 0.92f)};
                                break;
                            }
                        case "prop_kitchencupboard1":
                            {
                                setcolors = new Color[] { Color.white * 0.5f, Color.white * 0.76f, Color.white * 0.76f, Color.white * 0.4f };
                                break;
                            }
                        case "prop_kitchenmetaltable":
                            {
                                setcolors = new Color[] { Color.white, Color.white };
                                break;
                            }         
                        case "prop_kitchenplateshelf":
                            {
                                setcolors = new Color[] { Color.white, Color.white };
                                break;
                            }
                        case "prop_arcademachine1":
                            {
                                setcolors = new Color[] { Color.white, Color.white * 0.8f };
                                break;
                            }
                        case "prop_balloons1":
                            {
                                setcolors = new Color[] { new Color(0.9f, 0.31f, 0.31f), new Color(0.9f, 0.31f, 0.31f), new Color(0.9f, 0.31f, 0.31f), new Color(0.9f, 0.31f, 0.31f), new Color(0.9f, 0.31f, 0.31f), Color.white };
                                break;
                            }
                        case "prop_roundstage1":
                            {
                                setcolors = new Color[] { new Color(1f, 0f, 0.93f), Color.white * 0.05f, Color.white * 0.05f };
                                break;
                            }
                        case "prop_bigstage1":
                            {
                                setcolors = new Color[] { new Color(1f, 0f, 0.93f), Color.white * 0.05f, Color.white * 0.05f };
                                break;
                            }
                        case "prop_table2":
                            {
                                setcolors = new Color[] { new Color(0.65f, 0.6f, 0.57f), new Color(0.65f, 0.6f, 0.57f) };
                                break;
                            }
                        default:
                            {
                                ItemStatsT.text = "This can't be customized";
                                break;
                            }
                    }
                    for (int i = 0; i < setcolors.Length; i++)
                    {
                        GameObject col = Instantiate(customizeColorPrefab, customizeParent.transform);
                        CustomizeColorEntry ce = col.GetComponent<CustomizeColorEntry>();
                        ce.RS.value = setcolors[i].r;
                        ce.GS.value = setcolors[i].g;
                        ce.BS.value = setcolors[i].b;
                        ce.isPropColor = true;
                        customizeColors.Add(ce);
                    }
                    if (SelectedItem.Trim().ToLower().Contains("floor"))
                    {
                        EmployeesOnlyToggle.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                CustomizeO.SetActive(false);
                if (WallBuilding.Instance != null)
                {
                    if (isON)
                    {
                        WallBuilding.Instance.willEmp = false;
                    }
                }
                EmployeesOnlyToggle.isOn = false;
                ItemNameT.text = "Properties";
                ItemStatsT.text = $"";
                ItemPriceT.text = $"";
                CategoriesO.SetActive(true);
                inPreviewO.SetActive(false);
                mainPreviewO.SetActive(false);
                ChooseButtonO.SetActive(false);
                PreviewImgO.SetActive(false);
                ListsO.SetActive(true);
                foreach (Button b in CategoriesButtons)
                {
                    b.gameObject.SetActive(true);
                }

                PropertiesO.SetActive(true);

            }
        }
        public void SelectPreview(string w)
        {
            if (isON)
            {
                SelectedItem = w;

                if (w == "" && LastCategory != Categories.None)
                {
                    Category = LastCategory;
                    ApplyCategory();
                }
                else
                {
                    foreach (CustomizeColorEntry cce in customizeColors)
                    {
                        Destroy(cce.gameObject);
                    }
                    customizeColors.Clear();
                    if (PropPlacing.Instance != null && PropPlacing.Instance.SelColors != null)
                    {
                        PropPlacing.Instance.SelColors.Clear();
                    }
                    if (LightPlacing.Instance != null && LightPlacing.Instance.SelColors != null)
                    {
                        LightPlacing.Instance.SelColors.Clear();
                    }
                    if (DoorPlacing.Instance != null && DoorPlacing.Instance.SelColors != null)
                    {
                        DoorPlacing.Instance.SelColors.Clear();
                    }
                }
                ShowPrev();
            }
        }
        public void SelectStructure()
        {
            if (BuildManager.Instance != null)
            {

                isON = false;
                BuildManager.Instance.currentlyBuilding = SelectedItem;
                if (SelectedItem.ToLower().Contains("floor"))
                {
                    BuildManager.Instance.Building = BuildingType.Floor;
                    if (WallBuilding.Instance != null)
                    {
                        WallBuilding.Instance.Height = 0.5f;
                        WallBuilding.Instance.willEmp = EmployeesOnlyToggle.isOn;
                        Debug.Log(EmployeesOnlyToggle.isOn);
                        EmployeesOnlyToggle.isOn = false;
                    }
                    
                }
                else if (SelectedItem.ToLower().Contains("wall"))
                {
                    BuildManager.Instance.Building = BuildingType.Walls;
                    if (WallBuilding.Instance != null)
                    {
                        WallBuilding.Instance.Height = 4f;
                        WallBuilding.Instance.willEmp = false;
                    }
                    //BuildManager.Instance.
                }
                else if (SelectedItem.ToLower().Contains("prop_"))
                {
                    BuildManager.Instance.Building = BuildingType.Props;
                    if (Crosshair3D.Instance != null)
                    {
                        Crosshair3D.Instance.SnapToGrid = false;
                    }
                    if (PropPlacing.Instance != null)
                    {
                        PropPlacing.Instance.SetPreview();
                    }
                }
                else if (SelectedItem.ToLower().Contains("lamp_"))
                {
                    BuildManager.Instance.Building = BuildingType.Lights;
                    if (Crosshair3D.Instance != null)
                    {
                        Crosshair3D.Instance.SnapToGrid = false;
                    }
                    if (LightPlacing.Instance != null)
                    {
                        LightPlacing.Instance.SetPreview();
                        foreach (CustomizeColorEntry ce in customizeColors)
                        {
                            LightPlacing.Instance.SelColors.Add(ce.currColor);
                        }
                        LightPlacing.Instance.currIntensity = Light_Intensity.value;
                        LightPlacing.Instance.currRange = Light_Range.value;
                        LightPlacing.Instance.Shadows = Light_Shadows.isOn;
                        LightPlacing.Instance.GroundLight = !Light_Terrain.isOn;
                    }
                }
                else if (SelectedItem.ToLower().Contains("door_"))
                {
                    BuildManager.Instance.Building = BuildingType.Doors;
                    if (Crosshair3D.Instance != null)
                    {
                        Crosshair3D.Instance.SnapToGrid = false;
                    }
                    if (DoorPlacing.Instance != null)
                    {
                        DoorPlacing.Instance.SetPreview();
                        foreach (CustomizeColorEntry ce in customizeColors)
                        {
                            DoorPlacing.Instance.SelColors.Add(ce.currColor);
                        }
                        DoorPlacing.Instance.empl = EmployeesOnlyToggle.isOn;
                    }
                }
                else if (SelectedItem.ToLower().Contains("anm_"))
                {
                    BuildManager.Instance.Building = BuildingType.Animatronics;
                    if (Crosshair3D.Instance != null)
                    {
                        Crosshair3D.Instance.SnapToGrid = false;
                    }
                    if (AnimatronicPlacing.Instance != null)
                    {
                        AnimatronicPlacing.Instance.SetPreview();
                    }
                }
                SelectedItem = "";
                Category = Categories.None;
                ApplyCategory();
                ShowPrev();

            }
        }
        public void SnapChange(string c)
        {
            if (Crosshair3D.Instance != null)
            {
                if (c == "x")
                {
                    Crosshair3D.Instance.xSnap = !Crosshair3D.Instance.xSnap;
                }
                else if (c=="y")
                {
                    Crosshair3D.Instance.ySnap = !Crosshair3D.Instance.ySnap;
                }
                else if (c=="z")
                {
                    Crosshair3D.Instance.ySnap = !Crosshair3D.Instance.zSnap;
                }
            }
        }
        public void GridSwitch()
        {
            if (BuildManager.Instance != null && BuildingUI.Instance!=null)
            {
                BuildManager.Instance.SwitchGrid();
            }
        }
        public void FinishSlidingSens()
        {
            HeightScrollSens = Mathf.Clamp(HeightScrollSensS.value, 0.01f, 5f);
        }
    }
}