using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan
{
    public class CaptureSceneView : EditorWindow
    {
        private Vector2Int _screenSize = new(1920, 1080);
        private readonly Vector2Int _maxScreenSize = new(1920, 1080);
        private Texture _capturedTexture;

        [MenuItem("Tools/Capture Scene View")]
        static void Init()
        {
            var window = GetWindow<CaptureSceneView>(true, "Capture Scene View");
            window.Show();
        }

        private void OnGUI()
        {
            _screenSize = EditorGUILayout.Vector2IntField("Screen Size", _screenSize);
            GUILayout.Label("Screenshot Image", EditorStyles.boldLabel);
            // if(_capturedTexture != null)
            // {
            //     GUILayout.BeginHorizontal();
            //     GUILayout.FlexibleSpace();
            //     GUILayout.Label(_capturedTexture, GUILayout.Width(position.width), GUILayout.Height(200));
            //     GUILayout.FlexibleSpace();
            //     GUILayout.EndHorizontal();
            // }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var textureToDraw = _capturedTexture != null ? _capturedTexture : Texture2D.whiteTexture;
            GUILayout.Label(textureToDraw, GUILayout.Width(200), GUILayout.Height(200), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Capture"))
            {
                CaptureScene(_screenSize.x, _screenSize.y);
            }
        }

        private void CaptureScene(int width, int height)
        {
            if(width <= 0 || height <= 0)
            {
                Debug.LogError("Invalid screen size");
                return;
            }

            var camera = SceneView.lastActiveSceneView.camera;
            var renderTexture = new RenderTexture(width, height, 24);
            camera.targetTexture = renderTexture;
            camera.Render();
            RenderTexture.active = renderTexture;
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;
            camera.targetTexture = null;
            _capturedTexture = renderTexture;
            // DestroyImmediate(renderTexture);

            var bytes = texture.EncodeToPNG();
            // <path to project folder>/Assets/../Capture.png
            System.IO.File.WriteAllBytes(Application.dataPath + "/../Capture.png", bytes);
            DestroyImmediate(texture);
        }

        private string GetFileName()
        {
            string curTime = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            return $"Capture_{curTime}.png";
        }
    }
}
