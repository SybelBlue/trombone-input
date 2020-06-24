using System.Collections.Generic;
using CustomInput;
using MinVR;
using UnityEngine;
using UnityEngine.UI;
using static CustomInput.VREventFactory;

[System.Serializable]
public enum TrialExecutionMode
{
    Always,
    OnlyInEditor,
    Never,
}

#pragma warning disable 649
public class MainController : MonoBehaviour, VREventGenerator
{
    #region EditorSet
    [SerializeField]
    private bool leftHanded;

    [SerializeField]
    private TrialExecutionMode trialExecutionMode;

    [SerializeField]
    // The LayoutManager that is in charge of loading the layout
    private LayoutController layoutManager;

    [SerializeField]
    private StylusModelController stylusModel;

    // The simulated potentiometer input source
    [SerializeField]
    private InputFieldController inputPanel;

    // The transform of the layout display
    [SerializeField]
    private RectTransform displayRect;

    // The transform of the indicator
    [SerializeField]
    private RectTransform indicatorRect;

    // The place where typed letters go
    [SerializeField]
    private TextOutputController outputController;

    [SerializeField]
    private TextAsset[] trialAssets;

    [SerializeField]
    private List<Testing.Trial> trials;
    #endregion

    // The most up-to-date value reported by the InputFieldController
    private int? lastReportedValue;

    private int currentTrial = -1;

    // The manager's current layout, or null if no manager exists
    private Layout layout
        => layoutManager?.currentLayout;

    private bool usingIndicator
    {
        get => indicatorRect.gameObject.activeInHierarchy;
        set => indicatorRect.gameObject.SetActive(value);
    }

    public bool runTrial
        => trialExecutionMode == TrialExecutionMode.Always
        || (trialExecutionMode == TrialExecutionMode.OnlyInEditor && Application.isEditor);

    private InputData currentInputData => new InputData(lastReportedValue, stylusModel);

    private void Start()
    {
        Bindings.LEFT_HANDED = leftHanded;

        VRMain.Instance.AddEventGenerator(this);

        VRMain.Instance.AddOnVRAnalogUpdateCallback(_potentiometer_event_name, AnalogUpdate);

        VRMain.Instance.AddOnVRButtonDownCallback(_front_button_event_name, OnFrontButtonDown);
        VRMain.Instance.AddOnVRButtonUpCallback(_front_button_event_name, OnFrontButtonUp);

        VRMain.Instance.AddOnVRButtonDownCallback(_back_button_event_name, OnBackButtonDown);
        VRMain.Instance.AddOnVRButtonUpCallback(_back_button_event_name, OnBackButtonUp);

        outputController.ResetText();

        trials = new List<Testing.Trial>(trialAssets.Length);
        foreach (TextAsset trial in trialAssets)
        {
            var items = Testing.Utils.ReadTrialItems(trial, false);
            trials.Add(items);
            Debug.Log($"Loaded {items.Length} trial items");
        }

        RunNextTrial();
    }

    private void Update()
    {
        bool indicator = layout.usesSlider && Bindings.inputThisFrame;
        if (indicator != usingIndicator)
        {
            usingIndicator = indicator;
        }


        if (stylusModel.useLaser != layout.usesRaycasting)
        {
            stylusModel.useLaser = layout.usesRaycasting;
        }

        // TODO: Map to stylus events
        if (outputController.text.Length > 0 && Bindings.spaceDown)
        {
            outputController.TypedChar(' ');
        }

        if (Bindings.backspaceDown)
        {
            outputController.TypedBackspace();
        }

        if (Bindings.emulatingLayoutSwitch.HasValue)
        {
            layoutManager.DropdownValueSelected(Bindings.emulatingLayoutSwitch.Value);
        }

        if (stylusModel.transform.hasChanged)
        {
            RaycastHit? hit;
            var raycastable = stylusModel.Raycast(out hit);
            if (raycastable)
            {
                raycastable.hasRaycastFocus = true;
            }
            else if (IRaycastable.last)
            {
                IRaycastable.last.hasRaycastFocus = false;
            }
        }

        layout.UpdateState(currentInputData);
    }

    public void AddEventsSinceLastFrame(ref List<VREvent> eventList)
    {
        int minValue = 0;
        int gestureStartValue = lastReportedValue ?? inputPanel.maxValue / 2;
        Bindings.CaptureEmulatedSliderInput(ref eventList, gestureStartValue, lastReportedValue, minValue, inputPanel.maxValue);
        Bindings.CaptureEmulatedButtonInput(ref eventList, layout.usesSlider);
    }

    private void RunNextTrial()
    {
        currentTrial++;
        if (currentTrial < trials.Count && outputController is TestingController && runTrial)
        {
            (outputController as TestingController).RunTrial(trials[currentTrial]);
        }
        else
        {
            Debug.LogWarning("Skipped running trial!");
        }
    }

    #region Callbacks
    // Callback for when the InputFieldController value changes due to user input
    public void OnInputValueChange(int value)
    {
        lastReportedValue = value;
        float width = displayRect.rect.width;
        var pos = indicatorRect.localPosition;

        float normalized = value / (float)inputPanel.maxValue;
        pos.x = width * (normalized - 0.5f);

        indicatorRect.localPosition = pos;

        stylusModel.normalizedSlider = normalized;
    }

    private bool OnInputEnd(int? value)
    {
        lastReportedValue = value;
        (LayoutKey parentKey, SimpleKey simpleKey) = layout.KeysFor(currentInputData) ?? (null, null);

        bool success = parentKey != null;

        if (success)
        {
            (char typed, bool certain) = layout.GetSelectedLetter(currentInputData) ?? ('-', false);

            if (typed == '\b' && certain)
            {
                Debug.Log("Pressed Backspace");

                outputController.TypedBackspace();
            }
            else
            {
                Debug.Log($"Pressed {parentKey} @ {simpleKey} => {(typed, certain)}");

                outputController.TypedChar(typed);
            }
        }
        else
        {
            Debug.LogWarning(value.HasValue ? $"Ended gesture in empty zone: {value}" : "Ended gesture on invalid key");
        }

        stylusModel.normalizedSlider = null;

        lastReportedValue = null;

        return success;
    }

    // Callback for when the InputFieldController register a completed gesture
    public void OnSimulatedFingerUp(int value)
        => OnInputEnd(value);

    private void AnalogUpdate(float value)
        => OnInputValueChange(Mathf.RoundToInt(value));

    public void OnFrontButtonDown()
    {
        stylusModel.frontButtonDown = true;
        if (OnInputEnd(lastReportedValue)) return;

        RaycastHit? hit;
        var raycastable = stylusModel.Raycast(out hit);
        if (raycastable)
        {
            raycastable.GetComponent<Button>()?.onClick.Invoke();
            var dropdown = raycastable.GetComponent<Dropdown>();
            if (dropdown)
            {
                dropdown.value = (dropdown.value + 1) % dropdown.options.Count;
            }
        }
    }

    public void OnFrontButtonUp()
        => stylusModel.frontButtonDown = false;

    public void OnBackButtonDown()
    {
        stylusModel.backButtonDown = true;
        layout.useAlternate = !layout.useAlternate;
    }

    public void OnBackButtonUp()
        => stylusModel.backButtonDown = false;

    // used in editor!
    public void OnTestingLayoutChange(LayoutOption layout)
        => layoutManager.layout = layout;

    public void OnTrialCompleted(bool success)
    {
        if (success)
        {
            RunNextTrial();
        }
    }
    #endregion
}
