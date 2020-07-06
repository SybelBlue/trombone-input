using Utils.UnityExtensions;
using UnityEngine;

namespace SceneSwitching
{
    public class STRIALSToLobbyChange : MonoBehaviour, ITransitionable
    {
      // GameObject buttontwo;
      // public Camera skybox;
        // Start is called before the first frame update
        //void Start()
        //{

        //}

        // Called when Main requests a scene change
        public void Transition()
        {
            //ButtonReRETURN();
            Utils._STRIALS_scene.UnloadAsync();
        }


        // public void ButtonReRETURN()
        // {
        //     var newColor = new Color(184, 158, 195, 0.0f);
        //     buttontwo = GameObject.FindWithTag("StartButtonTag");
        //     buttontwo.SetActive(true);
        //
        //     // skybox = GameObject.FindWithTag("MainCamera");
        //     skybox = GetComponent<Camera>();
        //     skybox = Camera.main;
        //     skybox.clearFlags = CameraClearFlags.Skybox;
        //     skybox.backgroundColor = newColor;
        //
        //     //TODO: Set skybox to EBFFFF
        //     // skybox = GameObject.FindWithTag("MainCamera");
        //     // skybox.SetBackgroundColor("EBFFFF");
        // }
    }
}
