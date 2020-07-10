﻿using UnityEngine;
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
        private Vector3 defaultMinAngle, defaultMaxAngle;

        [SerializeField]
        private Vector2 sliderBounds;
        #endregion

        public Func<(Vector3 min, Vector3 max)?> angleProvider;

        private (bool front, bool back) highlighting;
        private (string path, float? last) saveData;
        private (int? frame, RaycastHit? hit, IRaycastable obj) lastFound;


        #region Properties
        public bool useLaser
        {
            get => laserPointer.active;
            set => laserPointer.active = value;
        }

        public (Vector3 origin, Vector3 direction) orientation
            => (transform.position, transform.forward);

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
            get => highlighting.front;
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
            get => highlighting.back;
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
            if (!recordSliderData) return;

            saveData = ($"{Application.persistentDataPath}/{Testing.Utils.UniqueYamlName("stylus-output")}", null);
        }

        private void Update()
        {
            if (!transform.hasChanged) return;

            normalizedAngles =
                eulerAngles
                .Map(x => useUnityEulerAngles ? x.ModIntoRange(-180, 180) : x)

                .Map((i, x) => Mathf.InverseLerp(LowerBound(i), UpperBound(i), x));

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

            var raycastable = IRaycastable.Raycast(orientation.origin, orientation.direction, out hit);
            lastFound = (Time.frameCount, hit, raycastable);

            return raycastable;
        }

        public void FillIndicatorDisplayIfNull()
        {
            var indicator = GetComponent<StylusIndicator>();
            if (indicator)
            {
                Static.FillWithTaggedIfNull(ref indicator.display, "AngleAndSliderIndicator");
            }
        }

        private void WriteSliderData(System.IO.StreamWriter writer)
            => writer.WriteLine($"{Time.time}: {normalizedSlider}");
    }
}