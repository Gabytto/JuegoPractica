using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabioController : MonoBehaviour
{
    [Header("Indicador de Misión")]
    public GameObject IconoMision;
    public GameObject PanelInteraccion;

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E;
    private bool jugadorCerca = false;

    [Header("Objetos Afectados por la Misión")]
    public SpriteRenderer MinaSpriteRenderer;
    public Sprite MinaActivadaSprite;

    // Referencia al Collider sólido de la mina para desactivarlo
    public Collider2D MinaColliderSolido;

    [Header("El Portal (Prefab)")]
    // ESTO ES LO QUE FALTA ASIGNAR EN EL INSPECTOR
    public GameObject MinaPortalPrefab;
    public Transform MinaPortalSpawnPoint;

    // --- NUEVAS VARIABLES PARA CONTROL DE FLUJO DE DIÁLOGO ---
    // Indica el estado al que debe avanzar la misión al terminar el diálogo.
    private int siguienteEstadoAlTerminar = -1;
    // -1: No hay avance de estado. 1: Iniciar Quest 1. 3: Finalizar Quest 1.
    // ------------------------------------------------------------------

    void Start()
    {
        ActualizarIconoMision();
        ActualizarSpriteMina();
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(TeclaInteraccion))
        {
            InteractuarConSabio();
        }
    }

    private void InteractuarConSabio()
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
        int estado = QuestManager.Instance.Estado_Quest_Sabio;
        string[] mensajes = new string[] { "" };

        // Reiniciar el indicador de avance para esta interacción
        siguienteEstadoAlTerminar = -1;

        switch (estado)
        {
            case 0:
                // Diálogo de inicio de misión
                mensajes = new string[] {
                    "Te estaba esperando Guerrero. El Ermitaño consiguió la ubicación de una mina de oro muy cercana.",
                    "Esta mina se encuentra por el camino que va hacia el bosque, pero hacerse de ella no es tan sencillo.",
                    "Está custodiada por Gobblins. Acaba con ellos y tráeme 8 'Sangre de Gobblin'.",
                    "Con ello prepararé una posión, luego sabrás más sobre eso. ¡Buena suerte!"
                };

                // Marcamos que el estado debe cambiar a 1 al terminar el diálogo
                siguienteEstadoAlTerminar = 1;
                break;

            case 1:
                // Diálogo de recordatorio (Misión en curso)
                int recolectado = QuestManager.Instance.Gobblin_Blood_Count;
                int requerido = QuestManager.Gobblin_Blood_Target;
                mensajes = new string[] { $"Vuelve cuando tengas los 8 'Sangre de Gobblin'. Llevas {recolectado}/{requerido}." };
                break;

            case 2:
                // Diálogo de entrega/completado de misión
                mensajes = new string[] {
                    "¡Excelente! Sabía que esos Gobblins no serían problema para tí. Pero ahora debo pedirte otro favor.",
                };

                // Marcamos que el estado debe cambiar a 3 al terminar el diálogo
                siguienteEstadoAlTerminar = 3;
                break;

            case 3:
                // Diálogo de misión post-completado
                if (QuestManager.Instance.Estado_Quest_Ermitaño == 0)
                {
                    
                    mensajes = new string[] {
                        "El Ermitaño no ha regresado de su exploración, me temo que está atrapado en el interior de la mina de oro",
                        "Le encargué que me trajera 5 'Fluidos de Slime Azul'. Ayuda al Ermitaño a conseguirlos y dile que me los traiga.",
                        $"Busca al viejo ermitaño dentro de la mina. Recolecta el 'fluido de Slime Azul'."
                    };
                    QuestManager.Instance.Estado_Quest_Ermitaño = 1;
                    ErmitanoController ermitano = QuestManager.Instance.ErmitanoRef;
                    if (ermitano != null)
                    {
                        ermitano.ForzarActualizacion();
                    }
                }
                else
                {
                    int slime_recolectado = QuestManager.Instance.Blue_Slime_Fluid_Count;
                    int slime_requerido = QuestManager.Blue_Slime_Fluid_Target;
                    mensajes = new string[] { $"Apresúrate, el Ermitaño podría estar en problemas.Llevas {slime_recolectado}/{slime_requerido} recolectado" };
                }
                break;
        }

        // Llamamos a MostrarMultiMensaje y le pasamos ESTA instancia del SabioController
        dialogeManager.Instance.MostrarMultiMensaje(mensajes, this);
    }

    // --- NUEVO MÉTODO LLAMADO POR dialogeManager AL FINAL DEL DIÁLOGO ---
    public void FinalizarDialogoCritico()
    {
        if (siguienteEstadoAlTerminar == 1)
        {
            // Lógica para el paso de Estado 0 a Estado 1
            QuestManager.Instance.IniciarQuestSabio();
            QuestManager.Instance.Estado_Quest_Sabio = 1;
        }
        else if (siguienteEstadoAlTerminar == 3)
        {
            // Lógica para el paso de Estado 2 a Estado 3
            QuestManager.Instance.Estado_Quest_Sabio = 3;
            QuestManager.Instance.Gobblin_Blood_Count = 0;

            // --- LÓGICA VISUAL Y FÍSICA ---
            ActualizarSpriteMina();

            // 1. Desactivamos el muro sólido
            if (MinaColliderSolido != null)
            {
                MinaColliderSolido.enabled = false;
            }

            // 2. Instanciamos el portal (El trigger)
            if (MinaPortalPrefab != null && MinaPortalSpawnPoint != null)
            {
                GameObject portalGO = Instantiate(MinaPortalPrefab, MinaPortalSpawnPoint.position, Quaternion.identity);
                MinePortal portalScript = portalGO.GetComponent<MinePortal>();
                if (portalScript != null)
                {
                    portalScript.ActivatePortal();
                }
            }
            else
            {
                Debug.LogError("¡ERROR! Te olvidaste de asignar el 'MinaPortalPrefab' o el 'SpawnPoint' en el script del Sabio.");
            }
        }

        // Limpiamos la variable de avance y actualizamos el ícono
        siguienteEstadoAlTerminar = -1;
        ActualizarIconoMision();
    }
    // ------------------------------------------------------------------

    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        int estado = QuestManager.Instance.Estado_Quest_Sabio;
        IconoMision.SetActive(estado == 0 || estado == 2);
    }

    private void ActualizarSpriteMina()
    {
        if (MinaSpriteRenderer == null || MinaActivadaSprite == null) return;
        if (QuestManager.Instance.Estado_Quest_Sabio >= 3) // Cambiado a >= 3 por seguridad
        {
            MinaSpriteRenderer.sprite = MinaActivadaSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            int estado = QuestManager.Instance.Estado_Quest_Sabio;
            // Solo mostramos la interacción si estamos en un estado de diálogo (0, 2, o incluso 3 antes de la quest del Ermitaño)
            if (estado == 0 || estado == 2 || (estado == 3 && QuestManager.Instance.Estado_Quest_Ermitaño == 0))
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