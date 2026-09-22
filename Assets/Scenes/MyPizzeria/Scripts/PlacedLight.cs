using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlacedLight : MonoBehaviour
{
    [Header("Base")]
    public uint ID;
    public string lightname;
    public Vector3 Pos;
    public Quaternion Rot;
    [Header("References")]
    public Light Light;
    public MeshRenderer LightRend;
    public GameObject DestructionLayer;
    public LayerMask noFloorMask;
    public LayerMask withFloorMask;
    public Material LightOnMat;
    public Material LightOFFMat;
    public MeshRenderer[] MyColorRenderers;
    [Header("Config")]
    public bool FloorCulling;
    public float LightIntensity;
    public Color32 LightColor;
    public float Range = 10f;
    public bool enableShadows;
    public Color[] myColors;
    [Header("Realtime")]
    public bool isOn = true;
    public bool lastOn;
    [Header("DEBUG")]
    public bool setMe;
    void Start()
    {
        
    }
    public void Set()
    {
        transform.position = Pos;
        transform.rotation = Rot;
        if (myColors.Length > 0)
        {
            LightColor = myColors[0];
        }
        Light.color = LightColor;
        Light.intensity = LightIntensity;
        Light.range = Range;
        if (enableShadows)
        {
            Light.shadows = LightShadows.Hard;
        }
        else
        {
            Light.shadows = LightShadows.None;
        }
        if (FloorCulling)
        {
            Light.cullingMask = withFloorMask;
        }
        else
        {
            Light.cullingMask = noFloorMask;
        }
        if (isOn)
        {
            LightRend.material = LightOnMat;
        }
        else
        {
            LightRend.material = LightOFFMat;
        }
        LightRend.material.SetColor("_BaseColor", LightColor);
        LightRend.material.SetColor("_EmissionColor", new Color(LightColor.r * (LightIntensity / 50f) , LightColor.g * (LightIntensity / 50f), LightColor.b * (LightIntensity / 50f)));
        if (MyColorRenderers.Length == myColors.Length - 1)
        {
            int i = 0;
            foreach(MeshRenderer r in MyColorRenderers)
            {
                r.material.SetColor("_BaseColor", myColors[i+1]);
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        lastOn = isOn;
        if (setMe)
        {
            setMe = false;
            Set();
        }
        Light.gameObject.SetActive(isOn);
        if (Crosshair3D.Instance != null && BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Destroy)
        {
            if (Crosshair3D.Instance.hits)
            {
                if (Crosshair3D.Instance.hit.transform.gameObject == gameObject)
                {
                    DestructionLayer.SetActive(true);
                }
                else
                {
                    DestructionLayer.SetActive(false);

                }
            }
            else
            {
                DestructionLayer.SetActive(false);

            }
        }
        else
        {
            DestructionLayer.SetActive(false);

        }
    }
}
