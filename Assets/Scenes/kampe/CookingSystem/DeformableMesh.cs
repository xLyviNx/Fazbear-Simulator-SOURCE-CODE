using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class DeformableMesh : MonoBehaviour
{

    //public float maxDepression = 1f; // no, not your depression while using unity lyvin...

    [Range(0f, 1f)]
    public float kindaSoftness = 0.2f; // Resistance but reversed

    List<Vector3> originalVerts;
    List<Vector3> modifiedVerts;

    List<Vector3> doneVerts;

    public GameObject doughObject;
    MeshFilter doughMeshFilter;
    Mesh doughMesh;
    MeshCollider doughMeshCollider;
    //public Mesh doughMesh;

    public GameObject doughDoneObject;
    MeshFilter doughDoneMeshFilter;
    Mesh doughDoneMesh;
    MeshCollider doughDoneMeshCollider;

    // Start is called before the first frame update
    void Start()
    {
        doughMeshCollider = doughObject.GetComponent<MeshCollider>();
        doughMeshFilter = doughObject.GetComponent<MeshFilter>();
        doughMesh = doughMeshFilter.mesh;
        //doughMeshCollider.sharedMesh = null;
        // doughMeshCollider.sharedMesh = doughMesh1;
        doughMesh.MarkDynamic();
        originalVerts = doughMesh.vertices.ToList();
        modifiedVerts = doughMesh.vertices.ToList();

        doughDoneMeshCollider = doughDoneObject.GetComponent<MeshCollider>();
        doughDoneMeshFilter = doughDoneObject.GetComponent<MeshFilter>();
        doughDoneMesh = doughDoneMeshFilter.mesh;
        doneVerts = doughDoneMesh.vertices.ToList();

        //modifiedVerts[5] = modifiedVerts[5] + Vector3.down;

        //modifiedVerts = doughMesh.vertices;

        //doughMesh.UploadMeshData(false); // nie ma ró¿nicy chyba bo domyœlnie przy nastêpnym renderze to stosuje jeœli modified

        //modifiedVerts[5] = Vector3.down;
        //doughMesh1.SetVertices(modifiedVerts);
        //Debug.Log("Test");
    }

    private void OnDisable()
    {
        //doughMesh.SetVertices(originalVerts);
    }

    public void AddDepression(Vector3 depressionPoint, float radius)
    {
        for (int i = 0; i < modifiedVerts.Count; ++i)
        {
            var distance = (depressionPoint - modifiedVerts[i]).magnitude;
            if (distance < radius)
            {
                Vector3 interpolated = Vector3.Lerp(modifiedVerts[i], doneVerts[i], kindaSoftness);
                //modifiedVerts[i] = doneVerts[i];
                //var newVert = modifiedVerts[i];
                //var newVert = originalVerts[i] + Vector3.down * maxDepression;
                modifiedVerts.RemoveAt(i);
                modifiedVerts.Insert(i, interpolated);
            }
        }
        doughMesh.SetVertices(modifiedVerts);
        doughMeshCollider.sharedMesh = null;
        doughMeshCollider.sharedMesh = doughMesh;
        //Debug.Log("Mesh depressed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
