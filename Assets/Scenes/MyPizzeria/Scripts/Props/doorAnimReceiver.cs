using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorAnimReceiver : MonoBehaviour
{
    public AudioSource OpenS;
    public AudioSource CloseS;
    public void OpenDoor()
    {
        OpenS.Play();
    }
    public void CloseDoor()
    {
        CloseS.Play();
    }
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
    public void DestroyParent()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
