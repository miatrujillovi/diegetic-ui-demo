using UnityEngine;

public class CommunicationSystem : MonoBehaviour
{
    private EnemyBrain brain;

    [SerializeField] private AudioClip alertClip;
    [SerializeField] [Range(0f, 1f)] private float alertVolume = 1f;
    private AudioSource audioSource;

    void Awake()
    {
        brain = GetComponent<EnemyBrain>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
    }

    void OnEnable()
    {
        AlertSystem.OnAlertRaised += OnAlertReceived;
    }

    void OnDisable()
    {
        AlertSystem.OnAlertRaised -= OnAlertReceived;
    }

    void OnAlertReceived(Transform source, AlertLevel level)
    {
        if (brain == null || brain.alert == null) return;

        float distance = Vector3.Distance(transform.position, source.position);

        if (distance < 10f)
        {
            // Reproducir sonido cuando se vaya a subir a combate (evita repetir si ya está en combate)
            bool willUpgradeToCombat = level == AlertLevel.Combat && brain.alert.currentLevel < AlertLevel.Combat;
            if (willUpgradeToCombat && alertClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(alertClip, alertVolume);
            }

            brain.alert.SetAlertLevel(level);

            // Si es alerta de combate, propagar la posición del jugador y forzar chase inmediatamente
            if (level == AlertLevel.Combat)
            {
                if (brain.player != null && brain.memory != null)
                {
                    brain.memory.SetLastKnownPosition(brain.player.position);
                }

                if (brain.stateMachine != null)
                {
                    // Cambiar directamente a ChaseState para que empiecen a seguir al jugador
                    brain.stateMachine.ChangeState(new ChaseState(brain));
                }
            }
        }
    }
}