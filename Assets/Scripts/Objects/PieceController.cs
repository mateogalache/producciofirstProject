using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : PickUpController
{
    protected override void PickUp(PlayerMovement player)
    {
        // Obtener el componente PlayerAudio del jugador
        PlayerAudio playerAudio = player.GetComponent<PlayerAudio>();
        if (playerAudio != null)
        {
            playerAudio.PlayCollectStarSound();
        }
        else
        {
            Debug.LogWarning("PlayerAudio no encontrado en el jugador.");
        }

        // Desactivar la estrella
        gameObject.SetActive(false);
    }
}
