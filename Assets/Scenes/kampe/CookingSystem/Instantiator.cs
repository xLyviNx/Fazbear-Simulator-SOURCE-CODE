using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public GameObject objectToInstantiate;

    // Start is called before the first frame update
    void Start()
    {
        GameObject instantiatedObject = Instantiate(objectToInstantiate, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
