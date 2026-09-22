using GameJolt.API;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class fixingnewplayrs : MonoBehaviour
{
    public TextMeshProUGUI logs;
    public Toggle loopToggle;
    public Toggle clearToggle;
    bool isloop;
    private void Start()
    {
        Application.targetFrameRate = 24;
        logs.text = $"";
        log("Welcome. LyviN can't make a game that isn't broken, so you need to fix it.");
    }
    public void Fix()
    {
        if (GameJoltAPI.myInstance != null)
        {
            
            StartCoroutine(fixing());
        }
    }
    public void StartLoop()
    {
        if (loopToggle.isOn)
        {
            if (!isloop)
            {
                log($"Starting the loop.");
                StartCoroutine(FixLoop());
            }
        }
    }
    IEnumerator FixLoop()
    {
        while(loopToggle.isOn)
        {
            if (loopToggle.isOn)
            {
                StartCoroutine(fixingFILL());
                if (clearToggle.isOn)
                {
                    logs.text = $"";
                }
            }
            for (int i = 3; i > 0; i--)
            {
                if (loopToggle.isOn)
                {
                    yield return new WaitForSeconds(5f);
                    if (i != 1)
                    {
                        log($"{(i - 1) * 5}");
                    }
                    if (!loopToggle.isOn)
                    {
                        log("Loop cancelled. You can start a new one");

                    }
                }
                else
                {
                    log("Loop cancelled. You can start a new one");
                    i = 0;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void Fill()
    {
        if (GameJoltAPI.myInstance != null)
        {
            StartCoroutine(givemaxall());
        }
    }
    public void FixFill()
    {
        if (GameJoltAPI.myInstance != null)
        {
            StartCoroutine(fixingFILL());
        }
    }
    IEnumerator fixing()
    {
        string fixingp = "";
        bool wait = false;
        log($"Starting Fixing Process. Don't quit the program.");
        DataStore.GetKeys(true, $"*_id", cb =>
        {
            log($"Found {cb.Length} players in total. If nothing happens now, it means every player is currently fixed, or even this program is broken.");
            if (cb.Length > 0)
            {
                foreach (string p in cb)
                {
                    string pname = p.Trim().Replace($"_id", "").Replace("userdata_", "");
                    DataStore.Get($"{p}", true, pid =>
                    {
                        int broken = 0;

                        string toppingskey = $"userdata_{pid.Trim()}_save_toppings";

                        DataStore.Get($"{toppingskey}", true, tcd =>
                        {
                            if (tcd == null && !GameJoltAPI.Instance.timedOut)
                            {
                                broken++;
                                log($"<color=\"red\">Player <color=\"yellow\">{pname}</color> - <color=\"red\">Toppings Data Missing. STARTING THE FIXING PROCESS.</color><color=\"white\">");
                                DataStore.Set(toppingskey, $"1|10", true, setcb => {
                                    log($"<color=\"yellow\">{pname}</color>'s toppings set: <color=\"green\">{setcb}</color>");   
                                });
                            }

                      });

                    });
                }
            }
        });
        yield return null;
    }
    IEnumerator givemaxall()
    {
        string save =
            $"1|255\n" +
            $"2|255\n" +
            $"3|255\n" +
            $"4|255\n" +
            $"5|255\n" +
            $"6|255\n" +
            $"7|255\n" +
            $"8|255\n" +
            $"9|255\n" +
            $"10|255\n" +
            $"11|255\n" +
            $"12|255\n" +
            $"13|255\n" +
            $"14|255\n" +
            $"15|255\n" +
            $"16|255\n" +
            $"17|255\n";
        log($"Starting FILLING Process. Don't quit the program.");
        DataStore.GetKeys(true, $"*_id", cb =>
        {
            log($"Found {cb.Length} players in total. If nothing happens now, it means this program is broken.");
            if (cb.Length > 0)
            {
                foreach (string p in cb)
                {
                    string pname = p.Trim().Replace($"_id", "").Replace("userdata_", "");
                    DataStore.Get($"{p}", true, pid =>
                    {
                        string toppingskey = $"userdata_{pid.Trim()}_save_toppings";

                        
                            if (!GameJoltAPI.Instance.timedOut)
                            {
                                DataStore.Set(toppingskey, save, true, setcb => {
                                    log($"<color=\"yellow\">{pname}</color>'s toppings <color=\"blue\">FILLED: <color=\"green\">{setcb}</color><color=\"white\">");
                                });
                            }
                    });
                }
            }
        });
        yield return null;
    }
    IEnumerator fixingFILL()
    {
        string save =
            $"1|255\n" +
            $"2|255\n" +
            $"3|255\n" +
            $"4|255\n" +
            $"5|255\n" +
            $"6|255\n" +
            $"7|255\n" +
            $"8|255\n" +
            $"9|255\n" +
            $"10|255\n" +
            $"11|255\n" +
            $"12|255\n" +
            $"13|255\n" +
            $"14|255\n" +
            $"15|255\n" +
            $"16|255\n" +
            $"17|255\n";
        log($"Starting Fixing FILLING Process. Don't quit the program.");
        DataStore.GetKeys(true, $"*_id", cb =>
        {
            log($"Found {cb.Length} players in total. If nothing happens now, it means every player is currently fixed, or even this program is broken.");
            if (cb.Length > 0)
            {
                foreach (string p in cb)
                {
                    string pname = p.Trim().Replace($"_id", "").Replace("userdata_", "");
                    DataStore.Get($"{p}", true, pid =>
                    {
                        int broken = 0;

                        string toppingskey = $"userdata_{pid.Trim()}_save_toppings";

                        DataStore.Get($"{toppingskey}", true, tcd =>
                        {
                            if (tcd == null && !GameJoltAPI.Instance.timedOut)
                            {
                                broken++;
                                log($"<color=\"red\">Player <color=\"yellow\">{pname}</color> - <color=\"red\">Toppings Data Missing. STARTING THE FIXING PROCESS.</color><color=\"white\">");
                                DataStore.Set(toppingskey, save, true, setcb => {
                                    log($"<color=\"yellow\">{pname}</color>'s toppings set FILLED: <color=\"green\">{setcb}</color>");
                                });
                            }

                        });

                    });
                }
            }
        });
        yield return null;
    }
    public void log(string log)
    {
        DateTime t = DateTime.Now;
        logs.text += $"\n[{t.ToLongTimeString()}] {log}";
    }
}
