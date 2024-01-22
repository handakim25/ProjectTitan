using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Titan
{
    /// <summary>
    /// Button Callback에서 사용할 Text Color 변경 컴포넌트
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextColorChanger : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private Color _originColor;
        [SerializeField] private Color _targetColor;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
            _originColor = text.color;
        }

        public void SetColor(bool isChange)
        {
            text.color = isChange ? _targetColor : _originColor;
        }
    }
}
