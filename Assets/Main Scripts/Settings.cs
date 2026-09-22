using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System;

public class Settings : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public UniversalRenderPipelineAsset[] qualityLevels;
    //public UniversalRenderPipelineAsset[] shadowDistanceLevels;
    public UniversalRendererData[] qualityLevelsRenderers;

    // SETTINGS TO CHANGE

    public Resolution[] screenResolutions;
    public TMP_Dropdown screenResolutionsDropdown;
    List<string> resolutionList;
    int currentResolutionIndex = 0;
    int resolutionApplyIndex;

    int currentResolutionWidth;
    int currentResolutionHeight;

    public int fullscreenModeValue;
    public TMP_Dropdown fullscreenModeDropdown;

    public bool vsyncON;
    public Toggle vsyncToggle;

    //[Range(0, 400)]
    public int maxFramerateValue;
    public Slider maxFramerateSlider;
    public TextMeshProUGUI maxFramerateStatus;

    [Range(0.1f, 2f)]
    public float resolutionScaleValue;
    public Slider resolutionScaleSlider;
    public TextMeshProUGUI resolutionScaleStatus;

    public int ssaoQualityValue;
    public TMP_Dropdown ssaoQualityDropdown;
    //public ScriptableRendererFeature ssaoFeature;
    //public ScriptableRendererFeature ssaoUltraFeature;
    //public UniversalRendererData[] ssaoQualityRenderers;
    public Camera mainCamera;
    //UniversalAdditionalCameraData mainCameraAdditionalData = mainCamera.GetUniversalAdditionalCameraData();
    UniversalAdditionalCameraData mainCameraAdditionalData;

    public int textureQualityValue;
    public TMP_Dropdown textureQualityDropdown;

    public bool anisotropicON;
    public Toggle anisotropicToggle;

    public int shadowDistanceValue;
    public TMP_Dropdown shadowDistanceDropdown;

    public int shadowCascadesValue;
    public TMP_Dropdown shadowCascadesDropdown;

    public int shadowResolutionValue;
    public TMP_Dropdown shadowResolutionDropdown;

    public bool additionalShadowsON;
    public Toggle additionalShadowsToggle;

    public int additionalShadowResolutionValue;
    public TMP_Dropdown additionalShadowResolutionDropdown;

    public int additionalLightLimitValue;
    public Slider additionalLightLimitSlider;
    public TextMeshProUGUI additionalLightLimitStatus;


    // Start is called before the first frame update
    void Start()
    {
        
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        mainCameraAdditionalData = mainCamera.GetUniversalAdditionalCameraData();

        screenResolutions = Screen.resolutions;
        resolutionList = new List<string>();

        //screenResolutions = Screen.resolutions;
        //resolutionList = new List<string>();
        //int currentResolutionIndex = 0;
        for (int i = 0; i < screenResolutions.Length; i++)
        {
            string screenResolution = screenResolutions[i].width + " x " + screenResolutions[i].height + " @" + screenResolutions[i].refreshRate + "Hz";
            resolutionList.Add(screenResolution);

            if (screenResolutions[i].width == Screen.width && screenResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        screenResolutionsDropdown.AddOptions(resolutionList);
        screenResolutionsDropdown.value = currentResolutionIndex;
        //Screen.SetResolution(screenResolutions[currentResolutionIndex].width, screenResolutions[currentResolutionIndex].height, Screen.fullScreen);

        LoadValues();

        //Debug.Log(qualityLevels[3].mainLightShadowmapResolution);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onValueChangedINT(int dropdownIndex)
    {
        resolutionApplyIndex = dropdownIndex;
    }

    public void refresh() // TO REFRESH VALUES IF SOMEONE LEAVES WITHOUT APPLYING
    {
        LoadValues();
    }

    public void onValueChangedResolutionScaleSlider(float sliderFloat)
    {
        //resolutionScaleStatus.text = sliderFloat.ToString("F2");
        resolutionScaleStatus.text = MathF.Round(sliderFloat, 2, MidpointRounding.AwayFromZero).ToString("F2");
    }

    public void onValueChangedAdditionalLightsSlider(float sliderFloat)
    {
        int sliderInt;
        sliderInt = Convert.ToInt32(MathF.Round(sliderFloat, 0, MidpointRounding.AwayFromZero));
        additionalLightLimitStatus.text = sliderInt.ToString();
    }

    public void onValueChangedMaxFramerateSlider(float sliderFloat)
    {
        int sliderInt;
        sliderInt = Convert.ToInt32(MathF.Round(sliderFloat, 0, MidpointRounding.AwayFromZero));
        switch (sliderInt)
        {
            case 0:
                maxFramerateStatus.text = "Unlimited";
                break;
            default:
                maxFramerateStatus.text = Convert.ToInt32(MathF.Round(sliderFloat, 0, MidpointRounding.AwayFromZero)).ToString();
                break;
        }
    }

    public void LoadValues()
    {
        try
        {
            //PlayerPrefs.SetInt("currentResolutionIndex", currentResolutionIndex); // NIE ZAPISUJ JESZCZE ROZDZIELCZOŒCI BO SIÊ ZESRASZ DEBILU
            currentResolutionWidth = PlayerPrefs.GetInt("LastResolutionWidth", Screen.currentResolution.width);
            currentResolutionHeight = PlayerPrefs.GetInt("LastResolutionHeight", Screen.currentResolution.height);
            fullscreenModeValue = PlayerPrefs.GetInt("FullscreenMode", 1);
            vsyncON = Convert.ToBoolean(PlayerPrefs.GetInt("VsyncON", 0));
            maxFramerateValue = PlayerPrefs.GetInt("MaxFramerate", 0);
            resolutionScaleValue = PlayerPrefs.GetFloat("ResolutionScale", 1.00f);
            textureQualityValue = PlayerPrefs.GetInt("TextureQuality", 3);
            ssaoQualityValue = PlayerPrefs.GetInt("SsaoQuality", 1);
            anisotropicON = Convert.ToBoolean(PlayerPrefs.GetInt("AnisotropicON", 1));
            shadowDistanceValue = PlayerPrefs.GetInt("ShadowDistance", 1);
            shadowCascadesValue = PlayerPrefs.GetInt("ShadowCascades", 1);
            shadowResolutionValue = PlayerPrefs.GetInt("ShadowResolution", 2);
            additionalShadowsON = Convert.ToBoolean(PlayerPrefs.GetInt("AdditionalShadows", 1));
            additionalShadowResolutionValue = PlayerPrefs.GetInt("AdditionalShadowsResolution", 1);
            additionalLightLimitValue = PlayerPrefs.GetInt("AdditionalLightLimit", 4);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Couldn't load settings values: " + ex.ToString());
        }
        finally
        {
            Debug.Log("Loaded settings values correctly");
        }

        LoadSettings();
    }

    public void SaveValues()
    {
        try
        {
            //PlayerPrefs.SetInt("currentResolutionIndex", currentResolutionIndex); // NIE ZAPISUJ JESZCZE ROZDZIELCZOŒCI BO SIÊ ZESRASZ DEBILU
            //for (int i = 0; i < screenResolutions.Length; i++)
            //{
            //    if (screenResolutions[i].width == Screen.width && screenResolutions[i].height == Screen.currentResolution.height)
            //    {
            //        currentResolutionWidth = screenResolutions[i].width;
            //        currentResolutionHeight = screenResolutions[i].height;
            //    }
            //}
            PlayerPrefs.SetInt("LastResolutionWidth", currentResolutionWidth);
            PlayerPrefs.SetInt("LastResolutionHeight", currentResolutionHeight);
            PlayerPrefs.SetInt("FullscreenMode", fullscreenModeValue);
            PlayerPrefs.SetInt("VsyncON", Convert.ToInt32(vsyncON));
            PlayerPrefs.SetInt("MaxFramerate", Convert.ToInt32(maxFramerateValue));
            //Debug.Log(resolutionScaleValue);
            //Debug.Log(MathF.Round(resolutionScaleValue, 2, MidpointRounding.AwayFromZero));
            //Debug.Log(Convert.ToSingle(resolutionScaleValue));
            PlayerPrefs.SetFloat("ResolutionScale", MathF.Round(resolutionScaleValue, 2, MidpointRounding.AwayFromZero));
            PlayerPrefs.SetInt("TextureQuality", textureQualityValue);
            PlayerPrefs.SetInt("SsaoQuality", ssaoQualityValue);
            PlayerPrefs.SetInt("AnisotropicON", Convert.ToInt32(anisotropicON));
            PlayerPrefs.SetInt("ShadowDistance", shadowDistanceValue);
            PlayerPrefs.SetInt("ShadowCascades", shadowCascadesValue);
            PlayerPrefs.SetInt("ShadowResolution", shadowResolutionValue);
            PlayerPrefs.SetInt("AdditionalShadows", Convert.ToInt32(additionalShadowsON));
            PlayerPrefs.SetInt("AdditionalShadowsResolution", additionalShadowResolutionValue);
            PlayerPrefs.SetInt("AdditionalLightLimit", Convert.ToInt32(additionalLightLimitValue));
            //PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Couldn't save settings values: " + ex.ToString());
        }
        finally
        {
            Debug.Log("Saved settings values correctly");
        }
    }

    public void LoadSettings()
    {
        // RESOLUTION PRZENIESIONE NA START

        ////screenResolutions = Screen.resolutions;
        ////resolutionList = new List<string>();
        ////int currentResolutionIndex = 0;
        //for (int i = 0; i < screenResolutions.Length; i++)
        //{
        //    string screenResolution = screenResolutions[i].width + " x " + screenResolutions[i].height + " @" + screenResolutions[i].refreshRate;
        //    resolutionList.Add(screenResolution);

        //    if (screenResolutions[i].width == Screen.width && screenResolutions[i].height == Screen.height)
        //    {
        //        currentResolutionIndex = i;
        //    }
        //}
        //screenResolutionsDropdown.AddOptions(resolutionList);
        //screenResolutionsDropdown.value = currentResolutionIndex;
        ////Screen.SetResolution(screenResolutions[currentResolutionIndex].width, screenResolutions[currentResolutionIndex].height, Screen.fullScreen);
        for (int i = 0; i < screenResolutions.Length; i++)
        {
            if (screenResolutions[i].width == Screen.width && screenResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        screenResolutionsDropdown.value = currentResolutionIndex;

        // FULLSCREEN MODE
        fullscreenModeDropdown.value = fullscreenModeValue;
        switch (fullscreenModeDropdown.value)
        {
            // EXCLUSIVE FULLSCREEN
            case 0:
                //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, FullScreenMode.ExclusiveFullScreen);
                break;
            // BORDERLESS FULLSCREEN
            case 1:
                //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, FullScreenMode.FullScreenWindow);
                break;
            // WINDOWED
            case 2:
                //Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(currentResolutionWidth, currentResolutionHeight, FullScreenMode.Windowed);
                break;
        }

        // VSYNC
        vsyncToggle.isOn = vsyncON;
        switch (vsyncToggle.isOn)
        {
            // OFF
            case false:
                QualitySettings.vSyncCount = 0;
                break;
            // ON
            case true:
                QualitySettings.vSyncCount = 1;
                break;
        }

        // MAX FRAMERATE
        maxFramerateSlider.value = maxFramerateValue;
        switch (maxFramerateSlider.value)
        {
            case 0:
                Application.targetFrameRate = -1;
                maxFramerateStatus.text = "Unlimited";
                break;
            default:
                Application.targetFrameRate = Convert.ToInt32(MathF.Round(maxFramerateValue, 0, MidpointRounding.AwayFromZero)); // JEST NA VALUE NIE NA SLIDER ¯EBY MO¯NA BY£O WY¯EJ Z REJESTRU USTAWIÆ
                maxFramerateStatus.text = Convert.ToInt32(MathF.Round(maxFramerateValue, 0, MidpointRounding.AwayFromZero)).ToString(); // JEST NA VALUE NIE NA SLIDER ¯EBY MO¯NA BY£O WY¯EJ Z REJESTRU USTAWIÆ
                break;
        }

        // RESOLUTION SCALE
        resolutionScaleSlider.value = MathF.Round(resolutionScaleValue, 2, MidpointRounding.AwayFromZero);
        qualityLevels[3].renderScale = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero);
        //resolutionScaleStatus.text = resolutionScaleValue.ToString("F2");
        resolutionScaleStatus.text = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero).ToString("F2");

        // TEXTURE QUALITY
        textureQualityDropdown.value = textureQualityValue;
        switch (textureQualityDropdown.value)
        {
            // EIGHTH
            case 0:
                QualitySettings.masterTextureLimit = 3;
                break;
            // QUARTER
            case 1:
                QualitySettings.masterTextureLimit = 2;
                break;
            // HALF
            case 2:
                QualitySettings.masterTextureLimit = 1;
                break;
            // FULL
            case 3:
                QualitySettings.masterTextureLimit = 0;
                break;
        }

        // AMBIENT OCCLUSION - SSAO
        ssaoQualityDropdown.value = ssaoQualityValue;
        switch (ssaoQualityDropdown.value)
        {
            // OFF
            case 0:
                // FAILED THINGS
                //ssaoFeature.
                //qualityLevelsRenderers[3].rendererFeatures.Add(ssaoUltraFeature);
                //qualityLevels[3].scr
                mainCameraAdditionalData.SetRenderer(0);
                break;
            // LOW
            case 1:
                mainCameraAdditionalData.SetRenderer(1);
                break;
            // MEDIUM
            case 2:
                mainCameraAdditionalData.SetRenderer(2);
                break;
            // HIGH
            case 3:
                mainCameraAdditionalData.SetRenderer(3);
                break;
            // ULTRA
            case 4:
                mainCameraAdditionalData.SetRenderer(4);
                break;
        }

        // ANISOTROPIC FILTERING - NO PER TEXTURE OPTION FOR NOW
        anisotropicToggle.isOn = anisotropicON;
        switch (anisotropicToggle.isOn)
        {
            // OFF
            case false:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;
            // FORCE ON
            case true:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                break;
        }

        // SHADOW DISTANCE
        shadowDistanceDropdown.value = shadowDistanceValue;
        switch (shadowDistanceValue)
        {
            // OFF
            case 0:
                qualityLevels[3].shadowDistance = 0f;
                break;
            // NEAR
            case 1:
                qualityLevels[3].shadowDistance = 50f;
                break;
            // FAR
            case 2:
                qualityLevels[3].shadowDistance = 100f;
                break;
        }

        // SHADOW CASCADES
        shadowCascadesDropdown.value = shadowCascadesValue;
        switch (shadowCascadesDropdown.value)
        {
            // ONE
            case 0:
                qualityLevels[3].shadowCascadeCount = 1;
                break;
            // TWO
            case 1:
                qualityLevels[3].shadowCascadeCount = 2;
                break;
            // THREE
            case 2:
                qualityLevels[3].shadowCascadeCount = 3;
                break;
            // FOUR
            case 3:
                qualityLevels[3].shadowCascadeCount = 4;
                break;
        }

        // SHADOW RESOLUTION
        shadowResolutionDropdown.value = shadowResolutionValue;
        switch (shadowResolutionDropdown.value)
        {
            // ONE
            case 0:
                //qualityLevels[3].mainLightShadowmapResolution = 256;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)256;
                break;
            // TWO
            case 1:
                //qualityLevels[3].mainLightShadowmapResolution = 512;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)512;
                break;
            // THREE
            case 2:
                //qualityLevels[3].mainLightShadowmapResolution = 1024;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)1024;
                break;
            // FOUR
            case 3:
                //qualityLevels[3].mainLightShadowmapResolution = 2048;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)2048;
                break;
            case 4:
                //qualityLevels[3].mainLightShadowmapResolution = 4096;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)4096;
                break;
        }

        // ADDTIONAL SHADOWS
        additionalShadowsToggle.isOn = additionalShadowsON;
        switch (additionalShadowsToggle.isOn)
        {
            // OFF
            case false:
                UnityGraphicsBullshit.AdditionalLightCastShadows = false;
                break;
            // ON
            case true:
                UnityGraphicsBullshit.AdditionalLightCastShadows = true;
                break;
        }

        // ADDITIONAL SHADOW RESOLUTION
        additionalShadowResolutionDropdown.value = additionalShadowResolutionValue;
        switch (additionalShadowResolutionDropdown.value)
        {
            // 256
            case 0:
                //qualityLevels[3].mainLightShadowmapResolution = 256;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)256;
                break;
            // 512
            case 1:
                //qualityLevels[3].mainLightShadowmapResolution = 512;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)512;
                break;
            // 1024
            case 2:
                //qualityLevels[3].mainLightShadowmapResolution = 1024;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)1024;
                break;
            // 2048
            case 3:
                //qualityLevels[3].mainLightShadowmapResolution = 2048;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)2048;
                break;
            // 4096
            case 4:
                //qualityLevels[3].mainLightShadowmapResolution = 4096;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)4096;
                break;
        }

        // ADDITIONAL LIGHTS LIMIT
        additionalLightLimitSlider.value = additionalLightLimitValue;
        qualityLevels[3].maxAdditionalLightsCount = Convert.ToInt32(MathF.Round(additionalLightLimitSlider.value, 0, MidpointRounding.AwayFromZero));
        additionalLightLimitStatus.text = Convert.ToInt32(MathF.Round(additionalLightLimitSlider.value, 0, MidpointRounding.AwayFromZero)).ToString();
        //Debug.Log(additionalLightLimitSlider.value);

        Debug.Log("Loaded Settings");
    }

    public void ApplySettings(int qualityLevelValue)
    {
        // RESOLUTION
        //Screen.SetResolution(screenResolutions[resolutionApplyIndex].width, screenResolutions[resolutionApplyIndex].height, Screen.fullScreen);

        // FULLSCREEN MODE
        switch (fullscreenModeDropdown.value)
        {
            // EXCLUSIVE FULLSCREEN
            case 0:
                //Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Screen.SetResolution(screenResolutions[resolutionApplyIndex].width, screenResolutions[resolutionApplyIndex].height, FullScreenMode.ExclusiveFullScreen);
                currentResolutionWidth = screenResolutions[resolutionApplyIndex].width;
                currentResolutionHeight = screenResolutions[resolutionApplyIndex].height;
                fullscreenModeValue = fullscreenModeDropdown.value;
                break;
            // BORDERLESS FULLSCREEN
            case 1:
                //Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(screenResolutions[resolutionApplyIndex].width, screenResolutions[resolutionApplyIndex].height, FullScreenMode.FullScreenWindow);
                currentResolutionWidth = screenResolutions[resolutionApplyIndex].width;
                currentResolutionHeight = screenResolutions[resolutionApplyIndex].height;
                fullscreenModeValue = fullscreenModeDropdown.value;
                break;
            // WINDOWED
            case 2:
                //Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(screenResolutions[resolutionApplyIndex].width, screenResolutions[resolutionApplyIndex].height, FullScreenMode.Windowed);
                currentResolutionWidth = screenResolutions[resolutionApplyIndex].width;
                currentResolutionHeight = screenResolutions[resolutionApplyIndex].height;
                fullscreenModeValue = fullscreenModeDropdown.value;
                break;
        }

        // VSYNC
        switch (vsyncToggle.isOn)
        {
            // OFF
            case false:
                QualitySettings.vSyncCount = 0;
                vsyncON = vsyncToggle.isOn;
                break;
            // ON
            case true:
                QualitySettings.vSyncCount = 1;
                vsyncON = vsyncToggle.isOn;
                break;
        }

        // MAX FRAMERATE
        switch (maxFramerateSlider.value)
        {
            case 0:
                Application.targetFrameRate = -1;
                maxFramerateValue = 0;
                maxFramerateStatus.text = "Unlimited";
                break;
            default:
                Application.targetFrameRate = Convert.ToInt32(MathF.Round(maxFramerateSlider.value, 0, MidpointRounding.AwayFromZero));
                maxFramerateValue = Convert.ToInt32(MathF.Round(maxFramerateSlider.value, 0, MidpointRounding.AwayFromZero));
                maxFramerateStatus.text = Convert.ToInt32(MathF.Round(maxFramerateSlider.value, 0, MidpointRounding.AwayFromZero)).ToString();
                break;
        }

        // RESOLUTION SCALE
        //resolutionScaleSlider.value = MathF.Round(resolutionScaleValue, 2, MidpointRounding.AwayFromZero);
        qualityLevels[3].renderScale = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero);
        resolutionScaleValue = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero);
        //resolutionScaleStatus.text = resolutionScaleValue.ToString("F2");
        resolutionScaleStatus.text = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero).ToString("F2");

        // TEXTURE QUALITY
        switch (textureQualityDropdown.value)
        {
            // EIGHTH
            case 0:
                QualitySettings.masterTextureLimit = 3;
                textureQualityValue = textureQualityDropdown.value;
                break;
            // QUARTER
            case 1:
                QualitySettings.masterTextureLimit = 2;
                textureQualityValue = textureQualityDropdown.value;
                break;
            // HALF
            case 2:
                QualitySettings.masterTextureLimit = 1;
                textureQualityValue = textureQualityDropdown.value;
                break;
            // FULL
            case 3:
                QualitySettings.masterTextureLimit = 0;
                textureQualityValue = textureQualityDropdown.value;
                break;
        }

        // AMBIENT OCCLUSION - SSAO
        switch (ssaoQualityDropdown.value)
        {
            // OFF
            case 0:
                // FAILED THINGS
                //ssaoFeature.
                //qualityLevelsRenderers[3].rendererFeatures.Add(ssaoUltraFeature);
                //qualityLevels[3].scr
                mainCameraAdditionalData.SetRenderer(0);
                ssaoQualityValue = ssaoQualityDropdown.value;
                break;
            // LOW
            case 1:
                mainCameraAdditionalData.SetRenderer(1);
                ssaoQualityValue = ssaoQualityDropdown.value;
                break;
            // MEDIUM
            case 2:
                mainCameraAdditionalData.SetRenderer(2);
                ssaoQualityValue = ssaoQualityDropdown.value;
                break;
            // HIGH
            case 3:
                mainCameraAdditionalData.SetRenderer(3);
                ssaoQualityValue = ssaoQualityDropdown.value;
                break;
            // ULTRA
            case 4:
                mainCameraAdditionalData.SetRenderer(4);
                ssaoQualityValue = ssaoQualityDropdown.value;
                break;
        }

        // ANISOTROPIC FILTERING - NO PER TEXTURE OPTION FOR NOW
        switch (anisotropicToggle.isOn)
        {
            // OFF
            case false:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                anisotropicON = anisotropicToggle.isOn;
                break;
            // FORCE ON
            case true:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                anisotropicON = anisotropicToggle.isOn;
                break;
        }

        // SHADOW DISTANCE
        switch (shadowDistanceDropdown.value)
        {
            // OFF
            case 0:
                qualityLevels[3].shadowDistance = 0f;
                shadowDistanceValue = shadowDistanceDropdown.value;
                break;
            // NEAR
            case 1:
                qualityLevels[3].shadowDistance = 50f;
                shadowDistanceValue = shadowDistanceDropdown.value;
                break;
            // FAR
            case 2:
                qualityLevels[3].shadowDistance = 100f;
                shadowDistanceValue = shadowDistanceDropdown.value;
                break;
        }

        // SHADOW CASCADES
        switch (shadowCascadesDropdown.value)
        {
            // ONE
            case 0:
                qualityLevels[3].shadowCascadeCount = 1;
                shadowCascadesValue = shadowCascadesDropdown.value;
                break;
            // TWO
            case 1:
                qualityLevels[3].shadowCascadeCount = 2;
                shadowCascadesValue = shadowCascadesDropdown.value;
                break;
            // THREE
            case 2:
                qualityLevels[3].shadowCascadeCount = 3;
                shadowCascadesValue = shadowCascadesDropdown.value;
                break;
            // FOUR
            case 3:
                qualityLevels[3].shadowCascadeCount = 4;
                shadowCascadesValue = shadowCascadesDropdown.value;
                break;
        }

        // SHADOW RESOLUTION
        switch (shadowResolutionDropdown.value)
        {
            // 256
            case 0:
                //qualityLevels[3].mainLightShadowmapResolution = 256;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)256;
                shadowResolutionValue = shadowResolutionDropdown.value;
                break;
            // 512
            case 1:
                //qualityLevels[3].mainLightShadowmapResolution = 512;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)512;
                shadowResolutionValue = shadowResolutionDropdown.value;
                break;
            // 1024
            case 2:
                //qualityLevels[3].mainLightShadowmapResolution = 1024;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)1024;
                shadowResolutionValue = shadowResolutionDropdown.value;
                break;
            // 2048
            case 3:
                //qualityLevels[3].mainLightShadowmapResolution = 2048;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)2048;
                shadowResolutionValue = shadowResolutionDropdown.value;
                break;
            // 4096
            case 4:
                //qualityLevels[3].mainLightShadowmapResolution = 4096;
                UnityGraphicsBullshit.MainLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)4096;
                shadowResolutionValue = shadowResolutionDropdown.value;
                break;
        }

        // ADDTIONAL SHADOWS
        switch (additionalShadowsToggle.isOn)
        {
            // OFF
            case false:
                UnityGraphicsBullshit.AdditionalLightCastShadows = false;
                additionalShadowsON = additionalShadowsToggle.isOn;
                break;
            // ON
            case true:
                UnityGraphicsBullshit.AdditionalLightCastShadows = true;
                additionalShadowsON = additionalShadowsToggle.isOn;
                break;
        }

        // ADDITIONAL SHADOW RESOLUTION
        switch (additionalShadowResolutionDropdown.value)
        {
            // 256
            case 0:
                //qualityLevels[3].mainLightShadowmapResolution = 256;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)256;
                additionalShadowResolutionValue = additionalShadowResolutionDropdown.value;
                break;
            // 512
            case 1:
                //qualityLevels[3].mainLightShadowmapResolution = 512;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)512;
                additionalShadowResolutionValue = additionalShadowResolutionDropdown.value;
                break;
            // 1024
            case 2:
                //qualityLevels[3].mainLightShadowmapResolution = 1024;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)1024;
                additionalShadowResolutionValue = additionalShadowResolutionDropdown.value;
                break;
            // 2048
            case 3:
                //qualityLevels[3].mainLightShadowmapResolution = 2048;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)2048;
                additionalShadowResolutionValue = additionalShadowResolutionDropdown.value;
                break;
            // 4096
            case 4:
                //qualityLevels[3].mainLightShadowmapResolution = 4096;
                UnityGraphicsBullshit.AdditionalLightShadowResolution = (UnityEngine.Rendering.Universal.ShadowResolution)4096;
                additionalShadowResolutionValue = additionalShadowResolutionDropdown.value;
                break;
        }

        // ADDITIONAL LIGHTS LIMIT
        qualityLevels[3].maxAdditionalLightsCount = Convert.ToInt32(additionalLightLimitSlider.value);
        additionalLightLimitValue = Convert.ToInt32(additionalLightLimitSlider.value);
        additionalLightLimitStatus.text = Convert.ToInt32(additionalLightLimitSlider.value).ToString();

        //resolutionScaleValue = MathF.Round(resolutionScaleSlider.value, 2, MidpointRounding.AwayFromZero);
        //Debug.Log(qualityLevels[3].maxAdditionalLightsCount);

        qualityLevelValue = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityLevelValue);
        //QualitySettings.renderPipeline = qualityLevels[value];
        //Debug.Log(qualityLevels[3].shadowDistance);

        Debug.Log("Applied Settings");

        //Debug.Log(qualityLevels[3].shadowDistance);

        SaveValues();
        //Invoke("SaveValues", 0.01f); // ALE GÓWNO
        //StartCoroutine(WaitFrameAndSave());


    }

    //public IEnumerator WaitFrameAndSave()
    //{
    //    yield return new WaitForEndOfFrame();
    //    SaveValues();
    //    yield return null;
    //}
}
