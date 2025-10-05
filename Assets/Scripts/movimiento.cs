using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class movimiento : MonoBehaviour
{
    // Propiedades del pj
    private Rigidbody2D rb;
    Animator animator;
    private bool giroIzq;
    private int vidaInicial = 200;
    private int vidaActual;
    private int dañoRecibido = 20;
    private int curaVida = 20;
    // Variables para movimiento del pj
    private int movHorizontal;
    private int movVertical;
    private Vector2 mov;
    // Variables sprintar
    [SerializeField] private float speed;
    private float ogSpeed;
    private float mulSpeed;
    [SerializeField] private float valMulSpeed = 1.5f;
    // Variables modo rage
    public bool rageOn = false;
    public float daño = 10f;
    private float ogDaño;
    private float mulDaño;
    // Variables timer
    [SerializeField] private timer timer;
    // Inventario
    public int rocasObtenidas;
    // Disparo
    [SerializeField] GameObject laser;
    [SerializeField] private cooldownTimer coolDownTimer;
    


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ogSpeed = speed;
        mulSpeed = speed * valMulSpeed;
        ogDaño = daño;
        mulDaño = daño * 5f;
        vidaActual = vidaInicial;
        
    }

    void Update()
    {
        // Sprintar
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = mulSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = ogSpeed;
        }

        // Activación del Modo rage
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!timer.timerOn)
            {
                daño = mulDaño;
                timer.timerOn = true;
                Debug.Log("Modo Rage activado. Daño: " + daño);
            }
        }

        // Desactivación del Modo Rage
        if (timer.tiempoRestante <= 0 && timer.timerOn)
        {
            daño = ogDaño;
            timer.timerOn = false;
            Debug.Log("Modo Rage desactivado. Daño: " + daño);
        }

        // Movimiento horizontal del personaje
        if (Input.GetKey(KeyCode.D))
        {
            movHorizontal = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movHorizontal = -1;
        }
        else { movHorizontal = 0; }

        // Movimiento vertical del personaje
        if (Input.GetKey(KeyCode.W))
        { movVertical = 1; }
        else if (Input.GetKey(KeyCode.S))
        { movVertical = -1; }
        else { movVertical = 0; }

        // Normalizado
        mov = new Vector2(movHorizontal, movVertical);
        mov = mov.normalized;

        Disparar();
    }
    private void FixedUpdate()
    {
        rb.velocity = mov * speed;
        if (Math.Abs(rb.velocity.x) > 0 || Math.Abs(rb.velocity.y) > 0)
        {
            animator.SetFloat("xVelocity", 1);
        }
        else
        {
            animator.SetFloat("xVelocity", 0);
        }
        FlipSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("enemy"))
        {
            RecibirDaño();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("potaVida"))
        {
            RecibirCuracion();
        }
    }
    private void RecibirDaño()
    {
        vidaActual -= dañoRecibido;
        Debug.Log("Auch!!!");
        Debug.Log("Tu vida actual es " + vidaActual);
        if (vidaActual <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("Te has muerto.");
        }
    }
    private void RecibirCuracion()
    {
        vidaActual += curaVida;
        Debug.Log("Tu vida actual es " + vidaActual);
    }
    void Disparar()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            
            if (!coolDownTimer.timerOn)
            {
                coolDownTimer.IniciarCooldown();
                Debug.Log("Disparo realizado");
                animator.SetTrigger("Attack");
                Instantiate(laser, transform.position, Quaternion.identity);
            }
        }
    }

    private void FlipSprite()
    {
        if (rb.velocity.x < 0 && !giroIzq)
        {
            giroIzq = true;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
        else if(rb.velocity.x > 0 && giroIzq)
        {
            giroIzq = false;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }
}
