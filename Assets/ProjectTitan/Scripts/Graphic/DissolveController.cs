using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.Graphics
{
    // Reference : https://www.youtube.com/watch?v=we406Hc_WrM
    // 개선할 점
    // 1. Dissolve 효과의 시인성이 부족하다. 단순한 발광으로는 해결하기 힘들듯 하다.
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class DissolveController : MonoBehaviour
    {
        [SerializeField] private float _dissolveTime = 1f;
        /// <summary>
        /// 갱신 속도
        /// </summary>
        [Range(0f,1f)]
        [SerializeField] private float _refreshRate = 0.025f;

        private Material[] materials;
        private float _dissolveAmount = 0f;
        private Coroutine dissolveCoroutine = null;

        /// <summary>
        /// Dissolve가 완료되었을 때 호출되는 이벤트. Dissolve가 진행이 되지 않았을 경우에는 호출되지 않는다.
        /// Dissolve를 여러번 호출했을 경우에는 마지막 호출에 대한 이벤트만 호출된다.
        /// </summary>
        public System.Action OnDissolveFinished;

        private readonly string DissolveAmountProperty = "_DissolveAmount";

        private void Awake()
        {
            materials = GetComponent<SkinnedMeshRenderer>().materials;
        }

        public void StartDissolve()
        {
            if(materials == null || materials.Length == 0 || _dissolveTime <= 0f || _refreshRate <= 0f)
            {
                Debug.LogError($"DissolveController : Invalid Parameters : material : {materials} / material length : {materials.Length} / Dissolve Time : {_dissolveTime} / Refresh Time : {_refreshRate} : ", this);
                return;
            }
            if(dissolveCoroutine != null)
            {
                StopCoroutine(dissolveCoroutine);
            }
            _dissolveAmount = 0f;

            dissolveCoroutine = StartCoroutine(StartDissolveCoroutine());
        }

        public void StartDissolve(float dissolveTime)
        {
            _dissolveTime = dissolveTime;
            StartDissolve();
        }

        IEnumerator StartDissolveCoroutine()
        {
            float step = 1f / (_dissolveTime / _refreshRate);
            var wait = new WaitForSeconds(_refreshRate);
            while(_dissolveAmount < 1f)
            {
                _dissolveAmount += step;
                for(int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat(DissolveAmountProperty, _dissolveAmount);
                }
                yield return wait;
            }
        }
    }
}
