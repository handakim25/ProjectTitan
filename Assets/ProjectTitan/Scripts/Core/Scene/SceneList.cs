using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Core.Scene
{
    /// <summary>
    /// Scene Data of Runtime
    /// Created from SceneCollectionObject
    /// </summary>
    [CreateAssetMenu(fileName = "SceneList", menuName = "Scene/SceneList")]
    public class SceneList : ScriptableObject
    {
        public string[] scenes;
    }
}
