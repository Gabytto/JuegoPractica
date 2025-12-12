using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class consumirPota : MonoBehaviour
{
    public AudioClip sonidoConsumo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (sonidoConsumo != null)
            {
                // Reproduce el sonido una sola vez, creando un objeto temporal en la posición.
                AudioSource.PlayClipAtPoint(sonidoConsumo, transform.position);
            }
            gameObject.SetActive(false);
        }
    }
}
