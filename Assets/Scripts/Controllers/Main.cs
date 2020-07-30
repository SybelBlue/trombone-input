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
    private Stylus stylus;   // Instantiates the Stylus gameObject (ZMS)

    [SerializeField]
    private GameObject ground;   // Instantiates the ground game object (ZMS)

    [SerializeField]
    public GameObject buttonBackground; // Instantiates the start button game object(ZMS)

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

    public Button backToLobby; // Instantiates the back to lobby button (ZMS)

    public GameObject laserPointerObject; // Instantiates the raycast line game object (ZMS)



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
      // This assigns the gameObject instances to their respected gameObjects. (ZMS)
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
        
        VRMain.Instance.vrDevice.unityKeysToVREvents.Add(Bindings._scene_advance_key);

        Bindings.AddSceneAdvanceCallback(OnSceneAdvance);

        Bindings.InitializeMinVRLayoutSwitching(server);

        Bindings.AddMinVRLayoutSwitchingHandlers(i => delegate { layoutManager.DropdownValueSelected(i); });

        Bindings.AddMinVRChallengeAndTrialCallbacks(SkipTrial, RestartTrial, SkipChallenge, RestartChallenge);

        outputDisplay?.ResetText();

        trials = Testing.Utils.ReadTrialsStaggered(logComments: false);

        RunNextTrial();

        // This section ensures that the stylus, the ground, and the start button art
        // not destroyed when the user/proctor switches between the Lobby and Trial
        // scenes. (ZMS)
        DontDestroyOnLoad(stylus.gameObject);
        DontDestroyOnLoad(ground.gameObject);
        DontDestroyOnLoad(buttonBackground.gameObject);
        DontDestroyOnLoad(backToLobby.gameObject);

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
        // Each time the update function is ran, this section checks to see
        // whether the program has loaded the _LOBBY scene or _STRIALS scene.
        // When the active scene is the _LOBBY, the program sets the start
        // button and the ray cast line as active, making them visible to the
        // user and proctor. (ZMS)
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
    public void SkipChallenge()
        => (outputDisplay as Proctor).AdvanceChallenge();

    public void RestartChallenge()
        => (outputDisplay as Proctor).RestartChallenge();

    public void SkipTrial()
        => (outputDisplay as Proctor).FinishTrial();

    public void RestartTrial()
        => (outputDisplay as Proctor).RestartTrial();

    // When the program calls OnSceneAdvance, the function first checks to see which
    // scene is active If the user is in the Lobby scene, the function announces to
    // the user/proctor that they are progressing to the trial scene, then loads
    // _STRIALS additively. Then the function deactivates the start button, hiding
    // it from the users view.
    // If the user is in the _STRIALS scenes, the function announces that the
    // user/proctor is advancing to the Lobby, where they started. Next, the
    // function invokes the onClick functions of the backToLobby button. Lastly, the
    // function off-loads the trial scene.
    // -ZMS

    public void OnSceneAdvance()
    {
        if (strialsIsLoaded)
        {
            Debug.LogWarning("Scene Advancing to Lobby");
            backToLobby.onClick.Invoke();
            SceneManager.UnloadSceneAsync("_STRIALS");

        }
        else
        {
            Debug.LogWarning("Scene Advancing to Trial");
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
