using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doughSaucePrefab : MonoBehaviour
{
    public Texture2D tex;
    public Texture2D tex2;
    public Renderer myRend;
    public LayerMask lay;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Pizza")
        {

            RaycastHit hit;
            Debug.Log(collision.contactCount);
            Ray ray = new Ray(collision.contacts[0].normal, collision.contacts[0].point - collision.contacts[0].normal);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            Debug.DrawLine(ray.origin, ray.direction, Color.blue);
            if (Physics.Raycast(ray, out hit, lay))
            {
                if (tex2 == null)
                {
                    Debug.Log("SRANIE");
                    tex2 = new Texture2D(tex.width, tex.height);
                    tex2.SetPixels(tex.GetPixels());
                    hit.collider.gameObject.GetComponent<Renderer>().materials[0].SetTexture("_BaseMap", tex2);
                    //myRend.materials[0].SetTexture("_BaseMap", tex2);
                    tex2.filterMode = FilterMode.Point;
                }
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= tex2.width;
                pixelUV.y *= tex2.height;
                tex2.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
                tex2.Apply();
                Debug.Log(hit.textureCoord);
                Debug.Log(hit.transform);
            }

        }
    }
}
