using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game의 사건을 기록하는 이벤트
    /// </summary>
    [System.Serializable]
    public class GameEvent
    {
        public int index;
        public string EventName;
        /// <summary>
        /// Status는 외부 파일로 저장되고 불러들어야 한다.
        /// </summary>
        [System.NonSerialized][HideInInspector] public bool Status;
        /// <summary>
        /// Status의 초기값
        /// </summary>
        public bool DefaultStatus;
        [JsonIgnore]
        public bool IsChanged => Status != DefaultStatus;
    }
}
