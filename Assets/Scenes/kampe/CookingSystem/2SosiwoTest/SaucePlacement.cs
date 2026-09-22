using UnityEngine;

public class SaucePlacement : MonoBehaviour
{
    //Texture2D tex;
    public Texture2D tex;
    public Texture2D tex2;
    //public Color color = new Color(255, 0, 0);
    Color32[] colors;
    //public float collisionRadius = 0.3f;

    Vector2 mousePos = new Vector2();
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        //Texture2D tex = new Texture2D(2048, 2048);
        colors = new Color32[tex.width * tex.height];
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Input.mousePosition; // Mouse position
        RaycastHit hit;
        Camera _cam = Camera.main; // Camera to use for raycasting
        Ray ray = _cam.ScreenPointToRay(pos);
        Physics.Raycast(_cam.transform.position, ray.direction, out hit, 10000.0f);
        Color c;
        if (hit.collider)
        {
            //tex = (Texture2D)hit.collider.gameObject.GetComponent<Renderer>().materials[0].mainTexture; // Get texture of object under mouse pointer
            //c = tex.GetPixelBilinear(hit.textureCoord.x, hit.textureCoord.y); // Get color from texture
            //tex.GetPixel(hit.textureCoord.x, hit.textureCoord.y);
            //Debug.Log(hit.textureCoord.x + " " + hit.textureCoord.y + " " + c);

            //tex.SetPixel(((int)hit.textureCoord.x), ((int)hit.textureCoord.y), Color.red);
            //tex.Apply();
            if (tex2 == null)
            {
                Debug.Log("Sranie");
                tex = (Texture2D)hit.collider.gameObject.GetComponent<Renderer>().materials[0].mainTexture as Texture2D;
                tex2 = new Texture2D(tex.width, tex.height);
                tex2.SetPixels(tex.GetPixels());
                hit.collider.gameObject.GetComponent<Renderer>().materials[0].SetTexture("_BaseMap", tex2);
            }
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex2.width;
            pixelUV.y *= tex2.height;
            tex2.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
            tex2.Apply();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            tex.SetPixel(1, 1, Color.red);
            //tex.SetPixels32(1,1,10,10,colors);
            tex.Apply();
            Debug.Log("Changed Color at " + ((int)contact.point.x) + " " + ((int)contact.point.y));
        }
    }
}
