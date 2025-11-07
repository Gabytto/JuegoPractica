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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentTarget = pointA;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (playerDistance < detectionRange)
        {

            MoveTowards(player.position);
        }
        else
        {

            Patrol();
        }

        void Patrol()
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

            if (distanceToTarget < 0.5f)
            {
                currentTarget = currentTarget == pointA ? pointB : pointA;
            }

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

}
