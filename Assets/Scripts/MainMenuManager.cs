using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel UI Settings")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject creditsMenu;
    [Space]
    [Header("Player Settings")]
    [SerializeField] private FPSController playerController;
    [SerializeField] private PlayerPickDropInteraction pickDropScript;
    [Space]
    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera menuCamera;
    [Space]
    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject creditsMenuFirst;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        OpenMainMenu();
    }

    #region Button Options
    public void PressPlay()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        DeactivateMainMenuMode();
    }

    public void OpenMainMenu()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);

        ActivateMainMenuMode();
    }

    public void OpenCreditsMenu()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(creditsMenuFirst);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    #endregion Button Options

    private void DeactivateMainMenuMode()
    {
        //Player Movement/Interactions
        playerController.enabled = true;
        pickDropScript.enabled = true;

        //Change Camera
        mainCamera.enabled = true;
        menuCamera.enabled = false;
    }

    private void ActivateMainMenuMode()
    {
        //Player Movement/Interactions
        playerController.enabled = false;
        pickDropScript.enabled = false;

        //Change Camera
        mainCamera.enabled = false;
        menuCamera.enabled = true;
    }
}
