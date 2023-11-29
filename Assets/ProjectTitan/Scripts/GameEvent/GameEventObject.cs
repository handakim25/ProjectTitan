using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game Event의 Scriptable Object 형식. 데이터는 최종적으로 GameEvent 형식으로 JSON으로 저장된다.
    /// 현재 역할은 Editor에서 Game Event를 Reference하기 위함이다.
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
