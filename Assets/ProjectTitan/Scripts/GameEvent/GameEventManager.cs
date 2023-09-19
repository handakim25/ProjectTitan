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
        // Key : GameEvent name, value : Game Event
        private Dictionary<string, GameEvent> _gameEventDictionary = new();
        // Key : GameEvent Name, value : Game Event Listener
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
            SetEventStatus(gameEvent.EventName, newStatus);
        }

        public void SetEventStatus(string gameEventName, bool newStatus)
        {
            if (_gameEventDictionary.TryGetValue(gameEventName, out GameEvent value) == false)
            {
                return;
            }
            if (value.Status == newStatus)
            {
                return;
            }

            value.Status = newStatus;
            InvokeEventActions(value, newStatus);
        }

        private void InvokeEventActions(GameEvent gameEvent, bool newStatus)
        {
            InvokeEventActions(gameEvent.EventName, newStatus);
        }

        private void InvokeEventActions(string gameEventName, bool newStatus)
        {
            if (!_gameEventActionDictionary.TryGetValue(gameEventName, out List<GameEventAction> actions))
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
            return GetEventStatus(gameEvent.EventName, out status);
        }

        public bool GetEventStatus(string gameEventName, out bool status)
        {
            if (_gameEventDictionary.TryGetValue(gameEventName, out GameEvent value) == false)
            {
                status = false;
                return false;
            }

            status = value.Status;
            return true;
        }

        public void RegisterEvent(GameEvent gameEvent, GameEventAction action)
        {
            RegisterEvent(gameEvent.EventName, action);
        }

        public void RegisterEvent(string gameEventName, GameEventAction action)
        {
            if(_gameEventActionDictionary.ContainsKey(gameEventName) == false)
            {
                _gameEventActionDictionary.Add(gameEventName, new List<GameEventAction>());
            }

            _gameEventActionDictionary[gameEventName].Add(action);
        }

        public void UnregisterEvent(GameEvent gameEvent, GameEventAction action)
        {
           UnregisterEvent(gameEvent.EventName, action);
        }

        public void UnregisterEvent(string gameEventName, GameEventAction action)
        {
            if (_gameEventActionDictionary.ContainsKey(gameEventName) == false)
            {
                return;
            }

            _gameEventActionDictionary[gameEventName].Remove(action);
        }
    }
}
