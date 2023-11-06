using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.GameEventSystem
{
    public class RaiseEvent : MonoBehaviour
    {
        [SerializeField] private ReferenceID<GameEventObject> _gameEvent;
        [Tooltip("Event를 Target Status로 변경")]
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
