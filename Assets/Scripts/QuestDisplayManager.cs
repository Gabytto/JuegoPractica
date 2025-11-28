using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    [Header("Referencias de la UI")]
    // Referencia al panel principal para activar/desactivar
    public GameObject QuestPanelGameObject;

    [Header("Referencias de la UI")]
    // 1. ItemIcon: Imagen del item necesario para completar la quest
    public Image ItemIcon;

    // 2. cantidad: Contador del item de quest ej: 5/8
    public TextMeshProUGUI CantidadTexto;

    // 3. infoMision: instrucciones de la misión
    public TextMeshProUGUI InstruccionesTexto;

    [Header("Recursos de la Misión del Sabio (Quest 1)")]
    // Sprite del ítem requerido para la misión
    public Sprite goblinBloodSprite;
    // Sprite del ítem requerido para la misión 2
    public Sprite blueSlimeFluidSprite;

    void Start()
    {
        // 1. Verificar referencias
        if (QuestPanelGameObject == null || ItemIcon == null || CantidadTexto == null || InstruccionesTexto == null)
        {
            Debug.LogError("¡ERROR! Faltan referencias de UI. Revisa el QuestUIManager en el Inspector.");
            enabled = false;
            return;
        }

        // 2. Desactivar el panel al inicio
        QuestPanelGameObject.SetActive(false);
    }

    void Update()
    {
        // Usamos el Singleton de QuestManager
        if (QuestManager.Instance != null)
        {
            UpdateQuestDisplay();
        }
    }

    private void UpdateQuestDisplay()
    {
        // Prioridad 1: Misión del Sabio
        if (QuestManager.Instance.Estado_Quest_Sabio <= 2)
        {
            ManejarMisionSabio();
        }
        // Prioridad 2: Misión del Ermitaño (Solo se activa si la del Sabio está completada/entregada)
        else if (QuestManager.Instance.Estado_Quest_Sabio == 3 && QuestManager.Instance.Estado_Quest_Ermitaño <= 1)
        {
            ManejarMisionErmitano();
        }
        // Caso por defecto: Ocultar Panel si ambas están en estado final (Sabio=3, Ermitaño=2+)
        else
        {
            if (QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(false);
        }
    }
    private void ManejarMisionSabio()
    {
        int estado = QuestManager.Instance.Estado_Quest_Sabio;
        int count = QuestManager.Instance.Gobblin_Blood_Count;
        int target = QuestManager.Gobblin_Blood_Target;

        // 1. Manejo de Visibilidad del Panel
        if (estado == 1 || estado == 2)
        {
            if (!QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(true);
        }
        else
        {
            if (QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(false);
            return;
        }

        // 2. Icono, Progreso e Instrucciones
        ItemIcon.sprite = goblinBloodSprite;

        switch (estado)
        {
            case 1: // Misión Aceptada / En Curso
                CantidadTexto.text = $"{count} / {target}";
                InstruccionesTexto.text = "Objetivo: Derrota Goblins cerca de la mina de oro para recolectar 'Sangre de Gobblin'.";
                break;

            case 2: // Objetivo Cumplido, listo para entregar
                CantidadTexto.text = $"{target} / {target}";
                InstruccionesTexto.text = "¡Misión Cumplida!\nRegresa a hablar con el Sabio.";
                break;
        }
    }
    private void ManejarMisionErmitano()
    {
        int estado = QuestManager.Instance.Estado_Quest_Ermitaño;
        int count = QuestManager.Instance.Blue_Slime_Fluid_Count;
        int target = QuestManager.Blue_Slime_Fluid_Target;

        // Solo mostrar si el Ermitaño nos mandó a buscar los fluidos (estado 1)
        if (estado == 1)
        {
            if (!QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(true);

            // Establecer el nuevo ícono y contador
            ItemIcon.sprite = blueSlimeFluidSprite;
            CantidadTexto.text = $"{count} / {target}";

            string instruccion;

            if (count < target)
            {
                // Estado Inicial: El texto que solicitaste
                instruccion = "Derrota a los Slimes mientras recorres el laberinto de la mina buscando al Ermitaño (Blue Slime's Fluid).";
            }
            else
            {
                // Estado Cumplido: Regresar a hablar
                instruccion = "¡Objetivo cumplido! Regresa con el Ermitaño al final del laberinto.";
            }

            InstruccionesTexto.text = instruccion;
        }
        else
        {
            if (QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(false);
        }
    }
}
