using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }
}