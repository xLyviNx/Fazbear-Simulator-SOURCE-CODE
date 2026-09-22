using LyviN.Map.Visiting;
using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace LyviN.Map
{
    public class BuiltWallMain : MonoBehaviour
    {
        public uint StructureID;
        public float X_Scale;
        public float Z_Scale;
        public float Height;
        public Vector3 Position;
        public string type;
        [SerializeField] private Renderer myRend;
        [SerializeField] private Material[] Mats;
        [SerializeField] private Material[] WallPatternMats;
        [SerializeField] private Material[] FloorPatternMats;
        public GameObject DestructionLayer;
        public GameObject wallO;
        public static BuiltWallMain mebaking;
        public static bool anyloadbaking;
        public bool EmployeesOnly;
        [Header("Customization")]
        public Color mainColor = Color.white;
        public int mainPattern;
        public bool hasStripe;
        public float StripeOffset;
        public bool areTiles;
        public Color stripeColor;
        public Color stripeBorderColor;
        public Color TilesColor;
        public GameObject stripeObject;
        public MeshRenderer stripeBorder1;
        public MeshRenderer stripeBorder2;
        public MeshRenderer stripeInside;

        public Material stripeInsideNormalMaterial;
        public Material stripeInsideTilesMaterial;
        public Material stripeTiles2Material;
        public Vector3 initpos;
        private void Start()
        {
            initpos = stripeObject.transform.localPosition;
        }
        public void Set()        {
            DestructionLayer.SetActive(false);
            transform.localScale = new Vector3(X_Scale, 0f, Z_Scale);
            transform.position = new Vector3(Position.x, 0f, Position.z);
            switch (type)
            {
                case "aacblockswall":
                    {
                        myRend.material = Mats[0];
                        tag = "Wall";
                        wallO.tag = "Wall";
                        break;
                    }
                case "concretefloor":
                    {
                        myRend.material = Mats[1];
                        tag = "Floor";
                        wallO.tag = "Floor";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            if(hasStripe)
            {
                stripeObject.SetActive(true);
                stripeObject.transform.localPosition = new Vector3(stripeObject.transform.localPosition.x, (Vector3.up * Mathf.Clamp(StripeOffset, -0.4f, 0.4f)).y, stripeObject.transform.localPosition.z);
                stripeBorder1.material.SetColor("_Top_Texture_Color", stripeBorderColor);
                stripeBorder1.material.SetColor("_Front_Texture_Color", stripeBorderColor);
                stripeBorder1.material.SetColor("_Side_Texture_Color", stripeBorderColor);
                stripeBorder2.material = stripeBorder1.material;
                if (!areTiles)
                {
                    stripeInside.materials = new Material[] { stripeInsideNormalMaterial };
                    stripeInside.material.SetColor("_Top_Texture_Color", stripeColor);
                    stripeInside.material.SetColor("_Front_Texture_Color", stripeColor);
                    stripeInside.material.SetColor("_Side_Texture_Color", stripeColor);
                }
                else
                {
                    stripeInside.materials = new Material[] { stripeInsideTilesMaterial, stripeTiles2Material };
                    stripeInside.materials[0].SetColor("_Top_Texture_Color", stripeColor);
                    stripeInside.materials[0].SetColor("_Front_Texture_Color", stripeColor);
                    stripeInside.materials[0].SetColor("_Side_Texture_Color", stripeColor);

                    stripeInside.materials[1].SetColor("_Top_Texture_Color", TilesColor);
                    stripeInside.materials[1].SetColor("_Front_Texture_Color", TilesColor);
                    stripeInside.materials[1].SetColor("_Side_Texture_Color", TilesColor);
                }
            }else
            {
                stripeObject.SetActive(false);

            }

            if (tag == "Wall")
            {
                if (mainPattern != 0)
                {
                    myRend.material = WallPatternMats[mainPattern];
                    if (mainColor != myRend.material.GetColor("_Front_Texture_Color"))
                    {
                        myRend.material.SetColor("_Top_Texture_Color", mainColor);
                        myRend.material.SetColor("_Front_Texture_Color", mainColor);
                        myRend.material.SetColor("_Side_Texture_Color", mainColor);
                    }
                }
            }
            else if (tag == "Floor")
            {

                if (mainPattern != 0)
                {
                    if (mainPattern != 2)
                    {
                        myRend.material = FloorPatternMats[mainPattern];
                        if (mainColor != myRend.material.GetColor("_Front_Texture_Color"))
                        {
                            myRend.material.SetColor("_Top_Texture_Color", mainColor);
                            myRend.material.SetColor("_Front_Texture_Color", mainColor);
                            myRend.material.SetColor("_Side_Texture_Color", mainColor);
                        }
                    }
                    else
                    {
                        myRend.materials = new Material[] { FloorPatternMats[mainPattern], stripeTiles2Material };
                        if (mainColor != myRend.materials[0].GetColor("_Front_Texture_Color"))
                        {
                            myRend.materials[0].SetColor("_Top_Texture_Color", mainColor);
                            myRend.materials[0].SetColor("_Front_Texture_Color", mainColor);
                            myRend.materials[0].SetColor("_Side_Texture_Color", mainColor);
                        }
                        if (TilesColor != myRend.materials[1].GetColor("_Front_Texture_Color"))
                        {
                            myRend.materials[1].SetColor("_Top_Texture_Color", TilesColor);
                            myRend.materials[1].SetColor("_Front_Texture_Color", TilesColor);
                            myRend.materials[1].SetColor("_Side_Texture_Color", TilesColor);
                        }
                    }
                }

            }
            StartCoroutine(WaitAndBuild());
        }
        IEnumerator WaitAndBuild()
        {
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                //transform.parent = FazNavMesh.Instance.surface.gameObject.transform;
                //yield return new WaitForEndOfFrame();
               // FazNavMesh.Instance.surface.BuildNavMesh();
                //Setlinks();
                while (transform.localScale != new Vector3(X_Scale,Height,Z_Scale))
                {
                    yield return new WaitForEndOfFrame();
                }
                LoadingScene l = GameObject.FindObjectOfType<LoadingScene>();
                if (SaveSystem.Instance != null && l != null)
                {
                    if (!SaveSystem.Instance.Loaded)
                    {
                        anyloadbaking = true;
                        mebaking = this;
                    }
                    while (mebaking == this && !SaveSystem.Instance.Loaded)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    anyloadbaking = false;
                }
                //Debug.Log("AA");
                
                if (!anyloadbaking || mebaking == this)
                {

                    Debug.Log(1);
                    if (SaveSystem.Instance != null)
                    {
                        Debug.Log(2);
                        SaveSystem.Instance.bakedNav = false;
                    }
                    else if (LoadOtherPizzeria.Instance != null)
                    {
                        LoadOtherPizzeria.Instance.bakedNav = false;
                    }
                    FazNavMesh.Instance.surface.BuildNavMesh();
                    anyloadbaking = false;
                    mebaking = null;
                    Debug.Log(3);
                    if (SaveSystem.Instance != null)
                    {
                        Debug.Log(4);
                        SaveSystem.Instance.bakedNav = true;
                    }
                    else if (LoadOtherPizzeria.Instance != null)
                    {
                        LoadOtherPizzeria.Instance.bakedNav = true;
                    }
                }
            }
            yield return null;
        }
        private void OnDestroy()
        {
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                FazNavMesh.Instance.surface.BuildNavMesh();
            }
        }
        private void Update()
        {
            if (Crosshair3D.Instance != null && BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.Destroy)
            {
                if (Crosshair3D.Instance.hits)
                {
                    if (Crosshair3D.Instance.hit.transform.gameObject == wallO)
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
            if (type.ToLower().Contains("floor"))
            {
                Height = Mathf.Clamp(Height, 0.1f, 1f);
            }
            else
            {
                Height = Mathf.Clamp(Height, 0.1f, 4f);
            }
            if (X_Scale != 0f && Z_Scale != 0f)
            {
                transform.localScale = new Vector3(X_Scale, Mathf.Lerp(transform.localScale.y, Height, Time.deltaTime * 4f), Z_Scale);
            }
            //transform.position = new Vector3(Position.x, Mathf.Lerp(transform.position.y, Position.y, Time.deltaTime * 6f), Position.z);
            if (Height - transform.localScale.y != Height)
            {
                transform.position = new Vector3(Position.x, Position.y - (Height - transform.localScale.y) / 2, Position.z);
            }
            else
            {
                transform.position = Position;
            }
        }
    }
}