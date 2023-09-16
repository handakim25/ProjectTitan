using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.GameEventSystem
{
    public class GameEventManager : MonoSingleton<GameEventManager>
    {
        // Temp solution
        // 지금은 Save를 고려하지 않을 것
        [SerializeField] private List<GameEvent> _gameEvents = new();
        private Dictionary<string, GameEvent> _gameEventDictionary = new();
        private Dictionary<string, List<GameEventAction>> _gameEventActionDictionary = new();

        public delegate void GameEventAction(GameEventContext context);
        public struct GameEventContext
        {
            public bool prevValue;
            public bool newValue;
        }

        private void Start()
        {
            // Load Event Settings
            _gameEventDictionary = _gameEvents.ToDictionary(x => x.EventName, x => x);
        }

        public void SetEventStatus(GameEvent gameEvent, bool newStatus)
        {
            if (_gameEventDictionary.TryGetValue(gameEvent.EventName, out GameEvent value) == false)
            {
                return;
            }   
            if (value.Status == newStatus)
            {
                return; 
            }         

            value.Status = newStatus;
            InvokeEventActions(gameEvent, newStatus);
        }

        private void InvokeEventActions(GameEvent gameEvent, bool newStatus)
        {
            if (!_gameEventActionDictionary.TryGetValue(gameEvent.EventName, out List<GameEventAction> actions))
            {
                return;
            }

            foreach (var action in actions)
            {
                action.Invoke(new GameEventContext
                {
                    prevValue = !newStatus,
                    newValue = newStatus
                });
            }
        }

        public bool GetEventStatus(GameEvent gameEvent, out bool status)
        {
            if (_gameEventDictionary.TryGetValue(gameEvent.EventName, out GameEvent value) == false)
            {
                status = false;
                return false;
            }

            status = value.Status;
            return true;
        }

        public void RegisterEvent(GameEvent gameEvent, GameEventAction action)
        {
            if(_gameEventActionDictionary.ContainsKey(gameEvent.EventName) == false)
            {
                _gameEventActionDictionary.Add(gameEvent.EventName, new List<GameEventAction>());
            }

            _gameEventActionDictionary[gameEvent.EventName].Add(action);
        }

        public void UnregisterEvent(GameEvent gameEvent, GameEventAction action)
        {
            if (_gameEventActionDictionary.ContainsKey(gameEvent.EventName) == false)
            {
                return;
            }

            _gameEventActionDictionary[gameEvent.EventName].Remove(action);
        }
    }
}
