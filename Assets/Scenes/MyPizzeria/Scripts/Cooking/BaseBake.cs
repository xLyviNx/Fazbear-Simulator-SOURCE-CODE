using GameJolt.API;
using LyviN.Input;
using LyviN.kampe1234.Player.Baking;
using LyviN.Map;
using LyviN.Map.Props;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking
{
    public class BaseBake : MonoBehaviour
    {
        public static BaseBake Instance;

        public Animator ViewportAnim;
        public GameObject RightHand;
        public GameObject doughPrefab;
        public Dough currDough;
        bool animdone;
        public KitchenTableProp holdingRoller;
        public KitchenTableProp holdingLadle;
        public KitchenTableProp holdingGrater;
        public RaycastHit hit;
        public bool hits;
        public L_Deformer currDeformer;
        float lastflat;
        [Header("Prefabs")]
        public GameObject Roller3DPrefab;
        public GameObject Ladle3DPrefab;
        public GameObject Grater3DPrefab;
        [Header("New")]
        public Camera normalCam;
        public Camera BakeCam;
        public string bakeMode;
        public Dough editingDough;
        [SerializeField] private float LerpSpeed;
        public GameObject currTool;
        public GameObject currToolRotObject;
        public CheeseGrater cheeseGraterScript;
        public float currToolRot;
        public bool grabbingTool;
        public LayerMask bakemodelayer;
        [Header("Toppings Addition")]
        public GameObject[] ToppingFullPrefabs;
        public PickedTopping currTopping;
        public CutTopping currToppingChopped;
        public GameObject myCam;
        public AudioSource KnifeSound;
        public AudioSource ChopSound;
        public AudioSource PickupDropSound;
        public CornCan currCorn;
        [Header("Ovens")]
        public ovenBase OvenInteraction;
        [Header("Plate")]
        public GameObject PlatePrefab;
        public PizzaPlate currPlate;
        public bool NotHoldingAnything()
        {
            return bakeMode == "" && currTopping == null && currTool == null && currDough == null && currToppingChopped == null && currCorn == null && currPlate == null;
        }
        // Start is called before the first frame update
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                //StartCoroutine(FlattingLoop());
            }
            else
            {
                enabled = false;
            }
        }
        public void enterBake(string w, Dough d)
        {
            if (bakeMode == "" && editingDough == null && currTool == null)
            {
                bakeMode = w;
                editingDough = d;
                editingDough.SetColliders(true);
                d.SetCamTargetPos(transform);
                switch (w)
                {
                    case "Roller":
                        {
                            currTool = Instantiate(Roller3DPrefab, null);
                            currToolRotObject = currTool.transform.Find("Roller").gameObject;
                            currTool.transform.position = editingDough.transform.position + Vector3.up * 0.3f;
                            currTool.transform.eulerAngles = editingDough.transform.eulerAngles - new Vector3(0f, 90f, 0f);
                            grabbingTool = false;
                            break;
                        }
                    case "Ladle":
                        {
                            currTool = Instantiate(Ladle3DPrefab, null);
                            currTool.transform.position = editingDough.transform.position + Vector3.up * 0.3f;
                            currTool.transform.eulerAngles = editingDough.transform.eulerAngles - new Vector3(0f, 90f, 0f);
                            grabbingTool = false;
                            break;
                        }
                    case "Grater":
                        {
                            currTool = Instantiate(Grater3DPrefab, null);
                            currTool.transform.position = editingDough.transform.position + Vector3.up * 0.3f;
                            currTool.transform.eulerAngles = editingDough.transform.eulerAngles - new Vector3(0f, 90f, 0f);
                            grabbingTool = false;
                            cheeseGraterScript = currTool.GetComponent<CheeseGrater>();
                            break;
                        }
                    default: { grabbingTool = true; break; }
                }
            }
            else
            {
                BuildingUI.Instance.ErrorS.Play();
            }
        }
        // Update is called once per frame
        void Update()
        {
            CursorManager.Instance.isOven = OvenInteraction != null && bakeMode == "oven" && !OvenInteraction.changingtemp && !OvenInteraction.changingtime;

            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.isCookNotGrabbingTool = !grabbingTool && currTool != null && editingDough != null;
            }
            if (Binds.Instance != null)
            {
                if (!string.IsNullOrEmpty(bakeMode) && bakeMode != "tophold" && bakeMode != "cuttophold" && bakeMode != "pizza")
                {
                    normalCam.enabled = false;
                    BakeCam.enabled = true;
                    if (editingDough != null)
                    {
                        if (editingDough.CameraTarget != null)
                        {
                            BakeCam.transform.parent = null;
                            Quaternion targetrot = editingDough.CameraTarget.rotation;
                            Vector3 targetpos = editingDough.CameraTarget.position;
                            if (grabbingTool)
                            {
                                if (currTool != null)
                                {
                                    Vector3 relativePos = currTool.transform.position - BakeCam.transform.position;
                                    targetrot = Quaternion.Lerp(editingDough.CameraTarget.rotation, Quaternion.LookRotation(relativePos), 0.6f);
                                    targetpos = Vector3.Lerp(editingDough.CameraTarget.position, currTool.transform.position, 0.1f);
                                }
                            }
                            BakeCam.transform.rotation = Quaternion.Lerp(BakeCam.transform.rotation, targetrot, Time.deltaTime * LerpSpeed);
                            BakeCam.transform.position = Vector3.Lerp(BakeCam.transform.position, targetpos, Time.deltaTime * LerpSpeed);
                            BakeCam.fieldOfView = Mathf.Lerp(BakeCam.fieldOfView, 60f, Time.deltaTime * LerpSpeed);


                        }
                        if (currTool != null)
                        {
                            if (bakeMode == "Grater" && cheeseGraterScript != null)
                            {
                                if (editingDough.cheeseDough.activeInHierarchy)
                                {
                                    cheeseGraterScript.cheeseusage = editingDough.FlatPercentage;
                                }
                                else
                                {
                                    cheeseGraterScript.cheeseusage = 0f;

                                }
                            }
                            if (grabbingTool)
                            {
                                currTool.transform.localScale = Vector3.one;
                                if (CursorManager.Instance.isLocked())
                                {
                                    float MouseX = Mathf.Clamp(UnityEngine.Input.GetAxis("Mouse X"), -1f, 1f);
                                    float MouseY = Mathf.Clamp(UnityEngine.Input.GetAxis("Mouse Y"), -1f, 1f);
                                    if (!UnityEngine.Input.GetMouseButton(0))
                                    {
                                        if (!UnityEngine.Input.GetMouseButton(1) && !UnityEngine.Input.GetMouseButton(2))
                                        {
                                            Vector3 offset = new Vector3(BakeCam.transform.forward.x, 0f, BakeCam.transform.forward.z) * (MouseY / 20f) + new Vector3(BakeCam.transform.right.x, 0f, BakeCam.transform.right.z) * (MouseX / 30f);
                                            if (bakeMode == "Roller")
                                            {
                                                currToolRotObject.transform.localRotation = Quaternion.Euler(new Vector3(currToolRotObject.transform.localRotation.eulerAngles.x + (35f * currTool.transform.forward.z * MouseX) + (35f * currTool.transform.forward.x * MouseY), currToolRotObject.transform.localRotation.eulerAngles.y, currToolRotObject.transform.localRotation.eulerAngles.z));
                                            }

                                            Vector3 pos = currTool.transform.position + offset;
                                            RaycastHit ch;
                                            if (Physics.Raycast(currTool.transform.position, offset, out ch, 0.1f))
                                            {
                                                pos = Vector3.Lerp(currTool.transform.position, ch.point, 0.5f);
                                            }
                                            if (!Physics.Raycast(currTool.transform.position, offset, out ch, 0.01f))
                                            {
                                                if (Vector3.Distance(editingDough.transform.position, pos) < 1.3f)
                                                {
                                                    currTool.transform.position = pos;
                                                }
                                            }
                                            else
                                            {
                                                currTool.transform.position = Vector3.Lerp(ch.point, editingDough.transform.position, 0.05f) + Vector3.up * 0.01f;
                                            }
                                        }
                                    }
                                    else if (UnityEngine.Input.GetMouseButton(0))
                                    {
                                        Vector3 offset = Vector3.up * MouseY / 10f;
                                        Vector3 pos = currTool.transform.position + offset;
                                        if (pos != currTool.transform.position)
                                        {
                                            RaycastHit ch;
                                            if (Physics.Raycast(currTool.transform.position, offset, out ch, 0.1f))
                                            {
                                                pos = Vector3.Lerp(currTool.transform.position, ch.point, 0.5f);
                                            }
                                            if (!Physics.Raycast(currTool.transform.position, offset, out ch, 0.01f))
                                            {

                                                if (Vector3.Distance(editingDough.transform.position, pos) < 1.3f && pos.y >= (editingDough.transform.position - Vector3.up).y)
                                                {
                                                    currTool.transform.position = pos;
                                                }
                                            }
                                            else
                                            {
                                                currTool.transform.position = ch.point + Vector3.up * 0.01f;
                                            }
                                        }
                                    }
                                    if (UnityEngine.Input.GetMouseButton(1) || UnityEngine.Input.GetMouseButton(2))
                                    {
                                        currTool.transform.eulerAngles = currTool.transform.eulerAngles - currTool.transform.up * MouseX * 2f;
                                    }

                                }
                                //currToolRotObject.transform.localEulerAngles = Vector3.Lerp(currToolRotObject.transform.localEulerAngles, new Vector3(currToolRot, currToolRotObject.transform.localEulerAngles.y, currToolRotObject.transform.localEulerAngles.z), Time.deltaTime * 9f * LerpSpeed);
                            }
                            else
                            {
                                RaycastHit cursorhit;
                                Ray cray = BakeCam.ScreenPointToRay(UnityEngine.Input.mousePosition);
                                if (Physics.Raycast(cray, out cursorhit, bakemodelayer))
                                {
                                    Transform objectHit = cursorhit.transform;
                                    if (objectHit == currTool.transform)
                                    {
                                        if (UnityEngine.Input.GetMouseButtonDown(0))
                                        {
                                            grabbingTool = true;
                                        }
                                        currTool.transform.localScale = Vector3.one * 1.1f;
                                    }
                                    else
                                    {
                                        currTool.transform.localScale = Vector3.one;
                                    }
                                }
                                else
                                {
                                    Debug.Log("Nah");
                                    currTool.transform.localScale = Vector3.one;
                                }
                            }
                        }
                    }
                    else if (OvenInteraction != null && OvenInteraction.CameraTarget != null)
                    {
                        BakeCam.transform.parent = null;


                        RaycastHit mhit;
                        if (!OvenInteraction.changingtemp && !OvenInteraction.changingtime)
                        {
                            Quaternion targetrot = OvenInteraction.CameraTarget.rotation;
                            Vector3 targetpos = OvenInteraction.CameraTarget.position;
                            if (Physics.Raycast(BakeCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out mhit, 10f))
                            {
                                Vector3 relativePos = mhit.point - BakeCam.transform.position;
                                targetrot = Quaternion.Lerp(OvenInteraction.CameraTarget.rotation, Quaternion.LookRotation(relativePos), 0.1f);
                                if (mhit.transform == OvenInteraction.transform)
                                {
                                    targetpos = Vector3.Lerp(OvenInteraction.CameraTarget.position, mhit.point, 0.07f);
                                }
                            }
                            BakeCam.fieldOfView = Mathf.Lerp(BakeCam.fieldOfView, 25f, Time.deltaTime * LerpSpeed);

                            BakeCam.transform.rotation = Quaternion.Lerp(BakeCam.transform.rotation, targetrot, Time.deltaTime * LerpSpeed);
                            BakeCam.transform.position = Vector3.Lerp(BakeCam.transform.position, targetpos, Time.deltaTime * LerpSpeed);
                        }
                        else if (OvenInteraction.changingtemp)
                        {


                            Quaternion targetrot = OvenInteraction.TempCTarget.transform.rotation;
                            Vector3 targetpos = OvenInteraction.TempCTarget.transform.position;

                            if (Physics.Raycast(BakeCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out mhit, 10f))
                            {
                                Vector3 relativePos = mhit.point - BakeCam.transform.position;
                                targetrot = Quaternion.Lerp(OvenInteraction.TempCTarget.transform.rotation, Quaternion.LookRotation(relativePos), 0.1f);
                                if (mhit.transform == OvenInteraction.transform)
                                {
                                    targetpos = Vector3.Lerp(OvenInteraction.TempCTarget.transform.position, mhit.point, 0.07f);
                                }
                            }

                            BakeCam.transform.rotation = Quaternion.Lerp(BakeCam.transform.rotation, targetrot, Time.deltaTime * LerpSpeed);
                            BakeCam.transform.position = Vector3.Lerp(BakeCam.transform.position, targetpos, Time.deltaTime * LerpSpeed);
                        }
                        else if (OvenInteraction.changingtime)
                        {


                            Quaternion targetrot = OvenInteraction.TimerCTarget.transform.rotation;
                            Vector3 targetpos = OvenInteraction.TimerCTarget.transform.position;

                            if (Physics.Raycast(BakeCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out mhit, 10f))
                            {
                                Vector3 relativePos = mhit.point - BakeCam.transform.position;
                                targetrot = Quaternion.Lerp(OvenInteraction.TimerCTarget.transform.rotation, Quaternion.LookRotation(relativePos), 0.1f);
                                if (mhit.transform == OvenInteraction.transform)
                                {
                                    targetpos = Vector3.Lerp(OvenInteraction.TimerCTarget.transform.position, mhit.point, 0.07f);
                                }
                            }

                            BakeCam.transform.rotation = Quaternion.Lerp(BakeCam.transform.rotation, targetrot, Time.deltaTime * LerpSpeed);
                            BakeCam.transform.position = Vector3.Lerp(BakeCam.transform.position, targetpos, Time.deltaTime * LerpSpeed);
                        }
                    }
                }
                else
                {
                    if (currTool != null)
                    {
                        Destroy(currTool);
                        currTool = null;
                    }
                    grabbingTool = false;
                    normalCam.enabled = true;
                    BakeCam.enabled = false;
                    BakeCam.transform.position = normalCam.transform.position;
                    BakeCam.transform.rotation = normalCam.transform.rotation;
                }
                if (currDough != null && bakeMode != "pizza")
                {
                    if (ViewportAnim != null)
                    {
                        if (ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL") && !animdone)
                        {
                            animdone = true;
                            ViewportAnim.CrossFade("HoldingDough", 0.1f);
                        }
                        else if (!ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                        {
                            animdone = false;
                        }
                        if (Binds.Press(Binds.Instance.Controls[9]))
                        {
                            ViewportAnim.CrossFade("ThrowDough", 0.05f);
                        }
                    }
                }
                else
                {
                    if (currDough != null && bakeMode == "pizza")
                    {
                        if (Binds.Press(Binds.Instance.Controls[9]))
                        {
                            currDough.Drop();

                        }
                    }
                    else if (editingDough != null && bakeMode != "")
                    {
                        if (Binds.Press(Binds.Instance.Controls[9]))
                        {
                            editingDough = null;
                            bakeMode = "";

                        }
                    }
                    else
                    {
                        if (holdingRoller != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                holdingRoller.SwitchRoller();
                            }
                            /*if (hits)
                            {
                                if (hit.transform.tag == "Dough1")
                                {
                                    if (currDeformer == null)
                                    {
                                        currDeformer = hit.transform.GetComponent<L_Deformer>();
                                    }
                                    else
                                    {
                                        if (currDeformer.gameObject != hit.transform.gameObject)
                                        {
                                            currDeformer = hit.transform.GetComponent<L_Deformer>();
                                        }
                                    }
                                }
                                else
                                {
                                    currDeformer = null;
                                }
                            }
                            else
                            {
                                currDeformer = null;
                            }*/
                        }
                        else if (holdingLadle != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                holdingLadle.SwitchLadle();
                            }
                        }

                        else if (holdingGrater != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                holdingGrater.SwitchGrater();
                            }

                        }
                        else if (currTopping != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                DropTopping();
                            }
                        }
                        else if (currToppingChopped != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                DropChoppedTopping();
                            }
                        }
                        else if (currCorn != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                DropCan();
                            }
                        }
                        else if (currPlate != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                DropPlate();
                            }
                        }
                        else if (OvenInteraction != null)
                        {
                            if (Binds.Press(Binds.Instance.Controls[9]))
                            {
                                OvenInteraction = null;
                                bakeMode = "";
                            }
                        }
                    }
                }
            }
        }
        public void SpawnDough()
        {
            if (currDough == null && holdingRoller == null)
            {
                GameObject d = Instantiate(doughPrefab, RightHand.transform);
                currDough = d.GetComponent<Dough>();
            }
        }
        public void GetTopping(ToppingsShelfSlot sl)
        {
            if (sl.what != Pizza.PossibleToppings.Shrimps && sl.what != Pizza.PossibleToppings.Basil && sl.what != Pizza.PossibleToppings.Spinach)
            {
                if (bakeMode == "" && sl.what != Pizza.PossibleToppings.Unknown && sl.howMany > 0)
                {
                    Pizza.PossibleToppings what = sl.what;
                    sl.SetAmount((int)(sl.howMany - 1), succ =>
                    {
                        if (succ)
                        {
                            bakeMode = "tophold";
                            GameObject top = (GameObject)Instantiate(ToppingFullPrefabs[(int)what], myCam.transform);
                            currTopping = top.GetComponent<PickedTopping>();
                            currTopping.myRigid.isKinematic = true;
                            currTopping.myRigid.useGravity = false;
                            currTopping.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                            currTopping.me = what;
                            PickupDropSound.Play();

                            switch ((int)sl.what)
                            {

                                case 15:
                                    { break; }

                                default:
                                    {
                                        currTopping.choppable = true;
                                        break;
                                    }
                            }
                            if (GameJoltAPI.myInstance != null)
                            {
                                if (GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0 && SaveSystem.Instance != null)
                                {
                                    SaveSystem.Instance.WaitFrameAndSave();
                                }
                            }
                        }
                        else
                        {
                            BuildingUI.Instance.ErrorS.Play();
                        }
                    });
                }
            }
            else
            {
                GetChoppedToppingFromSL(sl);
            }
        }
        public void InteractWithOven(ovenBase ov)
        {
            if (NotHoldingAnything())
            {
                bakeMode = "oven";
                OvenInteraction = ov;
            }
        }
        public void GetChoppedTopping(CutTopping top)
        {
            if (bakeMode == "")
            {

                bakeMode = "cuttophold";
                currToppingChopped = top;
                currToppingChopped.myRigid.isKinematic = true;
                currToppingChopped.myRigid.useGravity = false;
                currToppingChopped.transform.SetParent(myCam.transform);
                currToppingChopped.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                if (currToppingChopped.myDough != null)
                {
                    currToppingChopped.myDough.placedtoppings[(int)top.me].Remove(currToppingChopped);
                }
                currToppingChopped.locked = false;
                currToppingChopped.dropped = false;
                PickupDropSound.Play();

            }
        }
        public void GetChoppedToppingFromSL(ToppingsShelfSlot sl)
        {
            if (bakeMode == "" && sl.what != Pizza.PossibleToppings.Unknown && sl.howMany > 0)
            {
                Pizza.PossibleToppings what = sl.what;
                sl.SetAmount((int)(sl.howMany - 1), succ =>
                {
                    if (succ)
                    {
                        bakeMode = "cuttophold";
                        GameObject topc = (GameObject)Instantiate(ToppingFullPrefabs[(int)what], myCam.transform);
                        currToppingChopped = topc.GetComponent<CutTopping>();
                        currToppingChopped.myRigid.isKinematic = true;
                        currToppingChopped.myRigid.useGravity = false;
                        currToppingChopped.transform.SetParent(myCam.transform);
                        currToppingChopped.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                        currToppingChopped.locked = false;
                        currToppingChopped.dropped = false;
                        PickupDropSound.Play();
                        if (GameJoltAPI.myInstance != null)
                        {
                            if (GameJoltAPI.Instance.HasSignedInUser && GameJoltAPI.Instance.CurrentUser.ID != 0 && SaveSystem.Instance != null)
                            {
                                SaveSystem.Instance.WaitFrameAndSave();
                            }
                        }
                    }
                    else
                    {
                        BuildingUI.Instance.ErrorS.Play();
                    }
                });
            }
        }
        public void DropTopping()
        {
            currTopping.DropMe();
            PickupDropSound.Play();
        }
        public void DropChoppedTopping()
        {
            currToppingChopped.DropMe();
            PickupDropSound.Play();

        }
        public void PickupDroppedTopping(PickedTopping top)
        {
            if (NotHoldingAnything())
            {
                top.transform.SetParent(myCam.transform);
                bakeMode = "tophold";
                currTopping = top;
                currTopping.dropped = false;
                currTopping.myRigid.isKinematic = true;
                currTopping.myRigid.useGravity = false;
                currTopping.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                if (MainUI.Instance != null && MainUI.Instance.isToppingCut == currTopping)
                {
                    MainUI.Instance.isToppingCut = null;
                }
                PickupDropSound.Play();
            }
        }
        public void PickupCan(CornCan can)
        {
            if (NotHoldingAnything())
            {
                can.transform.SetParent(myCam.transform);
                bakeMode = "tophold";
                currCorn = can;
                currCorn.myRigid.isKinematic = true;
                currCorn.myRigid.useGravity = false;
                currCorn.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                currCorn.transform.localEulerAngles = new Vector3(0f, 0f, 75f);
                PickupDropSound.Play();
            }
        }
        public void DropCan()
        {
            if (currCorn != null)
            {
                currCorn.DropMe();
                PickupDropSound.Play();
            }
        }
        public void DropPlate()
        {
            if (currPlate != null)
            {
                currPlate.DropMe();
                PickupDropSound.Play();
            }
        }
        public void GetPlate()
        {

            if (bakeMode == "" && NotHoldingAnything())
            {
                bakeMode = "tophold";
                GameObject plate = (GameObject)Instantiate(PlatePrefab, myCam.transform);
                currPlate = plate.GetComponent<PizzaPlate>();
                currPlate.myRigid.isKinematic = true;
                currPlate.myRigid.useGravity = false;
                currPlate.transform.localPosition = new Vector3(0f, -0.09f, 1f);
                currPlate.dropped = false;
                PickupDropSound.Play();
            }
        }
    }

}