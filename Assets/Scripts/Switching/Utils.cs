namespace SceneSwitching
{
    public interface ITransitionable
    {
        void Transition();
    }

    public static class Scenes
    {
        public static readonly string _STRIALS = "_STRIALS";
        public static readonly string _LOBBY_name = "_LOBBY";
    }
}