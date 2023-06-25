using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Effects;

namespace Titan
{
    /// <summary>
    /// Data들을 가지고 있는 오브젝트
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        private static EffectData effectData = null;

        private void Start()
        {
            if(effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }
        }

        public static EffectData EffectData
        {
            get
            {
                if(effectData == null)
                {
                    effectData = ScriptableObject.CreateInstance<EffectData>();
                    effectData.LoadData();
                }
                return effectData;
            }
        }
    }
}
