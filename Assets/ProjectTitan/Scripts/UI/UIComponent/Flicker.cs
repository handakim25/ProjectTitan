using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Titan.UI
{
    /// <summary>
    /// UI Component에 깜빡임 효과를 주는 컴포넌트
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class Flicker : MonoBehaviour
    {
        [Tooltip("깜빡이는 시간, 한 번 0~1이 되는 간격이다.")]
        [SerializeField] private float _flickerDuration = 1f;
        [SerializeField] private bool _updateRealTime = true;

        private void OnEnable()
        {
            StartCoroutine(FlickerCoroutine());
        }

        private IEnumerator FlickerCoroutine()
        {
            var canvasRenderer = GetComponent<CanvasRenderer>();
            float elapsedTime = 0f;
            while (true)
            {
                elapsedTime += _updateRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = elapsedTime / _flickerDuration;
                float alpha = Mathf.PingPong(t, 1f);
                canvasRenderer.SetAlpha(alpha);
                yield return null;
            }
        }
    }
}
