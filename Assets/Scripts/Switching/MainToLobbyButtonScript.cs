public class MainToLobbyButtonScript : UnityEngine.MonoBehaviour
{
    public void ChangeToLobby()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("_STRIALS");

    }
}
