using GameJolt.API;
using LyviN.Map;
using LyviN.Player;
using LyviN.Player.Baking;
using LyviN.Player.Build;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LyviN.Player.Baking.Pizza;

public class Shared : MonoBehaviour
{
    ///////////////////////////////////////////////////////
    // OPEN SOURCE TODO:
    // Discord Logging - put your own
    private const string FEEDBACK_WEBHOOK = "";
    private const string START_MONEY_WEBHOOK = "";
    private const string TAKE_MONEY_WEBHOOK = "";
    private const string ADD_MONEY_WEBHOOK = "";
    private const string STORAGE_OUT_WEBHOOK = "";
    private const string STORAGE_ADD_WEBHOOK = "";
    private const string RESET_WEBHOOK = "";

    // GameJolt Leaderboard IDs - put your own 
    public const int MONEY_LEADERBOARD_ID = 0;
    public const int SOLD_PIZZAS_LEADERBOARD_ID = 0;
    
    ///////////////////////////////////////////////////////
    
    public static bool hasTransaction;
    public static DateTime lastTransaction;
    public static string myMoneyKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_money";
        }
        return key;
    }
    public static string myRemRatings()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_remainingratings";
        }
        return key;
    }
    public static string myRating()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_rating";
        }
        return key;
    }
    public static string mySoldPizzasKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_scores_soldpizzas";
        }
        return key;
    }
    public static string myStructuresKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_structures";
        }
        return key;
    }
    public static string myPositionKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_position";
        }
        return key;
    }    
    public static string myToppingsStorageKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_toppings";
        }
        return key;
    }
    public static string myPropsKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_props";
        }
        return key;
    }
    public static string myLightsKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_lights";
        }
        return key;
    }
    public static string myDoorsKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_doors";
        }
        return key;
    }
    public static string myMenuKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_pizzeriamenu";
        }
        return key;
    }  
    public static string myAnimatronicsKey()
    {
        string key = "trash";
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            key = $"userdata_{GameJoltAPI.Instance.CurrentUser.ID}_save_animatronics";
        }
        return key;
    }
    public static string OtherStructuresKey(string ID)
    {

        string key = $"userdata_{ID}_save_structures";

        return key;
    }    
    public static string OtherAnimatronicsKey(string ID)
    {

        string key = $"userdata_{ID}_save_animatronics";

        return key;
    }   
    public static string OtherDoorsKey(string ID)
    {

        string key = $"userdata_{ID}_save_doors";

        return key;
    }    
    public static string OtherLightsKey(string ID)
    {

        string key = $"userdata_{ID}_save_lights";

        return key;
    }  
    public static string OtherPropsKey(string ID)
    {

        string key = $"userdata_{ID}_save_props";

        return key;
    }
    public static void CheckAndSetStartMoney()
    {
        if (GameJoltAPI.Instance.HasSignedInUser)
        {
            DataStore.Get(Shared.myMoneyKey(), true, res =>
            {

                if (res != null)
                {
                    if (!string.IsNullOrEmpty(res.Trim()))
                    {
                        Debug.Log("[FSSS] Found Money Save File.");
                    }
                    else
                    {
                        Debug.LogError("[FSSS] Error - Found EMPTY Money Save File.");
                    }
                }
                else
                {
                    Debug.LogWarning("[FSSS] Money Save Not Found.");
                    DataStore.Get(Shared.myMoneyKey(), true, res2 =>
                    {

                        if (res2 == null)
                        {
                            Debug.LogWarning("[FSSS] Money Save is being Created.");
                            DataStore.Get("system_startMoney", true, cb =>
                            {

                                if (cb != null)
                                {
                                    if (!string.IsNullOrEmpty(cb.Trim()))
                                    {
                                        string cb1 = cb.Trim();
                                        DataStore.Set(myMoneyKey(), cb1, true, succ =>
                                        {
                                            Debug.Log($"[FSSS] My Start Money has been set to {cb1} $$");
                                            StartMoneyLog($"My Start Money has been set to {cb1} $$");
                                        });
                                    }
                                    else
                                    {
                                        Debug.LogError($"[FSSS] I wanted to get my money set, but the data was empty.");
                                        StartMoneyLog($"I wanted to get my money set, but the data was empty.");
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"[FSSS] I wanted to get my money set, but it couldn't be loaded.");
                                    StartMoneyLog($"I wanted to get my money set, but it couldn't be loaded.");
                                }
                            });
                        }
                    });
                }
            });

        }
    }
    public static void StorageRemove(int id, int amount, Action<bool> success)
    {
        if (GameJoltAPI.myInstance != null)
        {
            if (GameJoltAPI.Instance.HasSignedInUser)
            {
                if (GameJoltAPI.Instance.CurrentUser.ID != 0)
                {
                    DataStore.Get(myToppingsStorageKey(), true, tops => 
                    {
                        if (tops != null)
                        {
                            if (!string.IsNullOrEmpty(tops))
                            {
                                List<string> a = new List<string>(tops.Trim().Split('\n'));
                                int i = 0;
                                bool found = false;
                                foreach (string b in a)
                                {
                                    found = true;
                                    string[] c = b.Trim().Split('|');
                                    int fid = 0;
                                    if (!string.IsNullOrEmpty(c[0].Trim()))
                                    {
                                        fid = int.Parse(c[0].Trim());
                                        if (id != 0 && id == fid)
                                        {
                                            PossibleToppings t = (PossibleToppings)id;
                                            if (t != PossibleToppings.Unknown)
                                            {
                                                List<string> newA = new List<string>(a);
                                                int am = int.Parse(c[1].Trim());
                                                if (am > amount)
                                                {
                                                    string newsave = $"{(int)t}|{am - amount}";
                                                    newA.RemoveAt(i);
                                                    newA.Insert(i, newsave);
                                                }
                                                else if (am == amount)
                                                {
                                                    newA.RemoveAt(i);
                                                }


                                                string newFinalSave = string.Join('\n', newA);
                                                if (newFinalSave == "")
                                                {
                                                    newFinalSave = " ";
                                                }
                                                if (newFinalSave != tops)
                                                {
                                                    DataStore.Set(myToppingsStorageKey(), newFinalSave, true, cb => { success(cb); StorageOutLog($"Taking Out {amount} of {(PossibleToppings)id}: **{cb}**"); });
                                                }
                                                else
                                                {
                                                    success(false);
                                                }

                                            }
                                            else
                                            {
                                                success(false);
                                            }
                                            break;
                                        }
                                        i++;
                                    }
                                }
                                if (!found)
                                {
                                    success(false);
                                }
                            }
                            else
                            {
                                success(false);
                            }
                        }
                        else
                        {
                            if (SceneManager.GetActiveScene().name == "MyPizzeria")
                            {
                                SaveSystem.checkTimeOut();
                            }
                            success(false);
                        }
                    });
                }
                else
                {
                    success(false);
                }
            }
            else
            {
                success(false);
            }
        }
        else
        {
            success(false);
        }
    }
    public static void StorageAdd(int id, int amount, Action<bool> success, bool force = false)
    {
        if (GameJoltAPI.myInstance != null)
        {
            if (GameJoltAPI.Instance.HasSignedInUser)
            {
                if (GameJoltAPI.Instance.CurrentUser.ID != 0)
                {
                    DataStore.Get(myToppingsStorageKey(), true, atops =>
                    {
                        string ctops = "";
                        bool timedout = GameJoltAPI.Instance.timedOut;
                        Debug.Log(timedout);
                        if (atops != null || force || (!timedout && atops == null))
                        {

                            if (!string.IsNullOrEmpty(atops) || force)
                            {
                                ctops = atops;
                            }
                                List<string> a = new List<string>(ctops.Trim().Split('\n'));
                                int i = 0;
                                bool found = false;
                                List<string> newA = new List<string>(a);
                            if (a.Count > 0)
                            {
                                foreach (string b in a)
                                {
                                    string[] c = b.Trim().Split('|');
                                    int fid = 0;
                                    string eid = c[0].Trim();
                                    if (!string.IsNullOrEmpty(eid))
                                    {
                                        fid = int.Parse(eid);
                                        if (id != 0 && id == fid)
                                        {
                                            found = true;
                                            PossibleToppings t = (PossibleToppings)id;
                                            if (t != PossibleToppings.Unknown)
                                            {
                                                int am = int.Parse(c[1].Trim());
                                                amount = Mathf.Clamp(amount, 0, 256 - am);
                                                if (am + amount <= 256 && amount > 0)
                                                {
                                                    string newsave = $"{(int)t}|{am + amount}";
                                                    newA.RemoveAt(i);
                                                    newA.Insert(i, newsave);
                                                    string newFinalSave = string.Join('\n', newA);
                                                    if (newFinalSave == "")
                                                    {
                                                        success(false);
                                                    }
                                                    else
                                                    {
                                                        if (newFinalSave != atops)
                                                        {
                                                            DataStore.Set(myToppingsStorageKey(), newFinalSave, true, cb => { success(cb); StorageAddLog($"Adding {amount} of {(PossibleToppings)id}: **{cb}**"); });
                                                        }
                                                        else
                                                        {
                                                            success(false);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    success(false);
                                                }
                                            }
                                            else
                                            {
                                                success(false);
                                            }
                                            break;
                                        }
                                        i++;
                                    }
                                }
                            }
                            if (!found)
                            {
                                if (id != 0)
                                {
                                    string newsave = $"{(int)id}|{amount}";
                                    newA.Add(newsave);
                                    string newFinalSave = string.Join('\n', newA);
                                    if (newFinalSave == "")
                                    {
                                        success(false);
                                    }
                                    else
                                    {
                                        if (newFinalSave != atops)
                                        {
                                            DataStore.Set(myToppingsStorageKey(), newFinalSave, true, cb => { success(cb); StorageAddLog($"Adding {amount} of {(PossibleToppings)id}: **{cb}**"); });
                                        }
                                        else
                                        {
                                            success(false);
                                        }
                                    }
                                }
                                else
                                {
                                    success(false);
                                }

                            }
                            else
                            {
                                success(false);
                            }
                        }
                        else
                        {
                            if (SceneManager.GetActiveScene().name == "MyPizzeria")
                            {
                                SaveSystem.checkTimeOut();
                            }
                            else
                            {

                            }
                            
                        }
                    });
                }
                else
                {
                    success(false);
                }
            }
            else
            {
                success(false);
            }
        }
        else
        {
            success(false);
        }
    }
    public static void FeedbackLog(string feedback)
    {
        sendWebHook(FEEDBACK_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**)\n```{feedback}```", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }
    public static void StartMoneyLog(string Log)
    {
        sendWebHook(START_MONEY_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }   
    public static void TakeMoneyLog(string Log)
    {
        sendWebHook(TAKE_MONEY_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }
    public static void AddMoneyLog(string Log)
    {
        sendWebHook(ADD_MONEY_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }
    public static void StorageOutLog(string Log)
    {
        sendWebHook(STORAGE_OUT_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    } 
    public static void StorageAddLog(string Log)
    {
        sendWebHook(STORAGE_ADD_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }  
    public static void ResetLog(string Log)
    {
        sendWebHook(RESET_WEBHOOK, $"**{GameJoltAPI.Instance.CurrentUser.Name}** (**{GameJoltAPI.Instance.CurrentUser.ID}**) - {Log}", (GameJoltAPI.Instance.CurrentUser.Name + " (" + GameJoltAPI.Instance.CurrentUser.ID + ")"), GameJoltAPI.Instance.CurrentUser.AvatarURL);
    }
    public static void sendWebHook(string URL, string msg, string username, string img = "")
    {
        if (string.IsNullOrEmpty(URL)) return;

        NameValueCollection vc = new NameValueCollection()
        {
            { "username", username },
            { "content", msg},
            { "avatar_url", img}
        };
        PostAsync(URL, vc);
    }
    public static byte[] Post(string url, NameValueCollection pairs)
    {
        if (string.IsNullOrEmpty(url)) return null;

        WebClient webClient = new WebClient();
            webClient.Proxy = null;
            return webClient.UploadValues(url, pairs);
    }
    public static void PostAsync(string url, NameValueCollection pairs)
    {
        if (string.IsNullOrEmpty(url)) return;

        var client = new WebClient();
        client.UploadValuesCompleted += (sender, e) =>
        {
            byte[] a = e.Result;
            //do something with results 
        };

        client.UploadValuesAsync(new System.Uri(url), pairs);
    }
    public static string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);
        return finalString;
    }
    public static void AddRating(VisitRating vr)
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            string ratings = "";
            DataStore.Get(myRemRatings(), true, cb => 
            { 
                if (!GameJoltAPI.Instance.timedOut)
                {
                    if (cb != null)
                    {
                        ratings = ratings + cb;
                    }
                    string ratingst = $"{vr.PizzaRating}|{vr.ServiceRating}\n";
                    ratings = ratings + ratingst;
                    DataStore.Set(myRemRatings(), ratings, true, setcb => { if (setcb) { Debug.Log("Uploaded Awaiting Rating"); } });
                }
            });
        }
    }
    public static void TakeMoney(int howmuch, Action<TakeMoneyResult> callback, string source, bool forceMinus = false)
    {
        if (!hasTransaction)
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
            {
                TimeSpan span = DateTime.Now.Subtract(lastTransaction);
                if (span.TotalSeconds > 2)
                {
                    hasTransaction = true;
                    DataStore.Get(Shared.myMoneyKey(), true, cres =>
                    {
                        if (cres != null)
                        {
                            if (!string.IsNullOrEmpty(cres))
                            {
                                int money = int.Parse(cres.Trim());
                                if (money > -6942069)
                                {
                                    if (money >= howmuch || forceMinus)
                                    {
                                        int remMoney = money - howmuch;
                                        DataStore.Set(myMoneyKey(), remMoney + "", true, upcall =>
                                        {
                                            if (upcall)
                                            {
                                                TakeMoneyLog($"Paid {howmuch}$$. Had {money}$$. Should be {remMoney}$$ left. ({source})");
                                                callback(TakeMoneyResult.success);
                                                if (MainUI.Instance != null)
                                                {
                                                    MainUI.Instance.ChangeMoneyUI(false, howmuch);
                                                }
                                            }
                                            else
                                            {
                                                callback(TakeMoneyResult.uploadError);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        callback(TakeMoneyResult.notEnough);
                                    }
                                }
                                else
                                {
                                    callback(TakeMoneyResult.parseError);
                                }
                            }
                            else
                            {
                                callback(TakeMoneyResult.emptyData);
                            }
                        }
                        else
                        {
                            callback(TakeMoneyResult.noData);
                            CheckAndSetStartMoney();
                        }
                        hasTransaction = false;
                        Debug.Log("Transaction ENDS");
                    });
                    lastTransaction = DateTime.Now;
                }
                else
                {
                    callback(TakeMoneyResult.transactionCooldown);
                }
            }
            else
            {
                callback(TakeMoneyResult.notLoggedIn);
            }
        }
        else
        {
            callback(TakeMoneyResult.hasTransactionGoing);
        }
    }
    public static void AddSoldPizzaScore(Action<bool> callback)
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            int sold = 0;
            DataStore.Get(mySoldPizzasKey(), true, cb => { 
                if (cb!=null && !string.IsNullOrEmpty(cb))
                {
                    sold = int.Parse(cb.Trim());
                }
                else if(GameJoltAPI.Instance.timedOut)
                {
                    return;
                }
                int nsold = sold + 1;
                if (nsold > sold)
                {
                    DataStore.Set(mySoldPizzasKey(), nsold + "", true, res => {
                        
                        if (res)
                        {
                            GameJolt.API.Objects.Score nS = new GameJolt.API.Objects.Score(nsold, $"{nsold} Pizzas sold.", "", $"{GameJoltAPI.Instance.CurrentUser.AvatarURL}");
                            Scores.Add(nS, SOLD_PIZZAS_LEADERBOARD_ID, res2 => {
                                callback(true);
                            });
                        }
                        else
                        {
                            callback(false);
                        }
                    });
                }
                else
                {
                    callback(false);
                }
            });
        }
        else
        {
            callback(false);
        }
    }
    public static void AddMoney(int howmuch, Action<bool> callback, string source = "Unknown")
    {
        if (!hasTransaction)
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
            {
                TimeSpan span = DateTime.Now.Subtract(lastTransaction);
                if (span.TotalSeconds > 2)
                {
                    hasTransaction = true;
                    DataStore.Get(myMoneyKey(), true, cres =>
                    {
                        if (cres != null)
                        {
                            if (!string.IsNullOrEmpty(cres))
                            {
                                int money = int.Parse(cres.Trim());
                                if (money > -6942069)
                                {
                                    
                                        int remMoney = money + howmuch;
                                        DataStore.Set(myMoneyKey(), remMoney + "", true, upcall =>
                                        {
                                            if (upcall)
                                            {
                                                AddMoneyLog($"Added {howmuch}$$. Had {money}$$. Should be {remMoney}$$ left. ({source})");
                                                callback(true);
                                                GameJolt.API.Objects.Score nS = new GameJolt.API.Objects.Score(remMoney, $"{remMoney}$", "", GameJoltAPI.Instance.CurrentUser.AvatarURL);
                                                Scores.Add(nS, MONEY_LEADERBOARD_ID);
                                                if (MainUI.Instance != null)
                                                {
                                                    MainUI.Instance.ChangeMoneyUI(true, howmuch);
                                                }
                                            }
                                            else
                                            {
                                                callback(false);
                                            }
                                        });
                                    
                                    
                                }
                                else
                                {
                                    callback(false);
                                }
                            }
                            else
                            {
                                callback(false);
                            }
                        }
                        else
                        {
                            callback(false);
                            CheckAndSetStartMoney();
                        }
                        hasTransaction = false;
                        Debug.Log("ADDING MONEY ENDS");
                    });
                    lastTransaction = DateTime.Now;
                }
                else
                {
                    callback(false);
                }
            }
            else
            {
                callback(false);
            }
        }
        else
        {
            callback(false);
        }
    }
    public static void CheckStructurePrice(string structurename, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_structurePrices", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (CatalogueGameJolt.Instance != null)
                    {
                        CatalogueGameJolt.Instance.structureprices = allprices;
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (args[0].Trim() == structurename)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }
    public static void CheckAnimatronicPrice(string anmname, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_anmPrices", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (CatalogueGameJolt.Instance != null)
                    {
                        CatalogueGameJolt.Instance.animatronicprices = allprices;
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (args[0].Trim() == anmname)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }
    public static void CheckToppingPrice(int id, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_toppingInfo", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (StructureInteraction.Instance != null)
                    {
                        StructureInteraction.Instance.topPrices.Clear();
                        foreach (string ap in allprices)
                        {
                            string[] apr = ap.Split(':');
                            int price = int.Parse(apr[1].Trim());
                            StructureInteraction.Instance.topPrices.Add(price);

                        }
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (int.Parse(args[0].Trim()) == id)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }
    public static void CheckPropPrice(string propName, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_propPrices", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (CatalogueGameJolt.Instance != null)
                    {
                        CatalogueGameJolt.Instance.propprices = allprices;
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (args[0].Trim() == propName)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }

    public static void CheckLampPrice(string propName, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_lightPrices", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (CatalogueGameJolt.Instance != null)
                    {
                        CatalogueGameJolt.Instance.lampprices = allprices;
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (args[0].Trim() == propName)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }
    public static void CheckDoorPrice(string doorName, Action<int> price)
    {
        if (GameJoltAPI.myInstance != null)
        {

            DataStore.Get("system_doorPrices", true, cb =>
            {
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {

                    string[] allprices = cb.Split('\n');
                    if (CatalogueGameJolt.Instance != null)
                    {
                        CatalogueGameJolt.Instance.doorprices = allprices;
                    }
                    bool found = false;
                    foreach (string p in allprices)
                    {
                        string[] args = p.Split(':');
                        if (args[0].Trim() == doorName)
                        {
                            int fp = int.Parse(args[1].Trim());
                            price(fp);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        price(-1);
                    }
                }
                else
                {
                    price(-2);
                }
            });

        }
        else
        {
            price(-3);
        }
    }
    public static void GetGameJoltCredentials(out string username, out string token)
    {
        if (PlayerPrefs.HasKey("gj_remember_username") && PlayerPrefs.HasKey("gj_remember_token"))
        {
            username = PlayerPrefs.GetString("gj_remember_username");
            token = PlayerPrefs.GetString("gj_remember_token");
        }
        else
        {
            username = "";
            token = "";
        }
    }
    public static void SetGameJoltCredentials(string username, string token)
    {
        PlayerPrefs.SetString("gj_remember_username", username);
        PlayerPrefs.SetString("gj_remember_token", token);
    }
    public static void ClearGameJoltCredentials()
    {
        PlayerPrefs.DeleteKey("gj_remember_username");
        PlayerPrefs.DeleteKey("gj_remember_token");
    }
    public enum TakeMoneyResult
    {
        notLoggedIn,
        hasTransactionGoing,
        transactionCooldown,
        notEnough,
        noData,
        emptyData,
        parseError,
        uploadError,
        success,
    }
}