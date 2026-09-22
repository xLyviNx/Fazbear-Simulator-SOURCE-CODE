using GameJolt.API;
using LyviN.Map;
using LyviN.Map.Props;
using LyviN.Player;
using LyviN.Player.Baking;
using LyviN.Player.Control;
using LyviN.Player.Interactions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static LyviN.Player.Baking.Pizza;
using Random = UnityEngine.Random;

namespace LyviN.AI.Customers
{
    public class CustomerMain : MonoBehaviour
    {
        public NavMeshAgent myAgent;
        public Animator myAnim;
        [SerializeField] public CustomerStats myStats;
        public CustomerDialogManager Dialogs;
        public bool PointlessWandering;
        public GameObject DevPosTestPrefab;
        public GameObject DevPosTestPrefab2;
        public CashRegisterProp currCash;
        public byte currCashQueue;
        public bool RemoveMe;
        private bool orderedPizza;
        public bool talkingToPlayer = false;
        public int OrderID = -1;
        public GameObject orderNumberO;
        public TextMeshPro orderNumberT;
        bool lookingatme;
        DateTime whenOrdered;
        DateTime whenFoundChair;
        public ChairProp TargetChair;
        public bool chairSitting;
        private bool cantGetToChairDone;
        private int cantGetToChairCount;
        public GameObject[] mySkins;
        public GameObject mySkin;
        public int noChairCount;
        public bool GaveUp;
        public int myWallet;
        bool cancelqueuegiveup;
        public int PizzaPatienceMeter;
        public Vector3 tochairpos;
        public bool isEating;
        public LayerMask OnlyProps;
        public int BadPizzaCounter;
        public PizzaPlate myPizza;
        public int MovesDone;
        private int Etime;
        bool shortenedTime = false;
        public ArcadeMachine myArcade;
        
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int PIZZA_SOLD_TROPHY_ID = 169415;
        
        
        // Start is called before the first frame update
        void Start()
        {
            if (ThePizzeria.Instance != null) {
                int min = (int)(150f / (11f - ThePizzeria.Instance.Rating.GeneralRating()));
                int max = (int)(190f / (11f - ThePizzeria.Instance.Rating.GeneralRating()));
                myWallet = Random.Range(min, max);
                int htoppings = Random.Range(1, 6);
                int c = Random.Range(0, 10);
                if (c == 0)
                {
                    htoppings = 0;   
                }
                List<PossibleToppings> t = new List<PossibleToppings>();
                for (int i = 0; i < htoppings; i++)
                {
                    int id = Random.Range(1, 18);
                    if (!t.Contains((PossibleToppings)id))
                    {
                        t.Add((PossibleToppings)id);
                    }
                }
                int patience = (int)Mathf.Clamp(11f - ThePizzeria.Instance.Rating.GeneralRating(), 1, 10);
                int randomchance = Random.Range(0, 4);
                if (randomchance == 1)
                {
                    patience = Random.Range(1, 11);
                }
                myStats = new CustomerStats(patience, t);
                myStats.FavToppings = t;
            }
            StartCoroutine(StartLivingLol());
        }
        private void OnEnable()
        {
            foreach(GameObject s in mySkins)
            {
                s.SetActive(false);
            }
            int w = Random.Range(0, mySkins.Length);
            mySkin = mySkins[w];
            if (mySkin.tag == "Man")
            {
                float scale = Random.Range(0.93f, 1.15f);
                transform.localScale = Vector3.one * scale;
            }
            else if (mySkin.tag == "Woman")
            {
                float scale = Random.Range(0.86f, 0.98f);
                transform.localScale = Vector3.one * scale;
            }
            mySkin.SetActive(true);
        }
        
        // Update is called once per frame
        void Update()
        {
            if (ThePizzeria.Instance != null)
            {



                if (ThePizzeria.Instance.isOpened())
                {

                }
                else
                {
                    if (!GaveUp)
                    {
                        if (!chairSitting)
                        {
                            if (currCash != null)
                            {
                                currCash.RemoveFromQueue(this);
                            }
                            GaveUp = true;
                            if (!PointlessWandering)
                            {
                                StartCoroutine(StartLivingLol());
                            }
                        }
                    }
                }
            }
            if (noChairCount > 5)
            {
                if (OrdersManager.Instance != null)
                {
                    if (!GaveUp && Dialogs.myDialogText.text == "")
                    {
                        GaveUp = true;
                        VisitRating vr = new VisitRating();
                        vr.PizzaRating = 0f;
                        vr.ServiceRating = 1f;
                        Shared.AddRating(vr);
                        Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noSeatsGiveup)}", false);

                    }
                }
            }
            if (GaveUp)
            {
                if (OrderID >= 0)
                {
                    OrdersManager.Instance.RemoveOrderAt(OrderID);
                    OrderID = -1;
                }
                Vector3 toTarget = (transform.position - FirstPersonCamera.Instance.CameraAnim.transform.position).normalized;

                if (Vector3.Dot(toTarget, FirstPersonCamera.Instance.CameraAnim.transform.forward) > 0)
                {

                }
                else
                {
                    if (currCash != null)
                    {
                        currCash.RemoveFromQueue(this);
                    }
                    Destroy(gameObject);
                }
            }
            if (myAnim == null && mySkin != null)
            {
                myAnim = mySkin.GetComponent<Animator>();
            }
            else if (myAnim != null)
            {
                myAnim.SetFloat("Velocity", myAgent.velocity.magnitude);
            }
            if (RemoveMe)
            {
                if (currCash != null)
                {
                    currCash.RemoveFromQueue(this);
                }
                RemoveMe = false;
            }
            if (myAgent.velocity.magnitude < 0.1f)
            {
                if (!talkingToPlayer)
                {
                    if (currCash != null)
                    {
                        if (Vector3.Distance(myAgent.destination, transform.position) < 1.5f)
                        {

                            Vector3 relativePos = currCash.transform.position - transform.position;
                            Quaternion look = Quaternion.LookRotation(relativePos, Vector3.up);
                            transform.rotation = new Quaternion(transform.rotation.x, Mathf.Lerp(transform.rotation.y, look.y, (myAgent.angularSpeed / 250) * Time.deltaTime), transform.rotation.z, transform.rotation.w);
                        }
                    }
                }
                else
                {
                    Vector3 relativePos = FirstPersonCamera.Instance.transform.position - transform.position;
                    Quaternion look = Quaternion.LookRotation(relativePos, Vector3.up);
                    transform.rotation = new Quaternion(transform.rotation.x, look.y, transform.rotation.z, transform.rotation.w);

                }
                if (TargetChair != null && !chairSitting)
                {
                    TimeSpan secs = whenFoundChair - DateTime.Now;

                    if (Vector3.Distance(TargetChair.transform.position, transform.position) < 3f)
                    {
                        tochairpos = transform.position;
                        SitOnChair(TargetChair);
                        
                    }
                    else if (!cantGetToChairDone && Mathf.Abs(secs.Seconds) > 10)
                    {
                        Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noChairPath)}");
                        cantGetToChairDone = true;
                        Invoke("cantgetcooldown", 12f);
                        cantGetToChairCount++;
                    }
                }
            }
            if (CustomerInteraction.Instance != null)
            {
                if (CustomerInteraction.Instance.hit.transform == transform)
                {
                    Dialogs.myDialogObject.layer = 14;
                    Dialogs.myDialogText.gameObject.layer = 14;
                    if (Dialogs.myDialogChatbox != null)
                    {
                        Dialogs.myDialogChatbox.gameObject.layer = 14;
                    }
                    lookingatme = true;
                }
                else
                {
                    Dialogs.myDialogObject.layer = 11;
                    Dialogs.myDialogText.gameObject.layer = 11;
                    if (Dialogs.myDialogChatbox != null)
                    {
                        Dialogs.myDialogChatbox.gameObject.layer = 11;
                    }
                    lookingatme = false;
                }
            }

            if (orderedPizza && OrderID > -1 && currCash == null)
            {
                if (lookingatme)
                {
                    orderNumberO.SetActive(true);
                    orderNumberT.text = $"{OrderID}";
                }
                else
                {
                    orderNumberO.SetActive(false);
                }
            }
            else
            {
                orderNumberO.SetActive(false);
            }

        }
        void SitOnChair(ChairProp which)
        {
            chairSitting = true;
            myAgent.enabled = false;
            transform.position = which.transform.position;
            myAnim.Play("ChairSit");
            float ratio = Mathf.Clamp(((1f - transform.localScale.x)), 0f, 1f) * 1.15f;
            which.transform.localScale = which.transform.localScale + Vector3.one * ratio * 1.42f;
            transform.position = transform.position + Vector3.up * ratio * 1.03f + (Vector3.up * 0.65f);
            transform.eulerAngles = which.transform.eulerAngles;
            StartCoroutine(PizzaPatience());
        }
        private IEnumerator StartLivingLol()
        {
            if (!GaveUp)
            {
                PointlessWandering = true;
                float delay = Random.Range(0f, 5f);
                yield return new WaitForSeconds(delay);
                if (PointlessWandering)
                {
                    Vector3 np = RandomNonEmployeePos(false);
                    //Instantiate(DevPosTestPrefab, np, DevPosTestPrefab.transform.rotation, null);

                    if (np != Vector3.zero)
                    {
                        myAgent.SetDestination(np);
                    }
                    while (PointlessWandering)
                    {
                        while (myAgent.velocity.magnitude > 0.02f)
                        {
                            yield return new WaitForSeconds(0.1f);
                        }
                        delay = Random.Range(0.3f, 5f);
                        yield return new WaitForSeconds(delay);
                        if (PointlessWandering)
                        {
                            bool rd = false;
                            if (Random.Range(0, 3) == 2)
                            {
                                rd = true;
                            }
                            np = RandomNonEmployeePos(rd);
                            //Instantiate(DevPosTestPrefab, np, DevPosTestPrefab.transform.rotation, null);
                            if (np != Vector3.zero)
                            {
                                myAgent.SetDestination(np);
                            }
                            if (currCash == null && TargetChair == null && OrderID == -1)
                            {
                                MovesDone++;
                                if (MovesDone > Mathf.Clamp(myStats.Patience, 2, 5) * 5f && !GaveUp)
                                {
                                    Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.PatienceGiveUp)}");
                                    VisitRating vr = new VisitRating();
                                    vr.PizzaRating = 0f;
                                    vr.ServiceRating = 1f;
                                    Shared.AddRating(vr);
                                    while (Dialogs.myDialogText.text != "")
                                    {
                                        yield return new WaitForEndOfFrame();
                                    }
                                    talkingToPlayer = false;

                                    GaveUp = true;
                                    currCash = null;
                                    //StartCoroutine(StartLivingLol());
                                }
                            }
                            else
                            {
                                MovesDone = 0;
                            }
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else
            {
                PointlessWandering = true;
                float delay = Random.Range(0f, 5f);
                yield return new WaitForSeconds(delay);
                StartCoroutine(StaringCheck());
                while (PointlessWandering)
                {
                    while (myAgent.velocity.magnitude > 0.02f)
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                    delay = Random.Range(0.3f, 5f);
                    yield return new WaitForSeconds(delay);
                    Vector3 np = RandomGiveUpPos();
                    Instantiate(DevPosTestPrefab, np, DevPosTestPrefab.transform.rotation, null);
                    if (np != Vector3.zero)
                    {
                        myAgent.SetDestination(np);
                    }
                    yield return new WaitForEndOfFrame();

                }
                yield return new WaitForEndOfFrame();

            }
            yield return null;
        }
        void cantgetcooldown()
        {
            if (cantGetToChairCount == 3)
            {
                TargetChair = null;
                FindSeat();
                cantGetToChairCount = 0;
                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.OtherChair)}");
            }
            cantGetToChairDone = false;
        }
        public void GetInQueue(CashRegisterProp cash, byte position)
        {
            currCash = cash;
            currCashQueue = position;
            Vector3 np = cash.transform.position - (cash.transform.forward * ((position * 0.5f) + (position * cash.queueBetweenOffset) + cash.queueStartOffset));
            myAgent.SetDestination(np);
            Instantiate(DevPosTestPrefab, np, DevPosTestPrefab.transform.rotation, null);
            StartCoroutine(QueuePatienceMeter());
        }
        IEnumerator StaringCheck()
        {
            while(GaveUp)
            {
                yield return new WaitForSeconds(20f);
                if (GaveUp)
                {
                    if (Vector3.Distance(transform.position, FirstPersonCamera.Instance.transform.position) < 10f)
                    {
                        Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.QuitStaring)}");
                        while (Dialogs.myDialogText.text != "")
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                }
            }
            yield return null;
        }
        IEnumerator QueuePatienceMeter()
        {
            while (currCash != null && !GaveUp)
            {
                int timer = 0;
                int pmaxtime = myStats.Patience * 75;
                while (timer < pmaxtime)
                {
                    yield return new WaitForSeconds(1f);
                    timer++;
                    Etime++;
                }
                if (!orderedPizza && OrderID >= 0 && (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>"))
                {
                    Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.PatienceGiveUp)}");
                    VisitRating vr = new VisitRating();
                    vr.PizzaRating = 0f;
                    vr.ServiceRating = 1f;
                    Shared.AddRating(vr);
                    while (Dialogs.myDialogText.text != "")
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    currCash.RemoveFromQueue(this);
                    talkingToPlayer = false;
                    GaveUp = true;
                    currCash = null;
                    StartCoroutine(StaringCheck());
                    StartCoroutine(StartLivingLol());
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        IEnumerator PizzaPatience()
        {
            while (orderedPizza && chairSitting && !GaveUp && myPizza == null)
            {
                shortenedTime = false;
                int timer = 0;
                int pmaxtime = myStats.Patience * 150;
                while (timer < pmaxtime && myPizza == null)
                {
                    if (!ThePizzeria.Instance.isOpened() && !shortenedTime)
                    {
                        shortenedTime = true;
                        Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.timeShorterPizzeriaClosed)}");

                        while (Dialogs.myDialogText.text != "")
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        timer += (int)((pmaxtime - timer) * 0.4f);
                    }
                    yield return new WaitForSeconds(1f);
                    PizzaPatienceMeter = pmaxtime - timer;
                    timer++;
                    Etime++;
                }
                timer = 0;
                pmaxtime = myStats.Patience * 10;
                while (timer < pmaxtime && !shortenedTime && myPizza == null)
                {
                    yield return new WaitForSeconds(1f);
                    timer++;
                }
                if (myPizza == null)
                {
                    ChairGetUp();
                    Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.PatienceGiveUp)}");
                    VisitRating vr = new VisitRating();
                    vr.PizzaRating = 0f;
                    vr.ServiceRating = 1f;
                    Shared.AddRating(vr);
                    while (Dialogs.myDialogText.text != "")
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    GaveUp = true;
                    if (OrdersManager.Instance != null)
                    {
                        OrdersManager.Instance.RemoveOrder(this);
                        orderedPizza = false;
                        OrderID = -1;
                    }
                    StartCoroutine(StartLivingLol());
                    StartCoroutine(StaringCheck());
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return null;
        }
        public void ChairGetUp()
        {
            if (chairSitting)
            {
                if (TargetChair != null)
                {
                    transform.position = tochairpos;
                    TargetChair = null;
                    chairSitting = false;
                    myAgent.enabled = true;
                    myAnim.Play("NULL", 1);
                }
            }
        }
        private void OnDestroy()
        {
            if (currCash != null)
            {
                currCash.RefreshQueue();
            }
        }
        public void FindEmptyCash()
        {
            CashRegisterProp[] cashes = GameObject.FindObjectsOfType<CashRegisterProp>();
            CashRegisterProp best = null;
            foreach (CashRegisterProp c in cashes)
            {
                if (best == null || (c.Queue.Count < best.Queue.Count && best.Queue.Count < 5))
                {
                    best = c;
                }
            }
            if (best != null)
            {
                best.StartCoroutine(best.CheckQueueSomeone(this, true));
            }
            else
            {
                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noCashRegister)}");
            }
        }
        public Vector3 RandomGiveUpPos()
        {
            Vector3 pos = Vector3.zero;
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                float x = Random.Range(-FazNavMesh.Instance.surface.size.x/2f, FazNavMesh.Instance.surface.size.x / 2f);
                float z = Random.Range(-FazNavMesh.Instance.surface.size.z/2f, FazNavMesh.Instance.surface.size.z / 2f);
                pos = new Vector3(x, pos.y, pos.z);
                //Vector3 toTarget = (pos - FirstPersonCamera.Instance.CameraAnim.transform.position).normalized;
                //if (Vector3.Dot(toTarget, FirstPersonCamera.Instance.CameraAnim.transform.forward) > 0)
                //{
                 //   pos = RandomGiveUpPos();
                //}
            }
            NavMeshHit mnh;
            if (NavMesh.SamplePosition(pos, out mnh, 3.0f, myAgent.areaMask))
            {
                pos = mnh.position;
            }
            return pos;
        }
            public Vector3 RandomNonEmployeePos(bool closeToChairs)
        {
            Vector3 pos = Vector3.zero;
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                List<PlacedProp> pps = new List<PlacedProp>();
                List<BuiltWallMain> eo = new List<BuiltWallMain>();

                GameObject[] AllFloors = GameObject.FindGameObjectsWithTag("Floor");
                foreach (GameObject af in AllFloors)
                {
                    BuiltWallMain w = af.GetComponent<BuiltWallMain>();
                    if (w != null)
                    {
                        if (!w.EmployeesOnly)
                        {
                            eo.Add(w);
                        }
                    }
                }
                if (closeToChairs)
                {
                    PlacedProp[] allProps = GameObject.FindObjectsOfType<PlacedProp>();
                    foreach (PlacedProp af in allProps)
                    {
                        if (af.propname.ToLower().Trim().Contains("chair"))
                        {
                            pps.Add(af);
                        }
                    }
                }
                if (pps.Count <= 0)
                {
                    closeToChairs = false;
                }
                if (eo.Count > 0)
                {
                    if (!closeToChairs)
                    {
                        int which = Random.Range(0, eo.Count);
                        BuiltWallMain c = eo[which];
                        float x_r = Random.Range(c.transform.position.x + (-c.X_Scale / 2f), c.transform.position.x + (c.X_Scale / 2f));
                        float z_r = Random.Range(c.transform.position.z + (-c.Z_Scale / 2f), c.transform.position.z + (c.Z_Scale / 2f));
                        pos = new Vector3(x_r, c.transform.position.y + c.Height / 2f, z_r);
                    }
                    else
                    {
                        if (pps.Count > 0)
                        {
                            int which = Random.Range(0, pps.Count - 1);
                            PlacedProp c = pps[which];
                            BuiltWallMain closest = null;
                            float lastdist = -1f;
                            foreach (BuiltWallMain bw in eo)
                            {
                                if (closest == null || Vector3.Distance(bw.transform.position, c.transform.position) < lastdist)
                                {
                                    closest = bw;
                                    lastdist = Vector3.Distance(closest.transform.position, c.transform.position);
                                }
                            }
                            if (closest != null)
                            {
                                float x_r = Random.Range(closest.transform.position.x + (-closest.X_Scale / 2f), closest.transform.position.x + (closest.X_Scale / 2f));
                                float z_r = Random.Range(closest.transform.position.z + (-closest.Z_Scale / 2f), closest.transform.position.z + (closest.Z_Scale / 2f));
                                pos = new Vector3(x_r, closest.transform.position.y + closest.Height / 2f, z_r);
                            }
                        }
                    }
                }
                Instantiate(DevPosTestPrefab2, pos, DevPosTestPrefab.transform.rotation, null);
                Vector3 lp = pos;
                NavMeshHit mnh;
                if (NavMesh.SamplePosition(pos, out mnh, 3.0f, myAgent.areaMask))
                {
                    pos = mnh.position;
                }


            }
            return pos;
        }
        public void Interaction()
        {
            Debug.Log("Interact");
            if (currCash != null && currCashQueue == 0 && (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>"))
            {
                if (!orderedPizza)
                {
                    StartCoroutine(OrderPizza());
                }
            }
            else if (orderedPizza && OrderID >= 0 && BaseBake.Instance != null)
            {
                Debug.Log(1);
                if (BaseBake.Instance.currPlate != null)
                {
                    if (BaseBake.Instance.currPlate.myPizza != null)
                    {
                        StartCoroutine(GetMyPizza(BaseBake.Instance.currPlate));
                    }
                    else if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                    {
                        Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.EmptyPlate));
                    }
                }
                else
                {
                    if (BaseBake.Instance.currDough != null && BaseBake.Instance.bakeMode == "pizza")
                    {
                        if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                        {
                            Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.NoPlate));
                        }
                    }
                }
            }
        }
        public Order myOrder()
        {
            Order o = null;
            if (OrdersManager.Instance !=null)
            {
                if (OrderID >= 0 && OrdersManager.Instance.Orders.Count >= OrderID + 1)
                {
                    o = OrdersManager.Instance.Orders[OrderID];
                }
            }
            return o;
        }
        IEnumerator GetMyPizza(PizzaPlate plate, Dough elsePizza = null)
        {
            if (BadPizzaCounter < Mathf.Clamp(myStats.Patience, 2, 6))
            {
                if (plate.myPizza != null)
                {
                    if (plate.myPizza.BakePercentage > 10)
                    {
                        if (plate.myPizza.BurnPercentageF < 80f)
                        {
                            if (plate.myPizza.DirtyPercentage < 50)
                            {
                                bool hadtocut = false;
                                Ray r = new Ray(transform.position + transform.forward * 0.55f + Vector3.up * 2f, Vector3.down);
                                if (Physics.Raycast(r, out RaycastHit hit, 10f, OnlyProps))
                                {
                                    plate.DropMe();
                                    plate.myRigid.isKinematic = true;
                                    plate.BlockPickup = true;
                                    plate.transform.parent = null;
                                    plate.transform.position = hit.point;
                                    if (!plate.myPizza.CutDough.activeInHierarchy)
                                    {
                                        if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                                        {
                                            Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.NotCut));
                                        }
                                        while (Dialogs.myDialogText.text != "")
                                        {
                                            yield return new WaitForEndOfFrame();
                                        }
                                        plate.myPizza.Cut();
                                        hadtocut = true;
                                    }
                                    Order o = myOrder();
                                    if (o != null)
                                    {
                                        if (MyMenu.Instance != null)
                                        {
                                            Pizza p = MyMenu.Instance.GetPizzaByName(o.PizzaName);
                                            if (p != null)
                                            {
                                                int reqt = p.Toppings.Count;
                                                int i = 0;
                                                List<foundtop> ToppingsAmount = new List<foundtop>();
                                                List<foundtop> WantedToppingsAmount = new List<foundtop>();
                                                List<foundtop> UnWantedToppingsAmount = new List<foundtop>();
                                                foreach (List<CutTopping> l in plate.myPizza.placedtoppings)
                                                {
                                                    foundtop ft = new foundtop();
                                                    ft.amount = l.Count;
                                                    ft.index = i;
                                                    ToppingsAmount.Add(ft);
                                                    if (p.Toppings.Contains((PossibleToppings)i) && l.Count > 0)
                                                    {
                                                        Debug.Log($"Has: {(PossibleToppings)i}");
                                                        foundtop t = new foundtop();
                                                        t.amount = l.Count;
                                                        t.index = i;
                                                        WantedToppingsAmount.Add(t);

                                                    }
                                                    else if (l.Count > 0)
                                                    {
                                                        Debug.Log($"Doesn't Have: {(PossibleToppings)i}");

                                                        foundtop t = new foundtop();
                                                        t.amount = l.Count;
                                                        t.index = i;
                                                        UnWantedToppingsAmount.Add(t);
                                                    }
                                                    i++;
                                                }
                                                Debug.Log($"Wanted: {WantedToppingsAmount.Count}, didn't want: {UnWantedToppingsAmount.Count}, total: {ToppingsAmount.Count}");
                                                bool isGoodAmount = WantedToppingsAmount.Count == reqt && UnWantedToppingsAmount.Count == 0;
                                                if (isGoodAmount)
                                                {
                                                    float score = 0f;
                                                    foreach (foundtop t in WantedToppingsAmount)
                                                    {
                                                        if (t.index != 7)
                                                        {
                                                            float x = t.amount;
                                                            float kw = -1 * x * (x - 11);
                                                            score += kw;
                                                        }
                                                        else
                                                        {
                                                            if (t.index != 7)
                                                            {
                                                                float x = t.amount;
                                                                float kw = -1 * x * (x - 31);
                                                                score += kw;
                                                            }
                                                        }
                                                    }
                                                    score = (((score / WantedToppingsAmount.Count) / 15f) * reqt * 2);
                                                    Debug.Log($"Score for Pizza Toppings: {score}");
                                                    myPizza = plate;
                                                    float bakescore = 0f;
                                                    bakescore = plate.myPizza.BakePercentage / 23f;
                                                    Debug.Log($"Bake Score: {bakescore}");
                                                    float burnscore = Mathf.Clamp((70f - plate.myPizza.BurnPercentageF) / 30f, 0, 5f);
                                                    Debug.Log($"Burn Score: {burnscore}");
                                                    float avgtempscore = ((plate.myPizza.BakeAvgTemp) / 400f)*1.5f;
                                                    Debug.Log("Avg Temp Score: " + avgtempscore);
                                                    float priceScore = ((myWallet - p.Price) * 0.08f);
                                                    Debug.Log($"Price Score: " + priceScore);
                                                    TimeSpan ts = DateTime.Now - plate.myPizza.WhenSpawned;
                                                    float secs = Mathf.Abs((float)ts.TotalSeconds);
                                                    if (secs > 90)
                                                    {
                                                        secs = secs - 90;
                                                    }
                                                    else
                                                    {
                                                        secs = 0;
                                                    }
                                                    float OldTime = Mathf.Clamp(1.5f - (secs * 0.05f), 0f, 2f);
                                                    Debug.Log($"Price Score: " + priceScore);
                                                    Debug.Log($"Time: " + OldTime);

                                                    TimeSpan tsB = DateTime.Now - plate.myPizza.WhenBaked;
                                                    float secsB = Mathf.Abs((float)tsB.TotalSeconds);
                                                    if (secsB > 20)
                                                    {
                                                        secsB = secsB - 20;
                                                    }
                                                    else
                                                    {
                                                        secsB = 0;
                                                    }
                                                    float BakedTime = Mathf.Clamp(1.5f - (secsB * 0.02f), 0f, 2f);
                                                    Debug.Log($"Baked Time: " + BakedTime);
                                                    float waitingTime = Mathf.Clamp(PizzaPatienceMeter / 80f, 0, 2f);
                                                    Debug.Log("Waiting Time: " + waitingTime);
                                                    float cutscore = hadtocut ? 0 : 0.5f;
                                                    Debug.Log("Cut Score: " + cutscore);
                                                    float finalScore = score * 2f + bakescore * 2f + burnscore + avgtempscore * 0.7f + priceScore * 0.7f + OldTime + BakedTime + waitingTime + cutscore;
                                                    finalScore = (finalScore / 5f) - BadPizzaCounter * 0.1f ;
                                                    finalScore = Mathf.Clamp(MathF.Round(finalScore, 2), 0f, 10f);
                                                    Debug.Log($"Final Score: {finalScore}");
                                                    Shared.AddMoney(p.Price, cb =>
                                                    {
                                                        Shared.AddSoldPizzaScore(pscd =>
                                                        {
                                                            Trophies.TryUnlock(PIZZA_SOLD_TROPHY_ID);
                                                        });
                                                    }, "Selling Pizza");
                                                    StartCoroutine(EatingPizza());
                                                    VisitRating rr = new VisitRating();

                                                    int hadcutminus = hadtocut ? 1 : 0;
                                                    float servicerating = Mathf.Clamp(10f - (BadPizzaCounter * 0.7f)  - (Etime * 0.002f) - hadcutminus, 0.5f, 10f);

                                                    rr.PizzaRating = finalScore;
                                                    rr.ServiceRating = servicerating;
                                                    Shared.AddRating(rr);
                                                }
                                                else
                                                {
                                                    while (!Dialogs.isEmpty())
                                                    {
                                                        yield return new WaitForEndOfFrame();
                                                    }
                                                    Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.WrongPizza));
                                                    VisitRating vr = new VisitRating();
                                                    vr.PizzaRating = 0f;
                                                    vr.ServiceRating = 2f;
                                                    Shared.AddRating(vr);
                                                    while (Dialogs.myDialogText.text != "")
                                                    {
                                                        yield return new WaitForEndOfFrame();
                                                    }
                                                    Destroy(myPizza.gameObject);
                                                    myPizza = null;
                                                    GaveUp = true;
                                                    ChairGetUp();
                                                    StartCoroutine(StartLivingLol());
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                                {
                                    Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.DirtyText));
                                    BadPizzaCounter++;
                                }
                            }
                        }
                        else
                        {
                            if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                            {
                                Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.BurntText));
                                BadPizzaCounter++;
                            }
                        }
                    }
                    else
                    {
                        if (Dialogs.myDialogText.text == "" || Dialogs.myDialogText.text == "<>")
                        {
                            Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.Under10Bake));
                            BadPizzaCounter++;
                        }
                    }
                }
            }
            else
            {
                ChairGetUp();
                if (Dialogs.isEmpty())
                {
                    talkingToPlayer = true;
                    Dialogs.Type(CustomerTexts.RandomLine(CustomerTexts.BadGiveUp));
                }
                VisitRating vr = new VisitRating();
                vr.PizzaRating = 0f;
                vr.ServiceRating = 1f;
                Shared.AddRating(vr);
                while (Dialogs.myDialogText.text != "")
                {
                    yield return new WaitForEndOfFrame();
                }
                talkingToPlayer = false;
                GaveUp = true;
                StartCoroutine(StartLivingLol());
            }
            yield return null;
        }
        public IEnumerator EatingPizza()
        {
            if (myPizza != null)
            {
                myAnim.SetBool("Eating", true);
                int piecesleft = 7;
                while (piecesleft >= 0)
                {
                    myPizza.myPizza.CutColliders[piecesleft].gameObject.SetActive(false);
                    yield return new WaitForSeconds(13f + myStats.Patience);
                    piecesleft--;
                    yield return new WaitForEndOfFrame();
                }
                myAnim.SetBool("Eating", true);
                yield return new WaitForSeconds(2f);
                ChairGetUp();
                GaveUp = true;
                Destroy(myPizza.gameObject);
                myPizza = null;
                StartCoroutine(StartLivingLol());
            }
            yield return null;
        }
        public IEnumerator OrderPizza()
        {
            orderedPizza = true;
            while (Dialogs.myDialogText.text != "" && Dialogs.myDialogText.text != "<>")
            {
                yield return new WaitForEndOfFrame();
            }
            if (CustomerTexts.GreetingsText.Length > 0)
            {
                talkingToPlayer = true;
                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.GreetingsText)}", true);
                while (Dialogs.myDialogText.text != "")
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(1f);
                if (MyMenu.Instance != null)
                {
                    List<Pizza> fp = new List<Pizza>();
                    int lastfavtops = 0;
                    int tooExpensive = 0;
                    foreach (Pizza pizza in MyMenu.Instance.Menu)
                    {
                        
                        int favtops = 0;
                        foreach (Pizza.PossibleToppings t in pizza.Toppings)
                        {
                            if (myStats.FavToppings != null && myStats.FavToppings.Count > 0 && myStats.FavToppings.Contains(t))
                            {
                                favtops++;
                            }
                        }
                        if ((favtops > 0 && favtops > lastfavtops)|| myStats.FavToppings == null || myStats.FavToppings.Count == 0)
                        {
                            if (pizza.Price <= myWallet)
                            {
                                lastfavtops = favtops;
                                fp.Clear();
                                fp.Add(pizza);
                            }
                            else
                            {
                                tooExpensive++;
                            }
                        }
                        else
                        {
                            if (pizza.Price <= myWallet)
                            {
                                fp.Add(pizza);
                            }
                            else
                            {
                                tooExpensive++;
                            }
                        }
                    }
                    if (fp.Count > 0)
                    {
                        Pizza cheapest = null;
                        foreach (Pizza p in fp)
                        {
                            if (cheapest == null || p.Price < cheapest.Price)
                            {
                                cheapest = p;
                            }
                        }
                        Pizza sp = cheapest;
                        if (cheapest == null)
                        {
                            int e = Random.Range(0, fp.Count);
                            sp = fp[e];
                        }
                        Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.OrderText).Replace("%pizzaname%", sp.Name)}", true);
                        while (Dialogs.myDialogText.text != "")
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        yield return new WaitForSeconds(1f);
                        if (OrdersManager.Instance != null)
                        {
                            Order order = new Order(this, sp.Name, DateTime.Now);
                            OrdersManager.Instance.AddOrder(order, cb =>
                            {
                                whenOrdered = DateTime.Now;
                                currCash.RemoveFromQueue(this);
                                OrderID = cb;
                                bool emptychair = false;
                                ChairProp mychair = ChairAtTable(emptychair);
                                if (mychair != null)
                                {
                                    whenFoundChair = DateTime.Now;
                                    mychair.currentlySitting = this;
                                    myAgent.SetDestination(mychair.transform.position);
                                    TargetChair = mychair;
                                }
                                else
                                {
                                    TargetChair = null;
                                    Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noChairs)}");
                                    StartCoroutine(StartLivingLol());
                                    noChairCount++;
                                    Invoke("FindSeat", 15f);
                                }
                            });
                            talkingToPlayer = false;
                        }
                        else
                        {
                            if (tooExpensive > 0)
                            {
                                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.TooExpensiveGiveUp)}");
                            }
                            else
                            {
                                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.OrderGiveUpText)}");
                            }
                            while (Dialogs.myDialogText.text != "")
                            {
                                yield return new WaitForEndOfFrame();
                            }
                            currCash.RemoveFromQueue(this);
                            talkingToPlayer = false;
                            GaveUp = true;
                            StartCoroutine(StartLivingLol());
                        }
                    }
                    else
                    {
                        if (tooExpensive > 0)
                        {
                            Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.TooExpensiveGiveUp).Replace("%money%", myWallet + "")}");
                        }
                        else
                        {
                            Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.OrderGiveUpText)}");
                        }
                        while (Dialogs.myDialogText.text != "")
                        {
                            yield return new WaitForEndOfFrame();
                        }
                        currCash.RemoveFromQueue(this);
                        talkingToPlayer = false;
                        GaveUp = true;
                        StartCoroutine(StartLivingLol());
                    }
                }
                else
                {
                    Dialogs.Type($"Wait... there is nothing to order? Empty menu? What-");
                    while (Dialogs.myDialogText.text != "")
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    currCash.RemoveFromQueue(this);
                    talkingToPlayer = false;
                    VisitRating vr = new VisitRating();
                    vr.PizzaRating = 0f;
                    vr.ServiceRating = 2f;
                    Shared.AddRating(vr);
                    GaveUp = true;
                    StartCoroutine(StartLivingLol());

                }
            }
            yield return null;
        }
        void FindSeat()
        {
            CancelInvoke("FindSeat");
            bool emptychair = false;
            ChairProp mychair = ChairAtTable(emptychair);
            if (mychair != null)
            {
                mychair.currentlySitting = this;
                PointlessWandering = false;
                TargetChair = mychair;
                myAgent.SetDestination(mychair.transform.position);
                whenFoundChair = DateTime.Now;
            }
            else
            {
                Debug.Log("NoSeat");
                TargetChair = null;
                Dialogs.Type($"{CustomerTexts.RandomLine(CustomerTexts.noChairs)}");
                StartCoroutine(StartLivingLol());
                noChairCount++;
                Invoke("FindSeat", 15f);
            }

        }
        public ChairProp ChairAtTable(bool preferEmpty)
        {
            ChairProp cc = null;
            TableProp[] alltables = GameObject.FindObjectsOfType<TableProp>();
            List<TableProp> ptables = new List<TableProp>();
            int lastemptyseats = 0;
            foreach (TableProp table in alltables)
            {
                if (preferEmpty)
                {
                    List<ChairProp> chairs = new List<ChairProp>();
                    foreach (ChairProp c in table.myChairs)
                    {
                        if (c.currentlySitting == null)
                        {
                            chairs.Add(c);
                        }
                    }
                    if (chairs.Count == table.myChairs.Count)
                    {
                        ptables.Clear();
                        ptables.Add(table);
                        break;
                    }
                    else if (lastemptyseats == 0 || lastemptyseats < chairs.Count)
                    {
                        lastemptyseats = chairs.Count;
                        ptables.Clear();
                        ptables.Add(table);
                    }
                    else if (lastemptyseats == chairs.Count)
                    {
                        ptables.Add(table);
                    }

                }
                else
                {
                    List<ChairProp> chairs = new List<ChairProp>();
                    foreach (ChairProp c in table.myChairs)
                    {
                        if (c.currentlySitting == null)
                        {
                            chairs.Add(c);
                        }
                    }
                    if (chairs.Count > 0)
                    {
                        ptables.Add(table);
                    }
                }
            }
            if (ptables.Count > 0)
            {
                int wt = Random.Range(0, ptables.Count);
                List<ChairProp> chairs = new List<ChairProp>();
                foreach (ChairProp c in ptables[wt].myChairs)
                {
                    if (c.currentlySitting == null)
                    {
                        chairs.Add(c);
                    }
                }
                if (chairs.Count > 0)
                {
                    int wc = Random.Range(0, chairs.Count);
                    cc = chairs[wc];
                }
            }
            return cc;
        }
        [Serializable]
        public struct CustomerStats
        {
            public int Patience;
            public List<Pizza.PossibleToppings> FavToppings;
            public CustomerStats(int patience, List<Pizza.PossibleToppings> toppings)
            {
                Patience = patience;
                FavToppings = toppings;
            }
        }
        public struct foundtop
        {
            public int index;
            public int amount;
        }
    }
}