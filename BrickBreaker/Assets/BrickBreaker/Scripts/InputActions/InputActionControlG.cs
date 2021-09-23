// GENERATED AUTOMATICALLY FROM 'Assets/BrickBreaker/Scripts/Other/InputActions/InputActionControl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActionControlG : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActionControlG()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActionControl"",
    ""maps"": [
        {
            ""name"": ""ActionMap"",
            ""id"": ""8b9f393e-41d1-4a94-9b0f-d22e98679ce0"",
            ""actions"": [
                {
                    ""name"": ""LeftMove"",
                    ""type"": ""Button"",
                    ""id"": ""ce71cbd4-a441-47b0-a487-46c82c450f02"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""RightMove"",
                    ""type"": ""Button"",
                    ""id"": ""ae920e73-50de-4803-8d76-2b9ef3b2c02b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""ReleaseBall"",
                    ""type"": ""Button"",
                    ""id"": ""b81ad047-be8d-4123-b44b-b55f8ac9d59e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""c096ccc5-2226-4756-afb4-5857ee17281e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Menu_GoBack"",
                    ""type"": ""Button"",
                    ""id"": ""9a26ebc0-844f-4aad-ac73-f3753b63adf7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""435385f6-97b4-42b0-904c-5cd19cdc8045"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""LeftMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2544bfc-eafc-4f50-a3c7-850c7eee941f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""RightMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f9fd7eba-001b-4d09-8830-36ecf6f5bc43"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""Menu_GoBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ce34291-d29b-4825-9c12-dcd176b5bb62"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""ReleaseBall"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c53ea54-52ef-4e74-8214-2571a97525dc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""pc"",
            ""bindingGroup"": ""pc"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // ActionMap
        m_ActionMap = asset.FindActionMap("ActionMap", throwIfNotFound: true);
        m_ActionMap_LeftMove = m_ActionMap.FindAction("LeftMove", throwIfNotFound: true);
        m_ActionMap_RightMove = m_ActionMap.FindAction("RightMove", throwIfNotFound: true);
        m_ActionMap_ReleaseBall = m_ActionMap.FindAction("ReleaseBall", throwIfNotFound: true);
        m_ActionMap_Pause = m_ActionMap.FindAction("Pause", throwIfNotFound: true);
        m_ActionMap_Menu_GoBack = m_ActionMap.FindAction("Menu_GoBack", throwIfNotFound: true);
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

    // ActionMap
    private readonly InputActionMap m_ActionMap;
    private IActionMapActions m_ActionMapActionsCallbackInterface;
    private readonly InputAction m_ActionMap_LeftMove;
    private readonly InputAction m_ActionMap_RightMove;
    private readonly InputAction m_ActionMap_ReleaseBall;
    private readonly InputAction m_ActionMap_Pause;
    private readonly InputAction m_ActionMap_Menu_GoBack;
    public struct ActionMapActions
    {
        private @InputActionControlG m_Wrapper;
        public ActionMapActions(@InputActionControlG wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMove => m_Wrapper.m_ActionMap_LeftMove;
        public InputAction @RightMove => m_Wrapper.m_ActionMap_RightMove;
        public InputAction @ReleaseBall => m_Wrapper.m_ActionMap_ReleaseBall;
        public InputAction @Pause => m_Wrapper.m_ActionMap_Pause;
        public InputAction @Menu_GoBack => m_Wrapper.m_ActionMap_Menu_GoBack;
        public InputActionMap Get() { return m_Wrapper.m_ActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IActionMapActions instance)
        {
            if (m_Wrapper.m_ActionMapActionsCallbackInterface != null)
            {
                @LeftMove.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnLeftMove;
                @LeftMove.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnLeftMove;
                @LeftMove.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnLeftMove;
                @RightMove.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnRightMove;
                @RightMove.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnRightMove;
                @RightMove.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnRightMove;
                @ReleaseBall.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnReleaseBall;
                @ReleaseBall.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnReleaseBall;
                @ReleaseBall.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnReleaseBall;
                @Pause.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnPause;
                @Menu_GoBack.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenu_GoBack;
                @Menu_GoBack.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenu_GoBack;
                @Menu_GoBack.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenu_GoBack;
            }
            m_Wrapper.m_ActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftMove.started += instance.OnLeftMove;
                @LeftMove.performed += instance.OnLeftMove;
                @LeftMove.canceled += instance.OnLeftMove;
                @RightMove.started += instance.OnRightMove;
                @RightMove.performed += instance.OnRightMove;
                @RightMove.canceled += instance.OnRightMove;
                @ReleaseBall.started += instance.OnReleaseBall;
                @ReleaseBall.performed += instance.OnReleaseBall;
                @ReleaseBall.canceled += instance.OnReleaseBall;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Menu_GoBack.started += instance.OnMenu_GoBack;
                @Menu_GoBack.performed += instance.OnMenu_GoBack;
                @Menu_GoBack.canceled += instance.OnMenu_GoBack;
            }
        }
    }
    public ActionMapActions @ActionMap => new ActionMapActions(this);
    private int m_pcSchemeIndex = -1;
    public InputControlScheme pcScheme
    {
        get
        {
            if (m_pcSchemeIndex == -1) m_pcSchemeIndex = asset.FindControlSchemeIndex("pc");
            return asset.controlSchemes[m_pcSchemeIndex];
        }
    }
    public interface IActionMapActions
    {
        void OnLeftMove(InputAction.CallbackContext context);
        void OnRightMove(InputAction.CallbackContext context);
        void OnReleaseBall(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnMenu_GoBack(InputAction.CallbackContext context);
    }
}