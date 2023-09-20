using System.Collections;
using System.Collections.Generic;
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

        public bool IsChanged => Status != DefaultStatus;
    }
}
