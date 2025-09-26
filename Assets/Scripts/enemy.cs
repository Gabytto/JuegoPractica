using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private int vidaInicial = 100;
    [SerializeField] private int vidaActual;
    private Rigidbody2D rb;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaInicial;
    }

    // Update is called once per frame
    void Update()
    {
        Muerte();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser"))
        {
            Debug.Log("enemigo recibio un tiro");
            laser p = collision.GetComponent<laser>();
            vidaActual -= p.daño;

        }
    }
    private void Muerte()
    {
        if (vidaActual <= 0)
        {
            gameObject.SetActive(false);
        }      
    }
}
