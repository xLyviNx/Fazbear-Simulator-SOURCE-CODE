using GameJolt.API;
using LyviN.Map;
using LyviN.Map.Animatronics;
using LyviN.Map.Props;
using LyviN.Misc;
using LyviN.Player;
using LyviN.Player.Baking;
using LyviN.Player.Build;
using LyviN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.Map
{
    //DEAR GOD WHAT THE FUCK IS THAT?! ~2026 LyviN
    // I AM SO SO SO SO SORRY FOR YOU HAVING TO READ THIS CODE, PLEASE IF YOU ARE CRAZY IN THE HEAD ENOUGH TO REWORK THIS, PLEASE DO IT HOLY FUCK
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance;
        public bool Loaded;
        public GameObject WallPrefab;
        public GameObject[] propsPrefabs;
        public GameObject[] lampPrefabs;
        public GameObject[] doorPrefabs;
        public GameObject[] anmPrefabs;
        public bool bakedNav;
        bool loadedPos;
        public const string dateformat = "MM/dd/yyyy HH:mm:ss";
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                StartCoroutine(StartLoad());
            }
            else
            {
                enabled = false;
            }
        }

        IEnumerator StartLoad()
        {
            Loaded = false;
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    while (GameJoltAPI.Instance.CurrentUser.ID == 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    
                    LoadStructures(cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Loaded Structures");
                            
                        }
                        else
                        {
                            checkTimeOut();
                            Debug.Log("DID NOT LOAD Structures");
                            bakedNav = true;

                        }
                        LoadProps(cb =>
                        {
                            if (cb)
                            {
                                Debug.Log("Loaded Props");
                                ChairProp[] chairs = GameObject.FindObjectsOfType<ChairProp>();
                                foreach (ChairProp chair in chairs)
                                {
                                    chair.StartFindTable();
                                }
                            }
                            else if (GameJoltAPI.Instance.timedOut)
                            {
                                SceneManager.LoadScene("MainMenu");
                                GameJoltAPI.Instance.timedOut = false;
                            }
                            else
                            {
                                Debug.Log("DID NOT LOAD Props");
                            }
                            LoadLights(cb =>
                            {
                                if (cb)
                                {

                                }
                                else if (GameJoltAPI.Instance.timedOut)
                                {
                                    SceneManager.LoadScene("MainMenu");
                                    GameJoltAPI.Instance.timedOut = false;
                                }
                                else
                                {
                                    Debug.Log("DID NOT LOAD Lights");
                                }
                                LoadDoors(cb =>
                                {
                                    if (cb)
                                    {

                                    }
                                    else if (GameJoltAPI.Instance.timedOut)
                                    {
                                        SceneManager.LoadScene("MainMenu");
                                        GameJoltAPI.Instance.timedOut = false;
                                    }
                                    else
                                    {
                                        Debug.Log("DID NOT LOAD DOORS");
                                    }
                                    LoadAnimatronics(cb =>
                                    {
                                        if (cb)
                                        {
                                            Debug.Log("Loaded Animatronics.");
                                        }
                                        else if (GameJoltAPI.Instance.timedOut)
                                        {
                                            SceneManager.LoadScene("MainMenu");
                                            GameJoltAPI.Instance.timedOut = false;
                                        }
                                        else
                                        {
                                            Debug.Log("DID NOT LOAD THE ANIMATRONICS");
                                        }
                                        LoadMenu(cb =>
                                        {
                                            if (cb)
                                            {
                                                Debug.Log("Loaded Menu.");

                                            }
                                            else if (GameJoltAPI.Instance.timedOut)
                                            {
                                                SceneManager.LoadScene("MainMenu");
                                                GameJoltAPI.Instance.timedOut = false;
                                            }
                                            else
                                            {
                                                Debug.Log("DID NOT LOAD THE PIZZERIA MENU");
                                            }
                                            StartCoroutine(LoadPosition());
                                            LoadOH();
                                            Loaded = true;
                                            if (DiscordController.Instance != null)
                                            {
                                                DiscordController.Instance.SetActivity("In Game", "At my Pizzeria", true);
                                            }
                                        });

                                    });

                                });

                            });
                        });

                    });
                }
            }
            StartCoroutine(UploadPositions());
            yield return null;
        }
        IEnumerator LoadPosition()
        {
            
            while (GameJoltAPI.Instance.CurrentUser.ID == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log(Shared.myPositionKey());
            DataStore.Get(Shared.myPositionKey(), true, pos => {

                if (pos != null && !string.IsNullOrEmpty(pos))
                {
                    string[] l = pos.Trim().Split('\n');
                    for (int i = 0; i < pos.Length; i++)
                    {
                        if (i == 0)
                        {
                            string[] p = l[i].Trim().Split(';');
                            string xt = p[0].Trim();
                            string yt = p[1].Trim();
                            string zt = p[2].Trim();
                            //Debug.Log($"BEFORE  --  X: {xt}, Y: {yt}, Z: {zt}");
                            float x = float.Parse(xt);
                            float y = float.Parse(yt);
                            float z = float.Parse(zt);
                            //Debug.Log($"AFTER  --  X: {x}, Y: {y}, Z: {z}");
                            Vector3 mypos = new Vector3(x, y, z);
                            transform.position = mypos;
                            loadedPos = true;
                            //Debug.Log(mypos);
                        }
                        else if (i == 1)
                        {
                            transform.eulerAngles = new Vector3(0f, float.Parse(l[i].Trim(), NumberStyles.AllowThousands, CultureInfo.InvariantCulture), 0f);
                        }
                    }
                }
                else if (GameJoltAPI.Instance.timedOut)
                {
                    SceneManager.LoadScene("MainMenu");
                    GameJoltAPI.Instance.timedOut = false;
                }
                else
                {
                    loadedPos = true;
                    Debug.Log("DID NOT LOAD Position");
                }
                Loaded = true;

            });
            yield return null;
        }
        // Update is called once per frame
        void Update()
        {

        }
        public void SaveAll()
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
            {
                string code = Shared.RandomString(4);
                DataStore.Get("reset", false, rescb =>
                {
                    if (rescb == null && !GameJoltAPI.Instance.timedOut)
                    {
                        SaveStructures(succ =>
                        {
                            if (succ && !GameJoltAPI.Instance.timedOut)
                            {
                                Debug.Log("Saved structures");
                                SaveProps(succ2 =>
                                {
                                    if (succ2 && !GameJoltAPI.Instance.timedOut)
                                    {
                                        Debug.Log("Saved Props");
                                        SaveDoors(succ3 =>
                                        {

                                            if (succ3 && !GameJoltAPI.Instance.timedOut)
                                            {
                                                Debug.Log("Saved Lights");
                                                SaveLights(succ4 =>
                                                {

                                                    if (succ4 && !GameJoltAPI.Instance.timedOut)
                                                    {
                                                        Debug.Log("Saved Doors");
                                                        SaveAnimatronics(succ5 =>
                                                        {

                                                            if (succ5 && !GameJoltAPI.Instance.timedOut)
                                                            {
                                                                Debug.Log("Saved Animatronics");

                                                            }
                                                        }, code);
                                                    }
                                                }, code);
                                            }
                                        }, code);
                                    }
                                }, code);
                            }
                            else
                            {
                                Debug.Log("Failed Saving Structures");
                            }
                        }, code);
                    }else
                    {
                        SceneManager.LoadScene("MainMenu");
                    }
                });
                
                

            }
        }
        public IEnumerator UploadPositions()
        {
            while (!Loaded || !loadedPos)
            {
                yield return new WaitForEndOfFrame();
            }
            while (true)
            {
                while (!Loaded || !loadedPos)
                {
                    yield return new WaitForEndOfFrame();
                }
                bool done = false;
                if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
                {
                    DataStore.Set(Shared.myPositionKey(), $"{transform.position.x};{transform.position.y};{transform.position.z}\n{transform.eulerAngles.y}" ,true, cb => {
                        done = true;
                    });
                }
                while (!done)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(5f);
            }
            yield return null;
        }
        public void SaveStructures(Action<bool> SaveStatus, string saveCode = null, bool autoSave = false)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (string.IsNullOrEmpty(saveCode))
                    {
                        saveCode = Shared.RandomString(4);
                    }
                    BuiltWallMain[] walls = GameObject.FindObjectsOfType<BuiltWallMain>();
                    string save = $"{saveCode}\n";
                    uint i = 0;
                    foreach (BuiltWallMain w in walls)
                    {
                        w.StructureID = i;
                        string upgrades = "";
                        if (w.tag == "Wall")
                        {
                            string pattern = "";
                            string wallcol = "";

                            if (w.mainPattern != 0)
                            {
                                pattern = $"{w.mainPattern}";
                                wallcol = ColorUtility.ToHtmlStringRGB(w.mainColor);
                            }

                            string stripe = "";
                            string tiles = "";
                            string stripeo = "";
                            string stripeoutlinecol = "";
                            string stripeincol = "";
                            string stripetilcol = "";
                            if (w.hasStripe)
                            {
                                stripe = "true";
                                stripeo = $"{Math.Round(w.StripeOffset, 2)}";
                                stripeoutlinecol = ColorUtility.ToHtmlStringRGB(w.stripeBorderColor);
                                stripeincol = ColorUtility.ToHtmlStringRGB(w.stripeColor);
                                if (w.areTiles)
                                {
                                    tiles = "true";
                                    stripetilcol = ColorUtility.ToHtmlStringRGB(w.TilesColor);
                                }
                            }
                            

                            upgrades = $"{pattern};{wallcol};{stripe};{tiles};{stripeo};{stripeoutlinecol};{stripeincol};{stripetilcol}";

                        }
                        else if (w.tag == "Floor")
                        {
                            string pattern = "";
                            string floorcol = "";

                            if (w.mainPattern != 0)
                            {
                                pattern = $"{w.mainPattern}";
                                floorcol = ColorUtility.ToHtmlStringRGB(w.mainColor);
                            }
                            string tilescol = "";
                            if (w.mainPattern == 2)
                            {
                                tilescol = ColorUtility.ToHtmlStringRGB(w.TilesColor);
                            }


                            upgrades = $"{pattern};{floorcol};{tilescol}";

                        }
                        save = $"{save}{w.StructureID}|{w.type}|{w.Position.x};{w.Position.y};{w.Position.z}|{w.X_Scale}|{w.Z_Scale}|{w.Height}|{w.EmployeesOnly}|{upgrades}\n";
                        i++;
                    }
                    BuildManager.NextID = i;
                    string ad = "";
                    if (autoSave)
                    {
                        ad = "_autosave";
                    }
                    DataStore.SetSegmented(Shared.myStructuresKey() + ad, save, true, cb =>
                      {
                          if (cb)
                          {
                              Debug.Log("Structures Saved");
                              SaveStatus(true);
                          }
                          else
                          {
                              SaveStatus(false);
                          }
                      });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void SaveProps(Action<bool> SaveStatus, string saveCode = null, bool autoSave = false)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (string.IsNullOrEmpty(saveCode))
                    {
                        saveCode = Shared.RandomString(4);
                    }
                    PlacedProp[] props = GameObject.FindObjectsOfType<PlacedProp>();
                    string save = $"{saveCode}\n";
                    uint i = 0;
                    foreach (PlacedProp p in props)
                    {
                        p.ID= i;
                        string colors = $"";
                        int i2 = 0;
                        foreach (Color c in p.customColors)
                        {
                            if (i2 == 0)
                            {
                                colors = $"{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            else
                            {
                                colors = $"{colors};{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            i2++;
                        }
                        string propData = "";
                        MultipleTags tags = p.GetComponent<MultipleTags>();
                        if (p != null)
                        {
                            if (tags.has(CustomTags.ToppingsShelf))
                            {
                                ToppingsShelf ts = tags.GetComponent<ToppingsShelf>();
                                int i3 = 0;
                                int slotnumber = 0;
                                foreach(ToppingsShelfSlot sh in ts.slots)
                                {
                                    if (sh.what != Player.Baking.Pizza.PossibleToppings.Unknown)
                                    {
                                        string slot = $"{slotnumber};{(int)sh.what};{sh.howMany};{sh.when.ToString(dateformat)}";
                                        if (i3 == 0)
                                        {
                                            propData = slot;
                                        }
                                        else
                                        {
                                            propData = $"{propData},{slot}";
                                        }
                                        i3++;
                                    }
                                    slotnumber++;
                                }
                            }else if (tags.has(CustomTags.doughPot))
                            {
                                propData = $"{p.GetComponent<doughPotProp>().FillPercentage}";
                            }
                            else if (tags.has(CustomTags.curtain))
                            {
                                propData = $"{p.GetComponent<CurtainProp>().isOpened}";
                            }
                        }
                        save = $"{save}{p.ID}|{p.propname}|{p.Pos.x};{p.Pos.y};{p.Pos.z}|{p.Rot.x};{p.Rot.y};{p.Rot.z};{p.Rot.w}|{colors}|{propData}\n";
                        i++;
                    }
                    PropPlacing.nextID = i;
                    string ad = "";
                    if (autoSave)
                    {
                        ad = "_autosave";
                    }
                    DataStore.SetSegmented(Shared.myPropsKey() + ad, save, true, cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Props Saved");
                            SaveStatus(true);
                        }
                        else
                        {
                            SaveStatus(false);
                        }
                    });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void WaitAndSave(float time)
        {
            Invoke("SaveAll", time);
        }
        public void WaitFrameAndSave()
        {
            StartCoroutine(FrameAndSave());
        }
        IEnumerator FrameAndSave()
        {
            yield return new WaitForEndOfFrame();
            SaveAll();
            yield return null;
        }
        public void LoadStructures(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myStructuresKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                BuiltWallMain[] walls = GameObject.FindObjectsOfType<BuiltWallMain>();
                                string[] lines = cb.Trim().Split('\n');
                                foreach (BuiltWallMain bw in walls)
                                {
                                    Destroy(bw.gameObject);
                                }
                                foreach (string line in lines)
                                {
                                    if (line != lines[0])
                                    {
                                        string[] args = line.Trim().Split('|');
                                    //this is one of the walls.
                                        GameObject wall = Instantiate(WallPrefab, null);
                                        BuiltWallMain bw = wall.GetComponent<BuiltWallMain>();
                                        bw.StructureID = uint.Parse(args[0].Trim());
                                        bw.type = args[1].Trim();
                                        string[] PositionVec = args[2].Trim().Split(';');
                                        Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                        wall.transform.position = pos;
                                        bw.Position = pos;
                                        bw.X_Scale = float.Parse(args[3].Trim());
                                        bw.Z_Scale = float.Parse(args[4].Trim());
                                        bw.Height = float.Parse(args[5].Trim());
                                        if (args.Length > 6)
                                        {
                                            if (args[6].Trim().ToLower() == "true")
                                            {
                                                bw.EmployeesOnly = true;
                                            }
                                            else
                                            {
                                                bw.EmployeesOnly = false;
                                            }
                                        }
                                        if (bw.type.ToLower().Contains("wall"))
                                        {
                                            if (args.Length > 7)
                                            {
                                                string[] upgrades = args[7].Trim().Split(';');
                                                if (upgrades.Length > 0 && !string.IsNullOrEmpty(upgrades[0]))
                                                {
                                                    int pat = int.Parse(upgrades[0]);
                                                    bw.mainPattern = pat;
                                                    if (upgrades.Length > 1 && !string.IsNullOrEmpty(upgrades[1]))
                                                    {
                                                        Color mc = CustomizeColorEntry.hexToColor(upgrades[1].Trim());
                                                        bw.mainColor = mc;
                                                    }
                                                    else
                                                    {
                                                        bw.mainColor = Color.white;
                                                    }
                                                }
                                                else
                                                {
                                                    bw.mainPattern = 0;
                                                    bw.mainColor = Color.white;
                                                }
                                                if (upgrades.Length>2 && !string.IsNullOrEmpty(upgrades[2]) && upgrades[2].Trim().ToLower() == "true")
                                                {
                                                    bw.hasStripe = true;
                                                    if (upgrades.Length > 4 && !string.IsNullOrEmpty(upgrades[4].Trim()))
                                                    {
                                                        float of = (float)Math.Round(float.Parse(upgrades[4].Trim()), 2);
                                                        bw.StripeOffset = of;
                                                    }
                                                    if (upgrades.Length > 5 && !string.IsNullOrEmpty(upgrades[5].Trim()))
                                                    {
                                                        Color mc = CustomizeColorEntry.hexToColor(upgrades[5].Trim());
                                                        bw.stripeBorderColor = mc;
                                                    }
                                                    else
                                                    {
                                                        bw.stripeBorderColor = Color.black;
                                                    }
                                                    if (upgrades.Length >6 && !string.IsNullOrEmpty(upgrades[6].Trim()))
                                                    {
                                                        Color mc = CustomizeColorEntry.hexToColor(upgrades[6].Trim());
                                                        bw.stripeColor = mc;
                                                    }
                                                    else
                                                    {
                                                        bw.stripeColor = Color.white;
                                                    }
                                                    if (upgrades.Length > 3 && !string.IsNullOrEmpty(upgrades[3]) && upgrades[3].Trim().ToLower() == "true")
                                                    {
                                                        bw.areTiles = true;
                                                        if (upgrades.Length > 7 && !string.IsNullOrEmpty(upgrades[7].Trim()))
                                                        {
                                                            Color mc = CustomizeColorEntry.hexToColor(upgrades[7].Trim());
                                                            bw.TilesColor = mc;
                                                        }
                                                        else
                                                        {
                                                            bw.TilesColor = Color.black;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bw.areTiles = false;
                                                    }
                                                }
                                                else
                                                {
                                                    bw.hasStripe = false;
                                                    bw.areTiles = false;
                                                }
                                            }
                                        }
                                        else if (bw.type.ToLower().Contains("floor"))
                                        {
                                            if (args.Length > 7)
                                            {
                                                string[] upgrades = args[7].Trim().Split(';');
                                                if (upgrades.Length > 0 && !string.IsNullOrEmpty(upgrades[0]))
                                                {
                                                    int pat = int.Parse(upgrades[0]);
                                                    bw.mainPattern = pat;
                                                    if (pat != 0)
                                                    {
                                                        if (upgrades.Length > 1 && !string.IsNullOrEmpty(upgrades[1]))
                                                        {
                                                            Color mc = CustomizeColorEntry.hexToColor(upgrades[1].Trim());
                                                            bw.mainColor = mc;

                                                            if (upgrades.Length > 2 && !string.IsNullOrEmpty(upgrades[2]))
                                                            {
                                                                Color tc = CustomizeColorEntry.hexToColor(upgrades[2].Trim());
                                                                bw.TilesColor = tc;
                                                            }
                                                            else
                                                            {
                                                                bw.TilesColor = Color.black;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            bw.mainColor = Color.white;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bw.mainColor = Color.white;
                                                        bw.TilesColor = Color.black;
                                                    }
                                                }
                                                
                                            }
                                        }
                                        bw.Set();
                                        BuildManager.NextID = bw.StructureID + 1;
                                    }
                                    else
                                    {
                                        Debug.Log(line);
                                    //compare saves line (I will make a system that will detect if somebody did cut off his internet on purpose, to cheat half of his saves or sth)
                                }
                                }
                                LoadStatus(true);
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        public void LoadProps(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myPropsKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                PlacedProp[] props = GameObject.FindObjectsOfType<PlacedProp>();
                                string[] lines = cb.Trim().Split('\n');
                                foreach (PlacedProp pp in props)
                                {
                                    Destroy(pp.gameObject);
                                }
                                PropPlacing.nextID = 0;
                                foreach (string line in lines)
                                {
                                    if (line != lines[0])
                                    {
                                        string[] args = line.Trim().Split('|');
                                        GameObject sp = null;
                                        switch (args[1].Trim())
                                        {
                                            case "prop_table1":
                                                {
                                                    sp = propsPrefabs[0];
                                                    break;
                                                }
                                            case "prop_chair1":
                                                {
                                                    sp = propsPrefabs[1];
                                                    break;
                                                }
                                            case "prop_chair2_s1":
                                                {
                                                    sp = propsPrefabs[2];
                                                    break;
                                                }
                                            case "prop_chair2_s2":
                                                {
                                                    sp = propsPrefabs[3];

                                                    break;
                                                }
                                            case "prop_chair3_s1":
                                                {
                                                    sp = propsPrefabs[4];
                                                    break;
                                                }
                                            case "prop_chair3_s2":
                                                {
                                                    sp = propsPrefabs[5];
                                                    break;
                                                }
                                            case "prop_chair4_s1":
                                                {
                                                    sp = propsPrefabs[6];
                                                    break;
                                                }
                                            case "prop_chair4_s2":
                                                {
                                                    sp = propsPrefabs[7];
                                                    break;
                                                }
                                            case "prop_chair5_s1":
                                                {
                                                    sp = propsPrefabs[8];
                                                    break;
                                                }
                                            case "prop_chair5_s2":
                                                {
                                                    sp = propsPrefabs[9];
                                                    break;
                                                }
                                            case "prop_chair6":
                                                {
                                                    sp = propsPrefabs[10];
                                                    break;
                                                }
                                            case "prop_chair7":
                                                {
                                                    sp = propsPrefabs[11];
                                                    break;
                                                }
                                            case "prop_chair8":
                                                {
                                                    sp = propsPrefabs[12];
                                                    break;
                                                }
                                            case "prop_counter1":
                                                {
                                                    sp = propsPrefabs[13];
                                                    break;
                                                }
                                            case "prop_counter1b":
                                                {
                                                    sp = propsPrefabs[14];
                                                    break;
                                                }
                                            case "prop_cashregister1":
                                                {
                                                    sp = propsPrefabs[15];
                                                    break;
                                                }
                                            case "prop_kitchentable1":
                                                {
                                                    sp = propsPrefabs[16];
                                                    break;
                                                }
                                            case "prop_kitchendoughpot1":
                                                {
                                                    sp = propsPrefabs[17];
                                                    break;
                                                }
                                            case "prop_kitchentoppings":
                                                {
                                                    sp = propsPrefabs[18];
                                                    break;
                                                }
                                            case "prop_kitchenoven1":
                                                {
                                                    sp = propsPrefabs[19];
                                                    break;
                                                }
                                            case "prop_kitchenoven2":
                                                {
                                                    sp = propsPrefabs[20];
                                                    break;
                                                }
                                            case "prop_kitchenoven3":
                                                {
                                                    sp = propsPrefabs[21];
                                                    break;
                                                }
                                            case "prop_kitchencupboard1":
                                                {
                                                    sp = propsPrefabs[22];
                                                    break;
                                                }
                                            case "prop_kitchenmetaltable":
                                                {
                                                    sp = propsPrefabs[23];
                                                    break;
                                                }      
                                            case "prop_kitchenplateshelf":
                                                {
                                                    sp = propsPrefabs[24];
                                                    break;
                                                }
                                            case "prop_arcademachine1":
                                                {
                                                    sp = propsPrefabs[25];
                                                    break;
                                                }
                                            case "prop_balloons1":
                                                {
                                                    sp = propsPrefabs[26];
                                                    break;
                                                }
                                            case "prop_roundstage1":
                                                {
                                                    sp = propsPrefabs[27];
                                                    break;
                                                }
                                            case "prop_bigstage1":
                                                {
                                                    sp = propsPrefabs[28];
                                                    break;
                                                }
                                            case "prop_table2":
                                                {
                                                    sp = propsPrefabs[29];
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        if (sp != null)
                                        {
                                            GameObject prop = Instantiate(sp, null);
                                            PlacedProp p = prop.GetComponent<PlacedProp>();
                                            string[] PositionVec = args[2].Trim().Split(';');
                                            Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                            p.Pos = pos;
                                            string[] rotquat = args[3].Trim().Split(';');
                                            Quaternion rot = new Quaternion(float.Parse(rotquat[0].Trim()), float.Parse(rotquat[1].Trim()), float.Parse(rotquat[2].Trim()), float.Parse(rotquat[3].Trim()));

                                            p.Rot = rot;
                                            p.ID = PropPlacing.nextID;
                                            p.propname = args[1].Trim();
                                            if (args.Length > 4)
                                            {
                                                string[] ac = args[4].Trim().Split(';');
                                                List<Color> cols = new List<Color>();
                                                foreach (string c in ac)
                                                {
                                                    if (!string.IsNullOrEmpty(c.Trim()))
                                                    {
                                                        cols.Add(CustomizeColorEntry.hexToColor(c));
                                                    }
                                                }
                                                p.customColors = cols;
                                                p.SetColors();
                                                if (args.Length > 5)
                                                {
                                                    if (!string.IsNullOrEmpty(args[5]))
                                                    {
                                                        if (sp == propsPrefabs[18])
                                                        {
                                                            ToppingsShelf ts = p.GetComponent<ToppingsShelf>();
                                                            if (ts != null)
                                                            {
                                                                string[] slots = args[5].Trim().Split(',');
                                                                foreach (string slot in slots)
                                                                {
                                                                    string[] slotarg = slot.Trim().Split(';');
                                                                    int sslot = -999;
                                                                    int.TryParse(slotarg[0].Trim(), out sslot);
                                                                    if (sslot >= 0 && sslot < ts.slots.Count)
                                                                    {
                                                                        int tid = (int.Parse(slotarg[1].Trim()));
                                                                        if (tid > 0)
                                                                        {
                                                                            PossibleToppings top = (PossibleToppings)(tid);
                                                                            if (top != PossibleToppings.Unknown)
                                                                            {
                                                                                int hm = 0;
                                                                                int.TryParse(slotarg[2].Trim(), out hm);
                                                                                if (hm > 0)
                                                                                {
                                                                                    DateTime when = (DateTime.ParseExact(slotarg[3].Trim(), dateformat, null));
                                                                                    if (when.Year >= 2022)
                                                                                    {
                                                                                        ts.slots[sslot].SetWhat(top, hm, when, cb => { });
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }else if (sp == propsPrefabs[17])
                                                        {
                                                            int fill = int.Parse(args[5].Trim());
                                                            p.GetComponent<doughPotProp>().FillPercentage = fill;
                                                        }else if (sp == propsPrefabs[27] || sp == propsPrefabs[28])
                                                        {
                                                            bool on = args[5].ToLower().Trim() == "true";
                                                            p.GetComponent<CurtainProp>().isOpened = on;
                                                        }
                                                    }
                                                }
                                            }
                                            PropPlacing.nextID++;
                                            
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(line);
                                        //compare saves line (I will make a system that will detect if somebody did cut off his internet on purpose, to cheat half of his saves or sth)
                                    }
                                }
                                LoadStatus(true);
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        public void SaveLights(Action<bool> SaveStatus, string saveCode = null, bool autoSave = false)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (string.IsNullOrEmpty(saveCode))
                    {
                        saveCode = Shared.RandomString(4);
                    }
                    PlacedLight[] Lights = GameObject.FindObjectsOfType<PlacedLight>();
                    string save = $"{saveCode}\n";
                    uint i = 0;
                    foreach (PlacedLight l in Lights)
                    {
                        l.ID = i;
                        string colors = $"";
                        int i2 = 0;
                        foreach (Color c in l.myColors)
                        {
                            if (i2 == 0)
                            {
                                colors = $"{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            else
                            {
                                colors = $"{colors};{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            i2++;
                        }
                        
                        save = $"{save}{l.ID}|{l.lightname}|{l.Pos.x};{l.Pos.y};{l.Pos.z}|{l.Rot.x};{l.Rot.y};{l.Rot.z};{l.Rot.w}|{colors}|{l.enableShadows}|{l.FloorCulling}|{l.LightIntensity}|{l.Range}\n";
                        i++;
                    }
                    LightPlacing.nextID = i;
                    string ad = "";
                    if (autoSave)
                    {
                        ad = "_autosave";
                    }
                    DataStore.SetSegmented(Shared.myLightsKey() + ad, save, true, cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Lights Saved");
                            SaveStatus(true);
                        }
                        else
                        {
                            SaveStatus(false);
                        }
                    });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void LoadLights(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myLightsKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                PlacedLight[] lights = GameObject.FindObjectsOfType<PlacedLight>();
                                string[] lines = cb.Trim().Split('\n');
                                foreach (PlacedLight pp in lights)
                                {
                                    Destroy(pp.gameObject);
                                }
                                LightPlacing.nextID = 0;
                                foreach (string line in lines)
                                {
                                    if (line != lines[0])
                                    {
                                        string[] args = line.Trim().Split('|');
                                        GameObject sp = null;
                                        switch (args[1].Trim())
                                        {
                                            case "lamp_classicbulb":
                                                {
                                                    sp = lampPrefabs[0];
                                                    break;
                                                }
                                            case "lamp_ceilingflat":
                                                {
                                                    sp = lampPrefabs[1];
                                                    break;
                                                }
                                            case "lamp_wfnaf":
                                                {
                                                    sp = lampPrefabs[2];
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        if (sp != null)
                                        {
                                            GameObject light = Instantiate(sp, null);
                                            PlacedLight l = light.GetComponent<PlacedLight>();
                                            string[] PositionVec = args[2].Trim().Split(';');
                                            Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                            l.Pos = pos;
                                            string[] rotquat = args[3].Trim().Split(';');
                                            Quaternion rot = new Quaternion(float.Parse(rotquat[0].Trim()), float.Parse(rotquat[1].Trim()), float.Parse(rotquat[2].Trim()), float.Parse(rotquat[3].Trim()));

                                            l.Rot = rot;
                                            l.ID = LightPlacing.nextID;
                                            l.lightname = args[1].Trim();
                                            if (args.Length > 4)
                                            {
                                                string[] ac = args[4].Trim().Split(';');
                                                List<Color> cols = new List<Color>();
                                                foreach (string c in ac)
                                                {
                                                    if (!string.IsNullOrEmpty(c.Trim()))
                                                    {
                                                        cols.Add(CustomizeColorEntry.hexToColor(c));
                                                    }
                                                }
                                                l.myColors = cols.ToArray();
                                            }
                                            if (args.Length > 5 && args[5].Trim().ToLower()=="true")
                                            {
                                                l.enableShadows = true;
                                            }
                                            else
                                            {
                                                l.enableShadows = false;

                                            }
                                            if (args.Length > 6 && args[6].Trim().ToLower() == "true")
                                            {
                                                l.FloorCulling = true;
                                            }
                                            else
                                            {
                                                l.FloorCulling = false;

                                            }
                                            if (args.Length > 7)
                                            {
                                                l.LightIntensity = MathF.Round(float.Parse(args[7].Trim()), 2);
                                            }
                                            if (args.Length > 8)
                                            {
                                                l.Range = MathF.Round(float.Parse(args[8].Trim()), 2);
                                            }
                                            l.Set();
                                            LightPlacing.nextID++;

                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(line);
                                        //compare saves line (I will make a system that will detect if somebody did cut off his internet on purpose, to cheat half of his saves or sth)
                                    }
                                }
                                LoadStatus(true);
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        public static void checkTimeOut()
        {
            if (GameJoltAPI.Instance.timedOut)
            {
                SceneManager.LoadScene("MainMenu");
                GameJoltAPI.Instance.timedOut = false;

            }
        }
        public void SaveDoors(Action<bool> SaveStatus, string saveCode = null, bool autoSave = false)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (string.IsNullOrEmpty(saveCode))
                    {
                        saveCode = Shared.RandomString(4);
                    }
                    PlacedDoor[] Doors = GameObject.FindObjectsOfType<PlacedDoor>();
                    string save = $"{saveCode}\n";
                    uint i = 0;
                    foreach (PlacedDoor d in Doors)
                    {
                        d.ID = i;
                        string colors = $"";
                        int i2 = 0;
                        foreach (Color c in d.myColors)
                        {
                            if (i2 == 0)
                            {
                                colors = $"{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            else
                            {
                                colors = $"{colors};{ColorUtility.ToHtmlStringRGB(c)}";
                            }
                            i2++;
                        }

                        save = $"{save}{d.ID}|{d.doorname}|{d.Pos.x};{d.Pos.y};{d.Pos.z}|{d.Rot.x};{d.Rot.y};{d.Rot.z};{d.Rot.w}|{d.Scale.x};{d.Scale.y};{d.Scale.z}|{colors}|{d.employees}\n";
                        i++;
                    }
                    DoorPlacing.nextID = i;
                    string ad = "";
                    if (autoSave)
                    {
                        ad = "_autosave";
                    }
                    DataStore.SetSegmented(Shared.myDoorsKey() + ad, save, true, cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Doors Saved");
                            SaveStatus(true);
                        }
                        else
                        {
                            SaveStatus(false);
                        }
                    });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void SaveAnimatronics(Action<bool> SaveStatus, string saveCode = null, bool autoSave = false)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (string.IsNullOrEmpty(saveCode))
                    {
                        saveCode = Shared.RandomString(4);
                    }
                    PlacedAnimatronic[] ans = GameObject.FindObjectsOfType<PlacedAnimatronic>();
                    string save = $"{saveCode}\n";
                    uint i = 0;
                    foreach (PlacedAnimatronic d in ans)
                    {
                        d.ID = i;
                        save = $"{save}{d.ID}|{d.charname}|{d.Pos.x};{d.Pos.y};{d.Pos.z}|{d.Rot.x};{d.Rot.y};{d.Rot.z};{d.Rot.w}\n";
                        i++;
                    }
                    AnimatronicPlacing.nextID = i;
                    string ad = "";
                    if (autoSave)
                    {
                        ad = "_autosave";
                    }
                    DataStore.SetSegmented(Shared.myAnimatronicsKey() + ad, save, true, cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Animatronics Saved");
                            SaveStatus(true);
                        }
                        else
                        {
                            SaveStatus(false);
                        }
                    });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void SaveMenu(Action<bool> SaveStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser && MyMenu.Instance != null && MyMenu.Instance.Menu != null)
                {

                    string save = "";
                    foreach (Pizza p in MyMenu.Instance.Menu)
                    {
                        string toppings = "";
                        int i2 = 0;
                        foreach (PossibleToppings top in p.Toppings)
                        {
                            if (i2 == 0)
                            {
                                toppings = $"{(int)top}";
                            }
                            else
                            {
                                toppings = $"{toppings};{(int)top}";
                            }
                            i2++;
                        }

                        save = $"{save}{p.Name}|{p.Price}|{toppings}\n";
                    }
                    DataStore.SetSegmented(Shared.myMenuKey(), save, true, cb =>
                    {
                        if (cb)
                        {
                            Debug.Log("Pizzeria Menu Saved");
                            SaveStatus(true);
                        }
                        else
                        {
                            SaveStatus(false);
                        }
                    });
                }
                else
                {
                    SaveStatus(false);
                }
            }
            else
            {
                SaveStatus(false);
            }
        }
        public void LoadMenu(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myMenuKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                if (MyMenu.Instance != null)
                                {
                                    string[] lines = cb.Trim().Split('\n');
                                    MyMenu.Instance.Menu.Clear();
                                    foreach (string line in lines)
                                    {

                                        string[] args = line.Trim().Split('|');
                                        Pizza p = new Pizza();
                                        p.Name = args[0].Trim();
                                        p.Price = int.Parse(args[1].Trim());
                                        string[] tops = args[2].Trim().Split(';');
                                        List<PossibleToppings> toppings = new List<PossibleToppings>();
                                        foreach (string t in tops)
                                        {
                                            int e = int.Parse(t.Trim());
                                            PossibleToppings ct = (PossibleToppings)e;
                                            if (ct != PossibleToppings.Unknown)
                                            {
                                                toppings.Add(ct);
                                            }
                                        }
                                        p.Toppings = toppings;
                                        MyMenu.Instance.Menu.Add(p);

                                    }
                                    LoadStatus(true);
                                }
                                else
                                {
                                    LoadStatus(false);
                                }
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        public void LoadDoors(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myDoorsKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                PlacedDoor[] doors = GameObject.FindObjectsOfType<PlacedDoor>();
                                string[] lines = cb.Trim().Split('\n');
                                foreach (PlacedDoor d in doors)
                                {
                                    Destroy(d.gameObject);
                                }
                                DoorPlacing.nextID = 0;
                                foreach (string line in lines)
                                {
                                    if (line != lines[0])
                                    {
                                        string[] args = line.Trim().Split('|');
                                        GameObject sp = null;
                                        switch (args[1].Trim())
                                        {
                                            case "door_door1":
                                                {
                                                    sp = doorPrefabs[0];
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        if (sp != null)
                                        {
                                            GameObject door = Instantiate(sp, null);
                                            PlacedDoor d = door.GetComponent<PlacedDoor>();
                                            string[] PositionVec = args[2].Trim().Split(';');
                                            Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                            d.Pos = pos;
                                            string[] rotquat = args[3].Trim().Split(';');
                                            Quaternion rot = new Quaternion(float.Parse(rotquat[0].Trim()), float.Parse(rotquat[1].Trim()), float.Parse(rotquat[2].Trim()), float.Parse(rotquat[3].Trim()));
                                            string[] ScaleVec = args[4].Trim().Split(';');
                                            d.Rot = rot;
                                            Vector3 sc = new Vector3(float.Parse(ScaleVec[0].Trim()), float.Parse(ScaleVec[1].Trim()), float.Parse(ScaleVec[2].Trim()));
                                            d.Scale = sc;
                                            d.ID = DoorPlacing.nextID;
                                            d.doorname = args[1].Trim();
                                            if (args.Length > 5)
                                            {
                                                string[] ac = args[5].Trim().Split(';');
                                                List<Color> cols = new List<Color>();
                                                foreach (string c in ac)
                                                {
                                                    if (!string.IsNullOrEmpty(c.Trim()))
                                                    {
                                                        cols.Add(CustomizeColorEntry.hexToColor(c));
                                                    }
                                                }
                                                d.myColors = cols.ToArray();
                                            }
                                            if (args.Length > 6 && args[6].Trim().ToLower() == "true")
                                            {
                                                d.employees = true;
                                            }
                                            else
                                            {
                                                d.employees = false;

                                            }
                                           
                                            d.Set();
                                            DoorPlacing.nextID++;

                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(line);
                                        //compare saves line (I will make a system that will detect if somebody did cut off his internet on purpose, to cheat half of his saves or sth)
                                    }
                                }
                                LoadStatus(true);
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        public void LoadAnimatronics(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.myAnimatronicsKey(), true, cb =>
                    {
                        if (cb != null)
                        {
                            if (!string.IsNullOrEmpty(cb))
                            {
                                PlacedAnimatronic[] anms = GameObject.FindObjectsOfType<PlacedAnimatronic>();
                                string[] lines = cb.Trim().Split('\n');
                                foreach (PlacedAnimatronic a in anms)
                                {
                                    Destroy(a.gameObject);
                                }
                                AnimatronicPlacing.nextID = 0;
                                foreach (string line in lines)
                                {
                                    if (line != lines[0])
                                    {
                                        string[] args = line.Trim().Split('|');
                                        GameObject sp = null;
                                        switch (args[1].Trim())
                                        {
                                            case "anm_freddy1":
                                                {
                                                    sp = anmPrefabs[0];
                                                    break;
                                                }
                                            case "anm_bonnie1":
                                                {
                                                    sp = anmPrefabs[1];
                                                    break;
                                                }
                                            default:
                                                {
                                                    break;
                                                }
                                        }
                                        if (sp != null)
                                        {
                                            GameObject an = Instantiate(sp, null);
                                            PlacedAnimatronic d = an.GetComponent<PlacedAnimatronic>();
                                            d.charname = $"{args[1].Trim()}";
                                            string[] PositionVec = args[2].Trim().Split(';');
                                            Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                            d.Pos = pos;
                                            string[] rotquat = args[3].Trim().Split(';');
                                            Quaternion rot = new Quaternion(float.Parse(rotquat[0].Trim()), float.Parse(rotquat[1].Trim()), float.Parse(rotquat[2].Trim()), float.Parse(rotquat[3].Trim()));
                                            d.Rot = rot;
                                            d.Set();
                                            AnimatronicPlacing.nextID++;

                                        }
                                    }
                                    else
                                    {
                                        Debug.Log(line);
                                        //compare saves line (I will make a system that will detect if somebody did cut off his internet on purpose, to cheat half of his saves or sth)
                                    }
                                }
                                LoadStatus(true);
                            }
                            else
                            {
                                LoadStatus(false);
                            }
                        }
                        else
                        {
                            LoadStatus(false);
                        }
                    });
                }
                else
                {
                    LoadStatus(false);
                }
            }
            else
            {
                LoadStatus(false);
            }
        }
        void LoadOH()
        {
            if (PlayerPrefs.HasKey("oh") && PlayerPrefs.HasKey("ch"))
            {
                int oh = PlayerPrefs.GetInt("oh");
                int ch = PlayerPrefs.GetInt("ch");

                if (ch > oh)
                {
                    if (oh >= 6 && ch >= 7)
                    {
                        if (oh < 24 && ch <= 24)
                        {
                            if (ThePizzeria.Instance != null)
                            {
                                ThePizzeria.Instance.OpenHour = oh;
                                ThePizzeria.Instance.ClosingHour = ch;
                            }
                        }
                    }
                }
            }
        }
    }

}