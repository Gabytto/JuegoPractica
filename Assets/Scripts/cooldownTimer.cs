using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cooldownTimer : MonoBehaviour
{
    public float duracionCooldown = 0.25f;
    [HideInInspector] public float tiempoRestante;
    [HideInInspector] public bool timerOn = false;

    void Start()
    {
        tiempoRestante = 0f;
    }

    void Update()
    {
        if (timerOn)
        {
            tiempoRestante -= Time.deltaTime;

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0f;
                timerOn = false;
                Debug.Log("Cooldown completado. Listo para disparar.");
            }
        }
        
    }

    // Función pública para iniciar el cooldown desde otro script (la clase del jugador)
    public void IniciarCooldown()
    {
        if (!timerOn)
        {
            tiempoRestante = duracionCooldown;
            timerOn = true;
        }
    }
}

