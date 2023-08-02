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
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageFont : MonoBehaviour
    {
        public bool isPooling = true;
        public System.Action OnObjectDestroy;

        [SerializeField] private float _duration;
        private float _lifeTimer;
        [SerializeField] private Vector3 _direction = Vector3.up;
        [SerializeField] private float _distance;
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
        }

        private void Update()
        {
            _lifeTimer += Time.deltaTime;
            if(_lifeTimer > _duration)
            {
                DestroyInstance();
                return;
            }
        }

        private void Setup(float duration, float speed, Vector3 dir)
        {
            _duration = duration;
        }

        public void DestroyInstance()
        {
            if(isPooling)
            {
                OnObjectDestroy?.Invoke();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
