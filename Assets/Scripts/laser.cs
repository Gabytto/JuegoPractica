using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
    [SerializeField] private int velocidad = 10;
    [SerializeField] private float tiempoD = 2f;
    public int daño = 50;
    private Rigidbody2D rb;
    [SerializeField] private timer timer;
    //############################################
    private Vector2 direccionDisparo = Vector2.right;

    private void Awake() // Mejor Awake para asegurar el Rigidbody antes que Start
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // 2. NUEVO MÉTODO PÚBLICO para configurar la dirección
    public void SetDirection(bool isFacingLeft)
    {
        // Si isFacingLeft (giroIzq) es TRUE, xDir es -1. Si es FALSE, xDir es 1.
        float xDir = isFacingLeft ? -1f : 1f;
        direccionDisparo = new Vector2(xDir, 0);
    }
    //############################################

    private void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        rb.velocity = direccionDisparo * velocidad;
        Destroy(gameObject, tiempoD);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            Destroy(gameObject, 0f);
        }
    }
}
