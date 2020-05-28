using UnityEngine;
public abstract class AbstractDisplayItemController : MonoBehaviour
{
    public LayoutKey item;

    public abstract float Resize(float unitWidth);

    public abstract void SetHighlight(bool highlight);
}