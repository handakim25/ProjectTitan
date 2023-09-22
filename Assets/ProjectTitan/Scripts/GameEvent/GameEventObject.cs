using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Titan.GameEventSystem
{
    public class GameEventObject : ScriptableObject, IRefereceable
    {
        [JsonIgnore]
        public string ID => GameEvent?.EventName;
        public GameEvent GameEvent;
        public string EventDescription;
    }
}
