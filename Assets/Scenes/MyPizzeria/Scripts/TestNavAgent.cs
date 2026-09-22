using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TestNavAgent : MonoBehaviour
{
    public NavMeshAgent me;
    public NavMeshSurface surface;
    public GameObject surfacePrefab;
    public Transform Target;
    public float th;
    public Vector3 bp;
    public bool randompos;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        me.SetDestination(Target.transform.position);

        if (surface != null)
        {
            if (Vector3.Distance(bp, transform.position) > th)
            {
                //Bake();
            }
        }
        else
        {
            //GameObject e = Instantiate(surfacePrefab, null);
            //surface = e.GetComponent<NavMeshSurface>();
        }
    }
    void Bake()
    {
        bp = transform.position;
        surface.transform.position = bp;
        surface.RemoveData();
        surface.BuildNavMesh();
        Debug.Log("Bake");
    }
}
