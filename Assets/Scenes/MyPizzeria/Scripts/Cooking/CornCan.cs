using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Player.Baking
{
    public class CornCan : MonoBehaviour
    {
        public bool Pressing;
        bool adding;
        public Animator anim;
        public GameObject CornPrefab;
        public List<GameObject> SpawnedCorn = new List<GameObject>();
        public ParticleSystem CornParticles;
        public Rigidbody myRigid;
        int uses = 9;
        bool corncooldown;
        private void Update()
        {
            if (BaseBake.Instance != null)
            {
                if (BaseBake.Instance.currCorn == this)
                {
                    if (UnityEngine.Input.GetMouseButton(0))
                    {
                        Pressing = true;
                        if (!adding)
                        {
                            StartCoroutine(Sypanie());
                        }
                    }
                    else
                    {
                        Pressing = false;
                    }
                }
                else
                {
                    Pressing = false;
                }
            }
            if (adding)
            {
                if (CornParticles.isStopped)
                {
                    CornParticles.Play();
                }
            }
            else
            {
                CornParticles.Stop();
            }
            anim.SetBool("Add", adding);
        }
        public void DropMe()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.currCorn == this)
            {
                BaseBake.Instance.currCorn = null;
                transform.SetParent(null);
                transform.eulerAngles = Vector3.zero;
                myRigid.isKinematic = false;
                myRigid.useGravity = true;
                BaseBake.Instance.bakeMode = "";
            }
        }
        public IEnumerator Sypanie() //tak, po polsku, wiem ¿e cursed 
        {
            if (!adding)
            {
                adding = true;
                if (Pressing)
                {
                    while (Pressing)
                    {
                        CornParticles.gameObject.SetActive(true);
                        yield return new WaitForSeconds(0.85f);
                        if (uses > 0)
                        {
                            anim.CrossFade("cornadd", 0.005f);
                            uses--;
                        }
                        else
                        {
                            if (BaseBake.Instance.currCorn == this)
                            {
                                DropMe();
                                BaseBake.Instance.currCorn = null;
                            }
                            Destroy(gameObject);
                        }
                    }
                }
                if (adding)
                {

                    adding = false;
                }
            }
            yield return null;
        }
        public void collision(GameObject other)
        {
            if (other.tag == "Dough4")
            {
                if (!corncooldown)
                {
                    //corncooldown = true;
                    GameObject other2 = CutTopping.ParentDough4(other.transform);
                    List<ParticleCollisionEvent> ce = new List<ParticleCollisionEvent>();
                    CornParticles.GetCollisionEvents(other, ce);
                    GameObject corn = Instantiate(CornPrefab);
                    int i = Random.Range(0, ce.Count);
                    corn.transform.position = ce[i].intersection;
                    CutTopping ct = corn.GetComponent<CutTopping>();
                    Debug.Log(other2);
                    Dough dough = other2.transform.parent.GetComponent<Dough>();
                    dough.placedtoppings[7].Add(ct);
                    ct.myDough = dough;
                    corn.transform.eulerAngles = new Vector3(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
                    corn.transform.SetParent(other.transform);
                    //CancelInvoke("endcorncd");
                    //Invoke("endcorncd", 0.03f);
                }
            }
        }
        void endcorncd()
        {
            CancelInvoke("endcorncd");
            corncooldown = false;
        }
    }
}
