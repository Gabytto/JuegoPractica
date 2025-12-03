using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManagerEscenas : MonoBehaviour
{
    public GameObject panelControles;
    public AudioSource audioSourceUI; // Fuente de audio para los clicks de la UI
    public AudioClip clickSound;      // Clip de audio del click

    public void CambiarEscena(string a)
    {
        SceneManager.LoadScene(a);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para verificar en la consola
        Application.Quit();
    }
    public void AlternarControles()
    {
        // 1. Alternar Visibilidad: 
        // Establece el estado opuesto al actual. Si es true, lo pone a false, y viceversa.
        panelControles.SetActive(!panelControles.activeSelf);

        // 2. Reproducir Sonido:
        // Asegura que tengamos una fuente y un clip antes de intentar reproducir.
        if (audioSourceUI != null && clickSound != null)
        {
            // PlayOneShot es ideal para clicks, ya que no interrumpe otros sonidos en el mismo AudioSource
            audioSourceUI.PlayOneShot(clickSound);
        }
    }

    // Activa el pop-up
    public void MostrarControles()
    {
        panelControles.SetActive(true);
    }

    // Desactiva el pop-up (para el botón de cerrar dentro del pop-up)
    public void CerrarControles()
    {
        panelControles.SetActive(false);
        // Opcional: También puedes reproducir el click aquí si el botón de la cruz usa esta función.
        if (audioSourceUI != null && clickSound != null)
        {
            audioSourceUI.PlayOneShot(clickSound);
        }
    }
}
