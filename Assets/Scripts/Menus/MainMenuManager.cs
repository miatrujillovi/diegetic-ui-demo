using UnityEngine;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using System;

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
    [SerializeField] private int menuFOV = 40;
    [SerializeField] private int gameplayFOV = 60;
    [Space]
    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject creditsMenuFirst;

    public static Action onMoveUp;
    public static Action onMoveDown;
    public static Action onSubmit;
    public static Action onPlay;

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
        onPlay?.Invoke();
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


    #region Main Menu Mode
    private void DeactivateMainMenuMode()
    {
        //Player Movement/Interactions
        playerController.enabled = true;
        pickDropScript.enabled = true;

        TransitionToGameplay().Forget();
    }

    private void ActivateMainMenuMode()
    {
        //Player Movement/Interactions
        playerController.enabled = false;
        pickDropScript.enabled = false;
    }

    private async UniTask TransitionToGameplay()
    {
        float duration = 1f;

        //1.- Change the FOV
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = t * t * (3f - 2f * t);

            menuCamera.fieldOfView = Mathf.Lerp(menuFOV, gameplayFOV, t);

            await UniTask.Yield();
        }

        //2.- Rotate X Axis
        float startX = menuCamera.transform.eulerAngles.x;
        float targetX = 0f;

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = t * t * (3f - 2f * t);

            float newX = Mathf.LerpAngle(startX, targetX, t);

            Vector3 angles = menuCamera.transform.eulerAngles;
            angles.x = newX;
            menuCamera.transform.eulerAngles = angles;

            await UniTask.Yield();
        }

        //3.- Switch Cameras
        mainCamera.gameObject.SetActive(true);
        menuCamera.gameObject.SetActive(false);
    }

    #endregion Main Menu Mode
}
