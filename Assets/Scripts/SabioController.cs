using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SabioController : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public GameObject DialogoPanel; // Panel que contiene el texto de diálogo
    public TextMeshProUGUI DialogoTexto; // Componente de texto para mostrar el diálogo

    [Header("Indicador de Misión")]
    public GameObject IconoMision; // El objeto con el sprite del signo de admiración
    public GameObject PanelInteraccion;

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E; // La tecla que el jugador debe presionar
    private bool jugadorCerca = false;

    [Header("Objetos Afectados por la Misión")]
    public SpriteRenderer MinaSpriteRenderer; // Referencia al componente SpriteRenderer de la Mina
    public Sprite MinaActivadaSprite; // Sprite que se debe usar al completar la misión
    public MinePortal MinePortalController;

    void Start()
    {
        // Asegúrate de que el panel de diálogo esté oculto al inicio
        if (DialogoPanel != null)
            DialogoPanel.SetActive(false);

        ActualizarIconoMision();
        ActualizarSpriteMina();
    }

    void Update()
    {
        // Solo permite la interacción si el jugador está cerca
        if (jugadorCerca && Input.GetKeyDown(TeclaInteraccion))
        {
            InteractuarConSabio();
        }
    }

    // --- MANEJO DE LA INTERACCIÓN ---

    private void InteractuarConSabio()
    {
        // Si el diálogo está activo, lo cerramos (o podríamos avanzar el texto)
        if (DialogoPanel.activeSelf)
        {
            DialogoPanel.SetActive(false);
            return;
        }

        // Si el diálogo está inactivo, lo abrimos y establecemos el texto condicional
        DialogoPanel.SetActive(true);
        ManejarEstadosDeMision();
        ActualizarIconoMision(); // Actualizamos el icono después de la interacción
    }

    private void ManejarEstadosDeMision()
    {
        int estado = QuestManager.Instance.Estado_Quest_Sabio;

        switch (estado)
        {
            case 0: // Misión Inactiva (Primer encuentro)
                DialogoTexto.text = "Hola joven Guerrero, hay una horda de Gobblins que se apoderó de la mina de oro. Acaba con ellos y tráeme 8 'Gobblin's Blood' para estudiar de donde vienen esas creaturas.";
                QuestManager.Instance.Estado_Quest_Sabio = 1; // Misión Aceptada
                break;

            case 1: // Misión Aceptada (En curso)
                int recolectado = QuestManager.Instance.Gobblin_Blood_Count;
                int requerido = QuestManager.Gobblin_Blood_Target;
                DialogoTexto.text = $"Vuelve cuando tengas los 8 'Gobblin's blood'. Llevas {recolectado}/{requerido}. ¡Date prisa!";
                break;

            case 2: // Objetivo Cumplido (Listo para entregar)
                DialogoTexto.text = "¡Excelente! Gracias a ti, nuestro reino ha recuperado el control de la mina de oro. Además podré estudiar mejor a esas creaturas con las muestras que me haz traído.";

                // Acción de finalización:
                QuestManager.Instance.Estado_Quest_Sabio = 3; // Misión Entregada
                QuestManager.Instance.Gobblin_Blood_Count = 0; // Se consumen los items

                ActualizarSpriteMina(); // Cambia el sprite de la mina
                if (MinePortalController != null)
                {
                    MinePortalController.ActivatePortal(); // <--- ¡AQUÍ SE ACTIVA!
                }

                break;

            case 3: // Misión Entregada (Misión completada)
                // Verificamos si la Misión 2 ya fue aceptada
            if (QuestManager.Instance.Estado_Quest_Ermitaño == 0)
            {
                DialogoTexto.text = "Te has ganado mi respeto. Ahora, un nuevo desafío te espera: el conocimiento que busco está en manos de un viejo ermitaño que vive en el corazón de la mina. ¡Ve! El camino ya está abierto. Cuando lo encuentres, dile que vas de mi parte. ";
                
                // Misión 2: Aceptada
                QuestManager.Instance.Estado_Quest_Ermitaño = 1; 
            }
            else if (QuestManager.Instance.Estado_Quest_Ermitaño == 1)
            {
                DialogoTexto.text = "El ermitaño te espera. No pierdas tiempo. El camino a través de la mina está abierto.";
            }
            else // Misión 2 terminada o más avanzada
            {
                 DialogoTexto.text = "Gracias por tu servicio. Vuelve pronto si necesitas algo.";
            }
                break;
        }
    }

    // --- MANEJO DE TRIGGER Y VISUALES ---

    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;

        int estado = QuestManager.Instance.Estado_Quest_Sabio;

        // Mostrar $!$ si: 0 (para iniciar) O 2 (para entregar)
        if (estado == 0 || estado == 2)
        {
            IconoMision.SetActive(true);
        }
        else
        {
            IconoMision.SetActive(false);
        }
    }

    private void ActualizarSpriteMina()
    {
        if (MinaSpriteRenderer == null || MinaActivadaSprite == null) return;

        // El cambio solo ocurre si la misión fue entregada (Estado 3)
        if (QuestManager.Instance.Estado_Quest_Sabio == 3)
        {
            MinaSpriteRenderer.sprite = MinaActivadaSprite;
            Debug.Log("Sprite de la Mina cambiado a 'Activada'.");
        }
    }

    // Detección de Colisión (Asume que el Sabio tiene un Collider con IsTrigger activo)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            int estado = QuestManager.Instance.Estado_Quest_Sabio;

            // Mostrar el panel de interacción SOLAMENTE si el Sabio tiene una misión para dar (estado 0) 
            // o para recibir (estado 2).
            if (estado == 0 || estado == 2)
            {
                // 1. Ocultamos el signo de admiración
                if (IconoMision != null)
                    IconoMision.SetActive(false);

                // 2. Mostramos el panel de "Presioná 'E' para hablar..."
                if (PanelInteraccion != null)
                    PanelInteraccion.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // 1. Ocultamos el panel de "Presioná 'E' para hablar..."
            if (PanelInteraccion != null)
                PanelInteraccion.SetActive(false);

            // 2. Volvemos a mostrar el IconoMision, pero solo si debe estar activo 
            // (esto ya lo maneja la función ActualizarIconoMision, pero la llamamos
            // aquí para asegurar que se muestre si es necesario).
            ActualizarIconoMision();

            // También cerramos el panel de diálogo si estaba abierto al salir del trigger
            if (DialogoPanel != null)
                DialogoPanel.SetActive(false);
        }
    }
}
