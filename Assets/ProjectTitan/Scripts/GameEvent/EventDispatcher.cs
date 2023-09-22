using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.GameEventSystem
{
    public class EventDispatcher : MonoBehaviour
    {
        [SerializeField] private ReferenceID<GameEventObject> _gameEvent;

        public bool TargetStatus;
        public UnityEvent OnEventRaised;
        private bool _isInitialized = false;

        /// <summary>
        /// 현재 GameEvent의 상태를 반환합니다.
        /// </summary>
        public bool Status
        {
            get {
                if(GameEventManager.Instance == null)
                {
                    Debug.LogError("GameEventManager is not initialized");
                    return false;
                }
                var result = GameEventManager.Instance.GetEventStatus(_gameEvent.ID, out bool status);
                if(result == false)
                {
                    Debug.LogError($"GameEvent {_gameEvent.ID} is not registered");
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
            GameEventManager.Instance.RegisterEvent(_gameEvent.ID, HandleEvent);
        }

        private void OnDisable()
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
            GameEventManager.Instance.UnregisterEvent(_gameEvent.ID, HandleEvent);
        }

        private void Start()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            _isInitialized = true;
            GameEventManager.Instance.RegisterEvent(_gameEvent.ID, HandleEvent);
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
