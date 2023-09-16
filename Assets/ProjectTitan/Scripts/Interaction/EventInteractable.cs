using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.GameEventSystem;

namespace Titan.Interaction
{
    public class EventInteractable : MonoBehaviour
    {
        [SerializeField] private GameEvent _gameEvent;

        private void Start()
        {
            if(GameEventManager.Instance == null)
            {
                Debug.LogError("GameEventManager is not initialized");
                return;
            }
            // GameEventManager.Instance.RegisterEvent(_gameEvent);
        }
    }
}
