namespace SceneSwitching
{
    public interface ITransitionable
    {
        void Transition();
    }

    public static class Utils
    {
        public static readonly string _STRIALS_scene = "_STRIALS";
        public static readonly string _LOBBY_name = "_LOBBY";
    }
}