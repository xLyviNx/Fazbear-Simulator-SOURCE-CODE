using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoon : MonoBehaviour
{
    public float TimeToDestroy = 3f;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyMe", TimeToDestroy);
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }
}
