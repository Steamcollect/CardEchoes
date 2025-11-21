using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //[Header("Settings")]

    private bool isPaused = false;
    //[Header("References")]

    [SerializeField] private GameObject menuPause;

    //[Header("Input")]

    public InputActionReference pauseInput;
    //[Header("Output")]


    private void OnEnable()
    {
        pauseInput.action.started += HandlePause;
    }
    private void Start()
    {
        menuPause.SetActive(false);
    }


    private void HandlePause(InputAction.CallbackContext ctx)
    {
        if (isPaused == true)
        {
            ResumeGame();
        } else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        Debug.Log("PauseGame");
        if (menuPause != null)
        {
            menuPause.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        if (menuPause != null)
        {
            menuPause.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}