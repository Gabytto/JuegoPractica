using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] // Permite verla en el Inspector
public struct EnemyStats
{
    public enemy.EnemyType type; // Opcional, pero útil para referencia
    public int health;
    public float damage;
    public float patrolSpeed;
}
public class enemy : MonoBehaviour
{
    public enum EnemyType { None, Goblin, Knight, Minotaur }
    [Header("Identificación")]
    // Hacemos esta variable pública para seleccionarla en el Inspector de Unity
    public EnemyType thisEnemyType = EnemyType.None;
    [Header("Configuración de Stats por Tipo")]
    public EnemyStats[] enemyConfigurations;
    [Header("Stats Activos")]
    [SerializeField] private float damageDealt;
    //
    private int vidaInicial = 100;
    [SerializeField] private int vidaActual;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float duracionFlash = 0.15f;
    private Animator animator;
    [Header("Configuración de Knockback")] // NUEVO
    public float knockbackForce = 10f;       // Fuerza del empuje
    public float knockbackDuration = 0.2f;  // Duración del aturdimiento
    [SerializeField] public PatrolAndChaseAI aiController; // ¡NUEVO! Referencia al script de IA

    

    //PRUEBA
    private bool giroIzq;


    void Awake()
    {
        // ... (otras inicializaciones) ...

        // Buscar el componente en el mismo objeto
        aiController = GetComponent<PatrolAndChaseAI>();
        if (aiController == null)
        {
            Debug.LogError("Error: AI Controller no encontrado.");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadStats();
        vidaActual = vidaInicial;
    }
    private void LoadStats()
    {
        // Itera sobre todas las configuraciones definidas en el array
        foreach (EnemyStats config in enemyConfigurations)
        {
            // Si el tipo de enemigo coincide con la configuración
            if (config.type == thisEnemyType)
            {
                // 1. Asignar Vida
                vidaInicial = config.health;

                // 2. Asignar Daño (si el enemigo ataca al jugador)
                damageDealt = config.damage;

                // 3. Asignar Velocidad de Patrullaje (al script de IA)
                if (aiController != null)
                {
                    aiController.patrolSpeed = config.patrolSpeed;
                    // También podrías asignar chaseSpeed si lo incluyes en EnemyStats
                }

                Debug.Log($"Stats de {thisEnemyType} cargados: Vida={vidaInicial}, Daño={damageDealt}");
                return; // Salir de la función
            }
        }
        // Mensaje de error si no se encuentra el tipo de enemigo en el array
        Debug.LogError($"¡ERROR! Configuración de stats no encontrada para el tipo: {thisEnemyType} en {gameObject.name}.");
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("laser"))
        {
            Debug.Log("enemigo recibio un tiro");
            laser p = collision.GetComponent<laser>();
            vidaActual -= p.daño;
            // Aplicamos el empuje usando la posición del láser
            // Usamos StartCoroutine, ya que el empuje tiene duración
            StartCoroutine(ApplyKnockback(collision.transform.position));
            StartCoroutine(FlashEffect());

            if (vidaActual <= 0)
            {
                ManejarMuerte(); // Llamamos a la nueva función de muerte
            }
        }
    }
    private IEnumerator ApplyKnockback(Vector2 attackerPosition)
    {
        if (aiController != null)
        {
            // 1. Marca el estado de aturdimiento usando la referencia al otro script
            // Esto detendrá la IA en PatrolAndChaseAI.FixedUpdate()
            aiController.isKnockedBack = true;

            // Aseguramos que el Rigidbody esté en el origen de donde aplicaremos la fuerza.
            rb.velocity = Vector2.zero;

            // 2. Calcular la dirección: (Posición del enemigo) - (Posición del atacante)
            Vector2 direction = (rb.position - attackerPosition).normalized;

            // Añadir un componente vertical para que salte un poco
            direction.y = Mathf.Clamp(direction.y + 0.5f, -1f, 1f);

            // 3. Aplicar la fuerza de empuje
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);

            // 4. Esperar la duración del aturdimiento
            yield return new WaitForSeconds(knockbackDuration);

            // 5. Limpiar la velocidad y quitar la bandera de aturdimiento para reanudar el movimiento
            rb.velocity = Vector2.zero;
            aiController.isKnockedBack = false;
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
    private void ManejarMuerte()
    {
        // LÓGICA DE LA MISIÓN: Llama al QuestManager para registrar la baja
        if (QuestManager.Instance != null)
        {
            if(thisEnemyType == EnemyType.Goblin)
            {
                QuestManager.Instance.AddGobblinKill();
                // Muestra en consola que la muerte fue registrada (opcional para depuración)
                Debug.Log("Quest: Gobblin eliminado y contador actualizado.");
            }
            
        }

        gameObject.SetActive(false);
    }

    //PRUEBA
    private void FlipSprite()
    {
        if (rb.velocity.x < 0 && !giroIzq)
        {
            giroIzq = true;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
        else if (rb.velocity.x > 0 && giroIzq)
        {
            giroIzq = false;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }
}
