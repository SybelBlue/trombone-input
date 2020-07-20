using MinVR;
using static MinVR.VRMain;

namespace Utils.MinVRExtensions
{
    public static class MinVRExtensions
    {
        //
        // Summary:
        //     Adds button-up/-down callbacks to eventName.
        //
        // Parameters:
        //   f:
        public static void AddVRButtonCallbacks(
            this VRMain instance,
            string eventName,
            OnVRButtonUpEventDelegate onButtonUp,
            OnVRButtonDownEventDelegate onButtonDown)
        {
            instance.AddOnVRButtonUpCallback(eventName, onButtonUp);
            instance.AddOnVRButtonDownCallback(eventName, onButtonDown);
        }
    }
}