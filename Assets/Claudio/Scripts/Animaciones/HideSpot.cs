using UnityEngine;
using System.Collections;

public class HideSpot : MonoBehaviour
{
    [Header("Referencias del Mueble")]
    public Animator animatorCloset;
    [Tooltip("La cámara que vive DENTRO del clóset")]
    public Camera camaraDelCloset;

    [Header("Tiempos")]
    [Tooltip("Segundos que tarda en abrirse la puerta antes de regresarte el control")]
    public float tiempoEsperaSalida = 2.0f; // <--- ¡NUEVA VARIABLE!

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

        Camera camaraDelPlayer = player.cameraPivot.GetComponentInChildren<Camera>();

        // Bloqueamos al jugador
        player.canLook = false;
        player.GetComponent<CharacterController>().enabled = false;

        if (isPlayerInside)
        {
            // --- ENTRANDO ---
            camaraDelPlayer.gameObject.SetActive(false);
            camaraDelCloset.gameObject.SetActive(true);

            animatorCloset.SetTrigger(datos.triggerAnimacion);
        }
        else
        {
            // --- SALIENDO ---
            animatorCloset.SetTrigger(datos.triggerSalida);

            // ¡AQUÍ ESTÁ LA MAGIA! Ahora espera el tiempo que tú le digas en el Inspector
            yield return new WaitForSeconds(tiempoEsperaSalida);

            // Apagamos el clóset y encendemos al jugador
            camaraDelCloset.gameObject.SetActive(false);
            camaraDelPlayer.gameObject.SetActive(true);

            // Devolvemos el control físico
            player.GetComponent<CharacterController>().enabled = true;
            player.canLook = true;
        }

        if (datos != null) ActualizarEstadoLogico(player, isPlayerInside);

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