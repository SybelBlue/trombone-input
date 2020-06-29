using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnLoadingCanvasScene : MonoBehaviour
{
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
    void LeaveTrialScene()
    {
      if(!SceneManager.GetActiveScene().name.Equals("Basic Scene"))
      {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0)); //set's the active scene to the basic scene since the testObj scene is going to be unloaded.

      }
      SceneManager.sceneLoaded -= OnSceneLoaded;
      //SceneManager.LoadScene(0); //Transition Scene
      SceneManager.UnloadSceneAsync(sceneIndex);
      //SceneManager.LoadScene(0, LoadSceneMode.Single);

    }

    // SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
    // SceneManager.sceneLoaded += OnSceneLoaded;

}
