using UnityEngine;

public class HideSpot : MonoBehaviour
{
    public HideSpotData datos;
    private bool isPlayerInside = false;

    public void Interactuar(FPSController player)
    {
        Animator anim = player.GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError("No se encontró Animator en el jugador.");
            return;
        }

        if (!isPlayerInside)
        {
            // ENTRAR
            isPlayerInside = true;
            anim.SetTrigger(datos.triggerAnimacion);

            // Bloqueamos movimiento manual para que la animación tome el control
            player.canLook = false;
            // Podrías desactivar el CharacterController aquí si la animación mueve la raíz
        }
        else
        {
            // SALIR
            isPlayerInside = false;
            anim.SetTrigger(datos.triggerSalida);
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