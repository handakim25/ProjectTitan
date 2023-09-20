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
        [SerializeField] private List<GameEventObject> _gameEvents = new();
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
            // Load 과정은 GameEventData로 넘길 것
            _gameEventDictionary = _gameEvents.ToDictionary(x => x.ID, x => x.GameEvent);
        }

        /// <summary>
        /// 혹시 모를 SO 참조
        /// </summary>
        public void SetEventStatus(GameEventObject gameEvent, bool newStatus)
        {
            SetEventStatus(gameEvent.ID, newStatus);
        }

        public void SetEventStatus(string gameEventName, bool newStatus)
        {
            if (_gameEventDictionary.TryGetValue(gameEventName, out GameEvent gameEvent) == false)
            {
                return;
            }
            if (gameEvent.Status == newStatus)
            {
                return;
            }

            gameEvent.Status = newStatus;
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

        /// <summary>
        /// 혹시 모를 SO 참조
        /// </summary>
        public bool GetEventStatus(GameEventObject gameEvent, out bool status)
        {
            return GetEventStatus(gameEvent.ID, out status);
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

        /// <summary>
        /// /// 혹시 모를 SO 참조
        /// </summary>
        public void RegisterEvent(GameEventObject gameEvent, GameEventAction action)
        {
            RegisterEvent(gameEvent.ID, action);
        }

        public void RegisterEvent(string gameEventName, GameEventAction action)
        {
            if(_gameEventActionDictionary.ContainsKey(gameEventName) == false)
            {
                _gameEventActionDictionary.Add(gameEventName, new List<GameEventAction>());
            }

            _gameEventActionDictionary[gameEventName].Add(action);
        }

        /// <summary>
        /// 혹시 모를 SO 참조
        /// </summary>
        public void UnregisterEvent(GameEventObject gameEvent, GameEventAction action)
        {
           UnregisterEvent(gameEvent.ID, action);
        }

        public void UnregisterEvent(string gameEventName, GameEventAction action)
        {
            if (_gameEventActionDictionary.ContainsKey(gameEventName) == false)
            {
                return;
            }

            _gameEventActionDictionary[gameEventName].Remove(action);
        }

        // Temp Save / Load Logic
        // public void SaveGameEventStatus()
        // {

        //     var saveList = _gameEventDictionary.Values.Where(x => x.IsChanged).ToList();
        // }

        // public void LoadGameEventStatus()
        // {
        //     // Load Logic
        //     foreach(var gameEvent in _gameEventDictionary.Values)
        //     {
        //         gameEvent.Status = gameEvent.DefaultStatus;
        //     }

        // }
    }
}
