using LyviN.Input;
using LyviN.Map;
using LyviN.Player;
using LyviN.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LyviN.AI.Customers
{
    public class CustomersManager : MonoBehaviour
    {
        public static CustomersManager Instance;
        public GameObject CustomerPrefab;
        public LayerMask CheckBetween;
        public List<CustomerMain> Customers = new List<CustomerMain>();
        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                StartCoroutine(SpawnLoop());
            }
            else if (Instance != this)
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
            
        }
        public int FreeChairsAmount()
        {
            int available = 0;
            foreach (LyviN.Map.Props.ChairProp c in GameObject.FindObjectsOfType<Map.Props.ChairProp>())
            {
                if (c.currentlySitting == null && c.myTable != null)
                {
                    available++;
                }
            }
            return available;
        }
        public IEnumerator SpawnLoop()
        {
            int howmanytimesnotspawned = 0;
            while (enabled)
            {
                if (ThePizzeria.Instance != null)
                {
                    float delay = 60f / Mathf.Clamp(ThePizzeria.Instance.Rating.GeneralRating(), 1, 10);
                    Debug.Log($"60 divided by {Mathf.Clamp(ThePizzeria.Instance.Rating.GeneralRating(), 1, 10)}");
                    Debug.Log($"The Delay is: {delay}");
                    yield return new WaitForSeconds(delay);
                    if (ThePizzeria.Instance != null)
                    {
                        Debug.Log(1);
                        if (ThePizzeria.Instance.isOpened())
                        {
                            Debug.Log(2);
                            int randomspawn = Random.Range(0, 4);
                            if (FreeChairsAmount() > 0 || randomspawn == 1)
                            {
                                Debug.Log(3);
                                if (Customers.Count < 50 + (int)(ThePizzeria.Instance.Rating.GeneralRating() * 10f))
                                {
                                    Debug.Log(4);
                                    int chance = Random.Range(0, (int)Mathf.Clamp(10 * (11f - ThePizzeria.Instance.Rating.GeneralRating()) / 10, 2, 10));
                                    Debug.Log($"Spawn Chance: {chance}");
                                    if (chance == 0)
                                    {
                                        SpawnCustomer();
                                        howmanytimesnotspawned = 0;
                                    }
                                    else
                                    {
                                        howmanytimesnotspawned++;
                                        if (howmanytimesnotspawned > 10)
                                        {
                                            SpawnCustomer();
                                            howmanytimesnotspawned = 0;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                howmanytimesnotspawned++;
                            }
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        void SpawnCustomer()
        {
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                if (FirstPersonCamera.Instance != null)
                {
                    Debug.Log("I will spawn a customer");
                    NavMeshHit mnh;
                    Vector3 pos = OutOfPlayerPos();
                    //Vector3 pos = transform.position;
                    if (NavMesh.SamplePosition(pos, out mnh, 1.0f, NavMesh.AllAreas))
                    {
                        GameObject c = Instantiate(CustomerPrefab, mnh.position, CustomerPrefab.transform.rotation, null);
                        CustomerMain cm = c.GetComponent<CustomerMain>();
                        Customers.Add(cm);
                    }
                }
            }
        }
        Vector3 OutOfPlayerPos()
        {
            Vector3 pos = Vector3.zero;
            if (FazNavMesh.Instance != null && FazNavMesh.Instance.surface != null)
            {
                if (FirstPersonCamera.Instance != null)
                {
                    float x = Random.Range(-FazNavMesh.Instance.surface.size.x / 2, FazNavMesh.Instance.surface.size.x / 2);
                    float y = 0.1f;
                    float z = Random.Range(-FazNavMesh.Instance.surface.size.z / 2, FazNavMesh.Instance.surface.size.z / 2);
                    pos = new Vector3(x, y, z);
                    Vector3 toTarget = (pos - FirstPersonCamera.Instance.CameraAnim.transform.position).normalized;

                    bool front = false;
                    if (Vector3.Dot(toTarget, FirstPersonCamera.Instance.CameraAnim.transform.forward) > 0)
                    {
                        front = true;
                    }
                    if (Physics.Linecast(FirstPersonCamera.Instance.CameraAnim.transform.position, pos + Vector3.up, CheckBetween))
                    {
                        Debug.DrawLine(FirstPersonCamera.Instance.CameraAnim.transform.position, pos);
                    }
                    else if (!front)
                    {
                        Debug.DrawLine(FirstPersonCamera.Instance.CameraAnim.transform.position, pos);
                    }
                    else
                    {
                        pos = OutOfPlayerPos();
                    }
                }

            }
            return pos;
        }
    }
}