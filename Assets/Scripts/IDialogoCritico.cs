using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogoCritico
{
    // Este método será llamado por el dialogeManager cuando la cola de mensajes
    // del personaje esté vacía (el diálogo ha terminado).
    void FinalizarDialogoCritico();
}
