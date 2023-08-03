using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Titan.Utility;

namespace Titan.Battle
{
    // @Memo
    // 만약 성능상의 병목 현상이 존재할 경우
    // Update Loop를 통합시킬 것

    /// <summary>
    /// OnEnable을 통해서 작동한다.
    /// </summary>
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageFont : MonoBehaviour
    {
        public bool isPooling = false;
        public System.Action<DamageFont> OnObjectDestroy;

        [SerializeField] private float _duration;
        private float _lifeTimer;
        [SerializeField] private Vector3 _direction = Vector3.up;
        [SerializeField] private float _moveDist = 1f;
        private Vector3 _distance;
        [SerializeField] private Interpolate.EaseType _ease;
        private Vector3 _startPos;

        private TextMeshPro _text;

        private void Awake()
        {
            _direction.Normalize();
            _text = GetComponent<TextMeshPro>();    
        }

        private void OnEnable()
        {
            _lifeTimer = 0.0f;
            _startPos = transform.position;
            _distance = _moveDist * _direction;
        }

        private void Update()
        {
            _lifeTimer += Time.deltaTime;
            if(_lifeTimer > _duration)
            {
                DestroyInstance();
                return;
            }

            transform.position = Interpolate.Ease(Interpolate.GetEase(_ease), _startPos, _distance, _lifeTimer, _duration);
        }

        public void Setup(float duration, Vector3 direction, float moveDist, Interpolate.EaseType ease)
        {
            isPooling = true;
            _duration = duration;
            _direction = direction;
            _moveDist = moveDist;
            _ease = ease;
        }

        public string text {
            set {
                _text.text = value;
            }
        }

        public void DestroyInstance()
        {
            if(isPooling)
            {
                OnObjectDestroy?.Invoke(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
