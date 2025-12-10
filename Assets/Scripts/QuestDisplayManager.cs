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

    [Header("Referencias de la UI General")]
    // 1. ItemIcon: Imagen del item necesario para completar la quest
    public Image ItemIcon;

    // 2. cantidad: Contador del item de quest ej: 5/8
    public TextMeshProUGUI CantidadTexto;

    // 3. infoMision: instrucciones de la misión
    public TextMeshProUGUI InstruccionesTexto;

    [Header("Recursos de la Misión del Sabio (Quest 1)")]
    // Sprite del ítem requerido para la misión
    public Sprite goblinBloodSprite;

    [Header("Recursos de la Misión del Ermitaño (Quest 2)")]
    // Sprite del ítem requerido para la misión 2
    public Sprite blueSlimeFluidSprite;

    [Header("Recursos de la Misión de Rescate (Quest 3)")]
    // Sprite del enemigo a derrotar requerido para la misión 3 (Knight)
    public Sprite knightKillSprite;

    [Header("Recursos de la Misión de Rescate (Minotauro)")]
    public Sprite MinotaurSprite; // <-- ¡Ya está declarada correctamente!

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
        // El orden de prioridad debe estar alineado con la progresión del juego.
        // Asumo que la Misión de Rescate (Quest 3) tiene mayor prioridad de visualización
        // que la del Sabio (Quest 1) o la del Ermitaño (Quest 2), o que se activa en un punto
        // donde las otras ya están cumplidas.

        // Reajustando el orden de prioridad:

        // 1. Misión de Rescate (Si está activa)
        if (QuestManager.Instance.Estado_Quest_Rescate >= 1 && QuestManager.Instance.Estado_Quest_Rescate <= 11)
        {
            ManejarMisionRescate();
        }
        // 2. Misión del Sabio (Si está activa y no hay Rescate)
        else if (QuestManager.Instance.Estado_Quest_Sabio <= 2)
        {
            ManejarMisionSabio();
        }
        // 3. Misión del Ermitaño (Si la del Sabio se entregó y esta está activa)
        else if (QuestManager.Instance.Estado_Quest_Sabio == 3 && QuestManager.Instance.Estado_Quest_Ermitaño <= 1)
        {
            ManejarMisionErmitano();
        }
        // Caso por defecto: Ocultar Panel
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
                instruccion = "Derrota a los Slimes mientras recorres el laberinto de la mina buscando al Ermitaño (Fluido de Slime Azul).";
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

    // *** FUNCIÓN CLAVE CORREGIDA ***
    private void ManejarMisionRescate()
    {
        int estado = QuestManager.Instance.Estado_Quest_Rescate;
        int count = QuestManager.Instance.Knight_Kill_Count;
        int target = QuestManager.Knight_Kill_Target; // Target de Knights

        // Solo mostramos si el estado es 1 (Knights), 10 (Minotauro), 11 (Llave), o 2 (Rescatada)
        if (estado == 1 || estado == 10 || estado == 11 || estado == 2)
        {
            if (!QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(true);

            // 1. Lógica del Contador, Icono e Instrucciones
            switch (estado)
            {
                case 1: // Misión Aceptada: Matar Knights
                    ItemIcon.sprite = knightKillSprite;
                    CantidadTexto.text = $"{count} / {target}";
                    InstruccionesTexto.text = $"Objetivo: Derrota a los {target} Caballeros Negros que custodian la entrada a la mina 2.";
                    break;

                case 10: // Knights Derrotados: Matar Minotauro para obtener la llave
                    int keyCount = QuestManager.Instance.Key_Calabozo_Count;
                    int keyTarget = QuestManager.Key_Calabozo_Target;

                    // Usar el sprite del Minotauro si está asignado, si no, usa el de la Aldeana (por seguridad)
                    ItemIcon.sprite = MinotaurSprite != null ? MinotaurSprite : QuestManager.Instance.AldeanaIconSprite;

                    CantidadTexto.text = $"{keyCount} / {keyTarget}";
                    InstruccionesTexto.text = "¡Camino despejado! Entra en la mina. Derrota al Minotauro para obtener la llave del calabozo.";
                    break;

                case 11: // Llave Obtenida: Buscar Aldeana
                    ItemIcon.sprite = QuestManager.Instance.AldeanaIconSprite; // Mostrar ícono de la aldeana
                    CantidadTexto.text = "1 / 1"; // Llave obtenida
                    InstruccionesTexto.text = "¡Tienes la llave! Busca y libera a la Aldeana que está encerrada en el calabozo.";
                    break;

                case 2: // Aldeana Rescatada
                    ItemIcon.sprite = QuestManager.Instance.AldeanaIconSprite;
                    CantidadTexto.text = "¡Rescatada!";
                    InstruccionesTexto.text = "Regresa al pueblo. ¡La Aldeana ha sido rescatada!";
                    break;
            }
        }
        else // Estado 0 (inactiva) o cualquier otro no aplicable
        {
            if (QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(false);
        }
    }
}