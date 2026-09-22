using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thumbscreenshot : MonoBehaviour
{
    public RenderTexture rt;
    public bool Capture;
    // Use this for initialization
    private void Update()
    {
        if (Capture)
        {
            SaveTexture();
            Capture = false;
        }
    }
    public void SaveTexture()
    {
        ScreenCapture.CaptureScreenshot($"{Application.persistentDataPath}/export.png");
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        Destroy(tex);//prevents memory leak
        return tex;
    }
}
