using GameJolt.API;
using LyviN.Map;
using LyviN.Map.Visiting;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    public Slider Slider;
    public Animator anim;
    public static bool isLoading;
    public TextMeshProUGUI StatusT;
    public void LoadScene(string sceneName)
    {
        LoadingScene[] ls = GameObject.FindObjectsOfType<LoadingScene>();
        if (ls.Length <= 1)
        {
            StartCoroutine(LoadAsyncScene(sceneName));
            anim.Play("FadeIn");
        }else
        {
            Destroy(gameObject);
        }
    }
    IEnumerator LoadAsyncScene(string SceneName)
    {
        if (!isLoading)
        {
            DontDestroyOnLoad(gameObject);
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.
            StatusT.text = $"Loading the Scene";
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Loop"))
            {
                yield return new WaitForEndOfFrame();
            }
            isLoading = true;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);
            asyncLoad.allowSceneActivation = false;
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                Slider.value = asyncLoad.progress;
                yield return null;
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
            }
            StatusT.text = $"Finishing Loading the Scene.";
            isLoading = false;
            StartCoroutine(WannaHide());
        }
    }
    IEnumerator WannaHide()
    {
        bool hiding = false;
        while (!hiding)
        {
            if (SaveSystem.Instance != null)
            {
                if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
                {
                    while (GameJoltAPI.Instance.CurrentUser.ID == 0)
                    {
                        StatusT.text = $"Waiting for GameJolt Authorization";
                        Slider.value = 0.91f;
                        yield return new WaitForEndOfFrame();
                    }
                    StatusT.text = $"Loading Player's Save";
                    Slider.value = 0.93f;
                    if (SaveSystem.Instance.Loaded)
                    {
                        Slider.value = 0.94f;
                        if (FazNavMesh.Instance != null)
                        {
                            Slider.value = 0.96f;
                            StatusT.text = $"Baking NavMeshes.";

                            while (FazNavMesh.Instance.surface.navMeshData == null || (SaveSystem.Instance != null && !SaveSystem.Instance.bakedNav))
                            {
                                Slider.value = 0.98f;
                                yield return new WaitForEndOfFrame();
                            }
                            Slider.value = 1f;
                            Invoke("Hide", 1f);
                            hiding = true;
                        }
                        else
                        {
                            Slider.value = 1f;
                            Hide();
                            hiding = true;
                        }

                    }
                }
            }
            else if (LoadOtherPizzeria.Instance != null)
            {
                Slider.value = 0.93f;
                StatusT.text = $"Loading Player's Save";
                if (LoadOtherPizzeria.Instance.Loaded)
                {
                    Slider.value = 0.94f;
                    if (FazNavMesh.Instance != null)
                    {
                        Slider.value = 0.96f;
                        FazNavMesh.Instance.surface.RemoveData();
                        StatusT.text = $"Baking NavMeshes.";

                        while (FazNavMesh.Instance.surface.navMeshData == null || !(LoadOtherPizzeria.Instance != null && LoadOtherPizzeria.Instance.bakedNav))
                        {
                            Slider.value = 0.98f;
                            yield return new WaitForEndOfFrame();
                        }
                        Slider.value = 1f;
                        Invoke("Hide", 1f);
                        hiding = true;
                    }
                    else
                    {
                        Hide();
                        hiding = true;
                    }
                }
            }
            else
            {
                Slider.value = 1f;
                Hide();
                hiding = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    void Hide()
    {
        anim.SetTrigger("Hide");
        Invoke("destroyme",8f);
    }
    void destroyme()
    {
        Destroy(gameObject);
    }
}
