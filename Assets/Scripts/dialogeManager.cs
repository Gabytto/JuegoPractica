using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dialogeManager : MonoBehaviour
{
    // Singleton (acceso global)
    public static dialogeManager Instance;

    [Header("Referencias del Panel")]
    public GameObject DialogoPanel;
    public TextMeshProUGUI DialogoTexto;

    private Queue<string> colaDeMensajes = new Queue<string>();
    public bool DialogoActivo { get { return DialogoPanel != null && DialogoPanel.activeSelf; } }
    private SabioController scriptDeAvance = null; // Referencia del emisor

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (DialogoPanel != null)
        {
            DialogoPanel.SetActive(false);
        }
    }

    // Función para iniciar un diálogo con múltiples frases y recibir la referencia del emisor
    public void MostrarMultiMensaje(string[] mensajes, SabioController emisor = null)
    {
        if (DialogoActivo) return;

        colaDeMensajes.Clear();
        this.scriptDeAvance = emisor;

        foreach (string mensaje in mensajes)
        {
            colaDeMensajes.Enqueue(mensaje);
        }

        AvanzarDialogo();
    }

    // Función llamada al pulsar la tecla 'E'
    public void AvanzarDialogo()
    {
        if (DialogoPanel == null || DialogoTexto == null)
        {
            Debug.LogError("Panel o Texto del Diálogo no asignados.");
            return;
        }

        if (colaDeMensajes.Count > 0)
        {
            // Hay más mensajes
            string mensajeActual = colaDeMensajes.Dequeue();
            DialogoTexto.text = mensajeActual;

            if (!DialogoPanel.activeSelf)
            {
                DialogoPanel.SetActive(true);
            }
        }
        else
        {
            // Cola vacía: Es el final del diálogo.

            // 1. ¡CORRECCIÓN! Notificar al Sabio ANTES de cerrar el panel
            if (scriptDeAvance != null)
            {
                scriptDeAvance.FinalizarDialogoCritico();
            }

            // 2. Cerrar el panel (esto limpiará la referencia 'scriptDeAvance' internamente)
            CerrarPanel();
        }
    }

    public void MostrarMensaje(string texto)
    {
        MostrarMultiMensaje(new string[] { texto });
    }

    // Método para cerrar el panel
    public void CerrarPanel()
    {
        if (DialogoPanel != null && DialogoPanel.activeSelf)
        {
            DialogoPanel.SetActive(false);
            colaDeMensajes.Clear();

            // Limpiamos la referencia del Sabio (para evitar el avance si se cierra por salir del área)
            if (scriptDeAvance != null)
            {
                scriptDeAvance = null;
            }
        }
    }
}