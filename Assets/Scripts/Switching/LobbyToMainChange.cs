﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyToMainChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Space))
      {
        // SceneManager.LoadScene("_MAIN");
        Application.LoadLevel("_MAIN");

      }

      // Application.LoadLevel("_MAIN");

    }
}
