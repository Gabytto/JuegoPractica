using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class movimiento : MonoBehaviour
{
    // Propiedades del pj
    private Rigidbody2D rb;
    private int vidaInicial = 100;
    private int vidaActual;
    private int da�oRecibido = 20;
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
    public float da�o = 10f;
    private float ogDa�o;
    private float mulDa�o;
    // Variables timer
    [SerializeField] private timer timer;
    // Inventario
    public int rocasObtenidas;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ogSpeed = speed;
        mulSpeed = speed * valMulSpeed;
        ogDa�o = da�o;
        mulDa�o = da�o * 5f;
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

        // Activaci�n del Modo rage
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!timer.timerOn)
            {
                da�o = mulDa�o;
                timer.timerOn = true;
                Debug.Log("Modo Rage activado. Da�o: " + da�o);
            }
        }

        // Desactivaci�n del Modo Rage
        if (timer.tiempoRestante <= 0 && timer.timerOn)
        {
            da�o = ogDa�o;
            timer.timerOn = false;
            Debug.Log("Modo Rage desactivado. Da�o: " + da�o);
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
    }
    private void FixedUpdate()
    {
        rb.velocity = mov * speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("enemy"))
        {
            RecibirDa�o();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("potaVida"))
        {
            RecibirCuracion();
        }
    }
    private void RecibirDa�o()
    {
        vidaActual -= da�oRecibido;
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
}
