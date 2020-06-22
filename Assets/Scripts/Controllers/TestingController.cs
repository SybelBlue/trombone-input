using System;
using Testing;
using CustomInput;
using System.Linq;

[Serializable]
public class TestingControllerEvent : UnityEngine.Events.UnityEvent<LayoutOption>
{ }

public class TestingController : TextOutputController
{
    public string currentPrompt
    { get; set; }

    public string currentOutput
    { get; private set; }

    public int trialNumber
    { get; set; }

    [UnityEngine.Tooltip("Called when the trial requests a layout change")]
    public TestingControllerEvent OnLayoutChange;

    public ChallengeType? currentChallenge;

    private LayoutOption[] layoutOrder;

    private int _layoutIndex = 0;

    private LayoutOption currentLayout
        => layoutOrder[_layoutIndex];

    public void TypedLetter(char c)
    {
        if (!currentChallenge.HasValue)
        {
            throw new System.InvalidOperationException("uninitialized challengeType");
        }

        if (c == '\b')
        {
            TypedBackspace();
        }
        else
        {
            currentOutput += c;
        }

        UpdateDisplay();
    }

    public override void TypedBackspace()
    {
        switch (currentChallenge)
        {
            case null:
                throw new System.InvalidOperationException("uninitialized challengeType");
            case ChallengeType.Practice:
                text = text.Backspace();
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
            case ChallengeType.Blind:
                // TODO: play noise? shake UI? vibrate controller?
                UnityEngine.Debug.LogWarning("Disregarding Backspace!");
                return;
            case ChallengeType.Perfect:
                text = text.Backspace();
                currentOutput = currentOutput.Backspace();
                UpdateDisplay();
                return;
        }

        throw new System.ArgumentException(currentChallenge.ToString());
    }

    private void UpdateDisplay()
    {
        switch (currentChallenge)
        {
            case null:
                throw new System.InvalidOperationException("uninitialized challengeType");
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
        throw new System.ArgumentException(currentChallenge.ToString());
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
}