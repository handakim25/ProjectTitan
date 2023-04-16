using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Titan.UI
{
    public class TabGroup : MonoBehaviour
    {
        private List<TabButton> tabButtons;

        [SerializeField] protected Sprite tabIdle;
        [SerializeField] protected Sprite tabHover;
        [SerializeField] protected Sprite tabActive;

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

        public void OnTabEnter(TabButton button)
        {
            // ResetTabs();
            // if(selectedTab == null || button != selectedTab)
            // {
            //     button.background.sprite = tabHover;
            // }
        }

        public void OnTabExit(TabButton button)
        {
            // ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if(selectedTab != null)
            {
                selectedTab.Deselect();
            }

            selectedTab = button;
            // selectedTab.Select();

            // ResetTabs();
            // button.background.sprite = tabActive;

            OnTabSelectedEvent?.Invoke(button);
        }
        
        #endregion UnityEventSystem Methods

        #region Methods
        
        public void ResetTabs()
        {
            // foreach(TabButton button in tabButtons)
            // {
            //     if(selectedTab != null && button == selectedTab) {continue;}
            //     button.background.sprite = tabIdle;
            // }
        }

        #endregion Methods
    }
}
