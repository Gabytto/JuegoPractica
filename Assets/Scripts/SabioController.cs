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
    public GameObject IconoMision; // El objeto con el sprite del signo de admiración ($!$)

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E; // La tecla que el jugador debe presionar
    private bool jugadorCerca = false;

    [Header("Objetos Afectados por la Misión")]
    public SpriteRenderer MinaSpriteRenderer; // Referencia al componente SpriteRenderer de la Mina
    public Sprite MinaActivadaSprite; // Sprite que se debe usar al completar la misión

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
                DialogoTexto.text = "Joven héroe, necesito **8 Gotas de Sangre de Gobblin** de la mina para una pócima. ¡Límpiame esa mina!";
                QuestManager.Instance.Estado_Quest_Sabio = 1; // Misión Aceptada
                break;

            case 1: // Misión Aceptada (En curso)
                int recolectado = QuestManager.Instance.Gobblin_Blood_Count;
                int requerido = QuestManager.Gobblin_Blood_Target;
                DialogoTexto.text = $"Vuelve cuando tengas las 8 Gotas. Llevas {recolectado}/{requerido}. ¡Date prisa!";
                break;

            case 2: // Objetivo Cumplido (Listo para entregar)
                DialogoTexto.text = "¡Excelente! Gracias a ti, la mina vuelve a ser segura y tengo mis materiales.";

                // Acción de finalización:
                QuestManager.Instance.Estado_Quest_Sabio = 3; // Misión Entregada
                QuestManager.Instance.Gobblin_Blood_Count = 0; // Se consumen los items

                ActualizarSpriteMina(); // Cambia el sprite de la mina
                break;

            case 3: // Misión Entregada (Misión completada)
                DialogoTexto.text = "Te has ganado mi respeto. Ahora, un nuevo desafío te espera...";
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
            // Opcional: Mostrar un pequeño mensaje de "Presiona [E] para hablar"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            // Opcional: Ocultar el mensaje de interacción
            if (DialogoPanel != null)
                DialogoPanel.SetActive(false);
        }
    }
}
