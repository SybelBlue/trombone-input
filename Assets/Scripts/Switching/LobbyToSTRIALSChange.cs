using UnityEngine;
using Utils.UnityExtensions;
// using is = ButtonDisappear;

namespace SceneSwitching
{
    public class LobbyToSTRIALSChange : MonoBehaviour, ITransitionable
    {
        // public ButtonDisappear bd;
        GameObject button;
        public Camera skybox;


        // Called when Main requests a scene change
        public void Transition()
        {
            //ButtonRETURN();
            Utils._STRIALS_scene.LoadAdditive();
        }

        // public void ButtonRETURN()
        // {
        //     var newColor = new Color(235, 255, 255, 1.0f);
        //     button = GameObject.FindWithTag("StartButtonTag");
        //     button.SetActive(false);
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
