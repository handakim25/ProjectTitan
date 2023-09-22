using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Titan.GameEventSystem
{
    [System.Serializable]
    public class GameEvent
    {
        public int index;
        public string EventName;
        [System.NonSerialized][HideInInspector] public bool Status;
        public bool DefaultStatus;
        [JsonIgnore]
        public bool IsChanged => Status != DefaultStatus;
    }
}
