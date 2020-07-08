using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#region ProjectNamespaces
using Controller;
using CustomInput;
using MinVR;
using Utils.MinVRExtensions;
using Utils.UnityExtensions;
using Utils;
using static CustomInput.VREventFactory.Names;
using SceneSwitching;
#endregion

namespace Utils
{
    [System.Serializable]
    public enum TrialExecutionMode
    {
        Always,
        OnlyInEditor,
        Never,
    }
}

#pragma warning disable 649
public class Main : MonoBehaviour, VREventGenerator
{
    private static Main Instance;

    #region EditorSet
    [SerializeField]
    private bool leftHanded, useAutofilter;

    [SerializeField]
    private TrialExecutionMode trialExecutionMode;

    [SerializeField, Tooltip("Is CaveFronWall_Top in MinVR example, _LOBBY, required to run")]
    private VRDevice server;

    [SerializeField]
    // The LayoutManager that is in charge of loading the layout
    private LayoutManager layoutManager;

    [SerializeField]
    private Stylus stylus;

    [SerializeField]
    private GameObject ground;

    [SerializeField]
    private GameObject buttonBackground;

    // The transform of the indicator
    [SerializeField]
    private RectTransform indicatorRect;

    // The place where typed letters go
    [SerializeField]
    private TextOutputDisplay outputDisplay;

    [SerializeField]
    private TextAsset[] trialAssets;

    [SerializeField]
    private TrialProgress trialProgress;

    [SerializeField]
    private AutoFilter autoFilter;

    [SerializeField]
    private List<Testing.Trial> trials;

    [SerializeField]
    private bool strialsIsLoaded;
    #endregion

    public Button backToLobby;

    // The most up-to-date value reported by the InputFieldController
    private uint? lastReportedValue;

    private int currentTrial = -1;
    private int completedChallenges = -1;

    // The manager's current layout, or null if no manager exists
    private CustomInput.Layout.AbstractLayout layout
        => layoutManager?.currentLayout;

    private bool usingIndicator
    {
        get => indicatorRect && indicatorRect.gameObject.activeInHierarchy;
        set => indicatorRect.gameObject.SetActive(value);
    }

    private bool runTrial
        => trialExecutionMode == TrialExecutionMode.Always
        || (trialExecutionMode == TrialExecutionMode.OnlyInEditor && Application.isEditor);

    private void Start()
    {
        ground = GameObject.FindWithTag("GroundFloorTag");
        buttonBackground = GameObject.FindWithTag("ButtonBackgroundTag");
        backToLobby = GameObject.FindGameObjectWithTag("JumbBackToLobbyTag").GetComponent<Button>();
        if (Instance)
        {
            Debug.LogWarning("A second Main script has been created while another exists! This instance will not be saved!");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Bindings._left_handed = leftHanded;

        VRMain.Instance.AddEventGenerator(this);

        VRMain.Instance.AddOnVRAnalogUpdateCallback(_potentiometer, OnSliderAnalogUpdate);

        VRMain.Instance.AddVRButtonCallbacks(_front_button, OnFrontButtonUp, OnFrontButtonDown);
        VRMain.Instance.AddVRButtonCallbacks(_back_button, OnBackButtonUp, OnBackButtonDown);

        Bindings.AddSceneAdvanceCallback(OnSceneAdvance);

        Bindings.InitializeMinVRLayoutSwitching(server);

        Bindings.AddMinVRLayoutSwitchingHandlers(i => delegate { layoutManager.DropdownValueSelected(i); });

        outputDisplay?.ResetText();

        trials = Testing.Utils.ReadTrials(logComments: true);

        autoFilter.OnFilterOutput.AddListener(OnFilterEvent);

        RunNextTrial();

        DontDestroyOnLoad(stylus.gameObject);
        DontDestroyOnLoad(ground.gameObject);
        DontDestroyOnLoad(buttonBackground.gameObject);

        SceneManager.sceneLoaded += OnSceneChange;
    }

    private void Update()
    {
        bool indicator = layout && layout.usesSlider && Bindings.inputThisFrame;
        if (indicator != usingIndicator)
        {
            usingIndicator = indicator;
        }

        if (layout && stylus && stylus.useLaser != layout.usesRaycasting)
        {
            stylus.useLaser = layout.usesRaycasting;
        }

        if (outputDisplay && !outputDisplay.emptyText && Bindings.spaceDown)
        {
            outputDisplay.TypedChar(' ');
        }

        if (outputDisplay && Bindings.backspaceDown)
        {
            outputDisplay.TypedBackspace();
        }

        if (layoutManager && Bindings.emulatingLayoutSwitch.HasValue)
        {
            layoutManager.DropdownValueSelected(Bindings.emulatingLayoutSwitch.Value);
        }

        if (layout && stylus)
        {
            stylus.angleProvider = layout.StylusRotationBounds;
            layout.UpdateState(new InputData(lastReportedValue, stylus));
        }
    }

    public void LoadNullFields()
    {
        LoadFieldIfNull(ref stylus, "StylusTag");
        LoadFieldIfNull(ref layoutManager, "LayoutManager");
        LoadFieldIfNull(ref trialProgress, "TrialProgress");
        LoadFieldIfNull(ref indicatorRect, "SliderIndicator");

        stylus?.FillIndicatorDisplayIfNull();

        if (Static.FillWithTaggedIfNull(ref outputDisplay, "OutputDisplay"))
        {
            outputDisplay.ResetText();

            if (outputDisplay is Proctor)
            {
                var casted = outputDisplay as Proctor;

                casted.OnTrialEnd.AddListener(OnTrialEnd);
                casted.OnLayoutChange.AddListener(OnTestingLayoutChange);
                casted.OnChallengeEnd.AddListener(OnChallengeEnd);

                Debug.Log("Found \"OutputDisplay\" in scene and loaded into Main as Proctor.");
            }
            else
            {
                Debug.Log("Found \"OutputDisplay\" in scene and loaded into Main as TextOutputDisplay.");
            }
        }
    }

    private bool LoadFieldIfNull<T>(ref T obj, string sceneTag)
        where T : Component
    {
        bool result = Static.FillWithTaggedIfNull(ref obj, sceneTag);
        if (result)
        {
            Debug.Log($"Found \"{sceneTag}\" in scene and loaded into Main.");
        }
        return result;
    }

    public void AddEventsSinceLastFrame(ref List<VREvent> eventList)
    {
        var gestureStartValue = lastReportedValue ?? Bindings._slider_max_value / 2;
        Bindings.CaptureEmulatedSliderInput(ref eventList, gestureStartValue, lastReportedValue);
        Bindings.CaptureEmulatedButtonInput(ref eventList, layout && layout.usesSlider);
    }

    private void RunNextTrial()
    {
        if (!outputDisplay)
        {
            Debug.LogWarning("Skipped running trial because outputDisplay is null.");
            return;
        }
        currentTrial++;
        completedChallenges = -1;
        if (currentTrial < trials.Count && outputDisplay is Proctor && runTrial)
        {
            trialProgress.trialCount = (currentTrial, trials.Count);
            OnChallengeEnd();
            (outputDisplay as Proctor).RunTrial(trials[currentTrial]);
        }
        else
        {
            Debug.LogWarning("Skipped running trial!");
        }
    }

    #region Callbacks
    public void OnSceneAdvance()
    {
        Debug.LogWarning("Scene Advanced!");
        if (strialsIsLoaded)
        {
            Scenes._STRIALS.UnloadAsync();

        }
        else
        {
            Scenes._STRIALS.LoadAdditive();
        }

        strialsIsLoaded = !strialsIsLoaded;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode _)
    {
        if (!scene.name.Equals(Scenes._STRIALS)) return;

        LoadNullFields();

        if (layout && trialProgress)
        {
            RunNextTrial();
        }
        else
        {
            Debug.LogWarning("Can not run trial after scene change!");
        }
    }

    public void OnFilterEvent(Utils.SignalProcessing.FilterEventData e)
    {
        switch (e.type)
        {
            case Utils.SignalProcessing.EventType.NoTouches:
                return;
            case Utils.SignalProcessing.EventType.FingerUp:
                if (layout.keyOnFingerUp)
                {
                    // TODO: probably needs changing
                    TryFindKey(e.value);
                }
                else
                {
                    lastReportedValue = e.value;
                }
                Debug.LogWarning("Click!");
                return;
            default:
                if (e.value.HasValue)
                {
                    UpdateValue(e.value.Value);
                }
                return;
        }
    }

    private void OnSliderAnalogUpdate(float value)
    {
        var scrubbed = (uint)Mathf.RoundToInt(value);
        if (useAutofilter)
        {
            autoFilter.Provide(scrubbed);
        }
        else
        {
            UpdateValue(scrubbed);
        }
    }

    private void UpdateValue(uint value)
    {
        lastReportedValue = value;
        float normalized = value / (float)Bindings._slider_max_value;
        stylus.normalizedSlider = normalized;

        if (!layout || !indicatorRect) return;

        float width = layout.rectTransform.rect.width;
        var pos = indicatorRect.localPosition;

        pos.x = width * (normalized - 0.5f);

        indicatorRect.localPosition = pos;
    }

    private bool TryFindKey(uint? value)
    {
        var currentInputData = new InputData(value, stylus);
        CustomInput.KeyData.AbstractData parentKey = layout?.KeysFor(currentInputData)?.parent;

        bool success = parentKey != null;

        if (success)
        {
            char? typed = layout.GetSelectedLetter(currentInputData);

            if (typed == '\b')
            {
                Debug.Log("Pressed Backspace");

                outputDisplay.TypedBackspace();
            }
            else
            {
                Debug.Log($"Pressed {parentKey} @ {typed}");

                if (typed.HasValue)
                {
                    outputDisplay.TypedChar(typed.Value);
                }
            }
        }
        else
        {
            Debug.LogWarning(value.HasValue ? $"Ended gesture in empty zone: {value}" : "Ended gesture on invalid key");
        }

        stylus.normalizedSlider = null;

        lastReportedValue = null;

        return success;
    }

    public void OnFrontButtonDown()
    {
        stylus.frontButtonDown = true;
        if (TryFindKey(lastReportedValue)) return;

        var raycastable = stylus.Raycast(out _);
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
        => stylus.frontButtonDown = false;

    public void OnBackButtonDown()
    {
        stylus.backButtonDown = true;
        if (layout)
        {
            layout.useAlternate = !layout.useAlternate;
        }
    }

    public void OnBackButtonUp()
        => stylus.backButtonDown = false;

    public void OnTestingLayoutChange(LayoutOption layout)
        => layoutManager.layout = layout;

    public void OnChallengeEnd()
        => trialProgress.trialProgress = (++completedChallenges) / (float)trials[currentTrial].Length;

    public void OnTrialEnd(bool success)
    {
        if (success)
        {
            OnSceneAdvance();
            backToLobby.onClick.Invoke();
            // backToLobby.gameObject.SetActive(true);

            //TODO:Make it so the jump button is triggered
            // OnSceneChange("_STRIALS");
            // RunNextTrial();
        }
        else
        {
            RunNextTrial();
        }
    }
    #endregion
}
