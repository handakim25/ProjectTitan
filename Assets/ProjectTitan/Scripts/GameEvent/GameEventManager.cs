using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.GameEvent
{
    public class GameEventManager : MonoSingleton<GameEventManager>
    {
        // Temp solution
        // 에디터 편집 혹은 특정한 방법으로 데이터를 유지할 것
        [SerializeField] private List<GameEvent> _gameEvents = new List<GameEvent>();
        private Dictionary<string, GameEvent> _gameEventDictionary = new Dictionary<string, GameEvent>();

        private void Start()
        {
            _gameEventDictionary = _gameEvents.ToDictionary(x => x.EventName, x => x);
        }

        public void SetEventStatus(string eventName, bool status)
        {
            if(_gameEventDictionary.ContainsKey(eventName))
            {
                _gameEventDictionary[eventName].EventStatus = status;
            }
            else
            {
                Debug.LogWarning($"No event named {eventName}.");
            }
        }
    }
}
