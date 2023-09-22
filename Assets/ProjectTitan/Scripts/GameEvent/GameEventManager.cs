using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.GameEventSystem
{
    public class GameEventManager : MonoSingleton<GameEventManager>
    {
        // Key : GameEvent Name, value : Game Event Listener
        private Dictionary<string, List<GameEventAction>> _gameEventActionDictionary = new();

        public delegate void GameEventAction(GameEventContext context);
        public struct GameEventContext
        {
            public bool prevValue;
            public bool newValue;
        }

        public void SetEventStatus(string gameEventName, bool newStatus)
        {
            if (DataManager.GameEventData.TryGetEvent(gameEventName, out GameEvent gameEvent) == false)
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

        public bool GetEventStatus(string gameEventName, out bool status)
        {
            if (DataManager.GameEventData.TryGetEvent(gameEventName, out GameEvent value) == false)
            {
                status = false;
                return false;
            }

            status = value.Status;
            return true;
        }

        public void RegisterEvent(string gameEventName, GameEventAction action)
        {
            if(_gameEventActionDictionary.ContainsKey(gameEventName) == false)
            {
                _gameEventActionDictionary.Add(gameEventName, new List<GameEventAction>());
            }

            _gameEventActionDictionary[gameEventName].Add(action);
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
