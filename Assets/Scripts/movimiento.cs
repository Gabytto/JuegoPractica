using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class movimiento : MonoBehaviour
{
    // Propiedades del pj
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float duracionFlash = 0.20f;
    private bool giroIzq;
    public Image healthBar;
    private float vidaInicial = 200f;
    private float vidaActual;
    private int dañoRecibido = 20;
    private int curaVida = 100;
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

    [SerializeField] private ManagerEscenas managerEscenas;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ogSpeed = speed;
        mulSpeed = speed * valMulSpeed;
        ogDaño = daño;
        mulDaño = daño * 5f;
        vidaActual = vidaInicial;
        healthBar.fillAmount = (vidaActual / vidaInicial);

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
            StartCoroutine(FlashEffect());

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
        // 2. Asegurarse de que la vida no baje de 0
        vidaActual = Mathf.Max(vidaActual, 0);
        // Actualiza la barra de vida
        healthBar.fillAmount = (vidaActual / vidaInicial);

        Debug.Log("Auch!!!");
        Debug.Log("Tu vida actual es " + vidaActual);

        // --- LÓGICA DE MUERTE AGREGADA ---
        if (vidaActual <= 0)
        {
            // Detener el movimiento y cualquier otra acción (opcional)
            rb.velocity = Vector2.zero;

            // Llamar a la función del ManagerEscenas para cambiar a la escena de Game Over
            if (managerEscenas != null)
            {
                managerEscenas.CambiarEscena("fallaste");
                gameObject.SetActive(false); // Opcional: Desactivar el personaje mientras carga la escena
            }
            else
            {
                Debug.LogError("ERROR: ManagerEscenas no asignado en el Inspector de Movimiento.cs");
                gameObject.SetActive(false);
            }
        }
        // ---------------------------------
    }
    private void RecibirCuracion()
    {
        vidaActual += curaVida;
        // Asegurarse de que no supera la vida máxima
        vidaActual = Mathf.Min(vidaActual, vidaInicial);
        // Actualiza la barra de vida
        healthBar.fillAmount = (vidaActual / vidaInicial);
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
                // --- INICIO DE LA MODIFICACIÓN CRUCIAL ---##############################################################
                // 1. Instancia el láser y guarda la referencia
                GameObject nuevoLaserGO = Instantiate(laser, transform.position, Quaternion.identity);

                // 2. Obtiene el script 'laser'
                laser laserScript = nuevoLaserGO.GetComponent<laser>();

                // 3. Pasa la dirección usando la bandera 'giroIzq'
                if (laserScript != null)
                {
                    // Llama al método SetDirection (que definiremos en laser.cs)
                    laserScript.SetDirection(giroIzq);
                }
                // --- FIN DE LA MODIFICACIÓN CRUCIAL ---##########################################################
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
        else if (rb.velocity.x > 0 && giroIzq)
        {
            giroIzq = false;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }
    private IEnumerator FlashEffect()
    {
        // Cambiar el color del sprite a rojo
        spriteRenderer.color = Color.red;

        // Esperar el tiempo definido en duracionFlash
        yield return new WaitForSeconds(duracionFlash);

        // Volver al color original (blanco, que no altera los colores del sprite)
        spriteRenderer.color = Color.white;
    }
}
