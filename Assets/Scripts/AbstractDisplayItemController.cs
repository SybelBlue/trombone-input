using UnityEngine;
public abstract class AbstractDisplayItemController : MonoBehaviour
{
    public LayoutItem item;

    public abstract float Resize(float unitWidth);

    public abstract void SetHighlight(bool highlight);
}