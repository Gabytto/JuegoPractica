using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class movimiento : MonoBehaviour
{
    // Variables para movimiento del pj
    private Rigidbody2D rb;
    private int movHorizontal;
    private int movVertical;
    private Vector2 mov;
    // Variables sprintar
    [SerializeField]private float speed;
    private float ogSpeed;
    private float mulSpeed;
    [SerializeField] private float valMulSpeed = 1.5f;
    // Variables modo rage
    public bool rageOn = false;
    public float daño = 10f;   
    private float ogDaño;
    private float mulDaño;
    // Variables timer
    [SerializeField]private timer timer;
    // Inventario
    public int rocasObtenidas;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ogSpeed = speed;
        mulSpeed = speed * valMulSpeed;
        ogDaño = daño;
        mulDaño = daño * 5f;
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
        else if(Input.GetKey(KeyCode.A))
        {
            movHorizontal = -1;
        }    
        else { movHorizontal = 0; }

        // Movimiento vertical del personaje
        if (Input.GetKey(KeyCode.W))
        { movVertical = 1;}
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
        Debug.Log("Auch!!!");
    }
}
