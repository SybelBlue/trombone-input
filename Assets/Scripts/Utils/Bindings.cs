using MinVR;
using static UnityEngine.Input;
using static UnityEngine.KeyCode;

namespace CustomInput
{
    public static class Bindings
    {
        public const string _trigger = "XRI_Right_TriggerButton";
        public const string _grip = "XRI_Right_GripButton";
        public const string _trackpad_vertical = "XRI_Right_Primary2DAxis_Vertical";

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