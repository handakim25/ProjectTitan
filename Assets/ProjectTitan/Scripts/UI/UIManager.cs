using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Titan.Core;
using Titan.InventorySystem.Items;

namespace Titan.UI
{
    public sealed class UIManager : MonoSingleton<UIManager>
    {
        // @Refactor
        // Change to scalable design latter
        [SerializeField] private UIScene inventoryUI;

        [SerializeField] private List<UIScene> _UIList = new List<UIScene>();

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
            inventoryUI.OpenUI(1.0f);
        }

        public void OnCloseInventory()
        {
            inventoryUI.CloseUI(1.0f);
        }
    }
}
