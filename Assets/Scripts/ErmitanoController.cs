using UnityEngine;

public class ErmitanoController : MonoBehaviour
{
    [Header("Indicador de Misión")]
    public GameObject IconoMision;
    public GameObject PanelInteraccion; // Panel "Pulsa 'E' para hablar"

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E;
    private bool jugadorCerca = false;

    void Start()
    {
        ActualizarIconoMision();
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(TeclaInteraccion))
        {
            InteractuarConErmitano();
        }
    }

    private void InteractuarConErmitano()
    {
        // 1. Cierra el panel de diálogo si ya está abierto (diálogo anterior)
        if (dialogeManager.Instance.DialogoPanel.activeSelf)
        {
            dialogeManager.Instance.CerrarPanel();
            return;
        }

        // 2. Maneja el estado de la misión
        ManejarEstadosDeMision();
        ActualizarIconoMision();
        // Llamamos al gestor de UI para actualizar el panel de Quest
        QuestManager.Instance.ActualizarUI_QuestErmitano();
    }

    private void ManejarEstadosDeMision()
    {
        int estado = QuestManager.Instance.Estado_Quest_Ermitaño;
        string mensaje = "";

        switch (estado)
        {
            case 0:
                // Estado 0: El jugador lo encuentra antes de que el Sabio le dé la misión.
                mensaje = "No sé quién eres, pero estoy atrapado. Deberías volver al Sabio del pueblo.";
                break;

            case 1:
                int fluidos = QuestManager.Instance.Blue_Slime_Fluid_Count;
                int target = QuestManager.Blue_Slime_Fluid_Target;

                if (fluidos >= target)
                {
                    // Objetivo cumplido y listo para entregar
                    mensaje = "¡Maravilloso! Justo lo que necesito. ¡Muchas gracias por limpiar el camino y conseguirme estos fluidos!";

                    // Completar Misión 2
                    QuestManager.Instance.Estado_Quest_Ermitaño = 2; // Misión 2 Completada/Entregada

                    // INICIAR MISIÓN 3: Rescate de la Aldeana
                    QuestManager.Instance.Estado_Quest_Rescate = 1; // 1 = Misión Aceptada

                    // Mensaje de la nueva Misión 3
                    mensaje += "\nAhora que el camino al bosque está despejado, debes saber que en la mina del bosque hay una aldeana secuestrada por guerreros de armadura negra. Por favor, ve a rescatarla. ¡El reino te necesita!";
                }
                else
                {
                    // Misión aceptada, pero faltan ítems
                    mensaje = $"¡Gracias por venir! Pero no puedo irme sin los {target} 'Blue Slime's Fluid'. Llevas {fluidos}/{target}. ¿Los tienes?";
                }
                break;

            case 2:
                // Misión 2 completada, Misión 3 activa
                mensaje = "Ve por el camino del bosque y busca la mina. ¡Ten cuidado con los Caballeros Negros!";
                break;
        }

        // Muestra el mensaje usando el gestor central
        dialogeManager.Instance.MostrarMensaje(mensaje);
    }

    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        int estado = QuestManager.Instance.Estado_Quest_Ermitaño;

        // El icono aparece si la misión está en estado 1 (buscarlo)
        // y desaparece al hablar (estado 2)
        IconoMision.SetActive(estado == 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (PanelInteraccion != null) PanelInteraccion.SetActive(true);
            if (IconoMision != null) IconoMision.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (PanelInteraccion != null) PanelInteraccion.SetActive(false);
            ActualizarIconoMision(); // Vuelve a mostrar el ícono si la misión sigue activa
            dialogeManager.Instance.CerrarPanel(); // Cierra el panel al alejarse
        }
    }
    public void ForzarActualizacion()
    {
        ActualizarIconoMision();
    }
}