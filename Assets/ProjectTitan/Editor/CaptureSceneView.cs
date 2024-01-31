using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Titan
{
    public class CaptureSceneView : EditorWindow
    {
        private const int kInitWidth = 400;
        private const int kInitHeight = 600;

        private Vector2Int _screenSize = new(1920, 1080);
        private readonly Vector2Int _maxScreenSize = new(8000, 8000);
        private Texture2D _capturedTexture;

        private const string kDefaultFileName = "Capture";

        [MenuItem("Tools/Capture Scene View")]
        static void Init()
        {
            var window = GetWindow<CaptureSceneView>(true, "Capture Scene View");
            var pos = window.position;
            pos.width = kInitWidth;
            pos.height = kInitHeight;
            window.position = pos;
            window.Show();
        }

        private void OnGUI()
        {
            _screenSize = EditorGUILayout.Vector2IntField("Screen Size", _screenSize);
            _screenSize.x = Mathf.Clamp(_screenSize.x, 0, _maxScreenSize.x);
            _screenSize.y = Mathf.Clamp(_screenSize.y, 0, _maxScreenSize.y);

            GUILayout.Label("Screenshot Image");
            Rect textrueDrawRect = CalcaulateTextureDrawRect(position.width, position.height, _screenSize.x, _screenSize.y);
            GUI.DrawTexture(textrueDrawRect, _capturedTexture != null ? _capturedTexture : Texture2D.whiteTexture, ScaleMode.ScaleToFit);

            // Draw bottom buttons
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Capture"))
                {
                    _capturedTexture = CaptureScene(_screenSize.x, _screenSize.y);
                }
                EditorGUI.BeginDisabledGroup(_capturedTexture == null);
                if (GUILayout.Button("Save"))
                {
                    SaveTextureWithPannel(_capturedTexture);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnDisable()
        {
            if(_capturedTexture != null)
            {
                DestroyImmediate(_capturedTexture);
            }
        }

        /// <summary>
        /// SceneView Camera를 통해서 Texture2D로 캡쳐
        /// </summary>
        /// <param name="width">높이</param>
        /// <param name="height">넓이</param>
        /// <returns>렌더링 된 이미지, 만약 잘못된 입력일 경우 null을 반환</returns>
        private Texture2D CaptureScene(int width, int height)
        {
            if(width <= 0 || height <= 0)
            {
                Debug.LogError("Invalid screen size");
                return null;
            }

            if(_capturedTexture != null)
            {
                DestroyImmediate(_capturedTexture);
            }

            // SceneView Camera를 통해서 renderTexture에 렌더링
            var camera = SceneView.lastActiveSceneView.camera;
            var renderTexture = new RenderTexture(width, height, 24);
            camera.targetTexture = renderTexture;
            camera.Render();

            // Render된 RenderTexutre를 활성화 상태로 설정
            RenderTexture.active = renderTexture;

            // 활성화된 RenderTexure를 Texture2D로 변환
            var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            // clean up
            RenderTexture.active = null;
            camera.targetTexture = null;
            DestroyImmediate(renderTexture);

            return texture;
        }
        
        /// <summary>
        /// Texture를 그릴 Rect를 계산. TargetWidth에 맞춰서 Texture의 비율에 맞게 Height를 계산.
        /// 높이에 맞추는 기능은 아직 없다.
        /// </summary>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="textureWidth"></param>
        /// <param name="textureHeight"></param>
        /// <returns></returns>
        private Rect CalcaulateTextureDrawRect(float targetWidth, float targetHeight, int textureWidth, int textureHeight)
        {
            // texture width : textureHeight == windowWidth : x
            // x = windowWidth * textureHeight / textureWidth
            var ratio = (float)textureWidth / textureHeight;
            var rect = GUILayoutUtility.GetRect(targetWidth, targetWidth / ratio);
            return rect;
        }

        /// <summary>
        /// Save file pannel을 띄워서 Texture2D를 저장
        /// </summary>
        /// <param name="texture">저장할 Texture</param>
        private void SaveTextureWithPannel(Texture2D texture)
        {
            if(texture == null)
            {
                return;
            }

            var fileName = GetFileName();
            var path = EditorUtility.SaveFilePanel("Save Screenshot", "", fileName, "png");
            if(path.Length == 0)
            {
                return;
            }

            var bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);

            ShowNotification(new GUIContent("Save Complete"));
            // SaveFilePanel을 띄우면 EditorWindow가 포커스를 잃어버리기 때문에 다시 포커스를 얻어야 한다.
            Focus();
        }

        /// <summary>
        /// 파일 이름을 현재 시간을 기준으로 생성. 예) Capture_2021-08-31-16-00-00.png
        /// </summary>
        /// <returns>파일 이름</returns>
        private string GetFileName()
        {
            string curTime = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            return $"{kDefaultFileName}_{curTime}.png";
        }
    }
}
