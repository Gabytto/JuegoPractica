using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
    [SerializeField] private int velocidad = 10;
    [SerializeField] private float tiempoD = 2f;
    public int daño = 50;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.right * velocidad;
        Destroy(gameObject, tiempoD);
    }
}
