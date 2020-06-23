using System;
using Testing;
using CustomInput;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TestingLayoutEvent : UnityEngine.Events.UnityEvent<LayoutOption>
{ }

[Serializable]
public class TestingTrialEvent : UnityEngine.Events.UnityEvent<bool>
{ }

#pragma warning disable 649
public class TestingController : TextOutputController
{
    private string _prompt;
    public string currentPrompt
    {
        get => _prompt;
        set
        {
            _prompt = value;
            ResetText();
        }
    }

    public override string suggestionSource
    {
        get => currentOutput;
        protected set
        {
            currentOutput = value;
            UpdateDisplay();
        }
    }

    public string currentOutput
    { get; private set; }

    public int? trialNumber
        => currentTrial?.trialNumber;

    [SerializeField]
    private Button practiceEndButton;

    [SerializeField]
    private Text challengeTypeIndicator;

    [UnityEngine.Tooltip("Called when the trial requests a layout change")]
    public TestingLayoutEvent OnLayoutChange;

    [UnityEngine.Tooltip("Called when a trial finishes")]
    public TestingTrialEvent OnTrialEnd;

    public Challenge.Type? currentChallengeType;

    private LayoutOption[] layoutOrder;

    private int _layoutIndex = 0;

    private int _trialIndex;

    private LayoutOption currentLayout
        => layoutOrder[_layoutIndex];

    private string currentLayoutName
        => layoutOrder == null ? "" : Enum.GetName(typeof(LayoutOption), currentLayout);

    private TrialItem currentTrialItem
        => currentTrial?.items[_trialIndex];

    private Trial? currentTrial = null;

    private ResultBuilder builder;

    private bool completedChallenge
    {
        get
        {
            switch (currentChallengeType)
            {
                case null:
                    return false;
                case Challenge.Type.Practice:
                    return false;
                case Challenge.Type.Perfect:
                    return currentOutput.Equals(currentPrompt);
                case Challenge.Type.Blind:
                    return currentOutput.Length >= currentPrompt.Length;
            }
            throw new System.ArgumentException(currentChallengeType.ToString());
        }
    }

    public override void Start()
    {
        base.Start();
        practiceEndButton.onClick.AddListener(OnPracticeButtonDown);
        UpdateDisplay();
    }

    protected override void OnSuggestionButtonClick(string suggestionText)
    {
        builder?.AddKeypress(suggestionText);
        base.OnSuggestionButtonClick(suggestionText);
    }

    public override void ResetText()
    {
        base.ResetText();
        currentOutput = "";

        if (currentChallengeType.HasValue)
        {
            UpdateDisplay();
        }
    }

    public override void AppendLetter(char c)
    {
        base.AppendLetter(c);
        currentOutput += c;
        builder?.AddKeypress(c);
        UpdateDisplay();
    }

    public override void TypedBackspace()
    {
        builder?.AddKeypress('\b');
        switch (currentChallengeType)
        {
            case null:
                base.TypedBackspace();
                return;
            case Challenge.Type.Practice:
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
            case Challenge.Type.Blind:
                // TODO: play noise? shake UI? vibrate controller?
                Debug.LogWarning("Disregarding Backspace during Blind Challenge!");
                return;
            case Challenge.Type.Perfect:
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
        }

        throw new System.ArgumentException(currentChallengeType.ToString());
    }

    private void UpdateDisplay()
    {
        if (completedChallenge)
        {
            AdvanceTrial();
        }

        practiceEndButton.gameObject.SetActive(false);
        challengeTypeIndicator.text =
            currentChallengeType == null ?
                "Sandbox" :
                Enum.GetName(typeof(Challenge.Type), currentChallengeType);

        switch (currentChallengeType)
        {
            case null:
                return;
            case Challenge.Type.Practice:
                practiceEndButton.gameObject.SetActive(true);
                text = currentOutput.Length == 0 ? currentPrompt : currentOutput;
                return;
            case Challenge.Type.Blind:
                int l = currentOutput.Length;
                text = $"<color=blue>{"*".Repeat(l)}</color>{currentPrompt.Substring(l)}";
                return;
            case Challenge.Type.Perfect:
                string final = "";
                bool lastCorrect = false;
                for (int i = 0; i < currentOutput.Length; i++)
                {
                    // get char
                    char c = currentOutput[i];

                    // get target char
                    bool exceedsPrompt = i >= currentPrompt.Length;
                    char? target = exceedsPrompt ? (char?)null : currentPrompt[i];

                    // check if correct
                    bool correct = target == c;

                    // if it is not the same correctness as the last character...
                    if (lastCorrect != correct || i == 0)
                    {
                        // ... end the last color block and start a new properly colored one ...
                        final += $"{(i > 0 ? "</color>" : "")}<color={(correct ? "green" : "red")}>";
                        // ... save the change in correctness
                        lastCorrect = correct;
                    }

                    // add the character to the string, if it's wrong and ' ', use _ or -
                    if (!correct && c == ' ')
                    {
                        final += target != '_' ? "_" : "-";
                    }
                    else
                    {
                        final += c;
                    }
                }

                if (currentOutput.Length > 0)
                {
                    // close the last color block
                    final += "</color>";
                }

                // add any remaining characters in the prompt
                if (currentOutput.Length < currentPrompt.Length)
                {
                    final += currentPrompt.Substring(currentOutput.Length);
                }


                text = final;
                return;
        }
        throw new System.ArgumentException(currentChallengeType.ToString());
    }

    public void RandomizeLayouts()
    {
        System.Random rnd = new System.Random();
        layoutOrder = ((LayoutOption[])Enum.GetValues(typeof(LayoutOption))).OrderBy(_x => rnd.Next()).ToArray();
    }

    public void AdvanceLayout()
    {
        _layoutIndex = (_layoutIndex + 1) % layoutOrder.Length;
        OnLayoutChange.Invoke(currentLayout);
    }

    public void RunTrial(Trial t)
    {
        currentTrial = t;
        _trialIndex = -1;
        builder = new ResultBuilder();
        AdvanceTrial();
    }

    private void AdvanceTrial()
    {
        if (!currentTrial.HasValue) return;

        _trialIndex++;

        if (_trialIndex >= currentTrial.Value.Length)
        {
            FinishTrial();
            return;
        }

        builder.Push(currentTrialItem, currentOutput, currentLayoutName);

        if (currentTrialItem.Apply(this))
        {
            UpdateDisplay();
            return;
        }

        AdvanceTrial();
    }

    private void FinishTrial()
    {
        Debug.LogWarning($"Completed Trial {trialNumber}");

        currentTrial = null;

        try
        {
            Testing.Utils.Write(builder.Finish(currentOutput), Application.isEditor);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            builder = null;
        }

        OnTrialEnd.Invoke(true);
    }

    private void OnPracticeButtonDown()
    {
        if (currentChallengeType == Challenge.Type.Practice)
        {
            AdvanceTrial();
        }
        else
        {
            Debug.LogWarning("Practice End Button pressed outside of practice!");
        }
    }
}