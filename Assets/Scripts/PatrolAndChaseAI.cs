using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolAndChaseAI : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public float speed = 5f;
    public float detectionRange = 5f;
    private Transform currentTarget;
    private Rigidbody2D rb;
    public float pauseDuration = 2f;
    private bool isPaused = false;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentTarget = pointA;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isPaused)
        {
            if (anim != null)
            {
                anim.SetFloat("Speed", 0f);
            }
            return;
        }
        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance < detectionRange)
        {

            MoveTowards(player.position);
        }
        else
        {

            Patrol();
        }
        if (anim != null)
        {
            // Obtenemos la magnitud (longitud) del vector de velocidad. 
            // Si se está moviendo, el valor será alto; si está detenido, será 0.
            float currentSpeed = rb.velocity.magnitude;

            // Asignamos la velocidad al parámetro "Speed" del Animator
            anim.SetFloat("Speed", currentSpeed);
        }
    }
    void Patrol()
    {
        if (currentTarget == null || isPaused) return;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget < 0.5f)
        {
            // 1. Activar la pausa y detener el movimiento
            isPaused = true;

            // Detenemos la velocidad inmediatamente al llegar
            rb.velocity = Vector2.zero;

            // 2. Iniciar la corutina para el tiempo de espera
            StartCoroutine(StartPause());

            // NOTA: El cambio de 'currentTarget' se hará dentro de StartPause()            
        }
        if (!isPaused)
        {
            MoveTowards(currentTarget.position);
        }
            
    }
    void MoveTowards(Vector3 targetPosition)
    {
        // 1. Calcular la dirección
        Vector2 direction = (targetPosition - transform.position).normalized;

        // 2. Mover el objeto usando la velocidad del Rigidbody
        // Multiplicamos 'direction' por 'speed'
        rb.velocity = direction * speed;

        // Opcional: Volteo del sprite si estás en vista lateral
        if (direction.x > 0.01f) { /* Voltear derecha */ } 
        else if (direction.x < -0.01f) { /* Voltear izquierda */ }
    }
    IEnumerator StartPause()
    {
        // 1. Esperar el tiempo definido
        // La ejecución del script se detiene aquí hasta que pase 'pauseDuration'
        yield return new WaitForSeconds(pauseDuration);

        // 2. Cambiar al siguiente objetivo
        currentTarget = currentTarget == pointA ? pointB : pointA;

        // 3. Desactivar la pausa para que FixedUpdate pueda reanudar el movimiento
        isPaused = false;
    }
}
