using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenBlendShapes : MonoBehaviour
{

    public SkinnedMeshRenderer skinnedMeshRenderer;
    float b;
    public float speed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        b = 0;
    }

    // Update is called once per frame
    void Update()
    {
        b = b + 0.01f * speed * Time.deltaTime;
        b = Mathf.Clamp(b, 0, 100);
        //float a = Mathf.Lerp(0, 1, b);
        skinnedMeshRenderer.SetBlendShapeWeight(0, b);
        if (b >= 100)
        {
            this.enabled = false;
        }
    }
}
