using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditsUI : MonoBehaviour
{
    private PlayerInput inputActions;

    private void Awake()
    {
        inputActions = new();
    }
    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToPreviousScene()
    {
        GameManager.Instance.parentCanvas.SetActive(true);

        inputActions.MainMenu.Disable();
        inputActions.Player.Enable();

        SceneManager.UnloadSceneAsync("Test");
    }
}
