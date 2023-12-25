using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game Event를 Listen해서 특정 상태가 되면 Cadllback을 호출함
    /// 비활성화 상태에서는 Game Event를 Listen하지 않음
    /// </summary>
    public class EventDispatcher : MonoBehaviour
    {
        [SerializeField] private ReferenceID<GameEventObject> _gameEvent;

        /// <summary>
        /// Game Event의 상태가 TargetStatus가 되면 Callback을 호출함
        /// </summary>
        public bool TargetStatus;
        /// <summary>
        /// Game Event의 상태가 TargetStatus가 되면 호출되는 Callback
        /// </summary>
        public UnityEvent OnEventRaised;
        private bool _isInitialized = false;

        /// <summary>
        /// 현재 GameEvent의 상태를 반환. 만약 GameEvent가 등록되지 않았다면 false를 반환
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
            // GameEventManager가 초기화되어야 하기 때문에 Start에서 Register를 해야 한다.
            // 만약에 여기서 Register를 하면 같은 Scene에서 로드 시에 EventDispatcher의 OnEnable이 먼저 호출되어 버린다.
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
            // Instance가 파괴되었다면 Unregister를 할 필요가 없다.
            if(GameEventManager.Instance == null)
            {
                // Debug.LogError("GameEventManager is not initialized");
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
            if(context.newValue == TargetStatus)
            {
                OnEventRaised?.Invoke();
            }
        }
    }
}
