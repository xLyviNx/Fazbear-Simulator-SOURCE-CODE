using LyviN.Input;
using LyviN.Player;
using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static LyviN.Player.Baking.Pizza;
using LyviN.Map;
using GameJolt.API;

namespace LyviN.UI
{
    public class PizzeriaManagementUI : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Ids
        private const int SAVE_MENU_TROPHY_ID = 169417;
        private const int CHANGE_OPENING_HOURS_TROPHY_ID = 169416; // Change opening hours trophy
        
        
        public static PizzeriaManagementUI Instance;
        public GameObject ContentO;
        public bool isOn;
        public string currMenu;
        public GameObject[] Menus;
        [Header("Menu")]
        public GameObject Menu_PizzaPrefab;
        public GameObject Menu_ToppingPrefab;
        [Space(10)]
        [NonReorderable] public List<Pizza> currentPizzas;
        public List<MenuPizzaEntry> currentPizzaObjects;
        public GameObject Menu_PizzeriaOpened;
        public GameObject Menu_PizzeriaNotOpened;
        public GameObject Menu_noPizzas;
        public Transform Menu_PizzasParent;
        public Transform Menu_ToppingsParent;
        public MenuPizzaEntry Menu_SelectedPizza;
        public PosTopping[] PosToppings;
        public TMP_InputField PizzaName;
        public TMP_InputField Cost;
        public GameObject Menu_ToppingsAvailable;
        public GameObject Menu_ToppingsCurr;
        public GameObject Menu_Info;
        public GameObject Menu_Confirm;
        public Button Menu_AddingButton;
        public Button Menu_RemoveButton;
        [Header("Opening Hours")]
        public Slider OH_Open;
        public Slider OH_Close;
        public GameObject OH_OpenedAlready;
        public GameObject OH_ClosedCurr;
        public TextMeshProUGUI ClosedText;
        public GameObject CloseButton;
        [Header("Info")]
        public GameObject InfoO;
        public TextMeshProUGUI RatingsT;
        [Header("Other")]
        public GameObject OtherO;
        public GameObject SkipNightButton;
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
        }

        // Update is called once per frame
        void Update()
        {
            if (Binds.Instance != null)
            {
                if (Binds.Press(Binds.Instance.General[4]))
                {
                    Switch();
                }
            }
            ContentO.SetActive(isOn);
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isPizzeriaManagement = isOn;
            }
            SkipNightButton.SetActive(currMenu == "Other" && TimeSystem.Instance != null && TimeSystem.Instance.isNight() && !TimeSystem.Instance.skipnight);
        }
        void Switch()
        {
            if (!Cost.isFocused && !PizzaName.isFocused)
            {
                if (CursorManager.Instance != null)
                {
                    if (CursorManager.Instance.isLocked() || isOn)
                    {
                        if (BuildingUI.Instance != null)
                        {
                            BuildingUI.Instance.SwitchPosS.Play();
                        }
                        isOn = !isOn;
                        Init();
                    }
                }
            }
        }
        public void ChangeMenu(string w)
        {
            currMenu = w;
            Init();
        }
        public void Init()
        {
            if (isOn)
            {
                foreach (GameObject m in Menus)
                {
                    m.SetActive(false);
                }
                if (ThePizzeria.Instance != null)
                {
                    switch (currMenu)
                    {
                        case "Hours":
                            {
                                OH_OpenedAlready.SetActive(false);
                                OH_ClosedCurr.SetActive(false);
                                Menus[0].SetActive(true);
                                CloseButton.SetActive(false);
                                ClosedText.text = "";
                                if (ThePizzeria.Instance.isOpened())
                                {
                                    OH_OpenedAlready.SetActive(true);
                                }
                                else
                                {
                                    OH_ClosedCurr.SetActive(true);
                                    OH_Open.value = ThePizzeria.Instance.OpenHour;
                                    OH_Open.GetComponent<ChangeOnValueChange>().ChangeSliderH();
                                    OH_Close.GetComponent<ChangeOnValueChange>().ChangeSliderH();
                                    OH_Close.value = ThePizzeria.Instance.ClosingHour;
                                    if (ThePizzeria.Instance.OpenHour < 6 || ThePizzeria.Instance.ClosingHour < 7 || ThePizzeria.Instance.ClosingHour == ThePizzeria.Instance.OpenHour)
                                    {
                                        ClosedText.text = "Your Pizzeria is currently closed. You must set Opening Hours in order to make it eventually open.";
                                    }
                                    else
                                    {
                                        CloseButton.SetActive(true);
                                    }
                                }
                                break;
                            }
                        case "Menu":
                            {
                                Menus[1].SetActive(true);
                                Menu_PizzeriaOpened.SetActive(false);
                                Menu_PizzeriaNotOpened.SetActive(false);
                                PizzaName.gameObject.SetActive(false);
                                Cost.gameObject.SetActive(false);
                                Menu_ToppingsCurr.SetActive(false);
                                Menu_ToppingsAvailable.SetActive(false);
                                Menu_Confirm.SetActive(false);
                                Menu_Info.SetActive(false);
                                

                                Menu_PizzeriaNotOpened.SetActive(true);
                                if (MyMenu.Instance != null)
                                {
                                    foreach (Transform t in Menu_PizzasParent)
                                    {
                                        Destroy(t.gameObject);
                                    }
                                    Menu_noPizzas.SetActive(false);
                                    currentPizzas = MyMenu.Instance.Menu;
                                    if (currentPizzas.Count > 0)
                                    {
                                        int i = 0;
                                        foreach (Pizza p in currentPizzas)
                                        {
                                            GameObject po = (GameObject)Instantiate(Menu_PizzaPrefab, Menu_PizzasParent);
                                            MenuPizzaEntry e = po.GetComponent<MenuPizzaEntry>();
                                            e.myText.text = p.Name;
                                            e.myID = i;
                                            currentPizzaObjects.Add(e);
                                            i++;
                                        }
                                    }
                                    else
                                    {
                                        Menu_noPizzas.SetActive(true);
                                    }

                                }
                                if (ThePizzeria.Instance.isOpened())
                                {
                                    Menu_PizzeriaOpened.SetActive(true);
                                    Menu_ToppingsAvailable.SetActive(false);
                                    Menu_Confirm.SetActive(false);
                                    Menu_Info.SetActive(false);
                                    PizzaName.interactable = false;
                                    Cost.interactable = false;
                                    Menu_AddingButton.interactable = false;
                                    Menu_RemoveButton.interactable = false;
                                }
                                else
                                {
                                    PizzaName.interactable = true;
                                    Cost.interactable = true;
                                    Menu_AddingButton.interactable = true;
                                    Menu_RemoveButton.interactable = true;
                                }
                                break;
                            }
                        case "Info":
                            {
                                if (ThePizzeria.Instance != null)
                                {
                                    InfoO.SetActive(true);
                                    RatingsT.text = $"Pizzeria Rating (0-10)\n<size=80%>  Service: {ThePizzeria.Instance.Rating.ServiceRating}\n  Pizza: {ThePizzeria.Instance.Rating.PizzaRating}\n  <size=40%>(and more to come)\n------------------------------------------------------------------------------------------------------\n<size=80%>  Overall Rating: {ThePizzeria.Instance.Rating.GeneralRating()}\n\n<size=50%>The rating is a subject of change, as the game just got released and the rating system is being made 3 days before the deadline.\nKeep it in mind, the game is gonna be updated one day.";
                                }
                                else
                                {
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                                break;
                            }
                        case "Other":
                            {
                                OtherO.SetActive(true);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
        }
        public void EnableTopping(PossibleToppings t, bool en = true)
        {
            foreach (PosTopping top in PosToppings)
            {
                if (top.me == t)
                {
                    top.gameObject.SetActive(en);
                    break;
                }
            }
        }
        public void ChangeName()
        {
            if (!ThePizzeria.Instance.isOpened())
            {
                if (currentPizzas != null)
                {
                    if (Menu_SelectedPizza != null)
                    {
                        if (PizzaName.text.Trim() != "")
                        {
                            currentPizzas[Menu_SelectedPizza.myID].Name = PizzaName.text;
                            Init();
                        }
                        else
                        {
                            if (BuildingUI.Instance != null)
                            {
                                BuildingUI.Instance.ErrorS.Play();
                            }
                        }
                    }
                }
            }
        }
        public void ChangePrice()
        {
            if (!ThePizzeria.Instance.isOpened() && (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <=0))
            {
                if (currentPizzas != null)
                {
                    if (Menu_SelectedPizza != null)
                    {
                        int price = -1;
                        if (PizzaName.text.Trim() != "")
                        {
                            price = int.Parse(Cost.text);
                            if (price > 0 && price <= 130)
                            {
                                currentPizzas[Menu_SelectedPizza.myID].Price = price;
                                Init();
                            }
                            else
                            {
                                if (BuildingUI.Instance != null)
                                {
                                    BuildingUI.Instance.ErrorS.Play();
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
                }
            }
        }
        public void AddToppingToSelected(PosTopping top)
        {
            if (!ThePizzeria.Instance.isOpened() && (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <= 0))
            {
                if (currentPizzas != null)
                {
                    if (Menu_SelectedPizza != null)
                    {
                        if (currentPizzas[Menu_SelectedPizza.myID].Toppings == null)
                        {
                            currentPizzas[Menu_SelectedPizza.myID].Toppings = new List<PossibleToppings>();
                        }
                        if (!currentPizzas[Menu_SelectedPizza.myID].Toppings.Contains(top.me) && currentPizzas[Menu_SelectedPizza.myID].Toppings.Count < 5)
                        {
                            currentPizzas[Menu_SelectedPizza.myID].Toppings.Add(top.me);
                            InitPizza();
                        }
                    }
                }
            }else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        public void ConfirmMenu()
        {
            if (!ThePizzeria.Instance.isOpened() && (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <= 0))
            {
                bool anywrong = false;
                foreach (Pizza p in currentPizzas)
                {
                    if (p.Toppings == null || p.Toppings.Count <= 0 || p.Price <= 0 || p.Name.Trim() == "")
                    {
                        anywrong = true;
                        break;
                    }
                }
                if (!anywrong)
                {
                    if (MyMenu.Instance != null)
                    {
                        MyMenu.Instance.Menu = currentPizzas;
                        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
                        {
                            if (SaveSystem.Instance != null)
                            {
                                SaveSystem.Instance.SaveMenu(cb =>
                                {
                                    if (!cb)
                                    {
                                        Menu_Info.GetComponent<TextMeshProUGUI>().text = $"ERROR. Your Menu has been applied, but not saved in GameJolt Save System.";
                                        CancelInvoke("ResetInfoText");

                                        Invoke("ResetInfoText", 3f);
                                        if (BuildingUI.Instance != null)
                                        {
                                            BuildingUI.Instance.ErrorS.Play();
                                        }
                                    }
                                    else
                                    {
                                        Trophies.TryUnlock(SAVE_MENU_TROPHY_ID);   
                                        if (isOn)
                                        {
                                            Switch();
                                        }
                                    }
                                }
                                );
                            }
                        }
                        else
                        {
                            if (isOn)
                            {
                                Switch();
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
                else
                {
                    Menu_Info.GetComponent<TextMeshProUGUI>().text = $"There can't be any pizza without toppings, Price of 0 (or under), or without a proper name.";
                    CancelInvoke("ResetInfoText");
                    Invoke("ResetInfoText", 3f);
                    if (BuildingUI.Instance != null)
                    {
                        BuildingUI.Instance.ErrorS.Play();
                    }
                }

            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        void ResetInfoText()
        {
            CancelInvoke("ResetInfoText");
            Menu_Info.GetComponent<TextMeshProUGUI>().text = $"Click to Add or Remove Toppings.";
            ClosedText.text = $"";
        }
        public void InitPizza()
        {
            if (Menu_SelectedPizza != null)
            {
                PizzaName.gameObject.SetActive(true);
                Cost.gameObject.SetActive(true);
                Menu_ToppingsCurr.SetActive(true);
                Menu_ToppingsAvailable.SetActive(true);
                Menu_Confirm.SetActive(true);
                Menu_Info.SetActive(true);
                foreach (PosTopping t in PosToppings)
                {
                    t.gameObject.SetActive(true);
                }
                foreach (Transform t in Menu_ToppingsParent)
                {
                    Destroy(t.gameObject);
                }
                Cost.text = currentPizzas[Menu_SelectedPizza.myID].Price + "";
                PizzaName.text = currentPizzas[Menu_SelectedPizza.myID].Name + "";
                if (currentPizzas[Menu_SelectedPizza.myID].Toppings != null)
                {
                    foreach (PossibleToppings t in currentPizzas[Menu_SelectedPizza.myID].Toppings)
                    {
                        GameObject top = (GameObject)Instantiate(Menu_ToppingPrefab, Menu_ToppingsParent);
                        MenuToppingEntry mte = top.GetComponent<MenuToppingEntry>();
                        mte.Me = t;
                        mte.myPizza = Menu_SelectedPizza;
                        EnableTopping(t, false);
                    }
                }

            }
            else
            {
                foreach (PosTopping t in PosToppings)
                {
                    t.gameObject.SetActive(true);
                }
                foreach (Transform t in Menu_ToppingsParent)
                {
                    Destroy(t.gameObject);
                }
            }
            if (ThePizzeria.Instance.isOpened() || (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count > 0))
            {
                Menu_PizzeriaOpened.SetActive(true);
                Menu_ToppingsAvailable.SetActive(false);
                Menu_Confirm.SetActive(false);
                Menu_Info.SetActive(false);
                PizzaName.interactable = false;
                Cost.interactable = false;
                Menu_AddingButton.interactable = false;
                Menu_RemoveButton.interactable = false;
            }
        }
        public void RemoveSelectedPizza()
        {
            if (!ThePizzeria.Instance.isOpened() && (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <= 0))
            {
                if (Menu_SelectedPizza != null)
                {
                    currentPizzas.Remove(currentPizzas[Menu_SelectedPizza.myID]);
                    currentPizzaObjects.Remove(Menu_SelectedPizza);
                    Destroy(Menu_SelectedPizza.gameObject);
                    Menu_SelectedPizza = null;
                    Init();
                    InitPizza();

                }
            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        public void AddNewPizza()
        {
            if (!ThePizzeria.Instance.isOpened() && (OrdersManager.Instance != null && OrdersManager.Instance.Orders.Count <= 0))
            {
                Pizza newPizza = new Pizza();
                newPizza.Name = "New Pizza";
                newPizza.Price = 10;
                currentPizzas.Add(newPizza);
                Init();
            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        public void ChangeOpeningHours()
        {
            if (!ThePizzeria.Instance.isOpened())
            {
                int open = (int)OH_Open.value;
                int close = (int)OH_Close.value;
                if (open >= 6 && open < 24)
                {
                    if (close > 6 && close <= 24)
                    {
                        if (close > open)
                        {
                            if (open > TimeSystem.Instance.FinalHour && close > TimeSystem.Instance.FinalHour)
                            {
                                if (ThePizzeria.Instance != null)
                                {
                                    ThePizzeria.Instance.OpenHour = open;
                                    ThePizzeria.Instance.ClosingHour = close;
                                    PlayerPrefs.SetInt("oh", open);
                                    PlayerPrefs.SetInt("ch", close);
                                    Trophies.TryUnlock(CHANGE_OPENING_HOURS_TROPHY_ID);
                                    if (isOn)
                                    {
                                        Switch();
                                    }
                                }
                                else
                                {
                                    ClosedText.text = $"Error.";
                                    CancelInvoke("ResetInfoText");
                                    Invoke("ResetInfoText", 3f);
                                    BuildingUI.Instance.ErrorS.Play();
                                }
                            }
                            else
                            {
                                ClosedText.text = $"You can't just open the Pizzeria Right Now. Try again later.";
                                CancelInvoke("ResetInfoText");
                                Invoke("ResetInfoText", 3f);
                                BuildingUI.Instance.ErrorS.Play();
                            }
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
                }
                else
                {
                    BuildingUI.Instance.ErrorS.Play();

                }
            }else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        public void OpeningEdit()
        {
            int open = (int)OH_Open.value;
            open = Mathf.Clamp(open, 6, 23);
            OH_Close.minValue = open + 1;
        }
        public void ClosePizzeria()
        {
            ThePizzeria.Instance.OpenHour = 0;
            ThePizzeria.Instance.ClosingHour = 0;
            PlayerPrefs.SetInt("oh", 0);
            PlayerPrefs.SetInt("ch", 0);
            Init();
        }
        public void SkipNight()
        {
            if (TimeSystem.Instance != null && TimeSystem.Instance.isNight())
            {
                TimeSystem.Instance.skipnight = true;
            }
        }
    }
}