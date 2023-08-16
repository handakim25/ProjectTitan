using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Resource;

namespace Titan.Effects
{
    /// <summary>
    /// Effect Prefab과 경로와 타입등의 속성 데이터를 표현한다.
    /// 사전 로딩, 인스턴스 기능을 제공.
    /// 추후 풀링과 연동하는 것을 고려할 것.
    /// </summary>
    [System.Serializable]
    public class EffectClip
    {
        public int index = 0;
        public EffectType effectType = EffectType.Normal;
        [System.NonSerialized] public GameObject effectPrefab = null;

        // Full Path를 사용하지 않는 이유
        // 파일 경로와 에셋 이름이 다를 수 있기 때문
        // 가령 Assetbundle 등을 이용할 경우
        public string effectName = string.Empty;
        public string effectPath = string.Empty;
        [System.NonSerialized] public string effectFullPath = string.Empty;

        // XML
        public EffectClip() {}

        /// <summary>
        /// 데이터를 미리 Load
        /// </summary>
        public bool PreLoad()
        {
            if(effectPrefab != null)
            {
                return true;
            }

            effectFullPath = effectPath + effectName;
            if(effectFullPath == string.Empty)
            {
                return false;
            }

            effectPrefab = ResourceManager.Load(effectFullPath) as GameObject;
            return effectPrefab != null; // 이 코드 괜찮은 느낌이네
        }

        public void Release()
        {
            if(effectPrefab != null)
            {
                effectPrefab = null;
            }
        }

        /// <summary>
        /// 원하는 위치에 이펙트를 인스턴스합니다.
        /// </summary>
        /// <param name="pos">World Pos</param>
        /// <returns>Effect Go, 실패할 경우 null 반환</returns>
        public GameObject Instantiate(Vector3 pos)
        {
            if(effectPrefab == null && PreLoad() == false)
            {
                Debug.LogWarning($"Can not load Effect clip resource : {effectName}");
                return null;
            }
            return GameObject.Instantiate(effectPrefab, pos, Quaternion.identity);
        }
    }

    public enum EffectType
    {
        None = -1,
        Normal,
    }
}
