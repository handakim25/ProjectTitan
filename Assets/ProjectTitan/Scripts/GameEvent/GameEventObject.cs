using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game Event의 Scriptable Object 형식
    /// </summary>
    public class GameEventObject : ScriptableObject, IRefereceable
    {
        [JsonIgnore]
        public string ID => GameEvent?.EventName;
        public GameEvent GameEvent;
        /// <summary>
        /// 에디터용 설명
        /// </summary>
        public string EventDescription;
    }
}
