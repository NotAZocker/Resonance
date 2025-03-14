using UnityEngine;
using System.IO;

public class ScreenshotCapture : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12)) // Press F12 to take a screenshot
        {
            string path = Path.Combine(Application.dataPath, "Screenshot.png");
            ScreenCapture.CaptureScreenshot(path, 2);
            Debug.Log("Screenshot saved to: " + path);
        }
    }
}