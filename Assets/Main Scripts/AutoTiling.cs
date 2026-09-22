using UnityEngine;
using UnityEditor;
using System.Collections;

public class AutoTiling : MonoBehaviour
{
    public Renderer myRenderer;
    public Material myMat;
    public Transform A;
    public Transform B;
    public float ScaleMultiplier = 0.12f;
    public float Dist;
    public GameObject ColliderObject;
    void OnDrawGizmos()
    {
        if (A != null && B != null)
        {
            Dist = Vector3.Distance(A.position, B.position);
            if (myRenderer != null)
            {
                if (myMat == null)
                {
                    myMat = myRenderer.material;
                }
                if (myMat != null)
                {
                    myMat.mainTextureScale = new Vector2(Dist * ScaleMultiplier, 1f);


                    Vector3 between = A.position - B.position;
                    float distance = between.magnitude;
                    ColliderObject.transform.localScale = new Vector3(distance, ColliderObject.transform.localScale.y, ColliderObject.transform.localScale.z);
                    ColliderObject.transform.position = A.position + (between / 2f);
                    ColliderObject.transform.LookAt(B);
                }
            }
        }
    }
}