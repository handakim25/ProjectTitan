using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    /// <summary>
    /// TabButton들의 선택을 관리한다.
    /// Select Event가 발생하면 TabButton은 자신을 선택하고 TabGroup이 이전에 선택된 TabButton을 Deselect한다.
    /// </summary>
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> tabButtons;

        /// <summary>
        /// 현재 선택된 TabButton
        /// </summary>
        private TabButton _selectedTab;
        public TabButton SelectedTab => _selectedTab;

        public System.Action<TabButton> OnTabSelectedEvent;
        public System.Action<TabButton> OnTabDeselectedEvent;
        
        public void Subscribe(TabButton button)
        {
            tabButtons ??= new List<TabButton>();

            tabButtons.Add(button);
        }

        public void Unsubscribe(TabButton button)
        {
            tabButtons?.Remove(button);
        }

        #region UnityEventSystem Methods

        public void SelectTab(TabButton button)
        {
            if(_selectedTab == button)
            {
                return;
            }
            if(_selectedTab != null)
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
    }
}
