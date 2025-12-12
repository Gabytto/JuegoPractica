using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AldeanaController : MonoBehaviour, IDialogoCritico
{
    [Header("Indicador de Misión")]
    public GameObject IconoMision;
    public GameObject PanelInteraccion;

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E;
    private bool jugadorCerca = false;

    // --- NUEVAS VARIABLES PARA CONTROL DE FLUJO DE DIÁLOGO ---
    private int siguienteEstadoAlTerminar = -1;
    void Start()
    {
        ActualizarIconoMision();
    }

    // Update is called once per frame
    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(TeclaInteraccion))
        {
            InteractuarConAldeana();
        }

    }

    private void InteractuarConAldeana()
    {
        // CAMBIO: Si el diálogo está activo, avanzamos. Si no, iniciamos un nuevo diálogo.
        if (dialogeManager.Instance.DialogoActivo)
        {
            dialogeManager.Instance.AvanzarDialogo();
            return;
        }
        ManejarEstadosDeMision();
        ActualizarIconoMision();
    }

    // *** FUNCIÓN CLAVE MODIFICADA ***
    private void ManejarEstadosDeMision()
    {
        // Usamos el estado correcto
        int estado = QuestManager.Instance.Estado_Quest_Rescate;
        string[] mensajes = new string[] { "" };
        siguienteEstadoAlTerminar = -1;

        switch (estado)
        {
            case 1: // Misión Aceptada - Matar Knights (Aldeana encerrada)
            case 10: // Knights derrotados - Matar Minotauro (Aldeana encerrada)
                mensajes = new string[]
                {
                    "¡Ayuda! ¡Estoy encerrada! ¡Alguien, por favor, detenga al monstruo de la entrada!",
                    "(La puerta está cerrada con llave...)",
                };
                break;

            case 11: // NUEVO ESTADO: Llave obtenida - Rescatar Aldeana
                int llaves = QuestManager.Instance.Key_Calabozo_Count;
                int target = QuestManager.Key_Calabozo_Target;

                if (llaves >= target)
                {
                    // El jugador tiene la llave y la aldeana es liberada
                    mensajes = new string[]
                    {
                    "¡Gracias por salvarme Guerrero Azul! Creí que me quedaría encerrada por siempre en este calabozo",
                    "Ahora podré volver al pueblo en paz y tú podrás seguir explorando el bosque.",
                    "¡Mucha suerte en tu camino!"
                    };
                    // El siguiente estado es "Rescatada" (Estado 2)
                    siguienteEstadoAlTerminar = 2;
                }
                break;

            case 2: // Aldeana Rescatada (Diálogo de agradecimiento final o nulo)
                mensajes = new string[]
                {
                    "Por favor, vuelve a casa, ya estoy libre."
                };
                break;

        }
        dialogeManager.Instance.MostrarMultiMensaje(mensajes, this);
    }

    public void FinalizarDialogoCritico()
    {
        // Paso de Estado 11 (Aldeana Atrapada, llave obtenida) -> Estado 2 (Rescatada)
        if (siguienteEstadoAlTerminar == 2)
        {
            // 1. Marcar la misión como completada/avanzada
            QuestManager.Instance.Estado_Quest_Rescate = 2;

            // 2. LÓGICA DE FIN DE DEMO / CRÉDITOS
            Debug.Log("Misión de Rescate completada. FIN DE LA DEMO. Redirigiendo a créditos.");
            SceneManager.LoadScene("creditos");
        }

        // Limpiamos la variable de avance
        siguienteEstadoAlTerminar = -1;
    }

    // *** FUNCIÓN CLAVE MODIFICADA ***
    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        // El ícono solo se muestra cuando el jugador ya tiene la llave (Estado 11)
        int estado = QuestManager.Instance.Estado_Quest_Rescate;
        IconoMision.SetActive(estado == 11);
    }

    // *** FUNCIÓN CLAVE MODIFICADA ***
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            int estadoRescate = QuestManager.Instance.Estado_Quest_Rescate;

            // Es interactuable si la misión está ACEPTADA (1, 10, 11) o si ya está en Estado 2
            if (estadoRescate == 1 || estadoRescate == 10 || estadoRescate == 11 || estadoRescate == 2)
            {
                if (IconoMision != null) IconoMision.SetActive(false);
                if (PanelInteraccion != null) PanelInteraccion.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (PanelInteraccion != null) PanelInteraccion.SetActive(false);
            ActualizarIconoMision();

            // Si el jugador se va, cerramos el panel y reiniciamos el estado de avance.
            dialogeManager.Instance.CerrarPanel();
            siguienteEstadoAlTerminar = -1;
        }
    }
}