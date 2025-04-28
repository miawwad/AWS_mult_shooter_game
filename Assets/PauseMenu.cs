using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private bool isPaused = false;

    void Start()
    {
        // Instead of FindObjectOfType, we'll find the PlayerArmature GameObject
        GameObject player = GameObject.FindWithTag("Player"); // Make sure your player is tagged "Player"
        if (player != null)
        {
            starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
            thirdPersonController = player.GetComponent<ThirdPersonController>();
            animator = player.GetComponent<Animator>();
        }
        else
        {
            Debug.LogWarning("PauseManager could not find the Player GameObject!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Re-enable movement and input
        if (starterAssetsInputs != null) starterAssetsInputs.enabled = true;
        if (thirdPersonController != null) thirdPersonController.enabled = true;
        if (animator != null) animator.enabled = true;

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Disable movement and input
        if (starterAssetsInputs != null) starterAssetsInputs.enabled = false;
        if (thirdPersonController != null) thirdPersonController.enabled = false;
        if (animator != null) animator.enabled = false;

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure to replace "MainMenu" with the correct scene name
    }
}
