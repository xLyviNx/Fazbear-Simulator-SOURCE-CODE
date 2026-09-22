using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJolt;
using GameJolt.API;
using GameJolt.API.Objects;
using GameJolt.UI;
using TMPro;

public class GameJoltTemp : MonoBehaviour
{
    public GameObject LoginButton;
    public MyProfileTemp MyProfile;
    public GameObject MyProfileO;
    public TextMeshProUGUI LogText;
    public GameObject loadingUIPrefab;
    public bool forceprofilestart = false;
    public void QuitToMenu()
    {
        GameObject load = Instantiate(loadingUIPrefab, null);
        LoadingScene loadscene = load.GetComponent<LoadingScene>();
        loadscene.LoadScene("MainMenu");
    }
    private void Update()
    {
        if (GameJoltAPI.myInstance != null)
        {
            if (!GameJoltAPI.Instance.HasSignedInUser)
            {
                MyProfileO.gameObject.SetActive(false);
            }
            else
            {
                MyProfileO.gameObject.SetActive(true);
                if (LogText.text == "<>" && !forceprofilestart)
                {
                    Debug.Log("Force START");
                    MyProfile.Start();
                    forceprofilestart = true;
                }

            }
        }
        else
        {
            MyProfileO.gameObject.SetActive(false);
            LogText.text = "GameJolt API Not Found.";
        }
    }

}
