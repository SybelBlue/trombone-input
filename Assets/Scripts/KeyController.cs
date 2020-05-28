using UnityEngine;
public abstract class KeyController : MonoBehaviour
{
    public CustomInput.LayoutKey item;

    public abstract float Resize(float unitWidth);

    public abstract void SetHighlight(bool highlight);
}