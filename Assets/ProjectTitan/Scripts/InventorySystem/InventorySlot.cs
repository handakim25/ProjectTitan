using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.InventorySystem.Items;

namespace Titan.InventorySystem
{
    /// <summary>
    /// Item과 수량 정보를 가지고 있다.
    /// 실질적인 UI는 SlotUI에 의해 표시된다.
    /// UI Update는 OnPreUpdate, OnPostUpdate, OnEmpty로 이루어진다.
    /// MVC 패턴에서 Model에 해당한다.
    /// </summary>
    [Serializable]
    public class InventorySlot
    {
        #region Variables

        // UI
        // Load 되었을 경우 연결되는 과정 필요
        [NonSerialized] public GameObject SlotUI;

        [NonSerialized] public Action<InventorySlot> OnPreUpdate;
        [NonSerialized] public Action<InventorySlot> OnPostUpdate;
        [NonSerialized] public Action<InventorySlot> OnEmpty;

        public Item item;
        public int amount; 

        #endregion Variables

        #region Methods

        public InventorySlot() => UpdateSlot(new Item(), 0);
        public InventorySlot(Item item, int amount) => UpdateSlot(item, amount);

        /// <summary>
        /// 만약, amount가 0보다 작으면 빈 슬롯이 된다.
        /// 변경 전에 OnPreUPdate가 호출되고, 이후에 OnPostUpdate가 호출된다.
        /// </summary>
        /// <param name="item">표시할 Item</param>
        /// <param name="amount">수량</param>
        public void UpdateSlot(Item item, int amount)
        {
            if(amount < 0)
            {
                item = Item.NullItem;
            }

            OnPreUpdate?.Invoke(this);
            this.item = item;
            this.amount = amount;
            OnPostUpdate?.Invoke(this);
        }

        /// <summary>
        /// 수량을 바꾼다.
        /// </summary>
        /// <param name="value">바꿀 수량 개수, 음수도 가능하다.</param>
        public void AddAmount(int value) => UpdateSlot(item, amount += value);
        
        /// <summary>
        /// 유효한 아이템인지 확인한다.
        /// </summary>
        public bool IsValid => item.id >= 0;
        #endregion Methods
    }
}
