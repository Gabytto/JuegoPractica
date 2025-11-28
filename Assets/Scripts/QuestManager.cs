using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // 0 = Inactiva (Misión no dada), 1 = Aceptada (Rescatar Aldeana), 2 = Aldeana Rescatada/Entregada
    public int Estado_Quest_Rescate = 0;

    [Header("Referencias de Obstáculos")]
    public bloqueoCamino_1 BloqueoCamino1Ref;

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
            Debug.Log("Blue Slime's fluid recolectado: " + Blue_Slime_Fluid_Count + " / " + Blue_Slime_Fluid_Target);

            // La UI se actualiza automáticamente en QuestDisplayManager.Update() o...
            // Si tienes una referencia directa a la UI en QuestManager:
            // ActualizarUI_QuestErmitano(); 
        }
    }
    public void ActualizarUI_QuestErmitano()
    {
        // Este método puede estar vacío si tu QuestDisplayManager usa el Update(), 
        // pero es necesario que exista para que el código compile.
        // El QuestDisplayManager hará el trabajo real.
        Debug.Log("QuestManager: Actualizando UI para Misión del Ermitaño.");
    }

}
