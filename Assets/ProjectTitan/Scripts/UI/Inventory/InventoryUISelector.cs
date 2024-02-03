using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.UI.InventorySystem
{
    [RequireComponent(typeof(InventoryUI))]
    public class InventoryUISelector : MonoBehaviour
    {
        // SerializeField로 노출하지 않으면
        // Scene에 저장이 되지 않는다.
        [SerializeField] InventoryUI target;
        public ItemType type;

        public void SetFilter()
        {
            target.SetFilter(type);
        }

        public void RemoveFilter()
        {
            target.RemoveFilter(type);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            target = GetComponent<InventoryUI>();
        }
#endif
    }
}
