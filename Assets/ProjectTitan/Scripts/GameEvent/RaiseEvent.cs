using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.GameEventSystem
{
    /// <summary>
    /// Game Event를 발생시키는 Component
    /// </summary>
    public class RaiseEvent : MonoBehaviour
    {
        [Tooltip("발생시킬 Game Event")]
        [SerializeField] private ReferenceID<GameEventObject> _gameEvent;
        [Tooltip("Event를 Target Status로 변경, 같은 상태로 변경하면 Event가 발생하지 않음")]
        [SerializeField] private bool TargetStatus;
        
        public void Raise()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            if(_gameEvent == null)
            {
                Debug.LogError("GameEvent is not set");
                return;
            }
            GameEventManager.Instance.SetEventStatus(_gameEvent.ID, TargetStatus);
        }
    }
}
