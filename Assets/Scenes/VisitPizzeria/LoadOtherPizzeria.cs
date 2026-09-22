using GameJolt.API;
using LyviN.Map.Animatronics;
using LyviN.Map.Props;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.Map.Visiting
{
    public class LoadOtherPizzeria : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int VISIT_PLAYER_TROPHY_ID = 165485; // Visit another player's pizzeria trophy
        
        public static LoadOtherPizzeria Instance;
        public static string otherPlayerID;
        public static string otherPlayerName;
        public GameObject WallPrefab;
        public GameObject[] propsPrefabs;
        public GameObject[] lampPrefabs;
        public GameObject[] doorPrefabs;
        public GameObject[] anmPrefabs;
        public bool Loaded;
        public bool bakedNav;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                StartCoroutine(StartLoading());
            }
            else
            {
                enabled = false;
            }
        }
        public IEnumerator StartLoading()
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

                                        }
                                        else if (GameJoltAPI.Instance.timedOut)
                                        {
                                            SceneManager.LoadScene("MainMenu");
                                            GameJoltAPI.Instance.timedOut = false;
                                        }
                                        else
                                        {
                                            Debug.Log("DID NOT LOAD ANIMATRONICS");
                                        }
                                        Loaded = true;
                                        GameJolt.API.Trophies.TryUnlock(VISIT_PLAYER_TROPHY_ID, cb => {});
                                        if (DiscordController.Instance != null)
                                        {
                                            DiscordController.Instance.SetActivity("In Game", "Visiting other player.", true);
                                        }
                                    });
                                });



                            });
                        });

                    });
                }
            }
        }
        public void LoadAnimatronics(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.OtherAnimatronicsKey(otherPlayerID), true, cb =>
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
                                            string[] PositionVec = args[2].Trim().Split(';');
                                            Vector3 pos = new Vector3(float.Parse(PositionVec[0].Trim()), float.Parse(PositionVec[1].Trim()), float.Parse(PositionVec[2].Trim()));
                                            d.Pos = pos;
                                            string[] rotquat = args[3].Trim().Split(';');
                                            Quaternion rot = new Quaternion(float.Parse(rotquat[0].Trim()), float.Parse(rotquat[1].Trim()), float.Parse(rotquat[2].Trim()), float.Parse(rotquat[3].Trim()));
                                            d.Rot = rot;
                                            d.Set();
                                        }
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
        public void LoadLights(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.OtherLightsKey(otherPlayerID), true, cb =>
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
                                            if (args.Length > 5 && args[5].Trim().ToLower() == "true")
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
        public void LoadDoors(Action<bool> LoadStatus)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    string save = $"";
                    DataStore.Get(Shared.OtherDoorsKey(otherPlayerID), true, cb =>
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
        public void LoadStructures(Action<bool> LoadStatus)
        {
            if (!string.IsNullOrEmpty(otherPlayerID))
            {
                if (GameJoltAPI.myInstance != null)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser)
                    {
                        DataStore.Get($"{Shared.OtherStructuresKey(otherPlayerID)}", true, cb =>
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
                                                    if (upgrades.Length > 2 && !string.IsNullOrEmpty(upgrades[2]) && upgrades[2].Trim().ToLower() == "true")
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
                                                        if (upgrades.Length > 6 && !string.IsNullOrEmpty(upgrades[6].Trim()))
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
            else
            {
                LoadStatus(false);
            }
        }
        public void LoadProps(Action<bool> LoadStatus)
        {
            if (!string.IsNullOrEmpty(otherPlayerID))
            {
                if (GameJoltAPI.myInstance != null)
                {
                    if (GameJoltAPI.Instance.HasSignedInUser)
                    {
                        DataStore.Get($"{Shared.OtherPropsKey(otherPlayerID)}", true, cb =>
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
                                                                                        DateTime when = (DateTime.ParseExact(slotarg[3].Trim(), SaveSystem.dateformat, null));
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
                                                            }
                                                            else if (sp == propsPrefabs[17])
                                                            {
                                                                int fill = int.Parse(args[5].Trim());
                                                                p.GetComponent<doughPotProp>().FillPercentage = fill;
                                                            }
                                                            else if (sp == propsPrefabs[27] || sp == propsPrefabs[28])
                                                            {
                                                                bool on = args[5].ToLower().Trim() == "true";
                                                                p.GetComponent<CurtainProp>().isOpened = on;
                                                            }
                                                        }
                                                    }
                                                }
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
    }
}