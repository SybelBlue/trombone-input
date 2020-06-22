using System;
using UnityEngine;
using Testing;

public class TestingController : MonoBehaviour
{
    public string currentPrompt
    { get; set; }

    public string currentOutput
    { get; private set; }

    public int trialNumber
    { get; set; }

    public ChallengeType? currentChallenge;

    public void RandomizeLayouts()
        => throw new NotImplementedException();

    public void AdvanceLayout()
        => throw new NotImplementedException();
}