//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/ProjectTitan/Scripts/InputSystem/MainAction.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Titan.Core
{
    public partial class @MainAction: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @MainAction(InputActionAsset _asset)
        {
            asset = _asset;
            // Player
            m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
            m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
            m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
            m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
            m_Player_Dash = m_Player.FindAction("Dash", throwIfNotFound: true);
            m_Player_Skill = m_Player.FindAction("Skill", throwIfNotFound: true);
            m_Player_Hyper = m_Player.FindAction("Hyper", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Inventory = m_UI.FindAction("Inventory", throwIfNotFound: true);
            m_UI_Interact = m_UI.FindAction("Interact", throwIfNotFound: true);
            m_UI_InteractScroll = m_UI.FindAction("InteractScroll", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Player
        private readonly InputActionMap m_Player;
        private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
        private readonly InputAction m_Player_Move;
        private readonly InputAction m_Player_Look;
        private readonly InputAction m_Player_Jump;
        private readonly InputAction m_Player_Dash;
        private readonly InputAction m_Player_Skill;
        private readonly InputAction m_Player_Hyper;
        public struct PlayerActions
        {
            private @MainAction m_Wrapper;
            public PlayerActions(@MainAction wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Player_Move;
            public InputAction @Look => m_Wrapper.m_Player_Look;
            public InputAction @Jump => m_Wrapper.m_Player_Jump;
            public InputAction @Dash => m_Wrapper.m_Player_Dash;
            public InputAction @Skill => m_Wrapper.m_Player_Skill;
            public InputAction @Hyper => m_Wrapper.m_Player_Hyper;
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void AddCallbacks(IPlayerActions instance)
            {
                if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Skill.started += instance.OnSkill;
                @Skill.performed += instance.OnSkill;
                @Skill.canceled += instance.OnSkill;
                @Hyper.started += instance.OnHyper;
                @Hyper.performed += instance.OnHyper;
                @Hyper.canceled += instance.OnHyper;
            }

            private void UnregisterCallbacks(IPlayerActions instance)
            {
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Look.started -= instance.OnLook;
                @Look.performed -= instance.OnLook;
                @Look.canceled -= instance.OnLook;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
                @Dash.started -= instance.OnDash;
                @Dash.performed -= instance.OnDash;
                @Dash.canceled -= instance.OnDash;
                @Skill.started -= instance.OnSkill;
                @Skill.performed -= instance.OnSkill;
                @Skill.canceled -= instance.OnSkill;
                @Hyper.started -= instance.OnHyper;
                @Hyper.performed -= instance.OnHyper;
                @Hyper.canceled -= instance.OnHyper;
            }

            public void RemoveCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IPlayerActions instance)
            {
                foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public PlayerActions @Player => new PlayerActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
        private readonly InputAction m_UI_Inventory;
        private readonly InputAction m_UI_Interact;
        private readonly InputAction m_UI_InteractScroll;
        public struct UIActions
        {
            private @MainAction m_Wrapper;
            public UIActions(@MainAction wrapper) { m_Wrapper = wrapper; }
            public InputAction @Inventory => m_Wrapper.m_UI_Inventory;
            public InputAction @Interact => m_Wrapper.m_UI_Interact;
            public InputAction @InteractScroll => m_Wrapper.m_UI_InteractScroll;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void AddCallbacks(IUIActions instance)
            {
                if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @InteractScroll.started += instance.OnInteractScroll;
                @InteractScroll.performed += instance.OnInteractScroll;
                @InteractScroll.canceled += instance.OnInteractScroll;
            }

            private void UnregisterCallbacks(IUIActions instance)
            {
                @Inventory.started -= instance.OnInventory;
                @Inventory.performed -= instance.OnInventory;
                @Inventory.canceled -= instance.OnInventory;
                @Interact.started -= instance.OnInteract;
                @Interact.performed -= instance.OnInteract;
                @Interact.canceled -= instance.OnInteract;
                @InteractScroll.started -= instance.OnInteractScroll;
                @InteractScroll.performed -= instance.OnInteractScroll;
                @InteractScroll.canceled -= instance.OnInteractScroll;
            }

            public void RemoveCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IUIActions instance)
            {
                foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public UIActions @UI => new UIActions(this);
        private int m_PCSchemeIndex = -1;
        public InputControlScheme PCScheme
        {
            get
            {
                if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
                return asset.controlSchemes[m_PCSchemeIndex];
            }
        }
        private int m_MobileSchemeIndex = -1;
        public InputControlScheme MobileScheme
        {
            get
            {
                if (m_MobileSchemeIndex == -1) m_MobileSchemeIndex = asset.FindControlSchemeIndex("Mobile");
                return asset.controlSchemes[m_MobileSchemeIndex];
            }
        }
        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
            void OnSkill(InputAction.CallbackContext context);
            void OnHyper(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnInventory(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnInteractScroll(InputAction.CallbackContext context);
        }
    }
}
