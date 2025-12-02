using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloqueoCamino_2 : MonoBehaviour
{
    public Collider2D bloqueoFisico;
    // Mensaje solicitado por el usuario
    public string mensajePista = "Antes de seguirme adentrando en el bosque debería seguir las instrucciones del Sabio.";
    private bool mensajeMostrado = false;

    void Start()
    {
        if (bloqueoFisico == null)
        {
            Debug.LogError("¡ERROR! Falta asignar la referencia del 'bloqueoFisico' en el Inspector de " + gameObject.name);
            return;
        }
        ActualizarEstadoBloqueo();
    }

    public void ActualizarEstadoBloqueo()
    {
        // Condición para Bloqueo 2: 
        // El camino está bloqueado si la Misión de Rescate (Quest 3) NO ha iniciado.
        // Quest 3 inicia cuando QuestManager.Instance.Estado_Quest_Rescate pasa a 1.
        bool debeEstarBloqueado = QuestManager.Instance.Estado_Quest_Rescate < 1;

        bloqueoFisico.enabled = debeEstarBloqueado;
        // Solo mantenemos el script activo si está bloqueando (para poder capturar el trigger)
        this.enabled = debeEstarBloqueado;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo revisamos si está activo para el jugador y el bloqueo está activo
        if (other.CompareTag("Player") && bloqueoFisico.enabled)
        {
            // Solo mostramos el mensaje una vez por entrada
            if (!mensajeMostrado)
            {
                // Usamos MostrarMensaje, que es para diálogos simples sin avance de estado
                dialogeManager.Instance.MostrarMensaje(mensajePista);
                Debug.Log("Pista mostrada (Bloqueo Camino 2): " + mensajePista);
                mensajeMostrado = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Cierra el panel de diálogo si el jugador se aleja
            dialogeManager.Instance.CerrarPanel();

            // Permite que el mensaje se vuelva a mostrar si el jugador entra de nuevo.
            mensajeMostrado = false;
        }
    }
}
