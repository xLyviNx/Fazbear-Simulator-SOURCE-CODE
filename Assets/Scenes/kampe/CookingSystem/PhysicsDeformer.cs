using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsDeformer : MonoBehaviour
{
    public DeformableMesh deformableMesh;

    public float collisionRadius = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("chuj");
        foreach (var contact in collision.contacts)
        {
            deformableMesh.AddDepression(contact.point, collisionRadius);
        }
    }
}
