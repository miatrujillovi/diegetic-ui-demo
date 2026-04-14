using UnityEngine;
using System.Collections;

public class DemonState : IState
{
    private EnemyBrain brain;
    private float eventTimer = 0f;
    private bool eventActive = false;
    private int currentEvent = 0;

    private float gracePeriod = 3.0f; // Los 3 segundos después del audio
    private float timeBetweenEvents = 5.0f;

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
        AudioClip clipToPlay = null;
        Transform posToPlay = null;

        // 1. Asignamos el clip y la posición según el evento
        switch (currentEvent)
        {
            case 0:
                clipToPlay = brain.clipVentana;
                posToPlay = brain.posVentana;
                break;
            case 1:
                clipToPlay = brain.clipPuerta;
                posToPlay = brain.posPuerta;
                break;
            case 2:
                clipToPlay = brain.clipCama;
                posToPlay = brain.posCama;
                break;
        }

        if (clipToPlay != null && posToPlay != null)
        {
            // 2. Reproducimos el sonido
            AudioSource.PlayClipAtPoint(clipToPlay, posToPlay.position);

            // 3. Iniciamos la espera: Duración del audio + Gracia
            float totalWaitTime = clipToPlay.length + gracePeriod;
            brain.StartCoroutine(CheckPlayerSafety(totalWaitTime));

            Debug.Log($"Evento {currentEvent} iniciado. Esperando {totalWaitTime} segundos.");
        }
    }

    private IEnumerator CheckPlayerSafety(float waitTime)
    {
        // Espera el tiempo total (audio + 3 segundos)
        yield return new WaitForSeconds(waitTime);

        bool isSafe = false;

        // 4. Chequeo de seguridad
        if (currentEvent == 0 && brain.playerScript.isHidingCloset) isSafe = true;
        else if (currentEvent == 1 && brain.playerScript.isHidingTrunk) isSafe = true;
        else if (currentEvent == 2 && brain.playerScript.isOnElevatedSurface) isSafe = true;

        if (isSafe)
        {
            Debug.Log("Sobreviviste al ataque.");
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
            brain.playerScript.gameObject.SetActive(false);
        }
    }

    private void ResetTimer()
    {
        eventTimer = timeBetweenEvents;
    }

    public void Exit() { }
}