using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.Effects
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        private const string EffectRootName = "EffectRoot";
        private Transform effectRoot = null;

        private void Start()
        {
            if(effectRoot == null)
            {
                effectRoot = new GameObject(EffectRootName).transform;
                effectRoot.SetParent(transform);
            }
        }

        public GameObject EffectOneShot(int index, Vector3 position)
        {
            EffectClip clip = DataManager.EffectData.GetClip(index);
            var effectInstance = clip.Instantiate(position);
            effectInstance.SetActive(true);
            return effectInstance;
        }
    }
}
