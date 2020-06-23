public class PracticeEndButtonController : IRaycastable
{
    [UnityEngine.SerializeField]
    private UnityEngine.UI.Image background;
    protected override void OnRaycastFocusChange(bool value)
        => background.color = value ? UnityEngine.Color.gray : UnityEngine.Color.white;
}