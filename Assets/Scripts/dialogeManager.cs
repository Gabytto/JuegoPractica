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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // No es necesario DontDestroyOnLoad si es un objeto que vive solo en la escena.
            // Si el panel de diálogo persiste entre escenas, sí lo necesitarías.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Aseguramos que el panel esté inicialmente desactivado
        if (DialogoPanel != null)
        {
            DialogoPanel.SetActive(false);
        }
    }

    // Método CLAVE: Para que CUALQUIER script muestre un mensaje
    public void MostrarMensaje(string texto)
    {
        if (DialogoPanel == null || DialogoTexto == null)
        {
            Debug.LogError("Panel o Texto del Diálogo no asignados en el DialogueManager.");
            return;
        }

        // 1. Establece el texto
        DialogoTexto.text = texto;

        // 2. Activa el panel (si no está activo)
        if (!DialogoPanel.activeSelf)
        {
            DialogoPanel.SetActive(true);
        }
    }

    // Método para cerrar el panel (útil para interacciones del jugador)
    public void CerrarPanel()
    {
        if (DialogoPanel != null && DialogoPanel.activeSelf)
        {
            DialogoPanel.SetActive(false);
        }
    }
}

