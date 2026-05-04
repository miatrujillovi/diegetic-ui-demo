using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
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

    [Tooltip("Punto vacío frente a la tele para ver el video")]
    [SerializeField] private Transform puntoDeAcercamiento;

    [Space]
    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenuFirst;
    [SerializeField] private GameObject creditsMenuFirst;
    [Space]
    [Header("World Video & Event Settings")]
    [SerializeField] private VideoPlayer inWorldVideoPlayer;
    [SerializeField] private GameObject aiObject;

    public static Action onMoveUp;
    public static Action onMoveDown;
    public static Action onSubmit;
    public static Action onPlay;

    private void Awake()
    {
        // Nota: Si usas el mouse para dar clic al botón "Play", necesitas que sea visible.
        // Si usas teclado/mando, déjalo bloqueado.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        // 1. Bloqueamos al jugador apenas carga la escena
        playerController.enabled = false;
        pickDropScript.enabled = false;

        OpenMainMenu();

        // 2. INICIA EL JUEGO: Hacemos el zoom automático hacia la tele
        ZoomInicialHaciaLaTele().Forget();

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

    #region Button Options
    public void PressPlay()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        // 3. LE DAS PLAY: Arranca el video (la cámara ya está frente a la tele y se mantiene ahí)
        if (inWorldVideoPlayer != null)
        {
            inWorldVideoPlayer.Play();
        }

        // 4. Inicia la secuencia que espera el fin del video para alejarte
        SecuenciaVideoYDarControl().Forget();
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

    #region Secuencias de Animación de Cámara

    private async UniTask ZoomInicialHaciaLaTele()
    {
        if (puntoDeAcercamiento == null) return;

        float duration = 2.0f; // Segundos que tarda en acercarse al abrir el juego
        float time = 0f;

        // Guarda desde dónde empieza (donde sea que hayas dejado la menuCamera en Unity)
        Vector3 startPos = menuCamera.transform.position;
        Quaternion startRot = menuCamera.transform.rotation;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3f - 2f * t); // Suavizado curvo

            menuCamera.transform.position = Vector3.Lerp(startPos, puntoDeAcercamiento.position, t);
            menuCamera.transform.rotation = Quaternion.Slerp(startRot, puntoDeAcercamiento.rotation, t);

            await UniTask.Yield();
        }
    }

    private async UniTask SecuenciaVideoYDarControl()
    {
        // PASO A: ESPERAR A QUE EL VIDEO TERMINE
        if (inWorldVideoPlayer != null)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            await UniTask.WaitUntil(() => !inWorldVideoPlayer.isPlaying);
        }

        // PASO B: EL VIDEO ACABÓ -> ZOOM OUT (Viaje a la cabeza del jugador)
        float duration = 1.5f;
        float time = 0f;

        Vector3 startPos = menuCamera.transform.position;
        Quaternion startRot = menuCamera.transform.rotation;

        Vector3 targetPos = mainCamera.transform.position;
        Quaternion targetRot = mainCamera.transform.rotation;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            menuCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            menuCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            await UniTask.Yield();
        }

        // PASO C: TE DA EL CONTROL
        mainCamera.gameObject.SetActive(true);
        menuCamera.gameObject.SetActive(false);

        playerController.enabled = true;
        pickDropScript.enabled = true;
    }

    #endregion Secuencias de Animación de Cámara
}