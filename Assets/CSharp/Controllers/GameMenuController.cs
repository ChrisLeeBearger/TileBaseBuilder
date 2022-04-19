using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.CSharp.Controllers
{
    public class GameMenuController : MonoBehaviour
    {
        public void RestartScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}
