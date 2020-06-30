namespace SceneSwitching
{
    public class OffLoadingCanvasScene : UnityEngine.MonoBehaviour
    {
        public void EndTrialScene()
        {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("_STRIALS");
        }
    }
}