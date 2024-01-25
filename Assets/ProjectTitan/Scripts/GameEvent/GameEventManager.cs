using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game의 사건을 기록하는 이벤트. 데이터는 <see cref="GameEventData"/>에 저장되어 있다.
    /// </summary>
    sealed public class GameEventManager : MonoSingleton<GameEventManager>
    {
        // Key : GameEvent Name, value : Game Event Listener
        private Dictionary<string, List<GameEventAction>> _gameEventActionDictionary = new();

        public delegate void GameEventAction(GameEventContext context);
        public struct GameEventContext
        {
            public bool prevValue;
            public bool newValue;
        }

        /// <summary>
        /// GameEvent의 상태를 변경. 변경이 되면 등록된 Callback을 호출한다. 하지만, 상태가 변경되지 않으면 Callback을 호출하지 않는다.
        /// </summary>
        /// <param name="gameEventName">변경할 Game Event ID</param>
        /// <param name="newStatus">변경할 Event 상태</param>
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
                // Status가 변경되었을 때만 호출이 되므로 항상 PrevValue는 !newStatus가 된다.
                action.Invoke(new GameEventContext
                {
                    prevValue = !newStatus,
                    newValue = newStatus
                });
            }
        }

        /// <summary>
        /// Game Event의 상태를 반환한다.
        /// </summary>
        /// <param name="gameEventName">확인할 Game Event</param>
        /// <param name="status">Game Event 상태, 만약에 존재하지 않는 Event라면 false</param>
        /// <returns>만약 존재할 경우 True, 아닐 경우 False</returns>
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

        /// <summary>
        /// GameEvent 상태가 변경되었을 경우 호출되는 Callback을 등록한다.
        /// </summary>
        /// <param name="gameEventName">등록할려는 Game Evnet ID</param>
        /// <param name="action">상태가 변경됬을 때 호출되는 Callback </param>
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
