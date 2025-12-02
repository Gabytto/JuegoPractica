using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGenerico : MonoBehaviour
{
    // Usamos un Transform para obtener la posición de un objeto en la escena.
    // Esto hace el script genérico para cualquier mina o zona.
    [Header("Punto de Destino")]
    [Tooltip("Objeto Transform que marca dónde aparecerá el jugador.")]
    public Transform destinoTeleport;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Verifica que el que entra es el Jugador (asegúrate de que el jugador tiene el tag "Player")
        if (other.CompareTag("Player"))
        {
            // Verificación de seguridad
            if (destinoTeleport == null)
            {
                Debug.LogError("¡ERROR! El punto de destino del teleport no está asignado en " + gameObject.name);
                return;
            }

            // 2. Teletransporta al jugador a la posición del objeto Transform
            other.transform.position = destinoTeleport.position;
            Debug.Log($"Jugador teletransportado a la nueva zona de salida: {destinoTeleport.name}.");
        }
    }
}
