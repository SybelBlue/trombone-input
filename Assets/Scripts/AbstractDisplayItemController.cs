using UnityEngine;
public abstract class AbstractDisplayItemController : MonoBehaviour 
{
    public abstract float Resize (float unitWidth);

    public abstract void SetHighlight (bool highlight);
}