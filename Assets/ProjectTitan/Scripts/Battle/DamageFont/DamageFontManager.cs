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
    public class DamageFontManager : MonoSingleton<DamageFontManager>
    {
        private const string RootName = "DamageFontRoot";
        private const string TextName = "DamageFont";

        private Transform _root;
        [Header("Object Pool")]
        [SerializeField] private int _initPoolCount = 20;
        private int _capacity;
        private Stack<DamageFont> _objectPool = new();
        public int Capacity => _capacity;

        [Header("Damge Font Attribute")]
        [SerializeField] private GameObject _damageFontPrefab;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _moveDist;
        [SerializeField] private Interpolate.EaseType _easeType;        

        private void Start()
        {
            _root = new GameObject(RootName).transform;
            _root.SetParent(transform);

            // Init Pool
            ExpandPool(_initPoolCount);
        }

        public void SpawnDamageFont(Vector3 worldPos, float damage, bool isBig, Color color)
        {
            DamageFont newFont = GetDamageFont();
            newFont.transform.position = worldPos;
            newFont.text = damage.ToString();

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
                damgeFontGo.name = $"{TextName}_{index++ : 000}";
                damgeFontGo.SetActive(false);

                var damageFont = damgeFontGo.GetComponent<DamageFont>();
                damageFont.Setup(_duration, _direction, _moveDist, _easeType);
                damageFont.OnObjectDestroy += (DamageFont font) => {
                    _objectPool.Push(font);
                    font.gameObject.SetActive(false);
                };
                var renderer = damageFont.GetComponent<MeshRenderer>();

                _objectPool.Push(damageFont);
            }
        }
    }
}
