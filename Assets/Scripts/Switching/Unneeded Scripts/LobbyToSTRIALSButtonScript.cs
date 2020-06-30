using UnityEngine.SceneManagement;

namespace SceneSwitching
{
    public class LobbyToMainButtonScript : UnityEngine.MonoBehaviour
    {
        public void ChangeToSTrial()
        {
            SceneManager.LoadScene("_STRIALS", LoadSceneMode.Additive);
        }
    }
}