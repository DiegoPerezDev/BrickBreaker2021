// GENERATED AUTOMATICALLY FROM 'Assets/BrickBreaker/Scripts/InputActions/InputActionControl.inputactions'

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
                },
                {
                    ""name"": ""TouchPosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f67a296d-5a53-4d27-bdc1-9cfbdc82a34b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""TouchPress"",
                    ""type"": ""Button"",
                    ""id"": ""d75036d5-cb73-4652-a01d-a0cb6154d62d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""e0112c7c-c3cd-4505-a1a1-851079d208f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""MenuNavigationButtons"",
                    ""type"": ""Button"",
                    ""id"": ""d683668f-517e-4fb8-bbdb-39ed081b7377"",
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
                    ""id"": ""e62d70a6-139b-4845-bf76-f8debc6fd817"",
                    ""path"": ""<Keyboard>/leftArrow"",
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
                    ""id"": ""22834a70-1899-401c-b833-dc458d140f6f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""RightMove"",
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
                    ""id"": ""f6aa07e7-a6c6-4ea3-a27e-075c2b471645"",
                    ""path"": ""<Keyboard>/enter"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""b82b4247-6002-44d8-ba65-564a0e27b88c"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c708741b-e7e6-4b7e-a8d4-6c22490dbd76"",
                    ""path"": ""<Touchscreen>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""smartphone"",
                    ""action"": ""TouchPress"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""83f0f603-0cb6-4e80-8742-a8d43f7a474c"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""smartphone"",
                    ""action"": ""TouchPosition"",
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
                    ""id"": ""f629396a-319e-435d-b1ef-3bd638376c8a"",
                    ""path"": ""*/{Back}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""smartphone"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f997d790-ade7-445a-96c2-4194c238e005"",
                    ""path"": ""*/{Menu}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""smartphone"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""03f5f346-8dce-42a4-b8cd-ae2983fcf1f8"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90ee42c0-c17e-4963-9c28-96b2833d0081"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46e8cc48-0681-4809-a3c8-870b9a751307"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""70fc20cd-1cdc-4931-83c5-e457b60e67a2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c023e74-ade7-4d96-a015-bed4e22a69af"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee7480eb-db5d-462d-99e9-959e38438b8f"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a14ebd4e-ed51-466f-9ff0-81088ba62710"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""103f3392-2ebe-4ea7-b22a-f31c3f96fa60"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""pc"",
                    ""action"": ""MenuNavigationButtons"",
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
        },
        {
            ""name"": ""smartphone"",
            ""bindingGroup"": ""smartphone"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
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
        m_ActionMap_TouchPosition = m_ActionMap.FindAction("TouchPosition", throwIfNotFound: true);
        m_ActionMap_TouchPress = m_ActionMap.FindAction("TouchPress", throwIfNotFound: true);
        m_ActionMap_Back = m_ActionMap.FindAction("Back", throwIfNotFound: true);
        m_ActionMap_MenuNavigationButtons = m_ActionMap.FindAction("MenuNavigationButtons", throwIfNotFound: true);
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
    private readonly InputAction m_ActionMap_TouchPosition;
    private readonly InputAction m_ActionMap_TouchPress;
    private readonly InputAction m_ActionMap_Back;
    private readonly InputAction m_ActionMap_MenuNavigationButtons;
    public struct ActionMapActions
    {
        private @InputActionControlG m_Wrapper;
        public ActionMapActions(@InputActionControlG wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMove => m_Wrapper.m_ActionMap_LeftMove;
        public InputAction @RightMove => m_Wrapper.m_ActionMap_RightMove;
        public InputAction @ReleaseBall => m_Wrapper.m_ActionMap_ReleaseBall;
        public InputAction @Pause => m_Wrapper.m_ActionMap_Pause;
        public InputAction @Menu_GoBack => m_Wrapper.m_ActionMap_Menu_GoBack;
        public InputAction @TouchPosition => m_Wrapper.m_ActionMap_TouchPosition;
        public InputAction @TouchPress => m_Wrapper.m_ActionMap_TouchPress;
        public InputAction @Back => m_Wrapper.m_ActionMap_Back;
        public InputAction @MenuNavigationButtons => m_Wrapper.m_ActionMap_MenuNavigationButtons;
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
                @TouchPosition.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPosition;
                @TouchPosition.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPosition;
                @TouchPosition.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPosition;
                @TouchPress.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPress;
                @TouchPress.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPress;
                @TouchPress.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnTouchPress;
                @Back.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnBack;
                @MenuNavigationButtons.started -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenuNavigationButtons;
                @MenuNavigationButtons.performed -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenuNavigationButtons;
                @MenuNavigationButtons.canceled -= m_Wrapper.m_ActionMapActionsCallbackInterface.OnMenuNavigationButtons;
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
                @TouchPosition.started += instance.OnTouchPosition;
                @TouchPosition.performed += instance.OnTouchPosition;
                @TouchPosition.canceled += instance.OnTouchPosition;
                @TouchPress.started += instance.OnTouchPress;
                @TouchPress.performed += instance.OnTouchPress;
                @TouchPress.canceled += instance.OnTouchPress;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
                @MenuNavigationButtons.started += instance.OnMenuNavigationButtons;
                @MenuNavigationButtons.performed += instance.OnMenuNavigationButtons;
                @MenuNavigationButtons.canceled += instance.OnMenuNavigationButtons;
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
    private int m_smartphoneSchemeIndex = -1;
    public InputControlScheme smartphoneScheme
    {
        get
        {
            if (m_smartphoneSchemeIndex == -1) m_smartphoneSchemeIndex = asset.FindControlSchemeIndex("smartphone");
            return asset.controlSchemes[m_smartphoneSchemeIndex];
        }
    }
    public interface IActionMapActions
    {
        void OnLeftMove(InputAction.CallbackContext context);
        void OnRightMove(InputAction.CallbackContext context);
        void OnReleaseBall(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnMenu_GoBack(InputAction.CallbackContext context);
        void OnTouchPosition(InputAction.CallbackContext context);
        void OnTouchPress(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
        void OnMenuNavigationButtons(InputAction.CallbackContext context);
    }
}
