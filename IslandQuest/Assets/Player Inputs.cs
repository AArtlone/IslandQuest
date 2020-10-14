// GENERATED AUTOMATICALLY FROM 'Assets/Player Inputs.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputs : IInputActionCollection
{
    private InputActionAsset asset;
    public PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Inputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""52ba7820-4e17-46b5-b470-c7a5078a42de"",
            ""actions"": [
                {
                    ""name"": ""ButtonSouth"",
                    ""type"": ""Button"",
                    ""id"": ""a8489651-dd1f-437f-9adc-40712deb6cfb"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ButtonWest"",
                    ""type"": ""Button"",
                    ""id"": ""2cf4c213-a21b-432e-864b-6e5ef7459e1a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ButtonEast"",
                    ""type"": ""Button"",
                    ""id"": ""37f52215-a707-441f-bed2-3c879986e97e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""9be5b7b6-0a32-4402-b972-1ef52bd6691f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightShoulder"",
                    ""type"": ""Button"",
                    ""id"": ""0334661c-923b-4726-9ca4-c4ae77ebcc97"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightTrigger"",
                    ""type"": ""Button"",
                    ""id"": ""06aed2db-aa30-4327-88fc-d7f3bfa87897"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftStick"",
                    ""type"": ""Value"",
                    ""id"": ""b5ee230c-047d-455d-8395-a2c445f0ca3e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightStick"",
                    ""type"": ""Value"",
                    ""id"": ""fa8a026f-8a77-442f-9629-5b47a253b0ec"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DPad"",
                    ""type"": ""Button"",
                    ""id"": ""c2cacd94-e60b-4e95-93db-55d26e745038"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ButtonNorth"",
                    ""type"": ""Button"",
                    ""id"": ""d6641da2-7a25-4f05-8975-38fea0ab153f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7e58d621-1013-455c-a2f5-c5d5cc4f4b90"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(max=0.19)"",
                    ""groups"": ""Controller"",
                    ""action"": ""LeftStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1efacfb1-6319-4a6d-8ea6-6ef31342d74b"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(max=0.19)"",
                    ""groups"": ""Controller"",
                    ""action"": ""RightStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7fc4d81d-0047-4ae1-b86c-00e289238e93"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""DPad"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e39aaac1-cbc4-480d-afa5-81de64527d5d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""ButtonSouth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82c58038-ebbb-4eed-aea4-fd48f58f0958"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""ButtonWest"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b0362cfb-a934-41aa-9e1c-90184b0b01ec"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""ButtonEast"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb694c76-33f4-4192-bf19-8b7d481f18db"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""LeftTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd5e4fc5-de3d-412c-8ca0-3f3def3df0cd"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""RightShoulder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abf80266-91d8-4eea-ac50-0065683ae456"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""RightTrigger"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bfd368c3-f923-4952-90cb-d88552194f44"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""ButtonNorth"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_ButtonSouth = m_Player.GetAction("ButtonSouth");
        m_Player_ButtonWest = m_Player.GetAction("ButtonWest");
        m_Player_ButtonEast = m_Player.GetAction("ButtonEast");
        m_Player_LeftTrigger = m_Player.GetAction("LeftTrigger");
        m_Player_RightShoulder = m_Player.GetAction("RightShoulder");
        m_Player_RightTrigger = m_Player.GetAction("RightTrigger");
        m_Player_LeftStick = m_Player.GetAction("LeftStick");
        m_Player_RightStick = m_Player.GetAction("RightStick");
        m_Player_DPad = m_Player.GetAction("DPad");
        m_Player_ButtonNorth = m_Player.GetAction("ButtonNorth");
    }

    ~PlayerInputs()
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_ButtonSouth;
    private readonly InputAction m_Player_ButtonWest;
    private readonly InputAction m_Player_ButtonEast;
    private readonly InputAction m_Player_LeftTrigger;
    private readonly InputAction m_Player_RightShoulder;
    private readonly InputAction m_Player_RightTrigger;
    private readonly InputAction m_Player_LeftStick;
    private readonly InputAction m_Player_RightStick;
    private readonly InputAction m_Player_DPad;
    private readonly InputAction m_Player_ButtonNorth;
    public struct PlayerActions
    {
        private PlayerInputs m_Wrapper;
        public PlayerActions(PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @ButtonSouth => m_Wrapper.m_Player_ButtonSouth;
        public InputAction @ButtonWest => m_Wrapper.m_Player_ButtonWest;
        public InputAction @ButtonEast => m_Wrapper.m_Player_ButtonEast;
        public InputAction @LeftTrigger => m_Wrapper.m_Player_LeftTrigger;
        public InputAction @RightShoulder => m_Wrapper.m_Player_RightShoulder;
        public InputAction @RightTrigger => m_Wrapper.m_Player_RightTrigger;
        public InputAction @LeftStick => m_Wrapper.m_Player_LeftStick;
        public InputAction @RightStick => m_Wrapper.m_Player_RightStick;
        public InputAction @DPad => m_Wrapper.m_Player_DPad;
        public InputAction @ButtonNorth => m_Wrapper.m_Player_ButtonNorth;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                ButtonSouth.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonSouth;
                ButtonSouth.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonSouth;
                ButtonSouth.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonSouth;
                ButtonWest.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonWest;
                ButtonWest.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonWest;
                ButtonWest.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonWest;
                ButtonEast.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonEast;
                ButtonEast.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonEast;
                ButtonEast.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonEast;
                LeftTrigger.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftTrigger;
                LeftTrigger.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftTrigger;
                LeftTrigger.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftTrigger;
                RightShoulder.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightShoulder;
                RightShoulder.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightShoulder;
                RightShoulder.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightShoulder;
                RightTrigger.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightTrigger;
                RightTrigger.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightTrigger;
                RightTrigger.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightTrigger;
                LeftStick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftStick;
                LeftStick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftStick;
                LeftStick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLeftStick;
                RightStick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                RightStick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                RightStick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRightStick;
                DPad.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                DPad.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                DPad.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDPad;
                ButtonNorth.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonNorth;
                ButtonNorth.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonNorth;
                ButtonNorth.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonNorth;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                ButtonSouth.started += instance.OnButtonSouth;
                ButtonSouth.performed += instance.OnButtonSouth;
                ButtonSouth.canceled += instance.OnButtonSouth;
                ButtonWest.started += instance.OnButtonWest;
                ButtonWest.performed += instance.OnButtonWest;
                ButtonWest.canceled += instance.OnButtonWest;
                ButtonEast.started += instance.OnButtonEast;
                ButtonEast.performed += instance.OnButtonEast;
                ButtonEast.canceled += instance.OnButtonEast;
                LeftTrigger.started += instance.OnLeftTrigger;
                LeftTrigger.performed += instance.OnLeftTrigger;
                LeftTrigger.canceled += instance.OnLeftTrigger;
                RightShoulder.started += instance.OnRightShoulder;
                RightShoulder.performed += instance.OnRightShoulder;
                RightShoulder.canceled += instance.OnRightShoulder;
                RightTrigger.started += instance.OnRightTrigger;
                RightTrigger.performed += instance.OnRightTrigger;
                RightTrigger.canceled += instance.OnRightTrigger;
                LeftStick.started += instance.OnLeftStick;
                LeftStick.performed += instance.OnLeftStick;
                LeftStick.canceled += instance.OnLeftStick;
                RightStick.started += instance.OnRightStick;
                RightStick.performed += instance.OnRightStick;
                RightStick.canceled += instance.OnRightStick;
                DPad.started += instance.OnDPad;
                DPad.performed += instance.OnDPad;
                DPad.canceled += instance.OnDPad;
                ButtonNorth.started += instance.OnButtonNorth;
                ButtonNorth.performed += instance.OnButtonNorth;
                ButtonNorth.canceled += instance.OnButtonNorth;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.GetControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.GetControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnButtonSouth(InputAction.CallbackContext context);
        void OnButtonWest(InputAction.CallbackContext context);
        void OnButtonEast(InputAction.CallbackContext context);
        void OnLeftTrigger(InputAction.CallbackContext context);
        void OnRightShoulder(InputAction.CallbackContext context);
        void OnRightTrigger(InputAction.CallbackContext context);
        void OnLeftStick(InputAction.CallbackContext context);
        void OnRightStick(InputAction.CallbackContext context);
        void OnDPad(InputAction.CallbackContext context);
        void OnButtonNorth(InputAction.CallbackContext context);
    }
}
