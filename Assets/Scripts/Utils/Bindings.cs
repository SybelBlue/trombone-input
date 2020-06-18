using MinVR;
using static UnityEngine.Input;
using static UnityEngine.KeyCode;

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
    //      Shift                       =>      Hold for Precise Emulation
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
    public static class Bindings
    {
        public const string DOMINANT_HAND = "Right"; // or "Left"
        public static string _trigger = $"XRI_{DOMINANT_HAND}_TriggerButton";
        public static string _grip = $"XRI_{DOMINANT_HAND}_GripButton";

        // Trackpad/Joystick labels based on HP WMR 1440^2
        public static string _trackpad_vertical = $"XRI_{DOMINANT_HAND}_Primary2DAxis_Vertical";

        public static string _joystick_vertical = $"XRI_{DOMINANT_HAND}_Secondary2DAxis_Vertical";
        public static string _joystick_horizontal = $"XRI_{DOMINANT_HAND}_Secondary2DAxis_Horizontal";

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
            => UnityEngine.Mathf.FloorToInt((1 + GetAxis(_trackpad_vertical)) * 64 / 2);

        // either shift key held
        public static bool precisionMode
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

        public static int? emulatingLayoutSwitch
        {
            get
            {
                if (GetKeyDown(Alpha7))
                {
                    return 0;
                }
                if (GetKeyDown(Alpha8))
                {
                    return 1;
                }
                if (GetKeyDown(Alpha9))
                {
                    return 2;
                }
                if (GetKeyDown(Alpha0))
                {
                    return 3;
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
                if (vert > 0.5f)
                {
                    if (horz > 0.5f)
                    {
                        return 1;
                    }
                    if (horz < -0.5f)
                    {
                        return 2;
                    }
                    return null;
                }

                if (vert < -0.5f)
                {
                    if (horz > 0.5f)
                    {
                        return 4;
                    }
                    if (horz < -0.5f)
                    {
                        return 3;
                    }
                }

                return null;
            }
        }
    }

    public static class VREventFactory
    {
        public const string _potentiometer_event_name = "BlueStylusAnalog";
        public const string _front_button_event_name = "BlueStylusFrontBtn";
        public const string _back_button_event_name = "BlueStylusBackBtn";
        public const string _button_down_event_type = "ButtonDown";
        public const string _button_up_event_type = "ButtonUp";

        public static VREvent MakeButtonDownEvent(string name)
            => MakeEvent(name, _button_down_event_type);

        public static VREvent MakeButtonUpEvent(string name)
            => MakeEvent(name, _button_up_event_type);

        public static VREvent MakePotentiometerEvent(float analogValue)
            => MakeEvent(_potentiometer_event_name, "AnalogUpdate", analogValue);

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