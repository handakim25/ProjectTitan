using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> tabButtons;

        private TabButton selectedTab;
        public System.Action<TabButton> OnTabSelectedEvent;

        public void Subscribe(TabButton button)
        {
            if(tabButtons == null)
            {
                tabButtons = new List<TabButton>();
            }

            tabButtons.Add(button);
        }

        #region UnityEventSystem Methods

        public void OnTabSelected(TabButton button)
        {
            if(selectedTab != null && button != selectedTab)
            {
                selectedTab.Deselect();
            }

            selectedTab = button;

            OnTabSelectedEvent?.Invoke(button);
        }

        public void OnTabDeslected(TabButton button)
        {
            
        }
        
        #endregion UnityEventSystem Methods

        #region Methods

        #endregion Methods
    }
}
