namespace Controller
{
#pragma warning disable 649
    public class RaycastableButton : Utils.IRaycastable
    {
        [UnityEngine.SerializeField]
        private UnityEngine.UI.Image background;

        private UnityEngine.Color prev;

        private bool highlighted = false;

        protected override void OnRaycastFocusChange(bool value)
        {
            if (value && !highlighted)
            {
                highlighted = true;
                prev = background.color;
                background.color = UnityEngine.Color.gray;
            }
            else if (!value && highlighted)
            {
                highlighted = false;
                background.color = prev;
            }
        }
    }
}