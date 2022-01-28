using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auth0.Helpers
{
    public class SceneLoader : MonoBehaviour
    {
        void Start()
        {
            // Used by "Preload" scene to initialize auth manager and then, navigate to the next scene.
            SceneManager.LoadScene(1);
        }
    }
}
