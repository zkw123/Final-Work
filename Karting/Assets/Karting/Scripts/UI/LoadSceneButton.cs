using UnityEngine;
using UnityEngine.SceneManagement;

namespace KartGame.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        [Tooltip("What is the name of the scene we want to load when clicking the button?")]
        public string SceneName;

        public void LoadTargetScene() 
        {
            if (SceneName == "SinglePlayer"|| SceneName == "SinglePlayer2")
                Ranking.mode = false;
            if (SceneName == "MultiPlayer")
                Ranking.mode = true;
            SceneManager.LoadSceneAsync(SceneName);
        }
    }
}
