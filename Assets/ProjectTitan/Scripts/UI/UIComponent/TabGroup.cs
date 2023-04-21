using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    // @Note
    // Expand to general cases
    // For instance multiple tab
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> tabButtons;

        private TabButton _selectedTab;
        public System.Action<TabButton> OnTabSelectedEvent;
        public System.Action<TabButton> OnTabDeselectedEvent;

        #region Property
        
        public TabButton SelectedTab => _selectedTab;
        
        #endregion Property

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
            if(_selectedTab != null && button != _selectedTab)
            {
                _selectedTab.Deselect(); // button.Deselect -> OnTabDeselected -> OnTabDeselectedEvent
            }

            _selectedTab = button;

            OnTabSelectedEvent?.Invoke(button);
        }

        public void OnTabDeslected(TabButton button)
        {
            OnTabDeselectedEvent?.Invoke(button);
        }
        
        #endregion UnityEventSystem Methods

        #region Methods

        #endregion Methods
    }
}
