using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : MonoBehaviour
{
    public float tiempoInicial = 20f;
    public float tiempoRestante;
    private int tiempoMostrado;
    public bool timerOn;
    void Start()
    {
        tiempoRestante = tiempoInicial;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            TimerCheck();
            tiempoMostrado = (int)tiempoRestante;
            Debug.Log("Tiempo actual: " + tiempoMostrado);
        }
        else
        {
            tiempoRestante = tiempoInicial;
        }
    }

    void TimerCheck()
    {
        if (timerOn)
        {
            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Se termino el tiempo!");
                tiempoRestante = 0f;
            }  
        }
    }


}
