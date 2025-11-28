using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloqueoCamino_1 : MonoBehaviour
{
    public Collider2D bloqueoFisico;
    public string mensajePista = "El rey me dijo que hablara con el Sabio que se encuentra afuera del castillo. Antes de abandonar el pueblo debería hablar con él.";
    private bool mensajeMostrado = false;

    // Start is called before the first frame update
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
        bool debeEstarBloqueado = QuestManager.Instance.Estado_Quest_Sabio < 1;
        bloqueoFisico.enabled = debeEstarBloqueado;
        this.enabled = debeEstarBloqueado;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (QuestManager.Instance.Estado_Quest_Sabio == 0 && !mensajeMostrado)
            {
                dialogeManager.Instance.MostrarMensaje(mensajePista);
                Debug.Log("Pista mostrada: " + mensajePista);
                mensajeMostrado = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica que el que sale es el jugador
        if (other.CompareTag("Player"))
        {
            // 1. Cierra el panel de diálogo centralizado
            dialogeManager.Instance.CerrarPanel();

            // 2. Permite que el mensaje se vuelva a mostrar si el jugador entra de nuevo.
            mensajeMostrado = false;
        }
    }
}
