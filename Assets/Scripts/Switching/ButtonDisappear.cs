using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDisappear : MonoBehaviour
{
  GameObject button;
  // public Button StartPracticeButton;
    // // Start is called before the first frame update
    // void Start()
    // {
    //   button = GameObject.FindWithTag("StartButtonTag");
    //   // ButtonClick();
    //
    //   // button.onClick.AddListener(Butt)
    //   // Button sButton = StartPracticeButton.GetComponent<Button>();
    //   // sButton.onClick.AddListener(TaskOnClick);
    // }

    public void ButtonClick()
    {
      button = GameObject.FindWithTag("StartButtonTag");
      button.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
