using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPreview : MonoBehaviour
{
    public MeshRenderer LightRend;
    public MeshRenderer[] myRenderers;
    public MeshRenderer[] myColorRenderers;
    public List<Color> myColors;
    public Color LightColor;
    public float LightIntensity;
    public List<Material> myDefaultMaterials;
    public Material BadMat;
    public Material[] NormalMats;
    bool badmatswitched;
    bool goodmatswitched;
    public bool isColliding;
    public bool isFloor;
    // Start is called before the first frame update
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlacedProp" || other.tag == "Wall")
        {
            isColliding = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlacedProp" || other.tag == "Wall")
        {
            isColliding = false;
        }
    }

    public void Set()
    {
        if (CatalogueMain.Instance != null)
        {
            myColors = new List<Color>();

            foreach (CustomizeColorEntry ce in CatalogueMain.Instance.customizeColors)
            {
                myColors.Add(ce.currColor);
            }
            if (myColors.Count > 0)
            {
                LightColor = myColors[0];
            }
            LightRend.material.SetColor("_BaseColor", LightColor);
            LightRend.material.SetColor("_EmissionColor", new Color(LightColor.r * (LightIntensity / 9f), LightColor.g * (LightIntensity / 9f), LightColor.b * (LightIntensity / 9f)));
            if (myColorRenderers.Length == myColors.Count - 1)
            {
                int i = 0;
                foreach (MeshRenderer r in myColorRenderers)
                {
                    r.material.SetColor("_BaseColor", myColors[i + 1]);
                    i++;
                }
            }
        }
    }
    private void OnEnable()
    {
        Set();
    }
    private void OnDisable()
    {
        for (int i = 0; i < myColorRenderers.Length; i++)
        {
            myColorRenderers[i].material = myDefaultMaterials[i];
        }
    }
    private void Start()
    {
        myDefaultMaterials = new List<Material>();
        foreach (Renderer r in myColorRenderers)
        {
            myDefaultMaterials.Add(r.sharedMaterial);
        }
    }
    private void Update()
    {
        if (BuildManager.Instance != null)
        {
            if (BuildManager.Instance.currentlyBuilding != (name.Replace("_preview", "")))
            {
                gameObject.SetActive(false);
            }
        }
        if (isColliding || !isFloor)
        {
            if (!badmatswitched)
            {
                foreach (Renderer r in myRenderers)
                {
                    r.material = BadMat;
                }
                badmatswitched = true;
            }
            goodmatswitched = false;
        }
        else
        {
            if (!goodmatswitched)
            {
                int i = 0;
                foreach (Renderer r in myRenderers)
                {
                    r.material = NormalMats[i];
                    i++;
                }
                Set();
                goodmatswitched = true;
            }

            badmatswitched = false;
        }
    }
}
