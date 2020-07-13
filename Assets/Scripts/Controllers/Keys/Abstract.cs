using CustomInput.KeyData;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RectTransform;


namespace Controller.Key
{
    // The interface for all KeyControllers
    public abstract class IKey : Utils.IRaycastable
    {
        // set can cause cast exception!
        public abstract AbstractData layoutKey { get; set; }


        // defines the behavior of this.gameObject when it recieves a new
        // sensor width in Unity-units. Note that the potentiometer is
        // 64 sensors long, so sensorWidth generally equals the width of
        // the parent container divided by 64.
        public abstract void SetHighlight(bool highlight);

        // defines the behavior of this.gameObject when highlight is toggled
        public abstract float Resize(float sensorWidth);

        public abstract float ResizeHeight(float sensorHeight);

        protected override void OnRaycastFocusChange(bool value)
        { }
    }

    // The base component for all GameObjects that are meant to represent
    // a key in a Layout's GUI.
    //
    // See Layouts/LayoutKets.cs for usage
    public abstract class Key<T> : IKey
        where T : AbstractData
    {
        // this.gameObject's RectTransform (filled in Inspector)
        [SerializeField]
        protected RectTransform rectTransform;

        public override AbstractData layoutKey
        {
            get => data;
            set => data = (T)value;
        }

        // the internal data this.gameObject is meant to represent
        public T data;

        public override float Resize(float sensorWidth)
        {
            var width = sensorWidth * data.size;
            rectTransform.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
            return width;
        }
        public override float ResizeHeight(float sensorHeight)
        {
            var height = sensorHeight * data.size;
            rectTransform.SetSizeWithCurrentAnchors(Axis.Vertical, height);
            return height;
        }
    }

#pragma warning disable 649
    public abstract class AbstractSimple : Key<SimpleData>
    {
        [SerializeField]
        private Color highlightColor;

        private Color normalColor;
        private bool highlighting = false;

        [SerializeField]
        private Image background;

        public abstract string text { get; set; }

        [SerializeField]
        private char _symbol;

        public char symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                text = MakeLabel(value);
            }
        }

        private string MakeLabel(char c)
        {
            switch (c)
            {
                case ((char)0):
                    return "sym";
                case ' ':
                    return "spc";
                case '\b':
                    return "bksp";
            }

            return new string(new char[] { c });
        }

        public override void SetHighlight(bool h)
        {
            if (h)
            {
                normalColor = background.color;

                background.color = highlightColor;
            }
            else if (highlighting)
            {
                background.color = normalColor;
            }

            highlighting = h;
        }

        public bool useAlternate
        {
            set => symbol = data.CharWithAlternate(value);
        }
    }
}