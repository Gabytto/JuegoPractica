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

    private void ManejarEstadosDeMision()
    {
        // Usamos el estado correcto
        int estado = QuestManager.Instance.Estado_Quest_Rescate;
        string[] mensajes = new string[] { "" };
        siguienteEstadoAlTerminar = -1;

        switch (estado)
        {
            case 1: // Misión Aceptada - Rescatar Aldeana
                int llaves = QuestManager.Instance.Key_Calabozo_Count;
                int target = QuestManager.Key_Calabozo_Target;

                if (llaves >= target)
                {
                    // El jugador tiene la llave y la aldeana es liberada
                    mensajes = new string[]
                    {
                    "¡Gracias, infinitas gracias Guerrero Azul!",
                    "Creí que nunca podría salir de este calabozo."
                    };
                    // El siguiente estado es "Rescatada" (Estado 2)
                    siguienteEstadoAlTerminar = 2;
                }
                
                break;

        }
        dialogeManager.Instance.MostrarMultiMensaje(mensajes, this);
    }

    public void FinalizarDialogoCritico()
    {
        // Paso de Estado 1 (Aldeana Atrapada, llave obtenida) -> Estado 2 (Rescatada)
        // El 'siguienteEstadoAlTerminar' se estableció en 2 en ManejarEstadosDeMision() 
        // cuando el jugador tiene la llave.
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

        // NOTA: No es necesario llamar a ActualizarIconoMision() aquí si la escena va a cambiar inmediatamente.
    }
    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        // Usamos el estado de la misión de rescate (Aldeana)
        int estado = QuestManager.Instance.Estado_Quest_Rescate;

        // Muestra el ícono si la misión está aceptada (1) o si está pendiente de entrega (si tuvieras estado 1.5/2)
        // Para la lógica actual (llave): Muestra el icono en Estado 1 (Buscando llave)
        IconoMision.SetActive(estado == 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            // Obtenemos el estado de la Misión de Rescate (Quest 3)
            int estadoRescate = QuestManager.Instance.Estado_Quest_Rescate;

            // Solo es interactuable si la misión de Rescate está activa (Estado 1) 
            // o si fue recién completada (Estado 2, por si hay un diálogo final).
            if (estadoRescate == 1 || estadoRescate == 2)
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
