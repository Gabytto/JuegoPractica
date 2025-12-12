using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloqueoCamino_3 : MonoBehaviour
{
    public Collider2D bloqueoFisico;
    public string mensajePista = "Debo hallar la mina para rescatar a la Aldeana antes de adentrarme mas en el bosque.";
    private bool mensajeMostrado = false;

    void Start()
    {
        if (bloqueoFisico == null)
        {
            Debug.LogError("¡ERROR! Falta asignar la referencia del 'bloqueoFisico' en el Inspector de " + gameObject.name);
            return;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && bloqueoFisico.enabled)
        {
            if (!mensajeMostrado)
            {
                dialogeManager.Instance.MostrarMensaje(mensajePista);
                Debug.Log("Pista mostrada (Bloqueo Camino 3): " + mensajePista);
                mensajeMostrado = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogeManager.Instance.CerrarPanel();
            mensajeMostrado = false;
        }
    }
}

