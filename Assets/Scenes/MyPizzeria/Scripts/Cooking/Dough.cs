using LyviN.kampe1234.Player.Baking;
using LyviN.Misc;
using LyviN.Player.Control;
using LyviN.Player.Interactions;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking
{
    public class Dough : MonoBehaviour
    {
        public Rigidbody myRigid;
        public float FlatLeft;
        public float FlatPercentage;
        public float FlatLeftInit;
        public float DirtyPercentage;

        bool canstop;
        [Header("Prefabs")]
        [Header("References")]
        public GameObject flatingDough;
        public L_DeformableMesh flatting;
        public L_DeformableMesh sauce;
        public L_DeformableMesh cheese;
        public GameObject sauceDough;
        public GameObject sauceDough_c1;
        public GameObject sauceDough_c2;
        public GameObject cheeseDough_c1;
        public GameObject cheeseDough_c2;
        public GameObject cheeseDough_c3;
        public GameObject OvenDough;
        public GameObject CutDough;

        public GameObject cheeseDough;
        public Transform CameraTarget;
        public Material DoughMat;
        public Material DirtyMat;
        public Material DirtyMatEdited;

        public Renderer CurrentRenderer;
        public Renderer[] DoughRenderers;
        public float DirtynessSpeed = 1f;
        [SerializeField] public List<List<CutTopping>> placedtoppings;
        public MeshCollider[] colliders;
        public MeshCollider[] CutColliders;
        public GameObject[] colliderO;
        public ovenBase Oven;
        public SkinnedMeshRenderer DoughBakeRend;
        public SkinnedMeshRenderer SauceBakeRend;
        public SkinnedMeshRenderer CheeseBakeRend;
        public Material CheeseMeltMat;
        public int BakePercentage;
        public float BakePercentageF;
        public float BurnPercentageF;
        public float BakeAvgTemp;
        public GameObject SmokeParticleO;
        public ParticleSystem SmokeParticle;
        public Color InitDoughColor;
        public Color BakedDoughColor;
        public Color BurntDoughColor;
        public PizzaPlate myPlate;
        public Vector3 platepos;
        public System.DateTime WhenSpawned;
        public System.DateTime WhenBaked;
        private void Start()
        {
            InitDoughColor = DoughBakeRend.material.GetColor("_BaseColor");
            cheeseDough.SetActive(false);
            sauceDough.SetActive(false);
            flatingDough.SetActive(true);
            OvenDough.SetActive(false);
            CutDough.SetActive(false);
            CurrentRenderer = DoughRenderers[0];
            placedtoppings = new List<List<CutTopping>>();
            for (int i =0; i<18; i++)
            {
                placedtoppings.Add(new List<CutTopping>());
            }
            //FlatLeft = -1f;
            WhenSpawned = System.DateTime.Now;
        }
        public void SetColliders(bool e)
        {
            foreach (MeshCollider mc in colliders)
            {
                if (!mc.transform.name.Contains("Pizza"))
                {
                    mc.enabled = e;
                }
            }
            foreach (GameObject co in colliderO)
            {
                foreach (Collider mc in co.GetComponents<Collider>())
                {
                    mc.enabled = e;
                }
            }
            foreach (List<CutTopping> lc in placedtoppings)
            {
                foreach (CutTopping ct in lc)
                {
                    foreach (Collider c in ct.myCols)
                    {
                        c.enabled = false;
                    }
                }
            }
        }
        public void SetCutColliders(bool e)
        {
            foreach (MeshCollider mc in CutColliders)
            {
                mc.enabled = e;
            }
        }
        public void Cut(bool asCustomer = false)
        {
            if (OvenDough.activeInHierarchy && BakePercentage > 0 && myPlate != null)
            {
                GetComponent<Collider>().enabled = false;
                
                    SetColliders(false);
                    SetCutColliders(true);
                
                OvenDough.SetActive(false);
                CutDough.SetActive(true);
                foreach(List<CutTopping> lc in placedtoppings)
                {
                    foreach (CutTopping ct in lc)
                    {
                        ct.Reassign();
                    }
                }
                SetColliders(false);
                SetCutColliders(false);
                GetComponent<Collider>().enabled = false;
            }
        }
        private void Update()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.bakeMode == "pizza" && BaseBake.Instance.currDough == this)
            {
                transform.eulerAngles = Vector3.zero;
            }
            FlatPercentage = 100f - ((FlatLeft / FlatLeftInit) * 100f);

            if (MainUI.Instance != null && myPlate != null && OvenDough.activeInHierarchy && BakePercentage > 0 && CustomerInteraction.Instance != null && CustomerInteraction.Instance.hit.transform == transform)
            {
                MainUI.Instance.isPizzaCut = this;
            }
            else if (MainUI.Instance != null && MainUI.Instance.isPizzaCut == this)
            {
                MainUI.Instance.isPizzaCut = null;
            }
                if (transform.parent != null && transform.parent.gameObject.layer == 15)
            {
                gameObject.layer = 15;
                flatingDough.layer = 15;
                sauceDough.layer = 15;
                sauceDough_c1.layer = 15;
                sauceDough_c2.layer = 15;
                cheeseDough.layer = 15;
                cheeseDough_c1.layer = 15;
                cheeseDough_c2.layer = 15;
                cheeseDough_c3.layer = 15;
                myRigid.useGravity = false;
                myRigid.isKinematic = true;
                if (FlatLeft == FlatLeftInit)
                {
                    transform.localPosition = new Vector3(0.067f, 0.541f, 0.07f);
                    transform.localEulerAngles = new Vector3(-54f, 40f, 72f);
                    transform.localScale = Vector3.one * 0.3417408f;
                }
            }
            else
            {
                CameraTarget.LookAt(transform.position);
                if (myRigid.velocity.magnitude < 0.05f && canstop && !myRigid.isKinematic)
                {
                    myRigid.isKinematic = true;
                    SetColliders(true);
                }
            }
            if (Oven != null)
            {
                if (Oven.currTemp > 180 && !Oven.isOpen && (Oven.Timer > 0 || Oven.Timer == -1))
                {
                    if (BakePercentage < 100)
                    {
                        if (BakeAvgTemp == 0)
                        {
                            BakeAvgTemp = Oven.currTemp;
                        }
                        else
                        {
                            BakeAvgTemp = Mathf.Lerp(BakeAvgTemp, Oven.currTemp, Time.deltaTime * 0.01f);
                        }
                        if (BakePercentage > 10)
                        {
                            CheeseBakeRend.material = CheeseMeltMat;
                        }
                        BakePercentageF = BakePercentageF + (Time.deltaTime * (Oven.currTemp * 0.9f)) * 0.004f;
                        BakePercentage = (int)BakePercentageF;
                        DoughBakeRend.SetBlendShapeWeight(0, BakePercentageF);
                        SauceBakeRend.SetBlendShapeWeight(0, BakePercentageF);
                        CheeseBakeRend.SetBlendShapeWeight(0, BakePercentageF);
                        DoughBakeRend.material.SetColor("_BaseColor", Color.Lerp(InitDoughColor, BakedDoughColor, BakePercentageF / 100f));
                    }
                    else if (BurnPercentageF < 100f)
                    {
                        if (BurnPercentageF < 80f)
                        {
                            SmokeParticleO.SetActive(true);
                            SmokeParticle.Play();
                        }
                        BurnPercentageF = BurnPercentageF + (Time.deltaTime * (Oven.currTemp * 0.9f)) * 0.004f;
                        DoughBakeRend.material.SetColor("_BaseColor", Color.Lerp(BakedDoughColor, BurntDoughColor, BurnPercentageF / 100f));
                        CheeseBakeRend.material.SetColor("_BaseColor", Color.Lerp(CheeseMeltMat.GetColor("_BaseColor"), BurntDoughColor, BurnPercentageF / 100f));

                    }
                }
                else
                {
                    SmokeParticle.Stop();
                }
            }
            
        }
        public void Drop()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.currDough == this && BaseBake.Instance.bakeMode == "pizza")
            {
                SetColliders(false);
                transform.SetParent(null);
                gameObject.layer = 12;
                flatingDough.layer = 12;
                sauceDough.layer = 12;
                sauceDough_c1.layer = 12;
                sauceDough_c2.layer = 12;
                cheeseDough.layer = 12;
                cheeseDough_c1.layer = 12;
                cheeseDough_c2.layer = 12;
                cheeseDough_c3.layer = 12;
                myRigid.isKinematic = false;
                myRigid.useGravity = true;
                myRigid.constraints &= ~RigidbodyConstraints.FreezePosition;
                transform.eulerAngles = Vector3.zero;
                BaseBake.Instance.currDough = null;
                BaseBake.Instance.bakeMode = "";
                Invoke("allowstop", 2f);
                BaseBake.Instance.PickupDropSound.Play();


            }
        }
        public void Throw()
        {
            if (transform.parent.gameObject.layer == 15)
            {
                SetColliders(false);

                transform.SetParent(null);
                gameObject.layer = 12;
                flatingDough.layer = 12;
                sauceDough.layer = 12;
                sauceDough_c1.layer = 12;
                sauceDough_c2.layer = 12;
                cheeseDough.layer = 12;
                cheeseDough_c1.layer = 12;
                cheeseDough_c2.layer = 12;
                cheeseDough_c3.layer = 12;
                myRigid.isKinematic = false;
                myRigid.useGravity = true;
                transform.eulerAngles = Vector3.zero;
                transform.position = transform.position + Vector3.up * 0.2f;
                if (FirstPersonCamera.Instance != null)
                {
                    transform.position = transform.position - FirstPersonCamera.Instance.CameraAnim.transform.right * 0.3f - FirstPersonCamera.Instance.CameraAnim.transform.forward * 0.3f;

                    myRigid.AddForce(FirstPersonCamera.Instance.CameraAnim.transform.forward * 4f + Vector3.up * 1.2f + FirstPersonCamera.Instance.CameraAnim.transform.right * -0.2f, ForceMode.Impulse);
                }
                if (BaseBake.Instance != null && BaseBake.Instance.currDough == this)
                {
                    BaseBake.Instance.currDough = null;
                }
                Invoke("allowstop", 2f);
            }
        }
        void allowstop()
        {
            canstop = true;
        }
        public void SetCamTargetPos(Transform between)
        {
            Vector3 target = new Vector3(between.position.x, transform.position.y, between.position.z);
            Vector3 dir = (transform.position - target).normalized;
            target = transform.position - dir * 1.15f + Vector3.up * 1.0f;
            CameraTarget.transform.position = target;
        }
        public void Pickup()
        {
            if (FlatLeft == FlatLeftInit && flatingDough.activeInHierarchy)
            {
                if (BaseBake.Instance != null && BaseBake.Instance.RightHand != null && BaseBake.Instance.ViewportAnim != null && BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                {
                    if (BaseBake.Instance.currDough == null)
                    {
                        if (Oven != null)
                        {
                            Oven.myPizza = null;
                            Oven = null;
                        }
                        if (myPlate != null)
                        {
                            myPlate.myRigid.isKinematic = false;

                            myPlate.myPizza = null;
                            myPlate = null;
                        }
                        CancelInvoke("allowstop");
                        canstop = false;
                        BaseBake.Instance.currDough = this;
                        transform.SetParent(BaseBake.Instance.RightHand.transform);
                        BaseBake.Instance.ViewportAnim.CrossFade("HoldingDough", 0.1f);
                        SetColliders(true);
                    }
                }
            }
            else
            {
                if (BaseBake.Instance != null && BaseBake.Instance.ViewportAnim.GetCurrentAnimatorStateInfo(1).IsName("NULL"))
                {
                    if (BaseBake.Instance.currDough == null)
                    {
                        if (Oven != null)
                        {
                            Oven.myPizza = null;
                            Oven = null;
                            WhenBaked = System.DateTime.Now;
                        }
                        if (myPlate != null)
                        {
                            myPlate.myRigid.isKinematic = false;
                            myPlate.myPizza = null;
                            myPlate = null;
                        }
                        CancelInvoke("allowstop");
                        canstop = false;
                        BaseBake.Instance.currDough = this;
                        transform.SetParent(BaseBake.Instance.myCam.transform);
                        myRigid.isKinematic = true;
                        myRigid.useGravity = false;
                        transform.eulerAngles = Vector3.zero;

                        transform.localPosition = new Vector3(0f, -0.09f, 1f);
                        BaseBake.Instance.bakeMode = "pizza";
                        BaseBake.Instance.PickupDropSound.Play();
                    }
                }
            }
        }
        public void checkFlat(string w)
        {
            if (w == "Dough1") {
                if (FlatLeft <= 1.5f)
                {
                    flatingDough.SetActive(false);
                    sauceDough.SetActive(true);
                    CurrentRenderer = DoughRenderers[1];
                    SetDirty();
                    FlatLeft = 0f;
                }
                else if (FlatLeft <= 19f)
                {
                    flatting.TransformInto();
                }
            }
            else if (w == "Dough2")
            {
                if (FlatLeft <= 1f)
                {
                    sauceDough.SetActive(false);
                    cheeseDough.SetActive(true);
                    CurrentRenderer = DoughRenderers[2];
                    SetDirty();
                }
                else if (FlatLeft <= 2.2f)
                {
                    sauce.TransformInto();
                }
            }
            else if (w == "Dough3")
            {
                if (FlatLeft <= 1f)
                {
                    FlatLeft = 0f;
                    OvenDough.SetActive(true);
                    cheeseDough.SetActive(false);
                    CurrentRenderer = DoughRenderers[3];
                    SetDirty();

                }
                else if (FlatLeft <= 2.2f)
                {
                    cheese.TransformInto();
                }
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (DirtyPercentage < 100f && collision.transform.tag == "Floor" && Oven == null)
            {
                DirtyPercentage = Mathf.Clamp(DirtyPercentage + Time.fixedDeltaTime * DirtynessSpeed * 45f, 0f, 100f);
                SetDirty();

            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (DirtyPercentage < 100f && collision.transform.tag == "Floor" && Oven == null)
            {
                DirtyPercentage = Mathf.Clamp(DirtyPercentage + Time.fixedDeltaTime * DirtynessSpeed * 75f, 0f, 100f);
                SetDirty();
            }else if (OvenDough.activeInHierarchy && BaseBake.Instance != null)
            {
                MultipleTags mt = collision.transform.GetComponent<MultipleTags>();
                if (mt != null)
                {
                    if (mt.has(CustomTags.Oven) && myPlate == null)
                    {
                        ovenBase o = mt.GetComponent<ovenBase>();
                        if (o.myPizza == null && o.isOpen)
                        {
                            transform.SetParent(o.transform);
                            transform.eulerAngles = Vector3.zero;
                            o.myPizza = this;
                            Oven = o;
                            o.isOpen = false;
                            if (!o.isOn)
                            {
                                o.PowerSwitchSound.Play();
                                o.isOn = true;
                                if (o.Timer > 0)
                                {
                                    o.StartCoroutine(o.CountTime());
                                }
                            }
                            BaseBake.Instance.bakeMode = "";
                            if (BaseBake.Instance.currDough == this)
                            {
                                BaseBake.Instance.currDough = null;
                            }
                            if (myPlate != null)
                            {
                                myPlate.myPizza = null;
                                
                            }
                        }
                    }
                    else if (transform.parent == null && Oven == null && mt.has(CustomTags.PizzaPlate))
                    {
                        if (myPlate == null && BakePercentageF > 0f)
                        {
                            PizzaPlate pp = mt.GetComponent<PizzaPlate>();
                            if (pp.myRigid.velocity.magnitude < 0.1f && pp.myPizza == null)
                            {
                                platepos = transform.position;
                                SetColliders(false);
                                myPlate = pp;
                                pp.myPizza = this;
                                transform.parent = pp.transform;
                                myRigid.isKinematic = true;
                                myRigid.useGravity = false;
                                myPlate.myRigid.isKinematic = true;
                                transform.position = platepos;

                            }
                        }
                    }
                }
            }
        }
        void SetDirty()
        {
            if (DirtyPercentage > 0f)
            {
                if (DirtyMatEdited == null)
                {
                    DirtyMatEdited = new Material(DirtyMat);
                    DirtyMatEdited.enableInstancing = false;
                }
                else
                {
                    float alpha = DirtyPercentage / 100f;
                    DirtyMatEdited.SetColor("_BaseColor", new Color(0.14f, 0.12f, 0.11f, alpha));
                    CurrentRenderer.materials = new Material[] { CurrentRenderer.materials[0], DirtyMatEdited };

                }
            }
            else if (CurrentRenderer.materials.Length != 1)
            {
                CurrentRenderer.materials = new Material[] { CurrentRenderer.materials[0] };
            }
        }
    }
}
