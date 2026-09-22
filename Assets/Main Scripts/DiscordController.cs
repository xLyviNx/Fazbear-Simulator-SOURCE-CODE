using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
    public static DiscordController Instance;
    public Discord.Discord discordObject;

    public bool rpcEnabled = false;

    public string cDetails;
    public string cState;

    //OPEN SOURCE TODO: assign Discord App Id
    private const long DISCORD_APP_ID = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (DISCORD_APP_ID == 0)
        {
            this.enabled = false;
            return;
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            cDetails = $"In Game";
            cState = "Main Menu";
            if (discordObject == null)
            {
                try
                {
                    Debug.Log("Discord: trying to connect");
                    discordObject = new Discord.Discord(DISCORD_APP_ID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
                    Debug.Log("Discord: connected");

                    SetActivity("In Game", "Main Menu", true);
                }
                catch (Exception DiscordConnEx)
                {
                    Debug.LogWarning("Discord: cannot connect, probably not opened " + DiscordConnEx);
                }

            }
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    public void SetActivity(string details, string state, bool timestamp = false)
    {
        if (DISCORD_APP_ID == 0 || discordObject == null) return;

        cDetails = details;
        cState = state;
        var activityManager = discordObject.GetActivityManager();
        Discord.Activity activity = new Discord.Activity
        {
            Details = cDetails,
            State = cState,
            Type = ActivityType.Playing,
            Assets =
            {
                LargeImage = "icon",
            },
            Instance = true,
        };
        if (timestamp)
        {
            activity.Timestamps = new ActivityTimestamps
            {
                Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };
        }
        activityManager.UpdateActivity(activity, (result) =>
        {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Discord: default status set");
            }
            else
            {
                Debug.LogWarning("Discord: status NOT set");
            }
        });
    }
    void Reconnect()
    {
        if (DISCORD_APP_ID == 0) return;

        try
        {
            
            
            Debug.Log("Discord: trying to reconnect");
            discordObject = new Discord.Discord(DISCORD_APP_ID, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
            Debug.Log("Discord: reconnected");

            var activityManager = discordObject.GetActivityManager();
            var activity = new Discord.Activity
            {
                Details = cDetails,
                State = cState
        };

            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result == Discord.Result.Ok)
                {
                    Debug.Log("Discord: default status set");
                }
                else
                {
                    Debug.LogWarning("Discord: default status NOT set");
                }
            });
        }
        catch (Exception DiscordConnEx)
        {
            Debug.LogWarning("Discord: cannot reconnect, probably still not opened " + DiscordConnEx);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DISCORD_APP_ID == 0) return;

        if (!rpcEnabled)
        {
            //Destroy(this);
            this.enabled = false;
        }
        else
        {
            this.enabled = true; // well, it cant work bcuz script dead already
        }

        if (discordObject != null)
        {
            try
            {
                discordObject.RunCallbacks();
            }
            catch (Exception RunCallbacksEx)
            {
                Debug.LogWarning("Discord: cannot run callbacks anymore, probably got turned off " + RunCallbacksEx);
                discordObject = null;
            }
        }
        else
        {
            Reconnect();
        }
        
    }
    private void OnDisable()
    {
        reset();
    }
    private void OnDestroy()
    {
        reset();
    }
    private void reset()
    {
        if (discordObject != null)
        {
            var activityManager = discordObject.GetActivityManager();
            activityManager.ClearActivity(cb => { });
        }
    }
    //private void OnDisable()
    //{
    //    discordObject.Dispose();
    //    Debug.Log("Test");
    //    var activityManager = discordObject.GetActivityManager();
    //    activityManager.ClearActivity((result) =>
    //    {
    //        if (result == Discord.Result.Ok)
    //        {
    //            Debug.Log("Discord activity cleared!");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Discord activity NOT cleared!");
    //        }
    //        Debug.Log("Test1");
    //    });

    //}
}