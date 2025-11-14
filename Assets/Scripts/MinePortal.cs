using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePortal : MonoBehaviour
{
    // ASIGNAR EN EL INSPECTOR: El Transform del punto de spawn dentro del laberinto.
    public Transform mineInteriorSpawnPoint;

    // Bandera que será activada por el SabioController.
    public bool isPortalActive = false;

    // Este objeto debe tener un Collider2D con la opción "Is Trigger" marcada.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verifica si quien entró es el jugador (asegúrate que el jugador tenga el Tag "Player").
        // 2. Verifica si el portal ha sido activado por la misión.
        if (other.CompareTag("Player") && isPortalActive)
        {
            if (mineInteriorSpawnPoint != null)
            {
                TeleportPlayer(other.gameObject);
            }
            else
            {
                Debug.LogError("¡ERROR! 'Mine Interior Spawn Point' no está asignado en el Inspector de MinePortal.");
            }
        }
        else if (other.CompareTag("Player") && !isPortalActive)
        {
            // Opcional: Puedes mostrar un mensaje al jugador si intenta entrar antes de tiempo.
            Debug.Log("La mina está cerrada. El sabio debe tener la llave...");
        }
    }

    private void TeleportPlayer(GameObject playerToTeleport)
    {
        // Mueve la posición del jugador al punto de spawn dentro del laberinto
        playerToTeleport.transform.position = mineInteriorSpawnPoint.position;
        Debug.Log("Jugador teletransportado al interior del laberinto de la mina.");
    }

    // Método público llamado desde el SabioController al completar la Misión 1
    public void ActivatePortal()
    {
        isPortalActive = true;
        Debug.Log("Portal de la Mina Activado y Listo.");
    }
}