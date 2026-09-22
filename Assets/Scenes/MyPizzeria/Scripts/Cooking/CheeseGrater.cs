using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseGrater : MonoBehaviour
{
    public SkinnedMeshRenderer cheese;
    public float cheeseusage;
    public Animator Anim;
    public float lastCheese;
    public float lastcheck;
    // Update is called once per frame
    void Update()
    {
       //float chu = Mathf.Clamp(cheeseusage - Mathf.Clamp(20f / cheeseusage, 0, 15f), 0f, 100f);
        cheese.SetBlendShapeWeight(0, cheeseusage);
        Anim.SetFloat("CheeseUsage", cheeseusage);
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (BaseBake.Instance != null)
        {
            if (BaseBake.Instance.editingDough != null)
            {
                if (BaseBake.Instance.bakeMode == "Grater")
                {
                    if (collision.transform.IsChildOf(BaseBake.Instance.editingDough.transform))
                    {
                        Anim.SetBool("using", false);
                    }
                }
            }
        }

    }
    void CheckAnim()
    {
        CancelInvoke("CheckAnim");
        if (lastcheck == cheeseusage)
        {
            Anim.SetBool("using", false);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (BaseBake.Instance != null)
        {
            if (BaseBake.Instance.editingDough != null)
            {
                if (BaseBake.Instance.bakeMode == "Grater")
                {
                    if (collision.transform.IsChildOf(BaseBake.Instance.editingDough.transform))
                    {
                        if (lastCheese != cheeseusage)
                        {
                            if (BaseBake.Instance.editingDough.FlatPercentage <= 99.9f)
                            {
                                Anim.SetBool("using", true);
                                lastCheese = cheeseusage;
                            }
                            else
                            {
                                Anim.SetBool("using", false);
                                lastCheese = cheeseusage;
                            }
                        }else
                        {
                            lastcheck = cheeseusage;
                            Invoke("CheckAnim", 1f);
                        }
                    }
                }
            }
        }
    }
}
