using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using LyviN.Player.Baking;

namespace LyviN.kampe1234.Player.Baking
{
    public class L_DeformableMesh : MonoBehaviour
    {

        //public float maxDepression = 1f; // no, not your depression while using unity lyvin...

        [Range(0f, 1f)]
        public float kindaSoftness = 0.2f; // Resistance but reversed

        List<Vector3> originalVerts;
        [SerializeField] List<Vector3> modifiedVerts;

        [SerializeField] List<Vector3> doneVerts;

        public GameObject doughObject;
        [SerializeField] MeshFilter doughMeshFilter;
        [SerializeField] Mesh doughMesh;
        [SerializeField] MeshCollider doughMeshCollider;
        //public Mesh doughMesh;

        public GameObject doughDoneObject;
        [SerializeField] MeshFilter doughDoneMeshFilter;
        [SerializeField] Mesh doughDoneMesh;
        [SerializeField] MeshCollider doughDoneMeshCollider;
        public Dough Me;
        bool tr;
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
            float initleft = 0f;
            for (int i = 0; i < originalVerts.Count; ++i)
            {

                if (doneVerts.Count > i + 1 && originalVerts.Count > i + 1)
                {
                    float diff = Vector3.Distance(doneVerts[i], originalVerts[i]);
                    initleft = initleft + diff;
                }
            }
            Me.FlatLeftInit = initleft;
            Me.FlatLeft = initleft;
        }

        private void OnDisable()
        {
            //doughMesh.SetVertices(originalVerts);
        }
        public void TransformInto()
        {
            StartCoroutine(transforming());
        }      
        public void TransformInto2()
        {
            float done = 0f;
            for (int i = 0; i < modifiedVerts.Count; ++i)
            {

                Vector3 interpolated = Vector3.Lerp(modifiedVerts[i], doneVerts[i], Time.deltaTime * 10f);
                modifiedVerts.RemoveAt(i);
                modifiedVerts.Insert(i, interpolated);
                float diff = Vector3.Distance(doneVerts[i], modifiedVerts[i]);
                done = done + diff;
            }
            Me.FlatLeft = done;
            doughMesh.SetVertices(modifiedVerts);
            doughMeshCollider.sharedMesh = null;
            doughMeshCollider.sharedMesh = doughMesh;
        }
        IEnumerator transforming()
        {
            if (!tr)
            {
                tr = true;
                while (Me.FlatLeft <= 1f)
                {
                    TransformInto2();
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(1f);
                Me.checkFlat(this.tag);
                Debug.Log("Done");
            }
            yield return null;
            yield break;
        }
        public void AddDepression(Vector3 depressionPoint, float radius)
        {
            float done = 0f;
            for (int i = 0; i < modifiedVerts.Count; ++i)
            {
                var distance = Vector3.Distance(depressionPoint, modifiedVerts[i]);
                if (distance < radius)
                {
                    Vector3 interpolated = Vector3.Lerp(modifiedVerts[i], doneVerts[i], kindaSoftness);
                    //modifiedVerts[i] = doneVerts[i];
                    //var newVert = modifiedVerts[i];
                    //var newVert = originalVerts[i] + Vector3.down * maxDepression;
                    modifiedVerts.RemoveAt(i);
                    modifiedVerts.Insert(i, interpolated);

                }
                float diff = Vector3.Distance(doneVerts[i], modifiedVerts[i]);
                done = done + diff;
            }
            Me.FlatLeft = done;
            Me.checkFlat(this.tag);
            if (BaseBake.Instance != null)
            {
                if (BaseBake.Instance.bakeMode == "Roller" && BaseBake.Instance.currTool != null)
                {
                    BaseBake.Instance.currToolRot = BaseBake.Instance.currToolRotObject.transform.localEulerAngles.x + 10f;
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
}