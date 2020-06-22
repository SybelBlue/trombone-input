using System;
using UnityEngine;
using Testing;
using CustomInput;
using System.Linq;

public class TestingController : MonoBehaviour
{
    public string currentPrompt
    { get; set; }

    public string currentOutput
    { get; private set; }

    public int trialNumber
    { get; set; }

    public ChallengeType? currentChallenge;

    private LayoutOption[] layoutOrder;

    private int _layoutIndex;

    private LayoutOption currentLayout
        => layoutOrder[_layoutIndex];

    public void TypedLetter(char c)
        => throw new NotImplementedException();

    public void RandomizeLayouts()
    {
        System.Random rnd = new System.Random();
        layoutOrder = ((LayoutOption[])Enum.GetValues(typeof(LayoutOption))).OrderBy(_x => rnd.Next()).ToArray();
    }

    public void AdvanceLayout()
    {
        _layoutIndex++;
        // TODO: make the display change
    }
}