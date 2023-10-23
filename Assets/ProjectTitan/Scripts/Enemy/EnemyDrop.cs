using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;
using Titan.Interaction;

namespace Titan
{
    public class EnemyDrop : MonoBehaviour
    {
        [SerializeField] private ItemObject _itemObject;
        [SerializeField] private int _dropCount = 1;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Vector3 _offset = new(0, 0.5f, 0);

        public void OnDrop()
        {
            if(_itemObject == null || _itemPrefab == null)
            {
                return;
            }

            var itemGo = Instantiate(_itemPrefab, transform.position + _offset, Quaternion.identity);
            if(itemGo.TryGetComponent<ItemInteractable>(out var itemInteractable))
            {
                itemInteractable.Init(_itemObject, _dropCount);
            }
        }
    }
}
