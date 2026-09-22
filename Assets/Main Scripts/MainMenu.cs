using GameJolt.API;
using LyviN.Map.Visiting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject LoadingPrefab;
    public Animator ProfileAnim;
    public GameObject visitWindow;
    public TMP_InputField visitWindowPlayerName;
    public TextMeshProUGUI VisitText;
    public GameObject MenuButtons;
    public GameObject resetWindow;
    public float ResetTimer;
    public TMP_InputField ResetInputField;
    public AudioSource ErrorSound;
    public TextMeshProUGUI ResetText;
    public AudioSource beepS;
    [SerializeField] bool gotreset;
    public GameObject FeedbackButton;
    public GameObject FeedbackWindow;
    public TMP_InputField FeedbackInput;
    public static bool mereset = false;
    public static bool timedout = false;
    public GameObject timeoutwindow;
    // Start is called before the first frame update
    void Start()
    {
        if (DiscordController.Instance != null)
        {
            DiscordController.Instance.SetActivity("In Game", "Main Menu", true);
        }
        mereset = false;

        if (timedout)
        {
            timedoutwindow();
        }

    }
    void timedoutwindow()
    {
        if (timedout)
        {
            MenuButtons.SetActive(false);
            timedout = false;
            if (GameJoltAPI.myInstance != null)
            {
                GameJoltAPI.Instance.timedOut = false;
            }
            timeoutwindow.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0)
        {
            FeedbackButton.SetActive(true);

            if (UnityEngine.Input.GetKey(KeyCode.Delete) && MenuButtons.activeInHierarchy)
            {
                if (ResetTimer < 4)
                {
                    int lastTimer = (int)ResetTimer;
                    ResetTimer += Time.deltaTime;
                    if (ResetTimer < 4)
                    {
                        if ((int)ResetTimer > lastTimer)
                        {
                            beepS.Play();
                            ResetText.text = ResetText.text + ".";
                        }
                    }
                }
                else
                {
                    ResetOpen();
                }
            }
            else
            {
                if (!gotreset)
                {
                    ResetText.text = "Hold DEL to RESET your DATA";
                }
                ResetTimer = 0f;
            }
        }
        else
        {
            FeedbackButton.SetActive(false);
            ResetText.text = "";
        }
    }
    public void ResetOpen()
    {
        resetWindow.SetActive(true);
    }
    public void PlayGame()
    {
        if (GameJoltAPI.myInstance != null)
        {
            if (GameJoltAPI.Instance.HasSignedInUser)
            {
                Debug.Log("Loading PlayGame scene");
                GameObject load = Instantiate(LoadingPrefab, null);
                LoadingScene loadscene = load.GetComponent<LoadingScene>();
                loadscene.LoadScene("MyPizzeria");
            }
            else
            {
                ProfileAnim.SetTrigger("OpenProfile");
            }
        }
    }
    public void PressOtherPlayerVisit(bool open)
    {
        if (open)
        {
            if (GameJoltAPI.myInstance != null)
            {
                if (GameJoltAPI.Instance.HasSignedInUser)
                {
                    if (!visitWindow.activeInHierarchy)
                    {
                        MenuButtons.SetActive(false);

                        VisitText.text = "Be sure to enter player's name from their GameJolt Profile. Not the DevName. Do not put the \"@\".";
                        visitWindow.SetActive(true);
                        visitWindowPlayerName.text = "";
                    }
                }
                else
                {
                    ProfileAnim.SetTrigger("OpenProfile");

                }
            }
        }
        else
        {
            MenuButtons.SetActive(true);

            visitWindow.SetActive(false);
            visitWindowPlayerName.text = "";
        }
    }

    public void PressVisit()
    {
        if (!string.IsNullOrEmpty(visitWindowPlayerName.text.Trim()))
        {
            if (visitWindowPlayerName.text.Trim().ToLower() != GameJoltAPI.Instance.CurrentUser.Name.ToLower())
            {


                MenuButtons.SetActive(false);
                DataStore.Get($"userdata_{visitWindowPlayerName.text.Trim()}_ID", true, cb =>
                {

                    if (cb != null)
                    {
                        VisitText.text = $"<color=\"green\">Visiting \"{visitWindowPlayerName.text.Trim()}\".";
                        GameObject load = Instantiate(LoadingPrefab, null);
                        LoadingScene loadscene = load.GetComponent<LoadingScene>();
                        loadscene.LoadScene("VisitPizzeria");
                        LoadOtherPizzeria.otherPlayerID = cb.Trim();
                    }
                    else
                    {
                        VisitText.text = $"<color=\"red\">Player \"{visitWindowPlayerName.text.Trim()}\" does not exist (never played this game).";
                    }
                });
            }
            else
            {
                VisitText.text = $"<color=\"red\">You can't visit yourself.";
            }
        }
        else
        {
            VisitText.text = $"<color=\"red\">Please enter player's name.";
        }
    }
    public void QuitGame()
    {
        Debug.Log("Closing the game");
        Application.Quit();
    }
    public void PressReset()
    {
        if (ResetInputField.text.Trim() == "RESET MY DATA")
        {
            if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0)
            {
                DataStore.Set("reset", "yes", false, cb =>
                {
                    if (cb)
                    {
                        DataStore.Delete(Shared.myStructuresKey(), true, cb =>
                        {
                            if (cb)
                            {
                                Shared.ResetLog("Structures got RESET.");
                            }
                            DataStore.Delete(Shared.myPropsKey(), true, cb =>
                            {
                                if (cb)
                                {
                                    Shared.ResetLog("Props got RESET.");
                                }
                                DataStore.Delete(Shared.myLightsKey(), true, cb =>
                                {
                                    if (cb)
                                    {
                                        Shared.ResetLog("Lights got RESET.");
                                    }
                                    DataStore.Delete(Shared.myAnimatronicsKey(), true, cb =>
                                    {
                                        if (cb)
                                        {
                                            Shared.ResetLog("Animatronics got RESET.");
                                        }
                                        DataStore.Delete(Shared.myToppingsStorageKey(), true, cb =>
                                        {
                                            if (cb)
                                            {
                                                Shared.ResetLog("Toppings got RESET.");
                                            }
                                            DataStore.Delete(Shared.myDoorsKey(), true, cb =>
                                            {
                                                if (cb)
                                                {
                                                    Shared.ResetLog("Doors got RESET.");
                                                }
                                                DataStore.Delete(Shared.myPositionKey(), true, cb =>
                                                {
                                                    if (cb)
                                                    {
                                                        Shared.ResetLog("Position got RESET.");
                                                    }
                                                    DataStore.Delete(Shared.myRemRatings(), true, cb =>
                                                    {
                                                        if (cb)
                                                        {
                                                            Shared.ResetLog("Remaining Ratings got RESET.");
                                                        }

                                                        DataStore.Set(Shared.myRating(), "4|4", true, cb =>
                                                        {
                                                            if (cb)
                                                            {
                                                                Shared.ResetLog("Rating got RESET.");
                                                            }
                                                            DataStore.Delete(Shared.myMoneyKey(), true, cb =>
                                                            {
                                                                if (cb)
                                                                {
                                                                    Shared.ResetLog("Money got RESET.");
                                                                }
                                                                PlayerPrefs.DeleteKey("ch");
                                                                PlayerPrefs.DeleteKey("oh");
                                                                gotreset = true;
                                                                ResetText.text = $"<size=120%><color=red>Your data is now gone.";
                                                                resetWindow.SetActive(false);
                                                                ResetInputField.text = "";
                                                                Invoke("resetcd", 4f);
                                                                Shared.CheckAndSetStartMoney();
                                                                mereset = true;
                                                            });

                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    }
                    else
                    {
                        ResetText.text = $"<size=120%><color=red>Failed to reset your data.";
                        Invoke("resetcd", 4f);

                    }
                });
            }
            else
            {
                ErrorSound.Play();

            }
        }
        else
        {

            ErrorSound.Play();
        }
    }
    void resetcd()
    {
        gotreset = false;
    }
    public void CancelReset()
    {
        ResetInputField.text = "";
        resetWindow.SetActive(false);
    }
    public void CancelFeedback()
    {
        FeedbackInput.text = "";
        FeedbackWindow.SetActive(false);

    }
    public void OpenFeedback()
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            FeedbackInput.text = "";
            FeedbackWindow.SetActive(true);
        }
        else
        {
            ProfileAnim.SetTrigger("OpenProfile");
            ErrorSound.Play();
        }
    }
    public void SendFeedback()
    {
        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
        {
            string msg = FeedbackInput.text.Trim();
            if (!string.IsNullOrEmpty(msg) && msg.Length > 10 && msg != "Your message has been sent!" && msg!= "Your message is too short!")
            {
                Shared.FeedbackLog(msg);
                FeedbackInput.text = "Your message has been sent!";
            }
            else
            {
                FeedbackInput.text = "Your message is too short!";
                ErrorSound.Play();
            }
        }
        else
        {
            ErrorSound.Play();
        }
    }
    public void StartEditFeedback()
    {
        if (FeedbackInput.text == "Your message has been sent!" || FeedbackInput.text == "Your message is too short!")
        {
            FeedbackInput.text = "";
        }
    }
}
