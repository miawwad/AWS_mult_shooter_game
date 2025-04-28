using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // These must be public, return void, and take no parameters:
    public void PlayPunch()
    {
        SceneManager.LoadScene("PunchScene");
    }

    public void PlayShoot()
    {
        SceneManager.LoadScene("ShootScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ReloadPunch() 
    {
        SceneManager.LoadScene("PunchScene", LoadSceneMode.Single);
    }

}

