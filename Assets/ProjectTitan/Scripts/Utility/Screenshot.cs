using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Utility
{
    public class Screenshot
    {
        private const string k_screenshotPath = "Screenshots/";
        private const string k_screenshotPrefix = "Screenshot";
        public static void Capture()
        {
            Debug.Log($"Screenshot captured: {GetFilePath() + GetFileName()}");
            ScreenCapture.CaptureScreenshot(k_screenshotPath + GetFileName());
        }

        private static string GetFileName()
        {
            string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            return $"{k_screenshotPrefix}_{timeStamp}.png";
        }

        private static string GetFilePath()
        {
#if UNITY_EDITOR
            // path: ProjectTitan/Screenshots/
            if(System.IO.Directory.Exists(Application.dataPath + "/../" + k_screenshotPath) == false)
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/../" + k_screenshotPath);
            }
            return k_screenshotPath;
#else
            return k_screenshotPath;      
#endif
        }
    }
}
