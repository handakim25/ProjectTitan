using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Titan.Resource
{
    /// <summary>
    /// Resources.Load를 랩핑하는 클래스.
    /// 추후에 어셋번들로 바꾸기 위함이다.
    /// </summary>
    public class ResourceManager
    {
        /// <summary>
        /// Resource 로드이지만 추후에 어셋 로드로 바꿀 것
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityObject Load(string path)
        {
            return Resources.Load(path);
        }

        public static GameObject LoadAndInstantiate(string path)
        {
            UnityObject source = Load(path);
            if(source == null)
            {
                return null;
            }
            return GameObject.Instantiate(source) as GameObject;
        }
    }
}
