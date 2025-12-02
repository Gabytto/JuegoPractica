using System.Collections.Generic;
using UnityEngine;

public class ErmitanoController : MonoBehaviour, IDialogoCritico
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
        // 1. SI EL DIÁLOGO ESTÁ ACTIVO: AVANZAR AL SIGUIENTE MENSAJE
        if (dialogeManager.Instance.DialogoActivo)
        {
            dialogeManager.Instance.AvanzarDialogo();
            return;
        }

        // 2. SI EL DIÁLOGO NO ESTÁ ACTIVO: INICIAR EL DIÁLOGO
        ManejarEstadosDeMision();
        ActualizarIconoMision();
        // Llamamos al gestor de UI para actualizar el panel de Quest
        QuestManager.Instance.ActualizarUI_QuestErmitano();
    }

    private void ManejarEstadosDeMision()
    {
        int estado = QuestManager.Instance.Estado_Quest_Ermitaño;
        string[] mensajes = new string[] { "" };

        switch (estado)
        {
            case 0:
                // Estado 0: El jugador lo encuentra antes de que el Sabio le dé la misión.
                mensajes = new string[] {
                    "No sé quién eres, pero estoy atrapado. Deberías volver al Sabio del pueblo.",
                };
                break;

            case 1:
                int fluidos = QuestManager.Instance.Blue_Slime_Fluid_Count;
                int target = QuestManager.Blue_Slime_Fluid_Target;

                if (fluidos >= target)
                {
                    // Objetivo cumplido y listo para entregar

                    // Mensajes de entrega de Quest 2
                    string[] mensajes1 = new string[] {
                        "¡Oh, muchas gracias! Vine a por unos fluidos de Slime Azul, pero esos Slimes pueden ser realmente aterradores",
                        "¿Haz juntado los fluidos de Slime Azul por mí?. ¡Vaya, hoy estoy de suerte! Yo se los llevaré al Sabio, pero debo pedirte un favor.",
                    };


                    // Mensajes de inicio de Quest 3 (Rescate)
                    string[] mensajes2 = new string[] {
                        "Si continúas por el camino del bosque te encontrarás con la siguiente mina de oro. ¡Pero cuidado! Unos Guerreros de armadura negra la custodian.",
                        "Además pude ver como se llevaron a una Aldeana a su interior, me temo que la tienen de prisionera. ¡Rescátala por favor!"
                    };

                    // Juntamos ambos sets de mensajes para que sean una sola conversación
                    List<string> listaMensajes = new List<string>(mensajes1);
                    listaMensajes.AddRange(mensajes2);
                    mensajes = listaMensajes.ToArray();

                    // IMPORTANTE: El avance de estado se realiza en FinalizarDialogoCritico()
                }
                else
                {
                    // Misión aceptada, pero faltan ítems
                    mensajes = new string[] { $"¡Gracias por venir! Pero no puedo irme sin los {target} 'Blue Slime's Fluid'. Llevas {fluidos}/{target}. ¿Podrías juntar el resto y traermelos??" };
                }
                break;

            case 2:
                // Misión 2 completada, Misión 3 activa
                mensajes = new string[] { "Ve por el camino del bosque y busca la mina y rescata la Aldeana. ¡Ten cuidado con los Caballeros Negros!" };
                break;
        }

        dialogeManager.Instance.MostrarMultiMensaje(mensajes, this);
    }

    // Método llamado por dialogeManager cuando la conversación termina.
    public void FinalizarDialogoCritico()
    {
        // Solo avanzamos de estado si se cumplen las condiciones de entrega de misión.
        int fluidos = QuestManager.Instance.Blue_Slime_Fluid_Count;
        int target = QuestManager.Blue_Slime_Fluid_Target;

        // Condición para avanzar: Estamos en estado 1 y tenemos los ítems requeridos
        if (QuestManager.Instance.Estado_Quest_Ermitaño == 1 && fluidos >= target)
        {
            // 1. Completar Misión 2 (entrega de fluidos)
            QuestManager.Instance.Estado_Quest_Ermitaño = 2;
            QuestManager.Instance.Blue_Slime_Fluid_Count = 0; // Se consumen los ítems

            // 2. Iniciar Misión 3 (rescate de la aldeana)
            QuestManager.Instance.Estado_Quest_Rescate = 1;

            if (QuestManager.Instance.BloqueoCamino2Ref != null)
            {
                QuestManager.Instance.BloqueoCamino2Ref.ActualizarEstadoBloqueo();
            }

            // 3. Actualizar la interfaz (Quest UI) y el ícono
            ActualizarIconoMision();
            QuestManager.Instance.ActualizarUI_QuestErmitano();
        }
    }

    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        int estado = QuestManager.Instance.Estado_Quest_Ermitaño;

        // El icono aparece si la misión está en estado 1 (buscarlo/entregarlo)
        IconoMision.SetActive(estado == 1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (PanelInteraccion != null) PanelInteraccion.SetActive(true);

            // Ocultamos el ícono de misión cuando el jugador está cerca e interactuando
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