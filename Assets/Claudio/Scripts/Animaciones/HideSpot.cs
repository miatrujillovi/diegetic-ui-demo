using UnityEngine;

public class HideSpot : MonoBehaviour
{
    public HideSpotData datos;
    private bool isPlayerInside = false;

    public void Interactuar(FPSController player)
    {
        Animator anim = player.GetComponentInChildren<Animator>();

        if (!isPlayerInside)
        {
            // --- ENTRAR AL ESCONDITE ---
            isPlayerInside = true;
            anim.SetTrigger(datos.triggerAnimacion);

            // Bloqueamos el movimiento y la vista
            player.canLook = false;
            // Desactivamos el CharacterController para que no interfiera con la animación
            player.GetComponent<CharacterController>().enabled = false;
        }
        else
        {
            // --- SALIR DEL ESCONDITE ---
            isPlayerInside = false;
            anim.SetTrigger(datos.triggerSalida);

            // Devolvemos el control
            player.GetComponent<CharacterController>().enabled = true;
            player.canLook = true;
        }

        ActualizarEstadoLogico(player, isPlayerInside);
    }

    private void ActualizarEstadoLogico(FPSController player, bool estado)
    {
        if (datos.nombreEscondite == "Closet") player.isHidingCloset = estado;
        if (datos.nombreEscondite == "Baul") player.isHidingTrunk = estado;
        if (datos.nombreEscondite == "Silla") player.isOnElevatedSurface = estado;
    }
}