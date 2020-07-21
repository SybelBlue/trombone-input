using UnityEngine;
using Utils;
using Utils.UnityExtensions;
using Utils.SystemExtensions;
using System;

namespace Controller
{
#pragma warning disable 649
    public class Stylus : MonoBehaviour
    {
        #region EditorSet
        public bool recordSliderData;
        public bool useUnityEulerAngles;

        [SerializeField]
        private GameObject sliderIndicator;

        [SerializeField]
        private LaserPointer laserPointer;

        [SerializeField]
        private MeshRenderer frontButtonRenderer, backButtonRenderer;

        [SerializeField]
        private Material highlightMaterial, defaultMaterial;

        [SerializeField]
        [Tooltip("Must be in range [-180, 180]")]
        private Vector3 defaultMinAngle, defaultMaxAngle;

        [SerializeField]
        private Vector2 sliderBounds;
        #endregion

        public Func<Utils.Tuples.VBounds?> angleProvider;

        public Utils.Tuples.Orientation travel;

        private Highlighting highlighting;
        private SaveData saveData;
        private Found lastFound;

        private Utils.Tuples.Orientation lastTransform;

        #region Properties
        public bool useLaser
        {
            get { return laserPointer.active; }
            set { laserPointer.active = value; }
        }

        public Utils.Tuples.Orientation orientation
            => new Utils.Tuples.Orientation(transform.position, transform.forward);

        public Vector3 normalizedAngles { get; private set; }

        public Vector3 eulerAngles
            => useUnityEulerAngles ?
                transform.eulerAngles :
                new Vector3(
                Static.SignedAngleFromAxis(transform.forward, new Vector3(0, 1, 0), 0),
                Static.SignedAngleFromAxis(transform.forward, new Vector3(0, 0, 1), 1),
                Static.SignedAngleFromAxis(transform.forward, new Vector3(0, 1, 0), 2)
                );

        public float rawSlider { get; set; }

        public float? normalizedSlider
        {
            get
            {
                if (sliderIndicator.activeInHierarchy)
                {
                    return Mathf.InverseLerp(sliderBounds.x, sliderBounds.y, -sliderIndicator.transform.localPosition.z);
                }
                return null;
            }

            set
            {
                if (!value.HasValue)
                {
                    sliderIndicator.SetActive(false);
                    return;
                }

                if (!sliderIndicator.activeInHierarchy)
                {
                    sliderIndicator.SetActive(true);
                }

                var pos = sliderIndicator.transform.localPosition;
                pos.z = -Mathf.Lerp(sliderBounds.x, sliderBounds.y, value.Value);
                sliderIndicator.transform.localPosition = pos;
            }
        }

        public bool frontButtonDown
        {
            get { return highlighting.front; }
            set
            {
                highlighting.front = value;
                frontButtonRenderer.material =
                    value ?
                        highlightMaterial :
                        defaultMaterial;
            }
        }

        public bool backButtonDown
        {
            get { return highlighting.back; }
            set
            {
                highlighting.back = value;
                backButtonRenderer.material =
                    value ?
                        highlightMaterial :
                        defaultMaterial;
            }
        }

        public Vector3 minAngle
            => angleProvider == null ?
                    defaultMinAngle :
                    angleProvider()?.min ?? defaultMinAngle;

        public Vector3 maxAngle
            => angleProvider == null ?
                    defaultMaxAngle :
                    angleProvider()?.max ?? defaultMaxAngle;
        #endregion

        private void Start()
        {
            highlighting = new Highlighting { front = false, back = false };

            if (!recordSliderData) return;

            saveData = new SaveData($"{Application.persistentDataPath}/{Testing.Utils.UniqueYamlName("stylus-output")}", null);
        }

        private void Update()
        {
            if (!transform.hasChanged) return;

            normalizedAngles =
                eulerAngles
                .Map(x => useUnityEulerAngles ? x.ModIntoRange(-180, 180) : x)

                .Map((i, x) => {
                    float low = LowerBound(i);
                    float hi = UpperBound(i);
                   
                    if (x < 0) {
                        x = (180 + x) + 180;
                    }
                    if (low < 0)
                    {
                        low = (180 + low) + 180;
                    }
                    if (hi < 0)
                    {
                        hi = (180 + hi) + 180;
                    }
                    return Mathf.Clamp01((x - low) / (hi - low));
                    }
                    );

            travel.pos += (lastTransform.pos - transform.position).Map(Mathf.Abs);
            travel.rot += (lastTransform.rot - eulerAngles).Map(Mathf.Abs);
            lastTransform = new Utils.Tuples.Orientation(transform.position, eulerAngles);

            if (!recordSliderData) return;

            if (saveData.last == normalizedSlider) return;

            Testing.Utils.UsingStream(saveData.path, WriteSliderData);

            saveData.last = normalizedSlider;
        }

        private void OnDestroy()
        {
            if (recordSliderData)
            {
                Debug.LogWarning($"Saved data to:\n{saveData.path}");
            }
            else if (saveData.path != null)
            {
                Debug.LogWarning("Deleted saved data.");
                Testing.Utils.DeleteFile(saveData.path);
            }
        }

        public float LowerBound(int axis)
            => axis == 0 && !CustomInput.Bindings._left_handed ? maxAngle[axis] : minAngle[axis];

        public float UpperBound(int axis)
            => axis == 0 && !CustomInput.Bindings._left_handed ? minAngle[axis] : maxAngle[axis];

        public IRaycastable Raycast(out RaycastHit? hit)
        {
            if (lastFound.frame == Time.frameCount)
            {
                hit = lastFound.hit;
                return lastFound.obj;
            }

            var raycastable = IRaycastable.Raycast(orientation.pos, orientation.rot, out hit);
            lastFound = new Found(Time.frameCount, hit, raycastable);

            return raycastable;
        }

        public void FillIndicatorDisplayIfNull()
        {
            var indicator = GetComponent<StylusIndicator>();
            if (indicator)
            {
                Static.FillWithTaggedIfNull(ref indicator.display, "AngleAndSliderIndicator");
                indicator.stylus = this;
            }
        }

        private void WriteSliderData(System.IO.StreamWriter writer)
            => writer.WriteLine($"{Time.time}: {normalizedSlider}");


        
        
        // private (int? frame, RaycastHit? hit, IRaycastable obj) lastFound;
        private struct Highlighting
        { public bool front, back; }

        private struct SaveData
        { 
            public string path;
            public float? last;

            public SaveData(string path, float? last)
            {
                this.path = path;
                this.last = last;
            }
        }

        private struct Found 
        {
            public int? frame;
            public RaycastHit? hit;
            public IRaycastable obj;

            public Found(int? frame, RaycastHit? hit, IRaycastable obj)
            {
                this.frame = frame;
                this.hit = hit;
                this.obj = obj;
            }
        }
    }
}