using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.GameEventSystem;

namespace Titan.Interaction
{
    // Interact 시에 특정 Game Event를 발생시킴
    public class EventTriggerInteractable : Interactable
    {
        [SerializeField] private ReferenceID<GameEventObject> _gameEvent;
        [Tooltip("Event를 Target Status로 변경")]
        [SerializeField] private bool TargetStatus;

        public override void Interact()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            GameEventManager.Instance.SetEventStatus(_gameEvent.ID, TargetStatus);
        }
    }
}
