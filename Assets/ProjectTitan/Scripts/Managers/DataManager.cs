using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Effects;

namespace Titan
{
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
