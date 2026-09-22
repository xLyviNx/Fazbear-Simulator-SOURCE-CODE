using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LyviN.Player.Baking.Pizza;

public class CutTopping : MonoBehaviour
{
    public Rigidbody myRigid;
    public PossibleToppings me;
    public bool locked;
    public Dough myDough;
    public bool dropped;
    public Collider[] myCols;
    public Renderer myRend;
    public Color initcol;
    public Color initcol2;
    public Color bakecol;
    public Color bakecol2;
    Vector3 initscale;
    Vector3 initpos;
    public bool beenreassigned;
    public LayerMask ReassignmentLayer;
    // Start is called before the first frame update
    void Start()
    {
        initscale = Vector3.zero;
        initpos = Vector3.zero;
        if (myRigid == null)
        {
            myRigid = GetComponent<Rigidbody>();
        }
        if (myRigid != null)
        {
            myRigid.AddForce(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)), ForceMode.Impulse);
        }
        if (myCols.Length <= 0)
        {
            myCols = GetComponents<Collider>();
        }
        if (myRend == null)
        {
            myRend = GetComponent<Renderer>();
        }
        initcol = myRend.materials[0].GetColor("_BaseColor");
        bakecol = initcol * 0.6f;
        if (me == PossibleToppings.Shrimps)
        {
            initcol2 = myRend.materials[1].GetColor("_BaseColor");
            bakecol2 = initcol2 * 0.6f;

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!locked && dropped)
        {
            if (collision.transform.tag == "Dough4")
            {
                locked = true;
                StartCoroutine(StopMe(collision));
            }
        }
    }
    private void Update()
    {
        if (myDough != null)
        {
            if (myDough.Oven != null)
            {
                if (initscale == Vector3.zero)
                {
                    initscale = transform.localScale;
                }
                if (initpos == Vector3.zero)
                {
                    initpos = transform.position;
                }
                transform.localScale = initscale * Mathf.Lerp(1f, 0.95f, myDough.BakePercentageF/100f);
                //transform.position = initpos - Vector3.up * Mathf.Lerp(0f, 0.012f, myDough.BakePercentageF/100f);
                if (myDough.BurnPercentageF == 0)
                {
                    if (me != PossibleToppings.Shrimps)
                    {
                        myRend.material.color = Color.Lerp(initcol, bakecol, myDough.BakePercentageF / 100f);
                    }
                    else
                    {
                        myRend.materials[0].color = Color.Lerp(initcol, bakecol, myDough.BakePercentageF / 100f);
                        myRend.materials[1].color = Color.Lerp(initcol2, bakecol2, myDough.BakePercentageF / 100f);

                    }
                }
                else
                {
                    if (me != PossibleToppings.Shrimps)
                    {
                        myRend.material.color = Color.Lerp(bakecol, Color.white * 0.05f, myDough.BurnPercentageF / 100f);
                    }
                    else
                    {
                        myRend.materials[0].color = Color.Lerp(bakecol, Color.white * 0.05f, myDough.BurnPercentageF / 100f);
                        myRend.materials[1].color = Color.Lerp(bakecol2, Color.white * 0.05f, myDough.BurnPercentageF / 100f);

                    }
                }
            }
            else
            {
                initpos = Vector3.zero;
            }
            
        }
    }
    public void Reassign()
    {
        Debug.Log(1);
        if (!beenreassigned && myDough.CutDough.activeInHierarchy)
        {
            Debug.Log(2);
            beenreassigned = true;
            foreach(Collider c in myCols)
            {
                c.enabled = false;
            }
            Ray r = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            Debug.Log(3);
            if (Physics.Raycast(r, out RaycastHit hit, 1f, ReassignmentLayer))
            {
                Debug.Log(4);
                Debug.Log(hit.transform.name);
                if (hit.transform.tag == "Dough5")
                {
                    Debug.Log(5);
                    transform.parent = hit.transform;
                }
                else
                {
                    Debug.Log(hit.transform.tag);
                }
            }
            foreach (Collider c in myCols)
            {
                c.enabled = true;
            }
        }
    }
    public void DropMe()
    {
        if (BaseBake.Instance != null && BaseBake.Instance.currToppingChopped == this)
        {
            myRigid.useGravity = true;
            myRigid.isKinematic = false;
            transform.SetParent(null);
            BaseBake.Instance.bakeMode = "";
            BaseBake.Instance.currToppingChopped = null;
            dropped = true;
        }
    }
    IEnumerator StopMe(Collision col)
    {
        while(myRigid.velocity.magnitude > 0.2f)
        {
            yield return new WaitForEndOfFrame();
        }
        myDough = ParentDough(col.transform.parent).GetComponent<Dough>();
        if (myDough.placedtoppings[(int)me].Count < 10 && myDough.BakePercentage == 0)
        {
            myRigid.isKinematic = true;
            myRigid.useGravity = false;
            transform.SetParent(col.transform);
            myDough.placedtoppings[(int)me].Add(this);
        }else
        {
            myDough = null;
            myRigid.AddForce((transform.position - col.transform.position) * 3f + Vector3.up * 3.5f, ForceMode.Impulse);
            locked = false;
        }
        yield return null;
    }
    public static GameObject ParentDough(Transform t)
    {
        if ((t.parent != null && t.parent.tag == "Pizza"))
        {
            return t.parent.gameObject;
        }
        else if (t.parent != null && (t.parent.tag == "Dough4" || t.parent.tag == "Dough5"))
        {
            return ParentDough(t.parent);
        }
        else if (t.tag == "Pizza")
        {
            return t.gameObject;
        }
        else
        {
            return null;
        }
    }
    public static GameObject ParentDough4(Transform t)
    {
        if (t.tag == "Dough4" && t.parent.tag != "Dough4")
        {
            return t.gameObject;
        }
        else if (t.parent != null && t.parent.tag == "Dough4")
        {
            return ParentDough4(t.parent);
        }
        else
        {
            return null;
        }
    }
}
