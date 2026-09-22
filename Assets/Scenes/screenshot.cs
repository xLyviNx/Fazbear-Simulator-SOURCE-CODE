using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenshot : MonoBehaviour
{
    int i = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Input.GetMouseButtonDown(1))
        {
            screen();
        }
    }

    private void screen()
    {
        ScreenCapture.CaptureScreenshot("C:/Unity screens/screen" + i + ".png");
        Debug.Log("Caught in 4K");
        i++;
    }
}
