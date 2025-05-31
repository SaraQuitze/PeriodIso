using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    // Singleton (acceso público de solo lectura)
    public static PlayerController Instance { get; set; }

    [Header("Movement")]
    [SerializeField] public float baseMoveSpeed = 5.0f;
    [SerializeField][Range(0.5f, 0.9f)] private float isometricRatio = 0.707f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask walkableLayer;
    [SerializeField] private float collisionOffset = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private Vector2 movementInput;
    private Vector2 calculatedMovement;
    private bool isFacingRight = true;
    private float baseSpeed;
    private float currentSpeedModifier;

    private float currentMoveSpeed;
    public float moveSpeed
    {
        get => currentMoveSpeed;
        set
        {
            currentMoveSpeed = Mathf.Clamp(value, baseMoveSpeed * 0.1f, baseMoveSpeed * 2f);
            currentSpeedModifier = currentMoveSpeed / baseMoveSpeed;
        }
    }

    public float GetBaseSpeed()
    {
        return baseMoveSpeed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        currentMoveSpeed = baseMoveSpeed;

        // Configuración de Rigidbody
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // (Opcional si usas múltiples escenas)
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // Captura de input
        movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Conversión isométrica
        calculatedMovement = new Vector2(
            (movementInput.x - movementInput.y) * isometricRatio,
            (movementInput.x + movementInput.y) * isometricRatio * 0.5f
        ).normalized;

        if (moveSpeed <= 0.1f) moveSpeed = baseSpeed;

        UpdateAnimation();
    }


    private void FixedUpdate()
    {
        if (movementInput.magnitude > 0.1f)
        {
            // Añade esta verificación
            if (moveSpeed <= 0.1f)
            {
                moveSpeed = baseMoveSpeed * 0.1f; // Mínimo 10% de velocidad
            }

            TryMove(calculatedMovement);
        }
    }

    private void TryMove(Vector2 direction)
    {
        // Verificación de colisión
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = capsuleCollider.Cast(
            direction,
            hits,
            moveSpeed * Time.fixedDeltaTime + collisionOffset
        );

        if (hitCount == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimation()
    {
        bool isMoving = movementInput.magnitude > 0.1f;
        anim.SetBool("Caminar", isMoving);

        if (movementInput.x != 0)
        {
            bool shouldFaceRight = movementInput.x > 0;
            if (shouldFaceRight != isFacingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (rb != null && movementInput.magnitude > 0.1f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, calculatedMovement * (moveSpeed * 0.1f + collisionOffset));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Colisionando con: {collision.gameObject.name}");
    }

    public void SetTemporarySpeed(float newSpeed)
    {
        moveSpeed = Mathf.Clamp(newSpeed, baseMoveSpeed * 0.1f, baseMoveSpeed * 2f);
        currentSpeedModifier = moveSpeed / baseMoveSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseMoveSpeed;
        currentSpeedModifier = 1f;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}
