using GameJolt.API;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyProfileTemp : MonoBehaviour
{
    public TextMeshProUGUI mynameT;   
    public string myname;   
    public Sprite mypfp;
    public Image mypfpI;
    bool started = false;
    // Start is called before the first frame update
    public void Start()
    {
        mypfpI.enabled = false;
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            GameJoltAPI.Instance.CurrentUser.DownloadAvatar(av => { 
                if (av)
                {
                    Debug.Log("Avatar Fetched in Game");
                    mypfp = GameJoltAPI.Instance.CurrentUser.Avatar;
                    mypfpI.enabled = true;
                }
                else
                {
                    Debug.Log("Avatar ESC Fetch Failed");
                    mypfpI.enabled = false;
                }
            });
            myname = GameJoltAPI.Instance.CurrentUser.DeveloperName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            if (GameJoltAPI.Instance.CurrentUser.ID != 0)
            {
                if (!started)
                {
                    Start();
                    started = true;
                    Debug.Log("eh");
                }
            }
        }
        mynameT.text = myname;
        mypfpI.sprite = mypfp;

    }
}
