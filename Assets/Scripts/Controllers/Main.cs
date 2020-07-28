using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.KeyCode;

#region ProjectNamespaces
using Controller;
using CustomInput;
using MinVR;
using Utils;
using Utils.MinVRExtensions;
using SceneSwitching;
using static CustomInput.VREventFactory.Names;
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

    [SerializeField, Tooltip("The object that loads the current layout")]
    private LayoutManager layoutManager;

    [SerializeField]
    private Stylus stylus;

    [SerializeField]
    private GameObject ground;

    [SerializeField]
    public GameObject buttonBackground;

    // The transform of the indicator
    [SerializeField]
    private RectTransform indicatorRect;

    [SerializeField, Tooltip("The controller that manages the output of letters (usually a Proctor Component)")]
    private TextOutputDisplay outputDisplay;

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

    public GameObject laserPointerObject;





    // The most up-to-date value reported by the InputFieldController
    private uint? lastReportedValue;

    private int currentTrial = -1;
    private int completedChallenges = -1;

    // Unused at 7/28/20. Uncomment bodies of OnShiftDown, OnShiftUp
    //private bool isShiftDown = false;

    // The manager's current layout, or null if no manager exists
    private CustomInput.Layout.AbstractLayout layout
        => layoutManager?.currentLayout;

    private bool usingIndicator
    {
        get { return indicatorRect && indicatorRect.gameObject.activeInHierarchy; }
        set { indicatorRect.gameObject.SetActive(value); }
    }

    private bool runTrial
        => trialExecutionMode == TrialExecutionMode.Always
        || (trialExecutionMode == TrialExecutionMode.OnlyInEditor && Application.isEditor);

    #region UnityMessages
    private void Start()
    {
        ground = GameObject.FindWithTag("GroundFloorTag");
        buttonBackground = GameObject.FindWithTag("ButtonBackgroundTag");
        backToLobby = GameObject.FindWithTag("JumbBackToLobbyTag").GetComponent<Button>();
        laserPointerObject = GameObject.FindWithTag("LaserPointerTag");
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

        // ISSUE#49 //
        //TODO: hacking this in here since we have the server object to also initialize the other button events
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(Return);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(S);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(R);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(D);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(T);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(LeftShift);
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(RightShift);

        Bindings.InitializeMinVRLayoutSwitching(server);

        Bindings.AddMinVRLayoutSwitchingHandlers(i => delegate { layoutManager.DropdownValueSelected(i); });

        Bindings.AddMinVRSandRKeyHandlers(OnSDown, OnRDown, OnDDown, OnTDown, OnShiftDown, OnShiftUp);

        outputDisplay?.ResetText();

        trials = Testing.Utils.ReadTrialsStaggered(logComments: false);

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

        if(!strialsIsLoaded)
        {
          laserPointerObject.SetActive(true);
          buttonBackground.SetActive(true);
        }
    }
    #endregion

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

                casted.stylusProvider = () => stylus;

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

    private bool LoadFieldIfNull<T>(ref T obj, string sceneTag) where T : Component
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
        if (Bindings.doEmulate)
        {
            var gestureStartValue = lastReportedValue ?? Bindings._slider_max_value / 2;
            Bindings.CaptureEmulatedSliderInput(ref eventList, gestureStartValue, lastReportedValue);
            Bindings.CaptureEmulatedButtonInput(ref eventList, layout && layout.usesSlider);
        }
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
            trialProgress.trialCount = new Utils.Tuples.Rational(currentTrial, trials.Count);
            OnChallengeEnd();
            (outputDisplay as Proctor).RunTrial(trials[currentTrial]);
        }
        else
        {
            Debug.LogWarning("Skipped running trial!");
        }
    }

    #region Callbacks
    public void OnSDown()
        => (outputDisplay as Proctor).AdvanceChallenge();

    public void OnRDown()
        => (outputDisplay as Proctor).RestartChallenge();

    public void OnDDown()
        => (outputDisplay as Proctor).FinishTrial();

    public void OnTDown()
        => (outputDisplay as Proctor).RestartTrial();

    public void OnShiftDown()
    { /*isShiftDown = true;*/ }

    public void OnShiftUp()
    { /*isShiftDown = false;*/ }

    public void OnSceneAdvance()
    {
        if (strialsIsLoaded)
        {
            Debug.LogWarning("Scene Advancing to lobby");
            backToLobby.onClick.Invoke();
            SceneManager.UnloadSceneAsync("_STRIALS");
        }
        else
        {
            Debug.LogWarning("Scene Advancing to trial");
            SceneManager.LoadScene("_STRIALS", LoadSceneMode.Additive);
            buttonBackground.SetActive(false);

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
        if (e.type != Utils.SignalProcessing.EventType.FingerUp)
        {
            return;
        }

        if (layout.keyOnFingerUp)
        {
            UpdateValue((uint)e.value);
            TryFindKey(e.value);
        }
        else
        {
            lastReportedValue = e.value;
        }
    }

    private void OnSliderAnalogUpdate(float value)
    {
        stylus.rawSlider = value;
        // Debug.Log("slider: " + Time.time +"   "+ value);
        var scrubbed = (uint)Mathf.RoundToInt(value);
        if (useAutofilter)
        {
            autoFilter.Provide(scrubbed);
            UpdateValue(scrubbed);
        }
        else
        {
            UpdateValue(scrubbed);
        }
    }

    private void UpdateValue(uint value)
    {
        lastReportedValue = value;
        float normalized = ((float)value - (float)Bindings._slider_min_value)/ ((float)Bindings._slider_max_value - (float)Bindings._slider_min_value);
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

        RaycastHit? _hit;
        var raycastable = stylus.Raycast(out _hit);
        if (raycastable)
        {
            raycastable.GetComponent<Button>()?.onClick.Invoke();
            var dropdown = raycastable.GetComponent<Dropdown>();
            if (dropdown)
            {
                dropdown.value = (dropdown.value + 1) % dropdown.options.Count;
            }
        }

        if (TryFindKey(lastReportedValue)) return;
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
            laserPointerObject.SetActive(true);

        }
        else
        {
            RunNextTrial();
        }
    }
    #endregion
}
