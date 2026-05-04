using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.InputSystem;

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
    [Space]
    [Header("World Video & Event Settings")]
    [SerializeField] private VideoPlayer inWorldVideoPlayer;
    [SerializeField] private GameObject aiObject;
    [Space]
    [Header("Input Settings")]
    [SerializeField] private InputActionReference interactAction;

    public static Action onMoveUp;
    public static Action onMoveDown;
    public static Action onSubmit;
    public static Action onPlay;

    private GameObject lastSelected;
    private bool isPlaying = false;

    private void Awake()
    {
        // Nota: Si usas el mouse para dar clic al botón "Play", necesitas que sea visible.
        // Si usas teclado/mando, déjalo bloqueado.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inWorldVideoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnDisable()
    {
        inWorldVideoPlayer.loopPointReached -= OnVideoFinished;
    }

    private void Start()
    {
        // 1. Bloqueamos al jugador apenas carga la escena
        ActivateMainMenuMode();

        OpenMainMenu();

        if (inWorldVideoPlayer != null)
        {
            inWorldVideoPlayer.loopPointReached += ActivarIA;
        }
    }

    private void ActivarIA(VideoPlayer vp)
    {
        if (aiObject != null)
        {
            aiObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (inWorldVideoPlayer != null)
        {
            inWorldVideoPlayer.loopPointReached -= ActivarIA;
        }
    }

    private void Update()
    {
        //Skip Tutorial
        if (isPlaying)
        {
            if (interactAction.action.triggered)
            {
                inWorldVideoPlayer.SetDirectAudioVolume(0, 0.5f);
                TransitionToGameplay().Forget();
                aiObject.SetActive(true);
            }
        }

        //Always maintains something on the last menu Selection
        if (mainMenu.activeSelf || creditsMenu.activeSelf)
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;

            if (current != null)
            {
                lastSelected = current;
            }
            else if (lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }
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

        inWorldVideoPlayer.gameObject.SetActive(true);

        // 3. LE DAS PLAY: Arranca el video (la cámara ya está frente a la tele y se mantiene ahí)
        if (inWorldVideoPlayer != null)
        {
            inWorldVideoPlayer.Play();
            isPlaying = true;
        }
    }

    //Transitions to gameplay once the video is over (if it wasnt skipped)
    private void OnVideoFinished(VideoPlayer vp)
    {
        isPlaying = false;

        // Call your transition
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