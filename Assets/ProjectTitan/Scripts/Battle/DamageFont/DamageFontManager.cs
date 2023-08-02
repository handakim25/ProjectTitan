using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using Titan.Utility;

namespace Titan.Battle
{
    /// <summary>
    /// Damge font 생성을 책임지는 매니저
    /// - Pooling을 이용
    /// - Event bus pattern을 이용해서 작동
    /// </summary>
    public class DamageFontManager : MonoSingleton<MonoBehaviour>
    {
        private const string RootName = "DamageFontRoot";
        private const string TextName = "DamageFont";

        private Transform _root;
        [Header("Object Pool")]
        [SerializeField] private int _initPoolCount = 20;
        private int _capacity;
        private Stack<DamageFont> _objectPool;
        public int Capacity => _capacity;

        [Header("Damge Font Attribute")]
        [SerializeField] private GameObject _damageFontPrefab;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Vector3 _direction;
        [SerializeField] private Interpolate.EaseType _easeType;        

        private void Awake()
        {
            _root = new GameObject(RootName).transform;
            _root.SetParent(transform);

            // Init Pool
            ExpandPool(_initPoolCount);
        }

        public void SpawnDamageFont(Vector3 worldPos, float damage, bool isBig, Color color)
        {
            // 1. Get Damage font
            DamageFont newFont = GetDamageFont();
            // 2. Set Damage font

            // 3. Set Damage font transfrom

            newFont.gameObject.SetActive(true);
        }

        /// <summary>
        /// Get Damage font object.
        /// If no damage font object is available, expand pool.
        /// </summary>
        /// <returns></returns>
        private DamageFont GetDamageFont()
        {
            if(_objectPool.Count == 0)
            {
                ExpandPool(Capacity);
            }
            return _objectPool.Pop();
        }

        private void ExpandPool(int count)
        {
            int index = _capacity;
            _capacity += count;

            for(int i = 0; i < count; i++)
            {
                var damgeFontGo = Instantiate(_damageFontPrefab, _root);
                damgeFontGo.name = $"{TextName}_{count : 2}";
                damgeFontGo.SetActive(false);

                var damgeFont = damgeFontGo.GetComponent<DamageFont>();
                // setup

                _objectPool.Push(damgeFont);
            }
        }
    }
}
