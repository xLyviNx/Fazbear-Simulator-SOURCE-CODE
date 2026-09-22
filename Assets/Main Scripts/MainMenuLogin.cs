using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using GameJolt.API;
using LyviN.Input;

public class MainMenuLogin : MonoBehaviour
{
    //OPEN SOURCE TODO: Assign your own Login Trophy ID
    public const int LOGIN_TROPHY_ID = 165198;
    
    public GameObject loggedInPanel;
    public GameObject logInPanel;

    public TMP_InputField usernameField;
    public TMP_InputField tokenField;

    public Button logInButton;
    public Button logOutButton;

    public TMP_Text usernameText;
    public Image avatarImage;
    public Toggle RememberToggle;
    // Start is called before the first frame update
    void Start()
    {
        if (GameJoltAPI.myInstance != null)
        {
            GameJoltAPI.Instance.ClearUserCredentials();
            if (GameJoltAPI.Instance.HasSignedInUser)
            {
                loggedInSet(false);
                loggedInSet(true);
            }
            else
            {
                Shared.GetGameJoltCredentials(out string username, out string token);
                Debug.Log($"EEE: {username}, {string.IsNullOrEmpty(token)}");
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(token))
                {
                    logIn(true, username, token);
                    Debug.Log("Logging in automatically.");
                }
                

            }
        }
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.isGameScene = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LogInWithInput()
    {
        logIn();
    }
    public void logIn(bool autologin = false, string name = "", string token = "")
    {
        Debug.Log("Attempting Login...");

        if ((string.IsNullOrEmpty(usernameField.text.Trim()) || string.IsNullOrEmpty(tokenField.text.Trim())) && !autologin)
        {
            Debug.LogWarning("Login failed, username or token empty");
        }
        else
        {
            avatarImage.enabled = false;
            if (!RememberToggle.isOn && !autologin)
            {
                Shared.ClearGameJoltCredentials();
            }
            string usrname = usernameField.text.Trim();
            string tok = tokenField.text.Trim();
            if (autologin)
            {
                usrname = name;
                tok = token;
            }
            var user = new GameJolt.API.Objects.User(usrname, tok);

            user.SignIn(
                (bool success) =>
                {
                    if (success)
                    {
                        if (RememberToggle.isOn)
                        {
                            Shared.SetGameJoltCredentials(usrname, tok);
                            Debug.Log("I remember");
                        }

                        loggedInSet(false);
                        //avatarImage.sprite = GameJoltAPI.Instance.CurrentUser.Avatar;
                        //avatarImage.sprite = Sprite.Create(GameJoltAPI.Instance.CurrentUser.Avatar.texture, new Rect(0,0, GameJoltAPI.Instance.CurrentUser.Avatar.texture.height, GameJoltAPI.Instance.CurrentUser.Avatar.texture.width), new Vector2(0,0));
                        // GameJoltAPI.Instance.CurrentUser.Avatar;

                        Debug.Log("Login successful");
                        Trophies.TryUnlock(
                            LOGIN_TROPHY_ID,
                            unlockResult => { });
                    }
                },
                (bool fetch) =>
                {
                    if (fetch)
                    {
                        Debug.Log("GameJolt Fetched");
                        StartCoroutine(WSM());
                    }
                    else
                    {
                        Debug.LogError("GaneJolt Fetch FAILED! " + fetch);
                    }
                });
            usernameField.text = "";
            tokenField.text = "";
        }
    }
    void loggedInSet(bool fetched)
    {
        if (!fetched)
        {
            avatarImage.enabled = false;
            logInPanel.SetActive(false);
            loggedInPanel.SetActive(true);
            usernameText.text = "";
        }
        else
        {
            StartCoroutine(WSM());
        }
    }
    IEnumerator WSM()
    {
        if (GameJoltAPI.Instance.HasSignedInUser)
        {
            while (GameJoltAPI.Instance.CurrentUser.ID == 0)
            {
                yield return new WaitForEndOfFrame();
                
            }
            if (avatarImage.enabled == false)
            {
                GameJoltAPI.Instance.CurrentUser.DownloadAvatar(av =>
                {
                    if (av)
                    {
                        avatarImage.enabled = true;
                        avatarImage.sprite = GameJoltAPI.Instance.CurrentUser.Avatar;
                        Debug.Log("Avatar set");
                    }
                    else
                    {
                        avatarImage.enabled = false;
                        Debug.LogError("Avatar download error");
                    }

                });
            }
            DataStore.Set($"userdata_{GameJoltAPI.Instance.CurrentUser.Name}_id", GameJoltAPI.Instance.CurrentUser.ID+"", true, cb => { });
            usernameText.text = GameJoltAPI.Instance.CurrentUser.DeveloperName;
            Shared.CheckAndSetStartMoney();
            DataStore.Delete("reset", false, cb => { });
        }
        yield return null;
    }
    public void logOut()
    {
        Debug.Log("Attempting Logout...");
        if (GameJoltAPI.Instance.HasSignedInUser)
        {
            GameJoltAPI.Instance.CurrentUser.SignOut();
            loggedInPanel.SetActive(false);
            logInPanel.SetActive(true);
            Debug.Log("Logout successful");
        }
        else
        {
            loggedInPanel.SetActive(false);
            logInPanel.SetActive(true);
            Debug.LogError("Logout failed, user wasn't signed in, why are you even here wtf?");
        }
    }
}
