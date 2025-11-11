using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDisplayManager : MonoBehaviour
{
    [Header("Referencias de la UI")]

    // Nueva Referencia: ¡Referencia al panel principal que quieres activar/desactivar!
    public GameObject QuestPanelGameObject;

    [Header("Referencias de la UI")]
    // 1. ItemIcon: Dónde está la poción
    public Image ItemIcon;

    // 2. cantidad: Dónde está el 0/0
    public TextMeshProUGUI CantidadTexto;

    // 3. Text (TMP): Dónde están las instrucciones
    public TextMeshProUGUI InstruccionesTexto;

    [Header("Recursos de la Misión")]
    // Sprite del ítem requerido (¡Arrastra el sprite aquí!)
    public Sprite goblinBloodSprite;



    void Start()
    {
        // 1. Verificar referencias
        if (QuestPanelGameObject == null || ItemIcon == null || CantidadTexto == null || InstruccionesTexto == null)
        {
            Debug.LogError("¡ERROR! Faltan referencias de UI. Revisa el QuestUIManager en el Inspector.");
            enabled = false;
            return;
        }

        // 2. Desactivar el panel *externo* al inicio
        // Usamos la referencia pública QuestPanelGameObject
        QuestPanelGameObject.SetActive(false);
    }

    void Update()
    {
        // Usamos el Singleton de tu QuestManager
        if (QuestManager.Instance != null)
        {
            UpdateQuestDisplay();
        }
    }

    private void UpdateQuestDisplay()
    {
        int estado = QuestManager.Instance.Estado_Quest_Sabio;

        // 1. Manejo de Visibilidad del Panel
        // Mostrar solo en estado 1 (Activa) o 2 (Cumplida)
        if (estado == 1 || estado == 2)
        {
            if (!QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(true);
        }
        else // Si Inactiva (0) o Entregada (3), ocultar el panel
        {
            if(QuestPanelGameObject.activeSelf) QuestPanelGameObject.SetActive(false);
            return;
        }

        // 2. Establecer el Icono (Solo se hace una vez al activar)
        if (ItemIcon.sprite == null && goblinBloodSprite != null)
        {
            ItemIcon.sprite = goblinBloodSprite;
        }


        // 3. Obtener el Progreso
        int count = QuestManager.Instance.Gobblin_Blood_Count;
        int target = QuestManager.Gobblin_Blood_Target;

        // 4. Actualizar Texto de Progreso e Instrucciones según el Estado
        switch (estado)
        {
            case 1: // Misión Aceptada / En Curso
                // Texto de progreso: 5 / 8 Sangre de Goblin
                CantidadTexto.text = $"{count} / {target}";

                // Instrucción
                InstruccionesTexto.text = "Objetivo: Derrota Goblins en la mina para recolectar sus materiales.";
                break;

            case 2: // Objetivo Cumplido, listo para entregar
                // Texto de progreso: 8 / 8 Sangre de Goblin
                CantidadTexto.text = $"{target} / {target}";

                // Instrucción
                InstruccionesTexto.text = "¡Misión Cumplida!\nRegresa a hablar con el Sabio.";
                break;
        }
    }
}
