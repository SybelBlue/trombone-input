using UnityEngine;

namespace CustomInput
{
    public abstract class Layout : MonoBehaviour
    {
        public abstract void ResizeAll();

        public abstract void SetHighlightedKey(int? index);

        public abstract (LayoutKey, SimpleKey)? KeysAt(int index);

        public abstract string CharsFor(int index);

        public abstract string layoutName { get; }
    }
}