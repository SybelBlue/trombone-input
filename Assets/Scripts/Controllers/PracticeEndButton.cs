namespace Controller
{
#pragma warning disable 649
    public class PracticeEndButton : IRaycastable
    {
        [UnityEngine.SerializeField]
        private UnityEngine.UI.Image background;
        protected override void OnRaycastFocusChange(bool value)
            => background.color = value ? UnityEngine.Color.gray : UnityEngine.Color.white;
    }
}