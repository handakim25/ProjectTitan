using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.GameEventSystem
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "ScriptableObjects/GameEvent")]
    [System.Serializable]
    public class GameEvent : ScriptableObject, IRefereceable
    {
        public string ID => EventName;
        public string EventName;
        public bool Status;
#if UNITY_EDITOR
        [SerializeField] private string EventDescription;
#endif
    }
}
