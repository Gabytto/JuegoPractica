using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private int vidaInicial = 100;
    [SerializeField] private int vidaActual;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float duracionFlash = 0.15f;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            StartCoroutine(FlashEffect());

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
    private void Muerte()
    {
        if (vidaActual <= 0)
        {
            gameObject.SetActive(false);
        }      
    }
}
