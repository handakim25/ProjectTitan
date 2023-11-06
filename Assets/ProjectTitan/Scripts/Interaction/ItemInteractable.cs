using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.Interaction
{
    // Caution : Destroy를 위해서 Root Gameobject에 존재
    public class ItemInteractable : Interactable
    {
        [SerializeField] private ItemObject _itemObject;
        [SerializeField] private int _dropCount;

        public override string InteractText =>
             _itemObject != null ? _itemObject.ItemName : name;

        public void Init(ItemObject itemObject, int dropCount)
        {
            _itemObject = itemObject;
            _dropCount = dropCount;
        }

        public override void Interact()
        {
            EventBus.RaiseEvent(new ItemCollectedEvent
            {
                ItemID = _itemObject.data.id,
                Count = _dropCount,
            });

            Destroy(gameObject);
        }
    }
}
