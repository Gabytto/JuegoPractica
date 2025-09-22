using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class pickMe : MonoBehaviour
{
    [SerializeField] private int cantidad = 3;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            movimiento p = collision.GetComponent<movimiento>();
            p.rocasObtenidas += cantidad;
            Debug.Log(cantidad + " Rocas obtenidas");
            Debug.Log("Tienes " + p.rocasObtenidas + "Rocas en total.");
            gameObject.SetActive(false);
        }
    }
}



