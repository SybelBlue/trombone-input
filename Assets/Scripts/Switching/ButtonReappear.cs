using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonReappear : MonoBehaviour
{
  public void ButtonReClick()
  {
    var newColor = new Color (235,255,255,1.0f);
    button = GameObject.FindWithTag("StartButtonTag");
    button.SetActive(false);

    // skybox = GameObject.FindWithTag("MainCamera");
    skybox = GetComponent<Camera>();
    skybox = Camera.main;
    skybox.clearFlags = CameraClearFlags.Skybox;
    skybox.backgroundColor = newColor;

    //TODO: Set skybox to EBFFFF
    // skybox = GameObject.FindWithTag("MainCamera");
    // skybox.SetBackgroundColor("EBFFFF");
  }
    // // Start is called before the first frame update
    // void Start()
    // {
    //
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //
    // }
}
