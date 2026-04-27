using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject creditsMenu;
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
    }

    public void OpenMainMenu()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);

        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
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
}
