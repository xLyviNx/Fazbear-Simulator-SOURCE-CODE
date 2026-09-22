using LyviN.Player.Build;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair3D : MonoBehaviour
{
    public static Crosshair3D Instance;
    public GameObject Pointer;
    public Camera Camera;
    public RaycastHit hit;
    public RaycastHit hitWithBuildLayer;
    public bool hits;
    public bool hitsWithBuildLayer;
    public bool SnapToGrid;
    public bool xSnap;
    public bool ySnap;
    public bool zSnap;
    public float InterpolationSpeed;
    Vector3 lastpos;
    public Vector3 pos = Vector3.zero;
    public LayerMask SkipBuildLayer;


    //public float GridSize;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            this.enabled = false;
        }
    }
    void Update()
    {
        if (Camera != null)
        {
            if (Physics.Raycast(Camera.transform.position, Camera.transform.forward * 4f, out hitWithBuildLayer, 30f))
            {
                hitsWithBuildLayer = true;
            }
            else
            {
                hitsWithBuildLayer = false;
            }
            if (!Shared.hasTransaction)
            {

                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward * 1f, out hit, 10f, SkipBuildLayer))
                {
                    //Debug.Log(hit.transform);
                    float Xpos = hit.point.x;
                    float Ypos = hit.point.y;
                    float Zpos = hit.point.z;
                    
                   
                    if (BuildManager.Instance != null && BuildManager.Instance.Building != BuildingType.None)
                    {

                        if (SnapToGrid && BuildManager.Instance.Building != BuildingType.Destroy)
                        {
                            if (ySnap)
                            {
                                
                                    Ypos = Mathf.Round(Ypos);
                                
                                if (Ypos != lastpos.y)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }

                            }
                            if (xSnap)
                            {
                                Xpos = Mathf.Round(Xpos);
                                if (Xpos != lastpos.x)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }
                            }
                            if (zSnap)
                            {
                                Zpos = Mathf.Round(Zpos);
                                if (Zpos != lastpos.z)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }
                            }
                        }
                        if (BuildManager.Instance != null && WallBuilding.Instance != null)
                        {
                            if (BuildManager.Instance.A_Set)
                            {
                                Ypos = BuildManager.Instance.PointA_O.transform.position.y;

                            }
                            else if (BuildManager.Instance.B_Set)
                            {
                                Ypos = BuildManager.Instance.PointB_O.transform.position.y;
                            }
                        }
                        pos = new Vector3(Xpos, Ypos, Zpos);
                        if (BuildManager.Instance.Building != BuildingType.Destroy)
                        {
                            if (SnapToGrid)
                            {

                            }
                            Pointer.SetActive(true);
                            if (Pointer != null)
                            {
                                if (InterpolationSpeed != 0f)
                                {
                                    Pointer.transform.position = Vector3.Lerp(Pointer.transform.position, pos, Time.deltaTime * InterpolationSpeed);
                                }
                                else
                                {
                                    Pointer.transform.position = pos;
                                }
                            }
                            lastpos = pos;
                        }
                        else
                        {
                            Pointer.SetActive(false);
                        }
                        hits = true;
                    }
                    else
                    {
                        Pointer.SetActive(false);
                    }

                }
                else
                {
                    hits = false;
                    if (BuildManager.Instance != null && BuildManager.Instance.Building != BuildingType.None && BuildManager.Instance.Building != BuildingType.Destroy)
                    {
                        Vector3 posS = Camera.transform.position + Camera.transform.forward * 10f;
                        pos = posS;
                        float Xpos = pos.x;
                        float Ypos = pos.y;
                        float Zpos = pos.z;
                        if (BuildManager.Instance != null && WallBuilding.Instance != null)
                        {
                            if (BuildManager.Instance.A_Set)
                            {
                                Ypos = BuildManager.Instance.PointA_O.transform.position.y;

                            }
                            else if (BuildManager.Instance.B_Set)
                            {
                                Ypos = BuildManager.Instance.PointB_O.transform.position.y;
                            }
                        }
                        if (SnapToGrid)
                        {
                            if (ySnap)
                            {
                                if (Ypos != BuildManager.Instance.PointA_O.transform.position.y && Ypos != BuildManager.Instance.PointB_O.transform.position.y)
                                {
                                    Ypos = Mathf.Round(Ypos);
                                }
                                if (Ypos != lastpos.y)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }

                            }
                            if (xSnap)
                            {
                                Xpos = Mathf.Round(Xpos);
                                if (Xpos != lastpos.x)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }
                            }
                            if (zSnap)
                            {
                                Zpos = Mathf.Round(Zpos);
                                if (Zpos != lastpos.z)
                                {
                                    if (BuildingUI.Instance != null)
                                    {
                                        BuildingUI.Instance.SwitchPosS.Play();
                                    }
                                }
                            }
                        }
                        pos = new Vector3(Xpos, Ypos, Zpos);
                        if (InterpolationSpeed != 0f)
                        {
                            Pointer.transform.position = Vector3.Lerp(Pointer.transform.position, pos, Time.deltaTime * InterpolationSpeed);
                        }
                        else
                        {
                            Pointer.transform.position = pos;
                        }
                        lastpos = pos;
                    }
                }
            }
            else
            {
                Pointer.SetActive(false);
            }
        }
        else
        {
            hits = false;
            Pointer.SetActive(false);
        }
        if (Pointer.activeInHierarchy)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward);
            Pointer.transform.rotation = rot;
        }
    }
    IEnumerator PlayCursorSwitchClone()
    {
        if (BuildingUI.Instance != null)
        {
            GameObject x = Instantiate(BuildingUI.Instance.SwitchPosS.gameObject, BuildingUI.Instance.SwitchPosS.transform.parent);
            x.GetComponent<AudioSource>().Play();
            yield return new WaitForSecondsRealtime(0.1f);
            Destroy(x);
        }
        yield return null;
    }
}
