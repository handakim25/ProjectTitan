using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.Stage
{
    [CreateAssetMenu(fileName = "StageData", menuName = "Game Play/StageData", order = 0)]
    public class StageDataObject : ScriptableObject
    {
        public string SceneName;
        public string StageName;
        public SoundList BGM;
    }
}
