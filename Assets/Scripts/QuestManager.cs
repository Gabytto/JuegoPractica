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

    // Objetivo requerido (para que sea fácil de ajustar)
    public const int Gobblin_Blood_Target = 8;

    [Header("Variables de la Misión del Ermitaño (Quest 2)")]
    // 0 = Inactiva (Misión no dada), 1 = Aceptada (Buscar Ermitaño), 2 = Encontrado/Hablado
    public int Estado_Quest_Ermitaño = 0; // <-- ¡NUEVA LÍNEA!

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
   
}
