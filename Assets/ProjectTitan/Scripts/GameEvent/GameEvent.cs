using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.GameEvent
{
    public class GameEvent : ScriptableObject
    {
        public string EventName;
        /// <summary>
        /// 절대 값을 직접 수정하지 말 것
        /// </summary>
        public bool EventStatus = false;
#if UNITY_EDITOR
        [SerializeField] private string EventDescription;
#endif
    }
}
