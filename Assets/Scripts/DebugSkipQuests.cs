using UnityEngine;

public class DebugSkipQuests : MonoBehaviour
{
    [Header("Configuración de Debug")]
    [Tooltip("La tecla que activar\u00e1 el salto de misiones (ej: K)")]
    public KeyCode SkipKey = KeyCode.K;

    void Update()
    {
        // Verificar si se presiona la tecla de salto (por ejemplo, 'K')
        if (Input.GetKeyDown(SkipKey))
        {
            SkipToKnightQuest();
        }
    }

    private void SkipToKnightQuest()
    {
        // 1. Verificar si QuestManager est\u00e1 disponible
        if (QuestManager.Instance == null)
        {
            Debug.LogError("QuestManager no encontrado. Aseg\u00farate de que est\u00e9 activo en la escena.");
            return;
        }

        // --- Simular que las quests previas est\u00e1n completas o avanzadas ---

        // A. Misi\u00f3n del Sabio: Simular que ya fue entregada
        QuestManager.Instance.Estado_Quest_Sabio = 3;

        // B. Misi\u00f3n del Ermita\u00f1o: Simular que ya ha sido entregada
        QuestManager.Instance.Estado_Quest_Ermitaño = 2;

        // C. Misi\u00f3n de Rescate: Ponerla en estado ACEPTADA (1)
        QuestManager.Instance.Estado_Quest_Rescate = 1;

        // D. Limpiar contadores de la Misi\u00f3n de Rescate (si estaban activos)
        // Esto asegura que el contador de Knights empiece en 0 si no se ha matado ninguno.
        QuestManager.Instance.Knight_Kill_Count = 0;

        // E. (Opcional) Desbloquear camino 1 y 2 si existen
        if (QuestManager.Instance.BloqueoCamino1Ref != null)
        {
            QuestManager.Instance.BloqueoCamino1Ref.gameObject.SetActive(false);
        }
        if (QuestManager.Instance.BloqueoCamino2Ref != null)
        {
            QuestManager.Instance.BloqueoCamino2Ref.gameObject.SetActive(false);
        }

        Debug.Log("DEBUG: Misiones salteadas. El Estado_Quest_Rescate ahora es 1. \u00a1Listo para derrotar 4 Knights!");

        // Puedes desactivar este objeto despu\u00e9s de usarlo una vez, si lo deseas
        // gameObject.SetActive(false); 
    }
}