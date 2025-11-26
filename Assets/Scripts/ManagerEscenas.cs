using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ManagerEscenas : MonoBehaviour
{
    public GameObject panelControles;
    public void CambiarEscena(string a)
    {
        SceneManager.LoadScene(a);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego..."); // Mensaje para verificar en la consola
        Application.Quit();
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
    }
}
