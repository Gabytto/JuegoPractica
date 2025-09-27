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
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.right * velocidad;
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
