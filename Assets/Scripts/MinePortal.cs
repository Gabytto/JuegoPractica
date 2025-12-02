using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePortal : MonoBehaviour
{
    // Bandera que será activada por el SabioController.
    public bool isPortalActive = false;

    // Este objeto debe tener un Collider2D con la opción "Is Trigger" marcada.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPortalActive)
        {
            TeleportPlayer(other.gameObject);
        }
        else if (other.CompareTag("Player") && !isPortalActive)
        {
            Debug.Log("La mina está cerrada. El sabio debe tener la llave...");
        }
    }

    private void TeleportPlayer(GameObject playerToTeleport)
    {
        // Usa la referencia del QuestManager
        Transform destino = QuestManager.Instance.MineInteriorSpawnPoint;

        if (destino != null)
        {
            // Mueve la posición del jugador al punto de spawn dentro del laberinto
            playerToTeleport.transform.position = destino.position;
            Debug.Log("Jugador teletransportado al interior del laberinto de la mina.");
        }
        else
        {
            Debug.LogError("¡ERROR! El punto de spawn interior no está asignado en QuestManager. Por favor, asigna 'MineEntrance_InteriorSpawn' al QuestManager en el Inspector.");
        }
    }

    // Método público llamado desde el SabioController al completar la Misión 1
    public void ActivatePortal()
    {
        isPortalActive = true;
        Debug.Log("Portal de la Mina Activado y Listo.");
    }
}