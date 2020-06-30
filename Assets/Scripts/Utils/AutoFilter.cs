using System;
using UnityEngine;
using UnityEngine.Events;
using Utils.SignalProcessing;
using EventType = Utils.SignalProcessing.EventType;

namespace CustomEvent
{
    [Serializable]
    public class FilterEvent : UnityEvent<FilterEventData>
    { }
}

namespace Utils
{
#pragma warning disable 649
    public class AutoFilter : MonoBehaviour
    {
        public CustomEvent.FilterEvent OnFilterOutput;

        [SerializeField]
        private uint epsilon, deadzone;

        [SerializeField]
        private FrequencyType type;

        [SerializeField, Tooltip("After this amount of type without a value provided, will generate a value")]
        private double period;

        private Filter filter;

        private (uint? value, double time, int frame) last;

        private double currentTime => GetCurrentTime(type);

        private void Start()
        {
            last = (null, 0, 0);
            filter = new Filter(epsilon, deadzone);
            OnFilterOutput.AddListener(DefaultHandler);

            Provide(null);
        }

        private void DefaultHandler(FilterEventData e)
        {
            Debug.Log($"{(e.autoGenerated ? "Auto" : "Provided")}: {(e.value.HasValue ? e.value.ToString() : "(null)")}");
        }

        public void Provide(uint? value)
        {
            uint? filtered = PushToFilter(value);
            var type = GetTypeFor(last.value, filtered);

            var output = type == EventType.FingerUp ? last.value : filtered;
            FilterEventData e = new FilterEventData(type, output);

            last = (output, currentTime, Time.frameCount);

            OnFilterOutput.Invoke(e);
        }

        private void Generate()
        {
            var value = last.value;
            if (Time.frameCount == last.frame)
            {
                Debug.LogWarning($"Already generated a value this frame ({last.frame})! Skipping!");
            }
            else
            {
                value = PushToFilter(last.value);
                if (value != last.value)
                {
                    OnFilterOutput.Invoke(new FilterEventData(GetTypeFor(last.value, value), value, autoGenerated: true));
                }
            }
            last = (value, currentTime, Time.frameCount);
        }

        private void Update()
        {
            if (currentTime - last.time >= period)
            {
                Generate();
            }
        }

        private uint? PushToFilter(uint? value)
        {
            var filtered = filter.Push(value ?? 0);
            return filtered <= deadzone || !value.HasValue ? null : filtered;
        }

        private static EventType GetTypeFor(uint? last, uint? current)
        {
            if (last.HasValue == current.HasValue)
            {
                return current.HasValue ? EventType.Touching : EventType.NoTouches;
            }
            return current.HasValue ? EventType.FingerDown : EventType.FingerUp;
        }


        public static double GetCurrentTime(FrequencyType type)
        {
            switch (type)
            {
                case FrequencyType.Frames:
                    return Time.frameCount;
                case FrequencyType.Seconds:
                    return Time.time;
            }
            throw new ArgumentException(type.ToString());
        }
    }
}