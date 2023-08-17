using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Character
{
    public class MinimapMarker : MonoBehaviour
    {
        // Setting 관련 정보 필요
        // 1. 기초적인 높이
        // https://qkrguscjf176.tistory.com/22 참고

        private readonly string MarkerName = "Marker";
        private const float DefaultHeight = 10f;

        [SerializeField] private Sprite _maker;
        [SerializeField] private Color _color = new(1f, 1f, 1f);

        private GameObject _markerGo;

        private void Start()
        {
            _markerGo = new GameObject(MarkerName);
            _markerGo.transform.SetParent(gameObject.transform);
            _markerGo.transform.SetLocalPositionAndRotation(new(0f, DefaultHeight,0f), Quaternion.Euler(90f, 0f, 0f));
            _markerGo.layer = (int)TagAndLayer.LayerIndex.Minimap;

            var renderer = _markerGo.AddComponent<SpriteRenderer>();
            renderer.sprite = _maker;
            renderer.color = _color;
        }

        public bool MarkerOn
        {
            get
            {
                return _markerGo.activeSelf;
            }
            set
            {
                _markerGo.SetActive(value);
            }
        }
    }
}
