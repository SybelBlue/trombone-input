using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSwitching
{
    public class OnLoadingCanvasScene : UnityEngine.MonoBehaviour
    {

        public void StartTrialScene()
        {
          UnityEngine.SceneManagement.SceneManager.LoadScene("_STRIALS", LoadSceneMode.Additive);
        }

    }
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
// public void LeaveTrialScene()
// {
//   if(!SceneManager.GetActiveScene().name.Equals("Stylus Trial"))
//   {
//     SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0)); //set's the active scene to the basic scene since the testObj scene is going to be unloaded.
//
//   }
//   SceneManager.sceneLoaded -= OnSceneLoaded;
//   //SceneManager.LoadScene(0); //Transition Scene
//   SceneManager.UnloadSceneAsync(sceneIndex);
//   //SceneManager.LoadScene(0, LoadSceneMode.Single);
//
// }
