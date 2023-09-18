using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.GameEventSystem
{
    public class EventHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent;

        public UnityEvent OnEventRaised;

        public bool Status
        {
            get {
                if(GameEventManager.Instance == null)
                {
                    Debug.LogError("GameEventManager is not initialized");
                    return false;
                }
                var result = GameEventManager.Instance.GetEventStatus(_gameEvent, out bool status);
                if(result == false)
                {
                    Debug.LogError($"GameEvent {_gameEvent.EventName} is not registered");
                    return false;
                }
                return status;
            }
        }

        private void OnEnable()
        {
            // if(GameEventManager.Instance == null) 
            // {
            //     Debug.LogError("GameEventManager is not initialized");
            //     return;
            // }
            // GameEventManager.Instance.RegisterEvent(_gameEvent, OnEventRaised);
        }

        private void OnDisable()
        {
            // if(GameEventManager.Instance == null)
            // {
            //     Debug.LogError("GameEventManager is not initialized");
            //     return;
            // }
            // GameEventManager.Instance.UnregisterEvent(_gameEvent, (action) => {OnEventRaised?.Invoke();});
        }
    }
}
