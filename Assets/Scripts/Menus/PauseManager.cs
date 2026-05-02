using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    [SerializeField] private GameObject safeLight;
    [SerializeField] private AudioSource pauseAudioSource;
    [SerializeField] private GameObject ai;
    [SerializeField] private InputActionReference interactAction;

    private bool canInteract = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            safeLight.SetActive(true);
            pauseAudioSource.gameObject.SetActive(true);
            pauseAudioSource.Play();
            ai.SetActive(false);

            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            safeLight.SetActive(false);
            pauseAudioSource.gameObject.SetActive(false);
            pauseAudioSource.Pause();
            ai.SetActive(true);

            canInteract = false;
        }
    }

    private void Update()
    {
        if (!canInteract) return;

        if (interactAction.action.triggered)
        {
            Application.Quit();
        }
    }
}
