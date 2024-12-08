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

        // Actualizar el contador de estrellas en la UI
        if (StarUIManager.Instance != null)
        {
            Vector3 starWorldPosition = transform.position;
            StarUIManager.Instance.AddStar(starWorldPosition);
        }
        else
        {
            Debug.LogWarning("StarUIManager no está presente en la escena.");
        }

        // Desactivar la estrella
        gameObject.SetActive(false);
    }
}
