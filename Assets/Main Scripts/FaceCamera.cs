using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform what;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (what.gameObject.activeInHierarchy)
        {
            if (Camera.main != null)
            {
                what.transform.LookAt(Camera.main.transform);
            }
        }
    }
}
