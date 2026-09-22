using LyviN.Player.Baking;
using LyviN.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportArms : MonoBehaviour
{
    public void ThrowDough()
    {
        if (BaseBake.Instance != null && BaseBake.Instance.currDough != null && BaseBake.Instance.bakeMode != "pizza")
        {
            transform.GetComponentInChildren<Dough>().Throw();
        }
    }
    public void StepSound()
    {
        if (FirstPersonController.Instance != null)
        {
            FirstPersonController.Instance.StepSound();
        }
    }
    
}
