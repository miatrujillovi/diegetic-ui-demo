using UnityEngine;
using System.Collections;

public class DemonState : IState
{
    private EnemyBrain brain;
    private float eventTimer = 0f;
    private bool eventActive = false;
    private int currentEvent = 0;

    private float gracePeriod = 3.0f; // Los 3 segundos después del audio
    private float timeBetweenEvents = 30.0f;

    public DemonState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void Enter()
    {
        if (brain.playerScript == null)
        {
            Debug.LogError("DemonState: No hay PlayerScript asignado en el Brain.");
        }
        ResetTimer();
    }

    public void Execute()
    {
        if (!eventActive)
        {
            eventTimer -= Time.deltaTime;
            if (eventTimer <= 0)
            {
                TriggerRandomEvent();
            }
        }
    }

    private void TriggerRandomEvent()
    {
        eventActive = true;
        currentEvent = Random.Range(0, 3);

        AudioData dataToPlay = null;
        Vector3 targetPosition = Vector3.zero;

        // 1. Selección de datos modulares y posición
        switch (currentEvent)
        {
            case 0: // VENTANA
                dataToPlay = brain.dataVentana;
                targetPosition = brain.posVentana.position;
                break;
            case 1: // PUERTA
                dataToPlay = brain.dataPuerta;
                targetPosition = brain.posPuerta.position;
                break;
            case 2: // CAMA
                dataToPlay = brain.dataCama;
                targetPosition = brain.posCama.position;
                break;
        }

        if (dataToPlay != null)
        {
            // 2. Obtenemos un clip aleatorio del AudioData para calcular el tiempo exacto
            AudioClip selectedClip = dataToPlay.GetRandomClip();

            if (selectedClip != null)
            {
                // 3. Reproducción a través del AudioManager modular
                AudioManager.Instance.PlaySound3D(dataToPlay, targetPosition);

                // 4. Iniciamos la espera: Duración del clip específico + Tiempo de gracia
                float totalWaitTime = selectedClip.length + gracePeriod;
                brain.StartCoroutine(CheckPlayerSafety(totalWaitTime));

                Debug.Log($"Evento {currentEvent} ({dataToPlay.name}) iniciado. Esperando {totalWaitTime}s.");
            }
        }
    }

    private IEnumerator CheckPlayerSafety(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        bool isSafe = false;

        // 5. Verificación de los estados del FPSController
        if (currentEvent == 0 && brain.playerScript.isHidingCloset) isSafe = true;
        else if (currentEvent == 1 && brain.playerScript.isHidingTrunk) isSafe = true;
        else if (currentEvent == 2 && brain.playerScript.isOnElevatedSurface) isSafe = true;

        if (isSafe)
        {
            Debug.Log("Sobreviviste al ataque. El demonio se retira.");
            eventActive = false;
            ResetTimer();
        }
        else
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("EL MONSTRUO TE ATRAPÓ");
        if (brain.playerScript != null)
        {
            // Desactiva al jugador de la escena
            brain.playerScript.gameObject.SetActive(false);
        }
    }

    private void ResetTimer()
    {
        eventTimer = timeBetweenEvents;
    }

    public void Exit() { }
}