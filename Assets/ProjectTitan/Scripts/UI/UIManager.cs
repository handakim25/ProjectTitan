using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using Titan.InventorySystem.Items;

namespace Titan.UI
{
    public sealed class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] ItemDatabase itemDatabase;
        // @Refactor
        // Change to scalable design latter
        [SerializeField] private UIBase inventoryUI;

        [SerializeField] private List<UIBase> _UIList = new List<UIBase>();

        // Memo
        // Think UI as Scene
        // Once one scene open -> other scenes should be closed
        // Each scene close themselves
        // UIManger should take care of other objects
        // First thing first.
        // Set parent ? register?
        // One UI should not know other UIs

        // UI calls -> SceneManager.ChangeScene
        // or
        // UI Open UI -> Callback UIManager

        public void OnOpenInventory()
        {
            inventoryUI.OpenUI();
        }

        public void OnCloseInventory()
        {
            inventoryUI.CloseUI();
        }

        public ItemObject GetItemObject(int id)
        {
            return itemDatabase.itemObjects[id];
        }

        // onvalidate?
    }
}
