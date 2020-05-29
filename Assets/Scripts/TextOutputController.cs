using UnityEngine;
using UnityEngine.UI;

public class TextOutputController : MonoBehaviour
{
    public Text outputDisplay;

    public string text
    {
        get => outputDisplay.text;
        set => outputDisplay.text = value;
    }
}