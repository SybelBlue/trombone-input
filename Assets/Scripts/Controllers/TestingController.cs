using System;
using Testing;
using CustomInput;
using System.Linq;
using UnityEngine;

[Serializable]
public class TestingControllerEvent : UnityEngine.Events.UnityEvent<LayoutOption>
{ }

public class TestingController : TextOutputController
{
    public string currentPrompt
    { get; set; }

    public string currentOutput
    { get; private set; }

    public int? trialNumber
        => currentTrial?.trialNumber;

    [UnityEngine.Tooltip("Called when the trial requests a layout change")]
    public TestingControllerEvent OnLayoutChange;

    public ChallengeType? currentChallengeType;

    private LayoutOption[] layoutOrder;

    private int _layoutIndex = 0;

    private int _trialIndex;

    private LayoutOption currentLayout
        => layoutOrder[_layoutIndex];

    private TrialItem currentTrialItem
        => currentTrial?.items[_trialIndex];

    private Trial? currentTrial = null;

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
        UpdateDisplay();
    }

    public override void TypedBackspace()
    {
        switch (currentChallengeType)
        {
            case null:
                base.TypedBackspace();
                return;
            case ChallengeType.Practice:
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
            case ChallengeType.Blind:
                // TODO: play noise? shake UI? vibrate controller?
                Debug.LogWarning("Disregarding Backspace during Blind Challenge!");
                return;
            case ChallengeType.Perfect:
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
        }

        throw new System.ArgumentException(currentChallengeType.ToString());
    }

    private void UpdateDisplay()
    {
        switch (currentChallengeType)
        {
            case null:
                return;
            case ChallengeType.Practice:
                return;
            case ChallengeType.Blind:
                int l = currentOutput.Length;
                text = $"<color=blue>{"*".Repeat(l)}</color>{currentPrompt.Substring(l)}";
                return;
            case ChallengeType.Perfect:
                string final = "";
                bool lastCorrect = false;
                for (int i = 0; i < currentOutput.Length; i++)
                {
                    // get char and check if matches prompt
                    char c = currentOutput[i];
                    bool correct = i < currentPrompt.Length && currentPrompt[i] == c;

                    // if it is not the same correctness as the last character...
                    if (lastCorrect != correct || i == 0)
                    {
                        // ... end the last color block and start a new properly colored one ...
                        final += $"{(i > 0 ? "</color>" : "")}<color={(correct ? "green" : "red")}>";
                        // ... save the change in correctness
                        lastCorrect = correct;
                    }

                    // either way, add the character to the string
                    final += c;
                }
                // close the last color block
                final += "</color>";

                // add any remaining characters in the prompt
                if (final.Length < currentPrompt.Length)
                {
                    final += currentPrompt.Substring(final.Length);
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
        AdvanceTrial();
    }

    private void AdvanceTrial()
    {
        if (!currentTrial.HasValue) return;

        _trialIndex++;

        if (_trialIndex >= currentTrial.Value.Length)
        {
            Debug.LogWarning($"Completed Trial {trialNumber}");
            currentTrial = null;
            return;
        }

        if (currentTrialItem.Apply(this))
        {
            UpdateDisplay();
            return;
        }

        AdvanceTrial();
    }
}