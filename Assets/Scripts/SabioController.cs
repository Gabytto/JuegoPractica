using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SabioController : MonoBehaviour
{
    [Header("Configuración del Diálogo")]
    public GameObject DialogoPanel;
    public TextMeshProUGUI DialogoTexto;

    [Header("Indicador de Misión")]
    public GameObject IconoMision;
    public GameObject PanelInteraccion;

    [Header("Configuración de la Interacción")]
    public KeyCode TeclaInteraccion = KeyCode.E;
    private bool jugadorCerca = false;

    [Header("Objetos Afectados por la Misión")]
    public SpriteRenderer MinaSpriteRenderer;
    public Sprite MinaActivadaSprite;

    // NUEVO: Referencia al Collider sólido de la mina para desactivarlo
    public Collider2D MinaColliderSolido;

    [Header("El Portal (Prefab)")]
    // ESTO ES LO QUE FALTA ASIGNAR EN EL INSPECTOR
    public GameObject MinaPortalPrefab;
    public Transform MinaPortalSpawnPoint;

    void Start()
    {
        if (DialogoPanel != null) DialogoPanel.SetActive(false);
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
        if (DialogoPanel.activeSelf)
        {
            DialogoPanel.SetActive(false);
            return;
        }

        DialogoPanel.SetActive(true);
        ManejarEstadosDeMision();
        ActualizarIconoMision();
    }

    private void ManejarEstadosDeMision()
    {
        int estado = QuestManager.Instance.Estado_Quest_Sabio;

        switch (estado)
        {
            case 0:
                DialogoTexto.text = "Hola joven Guerrero, hay una horda de Gobblins que se apoderó de la mina de oro. Acaba con ellos y tráeme 8 'Gobblin's Blood'.";
                QuestManager.Instance.Estado_Quest_Sabio = 1;
                break;

            case 1:
                int recolectado = QuestManager.Instance.Gobblin_Blood_Count;
                int requerido = QuestManager.Gobblin_Blood_Target;
                DialogoTexto.text = $"Vuelve cuando tengas los 8 'Gobblin's blood'. Llevas {recolectado}/{requerido}.";
                break;

            case 2:
                DialogoTexto.text = "¡Excelente! Gracias a ti, nuestro reino ha recuperado el control de la mina.";

                // Completar misión
                QuestManager.Instance.Estado_Quest_Sabio = 3;
                QuestManager.Instance.Gobblin_Blood_Count = 0;

                // --- LÓGICA VISUAL Y FÍSICA ---
                ActualizarSpriteMina();

                // 1. Desactivamos el muro sólido para que puedas pasar
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
                break;

            case 3:
                if (QuestManager.Instance.Estado_Quest_Ermitaño == 0)
                {
                    DialogoTexto.text = "Busca al viejo ermitaño dentro de la mina. Dile que vas de mi parte.";
                    QuestManager.Instance.Estado_Quest_Ermitaño = 1;
                }
                else
                {
                    DialogoTexto.text = "El camino a través de la mina está abierto.";
                }
                break;
        }
    }

    private void ActualizarIconoMision()
    {
        if (IconoMision == null) return;
        int estado = QuestManager.Instance.Estado_Quest_Sabio;
        IconoMision.SetActive(estado == 0 || estado == 2);
    }

    private void ActualizarSpriteMina()
    {
        if (MinaSpriteRenderer == null || MinaActivadaSprite == null) return;
        if (QuestManager.Instance.Estado_Quest_Sabio == 3)
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
            if (estado == 0 || estado == 2)
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
            if (DialogoPanel != null) DialogoPanel.SetActive(false);
        }
    }
}
