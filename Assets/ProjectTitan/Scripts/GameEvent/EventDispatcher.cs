using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.GameEventSystem
{
    public class EventDispatcher : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent;

        public bool TargetStatus;
        public UnityEvent OnEventRaised;
        private bool _isInitialized = false;

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
            if(_isInitialized == false)
            {
                return;
            }
            if(GameEventManager.Instance == null) 
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            GameEventManager.Instance.RegisterEvent(_gameEvent, HandleEvent);
        }

        private void OnDisable()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            GameEventManager.Instance.UnregisterEvent(_gameEvent, HandleEvent);
        }

        private void Start()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            _isInitialized = true;
            GameEventManager.Instance.RegisterEvent(_gameEvent, HandleEvent);
        }

        private void HandleEvent(GameEventManager.GameEventContext context)
        {
            Debug.Log($"Get Event");
            if(context.newValue == TargetStatus)
            {
                OnEventRaised?.Invoke();
            }
        }
    }
}
