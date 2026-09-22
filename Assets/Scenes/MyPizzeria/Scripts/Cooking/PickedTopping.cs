using GameJolt.API;
using LyviN.Player.Interactions;
using LyviN.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

namespace LyviN.Player.Baking
{
    public class PickedTopping : MonoBehaviour
    {
        //OPEN SOURCE TODO: assign GameJolt Trophy Id
        private const int NINJA_TROPHY_ID = 168771;

        public PossibleToppings me;
        public Rigidbody myRigid;
        public bool dropped;
        public bool choppable;
        public GameObject CutPrefab;
        bool cutting;
        public void DropMe()
        {
            if (BaseBake.Instance != null && BaseBake.Instance.currTopping == this)
            {
                myRigid.useGravity = true;
                myRigid.isKinematic = false;
                transform.SetParent(null);
                BaseBake.Instance.bakeMode = "";
                BaseBake.Instance.currTopping = null;
                dropped = true;
            }
        }
        public IEnumerator Chop()
        {
            if (dropped && choppable)
            {
                if (me != PossibleToppings.Corn)
                {
                    float vel = Mathf.Abs(myRigid.velocity.y);
                    Debug.Log($"Velocity: {vel}");
                    if (vel > 1f)
                    {
                        if (GameJoltAPI.myInstance != null && GameJoltAPI.Instance.HasSignedInUser)
                        {
                            Trophies.TryUnlock(NINJA_TROPHY_ID, cb =>
                            {
                                Debug.Log($"Ninja Trophy Unlocking: {cb}");
                            });
                        }
                    }
                    myRigid.velocity = Vector3.zero;
                    myRigid.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                    BaseBake.Instance.KnifeSound.Play();
                    while (myRigid.velocity.y <= 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    while (myRigid.velocity.y > 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    BaseBake.Instance.ChopSound.Play();
                    GameObject meCut = Instantiate(CutPrefab);
                    meCut.transform.position = transform.position;
                    meCut.transform.rotation = transform.rotation;

                    Destroy(gameObject);

                }
                else
                {
                    GameObject meCut = Instantiate(CutPrefab);
                    meCut.transform.position = transform.position;
                    meCut.transform.rotation = transform.rotation;
                    Destroy(gameObject);

                }
            }
            yield return null;
        }
        private void Update()
        {
            if (dropped && choppable && !cutting)
            {
                if (MainUI.Instance != null)
                {
                    if (CustomerInteraction.Instance != null)
                    {
                        if (CustomerInteraction.Instance.hits && CustomerInteraction.Instance.hit.transform == transform)
                        {
                            MainUI.Instance.isToppingCut = this;
                            if (UnityEngine.Input.GetMouseButtonUp(0))
                            {
                                StartCoroutine(Chop());
                            }
                        }
                        else if (MainUI.Instance.isToppingCut == this)
                        {
                            MainUI.Instance.isToppingCut = null;
                        }
                    }
                }
            }
        }
        
    }
}