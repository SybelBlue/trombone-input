// ﻿using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
//
// namespace SceneSwitching
// {
//     public class KeyPressTest : MonoBehaviour
//     {
//         public Button backToLobby;
//         public Button toTrials;
//         public Main other;
//
//         // Start is called before the first frame update
//         //Finds and assigns the GameObject for the two buttons used to switch
//         //between the lobby and the _STRIALS
//         void Start()
//         {
//             backToLobby = GameObject.FindGameObjectWithTag("JumbBackToLobbyTag").GetComponent<Button>();
            // toTrials = GameObject.FindGameObjectWithTag("StartButtonTag").GetComponent<Button>();
//         }
//
//         // Update is called once per frame
//         // Checks if _STRIALS is loaded.
//         // If so, the return key triggers the off-load of _STRIALS and calls the
//         // onClick action for the return to lobby button.
//         // If not, the return key triggers the addtive onload of the _STRIALS
//         // scene
//         void Update()
//         {
//             Scene sceeneTwo = SceneManager.GetSceneByName("_STRIALS");
//             //Debug.Log($"Active Scene: \"{scene.name}\".");
//             if (Input.GetKeyDown(KeyCode.Return))
//             {
//                 if (sceeneTwo.isLoaded)
//                 {
//                     // other.OnSceneAdvance();
//                     // Debug.LogWarning("OnSceneAdvance WORKED");
                    // backToLobby.onClick.Invoke();
//                     // Debug.LogWarning("backtolobby");
//                 }
//                 else
//                 {
//                     toTrials.onClick.Invoke();
//                     // Debug.LogWarning("toTrials WORKED");
//                 }
//             }
//         }
//     }
// }
