using MinVR;
using VREventList = System.Collections.Generic.List<MinVR.VREvent>;
using UnityEngine;

namespace CustomInput
{
    // In Unity Editor:
    //    From _MAIN.scene
    //      Left Click on recommendations, dropdown via screen to canvas coordinates (Unity Eventsystem)
    //    From Bindings.cs
    //      Right Click + Mouse Wheel   =>      Stylus Slider
    //      Backquote                   =>      Stylus Front Button
    //      Tab                         =>      Stylus Back Button
    //      7/8/9/0                     =>      Fast Switch Layouts
    //      Shift + Stylus Emulation    =>      Hold for Precise Emulation
    //      Return                      =>      Switch to/from Lobby
    //      S                           =>      Skip Challenge
    //      R                           =>      Restart Challenge
    //      Shift + S                   =>      Skip Trial
    //      Shift + R                   =>      Restart Trial
    //      K                           =>      Capture Screenshot
    //    From MainController.cs
    //      Backspace                   =>      Force Backspace
    //      Space                       =>      Force Space
    //    From FakeTrackingInput.cs
    //      Mouse Delta                 =>      Stylus XY Position Delta
    //      Left Control + Mouse Delta  =>      Stylus XZ Rotation Delta
    //      X/Y/Z + Mouse Delta         =>      Stylus Strictly X/Y/Z Rotation Delta
    //      Up/Down/Left/Right          =>      Main Camera Delta
    //
    // In XR Build:
    //    From StylusModelController.Raycast
    //      Right Trigger to select recommendations, dropdown via raycast from stylus tip
    //    From _MAIN.scene
    //      Head Delta                  =>      Main Camera Delta
    //      Hand Pos/Rot Delta          =>      Stylus Pos/Rot Delta
    //    From Bindings.cs
    //     - <DOM> is dominant hand, set in Bindings.DOMINANT_HAND
    //     - Primary is Trackpad for HP WMR
    //     - Secondary is Joystick for HP WMR
    //      <DOM> Primary Y Position    =>      Stylus Slider
    //      <DOM> Secondary Button      =>      Stylus Front Button
    //      <DOM> Grip Button           =>      Stylus Back Button
    //      <DOM> Joystick Quadrant     =>      Fast Switch Layouts
    //      Return                      =>      Switch to/from Lobby
    //      7/8/9/0                     =>      Fast Switch Layouts
    //      S                           =>      Skip Challenge
    //      R                           =>      Restart Challenge
    //      Shift + S                   =>      Skip Trial
    //      Shift + R                   =>      Restart Trial
    //      K                           =>      Capture Screenshot
    using System;
    using static UnityEngine.Input;
    using static UnityEngine.KeyCode;
    using static VREventFactory;
    using static VREventFactory.Names;
    public static class Bindings
    {
        public static readonly uint _slider_max_value = 45;//64;
        public static readonly uint _slider_min_value = 15;
        public static readonly KeyCode[] _layout_switch_bindings
            = new KeyCode[] { Alpha7, Alpha8, Alpha9, Alpha0 };

        public static readonly KeyCode _scene_advance_key = Return;

        public static bool _left_handed = false;

        public static string _dominant_hand
            => _left_handed ? "Left" : "Right"; // or "Left"
        public static string _trigger
            => $"XRI_{_dominant_hand}_TriggerButton";
        public static string _grip
            => $"XRI_{_dominant_hand}_GripButton";

        // Trackpad/Joystick labels based on HP WMR 1440^2
        public static string _trackpad_vertical
            => $"XRI_{_dominant_hand}_Primary2DAxis_Vertical";

        public static string _joystick_vertical
            => $"XRI_{_dominant_hand}_Secondary2DAxis_Vertical";
        public static string _joystick_horizontal
            => $"XRI_{_dominant_hand}_Secondary2DAxis_Horizontal";

        public static bool doEmulate
            => Application.isEditor;

        public static bool inputThisFrame
            => touchCount > 0
            || GetMouseButton(0)
            || emulatingSlide
            || precisionMode
            || emulatingFront
            || emulatingBack;

        // right mouse down
        public static bool beginEmulatedSlide
            => GetMouseButtonDown(1);

        // right mouse held and slide delta exists
        public static bool emulatingSlide
            => GetMouseButton(1) || GetAxis(_trackpad_vertical) != 0;

        // right mouse up
        public static bool endEmulatedSlide
            => GetMouseButtonUp(1);

        public static float emulatedSlideDelta
            => mouseScrollDelta.y * 2;

        public static int emulatedSlideValue
            => Mathf.FloorToInt((1 + GetAxis(_trackpad_vertical)) * _slider_max_value / 2);

        // either shift key held
        public static bool precisionMode
            => shift;

        public static bool shift
            => GetKey(LeftShift) || GetKey(RightShift);

        public static bool emulatingFrontDown
            => GetKeyDown(BackQuote) || GetButtonDown(_trigger);

        public static bool emulatingFront
            => GetKey(BackQuote) || GetButton(_trigger);

        public static bool emulatingFrontUp
            => GetKeyUp(BackQuote) || GetButtonUp(_trigger);

        public static bool emulatingBackDown
            => GetKeyDown(Tab) || GetButtonDown(_grip);

        public static bool emulatingBack
            => GetKey(Tab) || GetButton(_grip);

        public static bool emulatingBackUp
            => GetKeyUp(Tab) || GetButtonUp(_grip);

        public static bool spaceDown
            => GetKeyDown(Space);

        public static bool backspaceDown
            => GetKeyDown(Backspace);

        public static bool advanceSceneDown
            => GetKeyDown(_scene_advance_key);

        public static bool skipChallenge
            => GetKeyDown(S) && !skipTrial;

        public static bool restartChallenge
            => GetKeyDown(R) && !restartTrial;

        public static bool skipTrial
            => shift && GetKeyDown(S);

        public static bool restartTrial
            => shift && GetKeyDown(R);

        public static bool takeScreenshot
            => GetKeyDown(K);

        public static int? emulatingLayoutSwitch
        {
            get
            {
                for (int i = 0; i < _layout_switch_bindings.Length; i++)
                {
                    if (GetKeyDown(_layout_switch_bindings[i]))
                    {
                        return i;
                    }
                }

                return joystickQuadrant - 1;
            }
        }

        public static int? joystickQuadrant
        {
            get
            {
                float vert = GetAxis(_joystick_vertical);
                float horz = GetAxis(_joystick_horizontal);
                if (vert > 0.3f)
                {
                    if (horz > 0.3f)
                    {
                        return 1;
                    }
                    if (horz < -0.3f)
                    {
                        return 2;
                    }
                    return null;
                }

                if (vert < -0.3f)
                {
                    if (horz > 0.3f)
                    {
                        return 4;
                    }
                    if (horz < -0.3f)
                    {
                        return 3;
                    }
                }

                return null;
            }
        }

        public static void InitializeMinVRLayoutSwitching(VRDevice server)
        {
            if (server == null)
            {
                throw new ArgumentNullException("server", "Provided VRDevice was null! Failed to initialize for MinVR Layout Switching!");
            }

            if (server.vrNodeType != VRDevice.VRNodeType.NetServer)
            {
                Debug.LogWarning("Provided VRDevice to initialize for MinVR Layout Switching does not have vrNodeType NetServer!");
            }

            foreach (var item in _layout_switch_bindings)
            {
                server.unityKeysToVREvents.Add(item);
            }

            //TODO: hacking this in here since we have the server object to also initialize the other button events
            server.unityKeysToVREvents.Add(Return);
            server.unityKeysToVREvents.Add(S);
            server.unityKeysToVREvents.Add(R);
            server.unityKeysToVREvents.Add(D);
            server.unityKeysToVREvents.Add(T);
            server.unityKeysToVREvents.Add(LeftShift);
            server.unityKeysToVREvents.Add(RightShift);
        }

        public static void AddMinVRLayoutSwitchingHandlers(Func<int, VRMain.OnVRButtonDownEventDelegate> LayoutHandlers)
        {
            for (int i = 0; i < _layout_switch_bindings.Length; i++)
            {
                var bindingName = KeyCodeToMinVRButtonDownName(_layout_switch_bindings[i]);
                VRMain.Instance.AddOnVRButtonDownCallback(bindingName, LayoutHandlers(i));
            }
        }

        public static void AddMinVRSandRKeyHandlers(VRMain.OnVRButtonDownEventDelegate onSDown, VRMain.OnVRButtonDownEventDelegate onRDown, VRMain.OnVRButtonDownEventDelegate onDDown, VRMain.OnVRButtonDownEventDelegate onTDown, VRMain.OnVRButtonDownEventDelegate onShiftDown, VRMain.OnVRButtonUpEventDelegate onShiftUp)
        {
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(LeftShift), onShiftDown);
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(RightShift), onShiftDown);
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(S), onSDown);
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(R), onRDown);
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(D), onDDown);
            VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(T), onTDown);

            VRMain.Instance.AddOnVRButtonUpCallback(KeyCodeToMinVRButtonUpName(LeftShift), onShiftUp);
            VRMain.Instance.AddOnVRButtonUpCallback(KeyCodeToMinVRButtonUpName(RightShift), onShiftUp);
        }

        public static string KeyCodeToMinVRButtonDownName(KeyCode binding)
            => $"Kbd{binding}_Down";

        public static string KeyCodeToMinVRButtonUpName(KeyCode binding)
            => $"Kbd{binding}_Up";


        // If emulatingFront or endEmulatedSlide when the layout accepts potentiometer input,
        // then it emulates the forward button down.
        // If emulatingBack then it emulates back button
        public static void CaptureEmulatedButtonInput(ref VREventList eventList, bool layoutUsesSlider)
        {
            if (emulatingFrontDown || (endEmulatedSlide && layoutUsesSlider))
            {
                eventList.Add(MakeButtonDownEvent(_front_button));
            }

            if (emulatingFrontUp)
            {
                eventList.Add(MakeButtonUpEvent(_front_button));
            }

            if (emulatingBackDown)
            {
                eventList.Add(MakeButtonDownEvent(_back_button));
            }

            if (emulatingBackUp)
            {
                eventList.Add(MakeButtonUpEvent(_back_button));
            }

            if (advanceSceneDown)
            {
                eventList.Add(MakeButtonDownEvent(_scene_advance_key.ToString()));
            }
        }

        public static void AddSceneAdvanceCallback(VRMain.OnVRButtonDownEventDelegate OnSceneAdvance)
        // => VRMain.Instance.AddOnVRButtonDownCallback(_scene_advance_key.ToString(), OnSceneAdvance);
            => VRMain.Instance.AddOnVRButtonDownCallback(KeyCodeToMinVRButtonDownName(_scene_advance_key), OnSceneAdvance);


        // Starts, updates, and ends emulated slider input when appropriate
        // Also accounts for precision mode
        public static void CaptureEmulatedSliderInput(ref VREventList eventList, uint slideStartValue, uint? currentValue)
        {
            if (beginEmulatedSlide)
            {
                eventList.Add(MakePotentiometerEvent(slideStartValue));
                return;
            }

            float delta = emulatedSlideDelta;
            if (!precisionMode)
            {
                delta *= 4;
            }

            int rawNext = Mathf.RoundToInt((currentValue ?? 0) + delta);
            int next = (int)Mathf.Clamp(rawNext, 0, _slider_max_value);

            if (!Application.isEditor && delta == 0)
            {
                next = emulatedSlideValue;
            }

            if (emulatingSlide)
            {
                eventList.Add(MakePotentiometerEvent(next));
            }
        }
    }

    public static class VREventFactory
    {
        public static class Names
        {
            public const string _potentiometer = "BlueStylusAnalog";
            public const string _front_button = "BlueStylusFrontBtn";
            public const string _back_button = "BlueStylusBackBtn";
            public const string _return = "Return";
        }

        public static class Types
        {
            public const string _button_down = "ButtonDown";
            public const string _button_up = "ButtonUp";
            public const string _analog_update = "AnalogUpdate";
        }

        public static VREvent MakeButtonDownEvent(string name)
            => MakeEvent(name, Types._button_down);

        public static VREvent MakeButtonUpEvent(string name)
            => MakeEvent(name, Types._button_up);

        public static VREvent MakePotentiometerEvent(float analogValue)
            => MakeEvent(_potentiometer, Types._analog_update, analogValue);

        public static VREvent MakeEvent(string name, string type, float? analogValue = null)
        {
            VREvent e = new VREvent(name);
            e.AddData("EventType", type);

            if (analogValue.HasValue)
            {
                e.AddData("AnalogValue", analogValue.Value);
            }

            return e;
        }
    }
}
