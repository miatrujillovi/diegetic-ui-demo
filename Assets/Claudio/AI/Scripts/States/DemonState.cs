using UnityEngine;
using System.Collections;

public class DemonState : IState
{
    private EnemyBrain brain;
    private float eventTimer = 0f;
    private bool eventActive = false;
    private int currentEvent = 0;

    private float gracePeriod = 3.0f; 
    private float timeBetweenEvents = 20.0f; // Modificado para que tengas respiro

    // NUEVA VARIABLE: Para recordar en que puerta/ventana estaba el monstruo
    private Vector3 ultimaPosicionAtaque; 

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
            AudioClip selectedClip = dataToPlay.GetRandomClip();

            if (selectedClip != null)
            {
                // Guardamos la posición del ataque en nuestra nueva variable
                ultimaPosicionAtaque = targetPosition; 

                AudioManager.Instance.PlaySound3D(dataToPlay, targetPosition);

                float totalWaitTime = selectedClip.length + gracePeriod;
                brain.StartCoroutine(CheckPlayerSafety(totalWaitTime));

                Debug.Log($"Evento {currentEvent} ({dataToPlay.name}) iniciado. Esperando {totalWaitTime}s.");
            }
        }
    }

    private IEnumerator CheckPlayerSafety(float waitTime)
    {
        // AQUI ES DONDE TRANSCURRE EL TIEMPO DE ATAQUE
        yield return new WaitForSeconds(waitTime);

        bool isSafe = false;

        if (currentEvent == 0 && brain.playerScript.isHidingCloset) isSafe = true;
        else if (currentEvent == 1 && brain.playerScript.isHidingTrunk) isSafe = true;
        else if (currentEvent == 2 && brain.playerScript.isOnElevatedSurface) isSafe = true;

        if (isSafe)
        {
            Debug.Log("Sobreviviste al ataque. El demonio se retira.");

            // >>> NUEVO: REPRODUCIR SONIDO AL SOBREVIVIR <<<
            if (brain.dataRetirada != null)
            {
                // Reproduce el sonido de que se va, exactamente en la ventana o puerta donde atacó
                AudioManager.Instance.PlaySound3D(brain.dataRetirada, ultimaPosicionAtaque);
            }

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
            brain.loseTimeLine.gameObject.SetActive(true);
            brain.Pluche.gameObject.SetActive(true);
        }
    }

    private void ResetTimer()
    {
        eventTimer = timeBetweenEvents;
    }

    public void Exit() { }
}