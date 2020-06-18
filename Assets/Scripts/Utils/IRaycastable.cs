using UnityEngine;

public abstract class IRaycastable : MonoBehaviour
{
    public abstract bool hasRaycastFocus { get; set; }
}