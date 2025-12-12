using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioZoneController : MonoBehaviour
{
    // Asegúrate de que este campo esté enlazado en el Inspector
    public AudioSource ambienteSource;

    // **CAMBIO CLAVE:** Usamos OnTriggerEnter2D y Collider2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Comprueba si el objeto que entró es el jugador con la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador ENTRO en la Zona 2D."); // Mensaje de prueba
            if (ambienteSource != null)
            {
                ambienteSource.Play();
            }
        }
    }

    // **CAMBIO CLAVE:** Usamos OnTriggerExit2D y Collider2D
    private void OnTriggerExit2D(Collider2D other)
    {
        // Comprueba si el objeto que salió es el jugador con la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador SALIO de la Zona 2D."); // Mensaje de prueba
            if (ambienteSource != null)
            {
                ambienteSource.Stop();
            }
        }
    }
}
