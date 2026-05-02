using UnityEngine;
using System.Collections;

public class HideSpot : MonoBehaviour
{
    [Header("Referencias del Mueble")]
    public Animator animatorCloset;
    [Tooltip("La cámara que vive DENTRO del clóset")]
    public Camera camaraDelCloset;

    [Header("Datos")]
    public HideSpotData datos;

    private bool isPlayerInside = false;
    private bool isAnimating = false;

    public void Interactuar(FPSController player)
    {
        if (isAnimating) return;
        StartCoroutine(RutinaCambioCamara(player));
    }

    private IEnumerator RutinaCambioCamara(FPSController player)
    {
        isAnimating = true;
        isPlayerInside = !isPlayerInside;

        // Buscamos la cámara del jugador (la que está dentro de CameraCointelner)
        Camera camaraDelPlayer = player.cameraPivot.GetComponentInChildren<Camera>();

        // 1. Bloqueamos al jugador (su cuerpo invisible se queda quieto afuera)
        player.canLook = false;
        player.GetComponent<CharacterController>().enabled = false;

        if (isPlayerInside)
        {
            // --- ENTRANDO ---
            // Apagamos los "ojos" del jugador y encendemos los del clóset
            camaraDelPlayer.gameObject.SetActive(false);
            camaraDelCloset.gameObject.SetActive(true);

            // Disparamos tu animación normal
            animatorCloset.SetTrigger(datos.triggerAnimacion);
        }
        else
        {
            // --- SALIENDO ---
            animatorCloset.SetTrigger(datos.triggerSalida);

            // Opcional: Esperar un segundo a que termine la animación de abrir la puerta
            // antes de regresarte a tu cuerpo. Ajusta este número según tu animación.
            yield return new WaitForSeconds(1f);

            // Apagamos el clóset y encendemos al jugador
            camaraDelCloset.gameObject.SetActive(false);
            camaraDelPlayer.gameObject.SetActive(true);

            // Devolvemos el control físico
            player.GetComponent<CharacterController>().enabled = true;
            player.canLook = true;
        }

        if (datos != null) ActualizarEstadoLogico(player, isPlayerInside);

        // Evitar doble clic accidental
        yield return new WaitForSeconds(0.5f);
        isAnimating = false;
    }

    private void ActualizarEstadoLogico(FPSController player, bool estado)
    {
        if (datos.nombreEscondite == "Closet") player.isHidingCloset = estado;
        if (datos.nombreEscondite == "Baul") player.isHidingTrunk = estado;
        if (datos.nombreEscondite == "Silla") player.isOnElevatedSurface = estado;
    }
}