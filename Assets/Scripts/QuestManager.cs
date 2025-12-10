using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    // 1. Singleton para acceso global
    public static QuestManager Instance;

    [Header("Variables de la Misión del Sabio")]
    // 0 = Inactiva, 1 = Aceptada, 2 = Objetivo Cumplido, 3 = Entregada
    public int Estado_Quest_Sabio = 0;
    // Contador para el objetivo de la misión
    public int Gobblin_Blood_Count = 0;
    // Objetivo requerido
    public const int Gobblin_Blood_Target = 8;

    [Header("Referencias Globales")]
    public Transform MineInteriorSpawnPoint;

    [Header("Variables de la Misión del Ermitaño (Quest 2)")]
    // 0 = Inactiva (Misión no dada), 1 = Aceptada (Buscar Ermitaño), 2 = Hablado y Completa.
    public int Estado_Quest_Ermitaño = 0;
    // Contador para el objetivo de la misión
    public int Blue_Slime_Fluid_Count = 0;
    // Objetivo requerido (5 muestras)
    public const int Blue_Slime_Fluid_Target = 5;

    [Header("Variables de la Misión de Rescate (Quest 3)")]
    // 0 = Inactiva, 1 = Matar Knights, 10 = Matar Minotauro, 11 = Buscar Aldeana, 2 = Rescatada
    public int Estado_Quest_Rescate = 0;
    public int Knight_Kill_Count = 0;
    public const int Knight_Kill_Target = 4;

    [Header("UI de Misión (Panel)")]
    public TextMeshProUGUI MissionTitleText;        // Componente de texto para el título/descripción
    public Image MissionIconImage;          // Componente de imagen para el ícono
    public Sprite AldeanaIconSprite;        // Sprite de la Aldeana para el panel
    // Si tienes un sprite del Minotauro, añádelo aquí
    // public Sprite MinotaurIconSprite;       

    [Header("Referencias de la Mina de Rescate")]
    public SpriteRenderer MinaRescateSpriteRenderer; // Para cambiar la imagen
    public Sprite MinaRescateActivadaSprite;         // El nuevo sprite "activado"
    public Collider2D MinaRescateTriggerCollider;     // El BoxCollider2D que actúa como teleport trigger

    public int Key_Calabozo_Count = 0;
    public const int Key_Calabozo_Target = 1;

    [Header("Referencias de Obstáculos")]
    public bloqueoCamino_1 BloqueoCamino1Ref;
    public bloqueoCamino_2 BloqueoCamino2Ref;

    [Header("Referencias de NPCs")]
    // Referencia al Ermitaño para poder llamarlo
    public ErmitanoController ErmitanoRef;

    private void Awake()
    {
        // Asegura que solo haya una instancia de este script
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: para que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void IniciarQuestSabio()
    {
        if (Estado_Quest_Sabio == 0)
        {
            Estado_Quest_Sabio = 1; // 1 = Misión Aceptada
            Debug.Log("Misión del Sabio iniciada. Revisando bloqueos.");

            // ** Desbloquear el camino 1 **
            if (BloqueoCamino1Ref != null)
            {
                // Llama al método del bloqueo para desactivar el Collider 2D físico.
                BloqueoCamino1Ref.ActualizarEstadoBloqueo();
            }
        }
    }

    // Método para aumentar el contador cuando se mata un Gobblin
    public void AddGobblinKill()
    {
        // Solo aumentar si la misión está en estado 1 (Aceptada)
        if (Estado_Quest_Sabio == 1)
        {
            Gobblin_Blood_Count++;
            Debug.Log("Sangre de Gobblin recolectada: " + Gobblin_Blood_Count + " / " + Gobblin_Blood_Target);

            // Verificar si el objetivo se ha cumplido
            if (Gobblin_Blood_Count >= Gobblin_Blood_Target)
            {
                Estado_Quest_Sabio = 2; // Cambia el estado a Objetivo Cumplido
                Debug.Log("¡Misión del Sabio: Objetivo cumplido! Vuelve a hablar con él.");
            }
        }
    }
    public void AddBlueSlimeFluid()
    {
        // Solo aumentamos si estamos en la misión 1 del Ermitaño Y no hemos alcanzado el límite
        if (Estado_Quest_Ermitaño == 1 && Blue_Slime_Fluid_Count < Blue_Slime_Fluid_Target)
        {
            Blue_Slime_Fluid_Count++;
            Debug.Log("Fluido de Slime Azul recolectado: " + Blue_Slime_Fluid_Count + " / " + Blue_Slime_Fluid_Target);

            // La UI se actualiza automáticamente en QuestDisplayManager.Update() o...
            // Si tienes una referencia directa a la UI en QuestManager:
            // ActualizarUI_QuestErmitano(); 
        }
    }

    // *** FUNCIÓN CLAVE MODIFICADA ***
    public void AddKeyCalabozo()
    {
        // Solo se obtiene la llave si la misión de Rescate está en el estado de "Matar Minotauro" (Estado 10)
        if (Estado_Quest_Rescate == 10 && Key_Calabozo_Count < Key_Calabozo_Target)
        {
            Key_Calabozo_Count++;
            Debug.Log("Haz conseguido la llave del calabozo.");

            // Nuevo estado para la búsqueda final (usamos 11)
            if (Key_Calabozo_Count >= Key_Calabozo_Target)
            {
                Estado_Quest_Rescate = 11; // 11 = Llave obtenida. Objetivo: Buscar Aldeana.

                // Actualización de UI: Buscar a la aldeana
                if (MissionTitleText != null)
                {
                    MissionTitleText.text = "Busca a la aldeana en el interior de la mina";
                }
                if (MissionIconImage != null && AldeanaIconSprite != null)
                {
                    // Ahora es el momento de cambiar al ícono de la Aldeana
                    MissionIconImage.sprite = AldeanaIconSprite;
                }

                Debug.Log("Misión de Rescate: Llave obtenida. Nuevo objetivo: ¡Busca a la aldeana!");
            }
            else
            {
                // Actualiza el contador si el objetivo fuera mayor a 1
                if (MissionTitleText != null)
                {
                    MissionTitleText.text = "Derrota al Minotauro que custodia el calabozo (" + Key_Calabozo_Count + "/" + Key_Calabozo_Target + ")";
                }
            }
        }
    }

    public void ActualizarUI_QuestErmitano()
    {
        Debug.Log("QuestManager: Actualizando UI para Misión del Ermitaño.");
    }

    // *** FUNCIÓN CLAVE MODIFICADA ***
    public void AddKnightKill()
    {
        // Solo aumentar si la misión está en estado 1 (Rescate Aceptada)
        if (Estado_Quest_Rescate == 1)
        {
            Knight_Kill_Count++;
            Debug.Log("Knight derrotado: " + Knight_Kill_Count + " / " + Knight_Kill_Target);

            // Verificar si el objetivo de los Knights se ha cumplido
            if (Knight_Kill_Count >= Knight_Kill_Target)
            {
                // *** CAMBIO CLAVE AQUÍ: Nuevo estado intermedio (usamos 10) ***
                // 10 = Knights derrotados y Mina activada. Objetivo: Matar Minotauro.
                Estado_Quest_Rescate = 10;

                Debug.Log("¡Objetivo de Knights cumplido! La Mina de Rescate se ha activado. NUEVO OBJETIVO: Minotauro.");

                // 1. LÓGICA DE ACTIVACIÓN DE LA MINA
                if (MinaRescateSpriteRenderer != null && MinaRescateActivadaSprite != null)
                {
                    // Cambiar el sprite de la mina
                    MinaRescateSpriteRenderer.sprite = MinaRescateActivadaSprite;
                }
                if (MinaRescateTriggerCollider != null)
                {
                    // Activar el Box Collider 2D que teletransporta
                    MinaRescateTriggerCollider.enabled = true;
                }

                // 2. LÓGICA DE ACTUALIZACIÓN DEL PANEL DE MISIÓN: Derrotar Minotauro
                if (MissionTitleText != null)
                {
                    // Mostrar el nuevo objetivo.
                    MissionTitleText.text = "Derrota al Minotauro que custodia el calabozo (0/" + Key_Calabozo_Target + ")";

                    // Si tienes un Sprite para el Minotauro, la lógica sería algo así:
                    // if (MissionIconImage != null && MinotaurIconSprite != null)
                    // {
                    //     MissionIconImage.sprite = MinotaurIconSprite;
                    // }
                }
            }
        }
    }
}
