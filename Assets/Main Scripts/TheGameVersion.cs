using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using GameJolt.API;
namespace LyviN.Misc
{
    public class TheGameVersion : MonoBehaviour
    {
        public TextMeshProUGUI VersionText;
        public static int Major;
        public static int Minor;
        public static int Patch; 
        
        public static int rMajor;
        public static int rMinor;
        public static int rPatch;
        public static string verString;

        public GameObject updateBoxO;
        public TextMeshProUGUI updateBoxText;
        public GameObject buttonsO;
        public static bool Shown;
        bool loadingu;
        private void Start()
        {
            verString = Application.version;
            VersionText.text = $"v{verString}";

            string[] vers = verString.Trim().Split('.');
            Major = int.Parse(vers[0].Trim());
            Minor = int.Parse(vers[1].Trim());
            Patch = int.Parse(vers[2].Trim());
        }
        private void Update()
        {
            if (!loadingu && GameJoltAPI.myInstance != null)
            {
                loadingu = true;
                CheckUpdate();
            }
        }
        public void CheckUpdate()
        {
            if (GameJoltAPI.myInstance != null)
            {
                DataStore.Get($"system_version", true, vercb => 
                { 
                    if (vercb != null)
                    {
                        string[] lines = vercb.Trim().Split('\n');
                        foreach(string l in lines)
                        {
                            string[] vers = l.Trim().Split('.');
                            rMajor = int.Parse(vers[0].Trim());
                            rMinor = int.Parse(vers[1].Trim());
                            rPatch = int.Parse(vers[2].Trim());
                            if (rMajor + rMinor + rPatch > 0)
                            {
                                if (rMajor > Major)
                                {
                                    newUpdateAvailable();
                                }
                                else if (rMajor == Major)
                                {
                                    if (rMinor > Minor)
                                    {
                                        newUpdateAvailable();
                                    }
                                    else if (rMinor == Minor)
                                    {
                                        if (rPatch > Patch)
                                        {
                                            newUpdateAvailable();
                                        }
                                        else if (rPatch == Patch)
                                        {
                                            sameVersion();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        VersionText.text = $"<color=red>v{verString}\nGameJolt Connection Problem.";

                    }
                });
            }
        }
        void newUpdateAvailable()
        {
            VersionText.text = $"<color=red>v{verString}\nA new update is available.";
            DataStore.Get($"system_changelog_{rMajor}{rMinor}{rPatch}", true, cb => {
                string changelogs = "No changelogs for this version are available.";
                if (cb != null && !string.IsNullOrEmpty(cb.Trim()))
                {
                    changelogs = cb.Trim();
                }
                if (!Shown)
                {
                    buttonsO.SetActive(false);
                    updateBoxO.SetActive(true);
                    updateBoxText.text = $"A NEW UPDATE IS AVAILABLE\n\n<align=\"left\"><size=70%>By updating your game, you make sure you're playing a safer and more polished version. It's really important to keep up with the updates. Please, download the new version.\n{changelogs}";
                }
            });
            
        }
        void sameVersion()
        {
            VersionText.text = $"v{verString}\nYour game is up to date.";
        }
    }
}
