using UnityEngine;

public abstract class IRaycastable : MonoBehaviour
{
    public static IRaycastable last;

    public static IRaycastable current => last.hasRaycastFocus ? last : null;

    private bool _inFocus;

    public bool hasRaycastFocus
    {
        get => _inFocus;
        set
        {
            if (_inFocus != value)
            {
                OnRaycastFocusChange(value);
            }

            if (value)
            {
                if (last && last != this && last._inFocus)
                {
                    last.hasRaycastFocus = false;
                }

                last = this;
            }

            _inFocus = value;
        }
    }

    protected abstract void OnRaycastFocusChange(bool value);
}